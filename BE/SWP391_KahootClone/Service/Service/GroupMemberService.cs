using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using static Repository.DTO.RequestDTO;

namespace Service.Service // Correct namespace
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GroupMemberRepository _groupMemberRepository; // Inject the GroupMemberRepository


        public GroupMemberService(IUnitOfWork unitOfWork, GroupMemberRepository groupMemberRepository) // Inject IUnitOfWork and  GroupMemberRepository
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _groupMemberRepository = groupMemberRepository ?? throw new ArgumentNullException(nameof(groupMemberRepository));

        }

        public async Task<ResponseDTO> GetGroupMemberByIdAsync(int groupId, int playerId)
        {
            try
            {
                var groupMember = await _groupMemberRepository.GetGroupMember(groupId, playerId); // Use the repository
                if (groupMember == null)
                {
                    return new ResponseDTO(404, "Group member not found.");
                }

                var groupMemberDto = MapGroupMemberToDto(groupMember);
                return new ResponseDTO(200, "Group member retrieved successfully.", groupMemberDto);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error retrieving group member: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetGroupMembersByGroupIdAsync(int groupId)
        {
            try
            {
                var groupMembers = await _groupMemberRepository.GetGroupMemberByGroupIdAsync(groupId); // Use the repository
                if (groupMembers == null)
                {
                    return new ResponseDTO(404, "No group members found for this group.");
                }
                var groupMemberDtos = groupMembers.Select(MapGroupMemberToDto).ToList();
                return new ResponseDTO(200, "Group members retrieved successfully.", groupMemberDtos);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error retrieving group members: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> AddGroupMemberAsync(GroupMemberDTO groupMemberDto)
        {
            try
            {
                if (groupMemberDto == null)
                {
                    return new ResponseDTO(400, "Invalid request data.");
                }
                if(await _groupMemberRepository.GetGroupMember(groupMemberDto.GroupId, groupMemberDto.PlayerId) != null)
                {
                    return new ResponseDTO(409, "Group member already exists.");
                }
                if(  _unitOfWork.PlayerRepository.GetById(groupMemberDto.PlayerId) == null ||  _unitOfWork.GroupRepository.GetById(groupMemberDto.GroupId) == null)
                {
                    return new ResponseDTO(400, "Group ID and Player ID not exists.");
                }  
                var request = MapDtoToGroupMember(groupMemberDto);

                await _groupMemberRepository.CreateAsync(request);
                await _unitOfWork.SaveChangesAsync();

                //groupMemberDto.GroupId = request.GroupId;
                //groupMemberDto.PlayerId = request.PlayerId;//update the dto
                return new ResponseDTO(201, "Group member added successfully.", groupMemberDto);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error adding group member: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> UpdateGroupMemberAsync(int groupId, int playerId, GroupMemberDTO groupMemberDto)
        {
            try
            {
                var existingGroupMember = await _groupMemberRepository.GetGroupMember(groupId, playerId); // Use the repository
                if (existingGroupMember == null)
                {
                    return new ResponseDTO(404, "Group member not found.");
                }

                UpdateGroupMemberFromDto(groupMemberDto, existingGroupMember); //use manual update
                await _groupMemberRepository.UpdateAsync(existingGroupMember); // Use the repository
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(200, "Group member updated successfully.", groupMemberDto);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error updating group member: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> DeleteGroupMemberAsync(int groupId, int playerId)
        {
            try
            {
                var existingGroupMember = await _groupMemberRepository.GetGroupMember(groupId, playerId); // Use the repository
                if (existingGroupMember == null)
                {
                    return new ResponseDTO(404, "Group member not found.");
                }
                await _groupMemberRepository.RemoveAsync(existingGroupMember); // Use the repository
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(200, "Group member deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error deleting group member: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetGroupMemberByGroupId(int groupId)
        {
            try
            {
                var groupMembers = await _groupMemberRepository.GetGroupMemberByGroupIdAsync(groupId); // Use the repository
                if (groupMembers == null)
                {
                    return new ResponseDTO(404, "No group members found for the specified Group ID.");
                }
                var groupMemberDTOs = groupMembers.Select(MapGroupMemberToDto).ToList();
                return new ResponseDTO(200, "Group members retrieved successfully.", groupMemberDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "An error occurred while retrieving group members.", ex.Message);
            }
        }

        public async Task<ResponseDTO> GetGroupMemberByPlayerId(int playerId)
        {
            try
            {
                var groupMembers = await _groupMemberRepository.GetGroupMemberByPlayerIdAsync(playerId); // Use the repository
                if (groupMembers == null)
                {
                    return new ResponseDTO(404, "No group members found for the specified Player ID.");
                }
                var groupMemberDTOs = groupMembers.Select(MapGroupMemberToDto).ToList();
                return new ResponseDTO(200, "Group members retrieved successfully.", groupMemberDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "An error occurred while retrieving group members.", ex.Message);
            }
        }
       

        private GroupMemberDTO MapGroupMemberToDto(GroupMember groupMember)
        {
            return new GroupMemberDTO
            {
                GroupId = groupMember.GroupId,
                PlayerId = groupMember.PlayerId,
                Rank = groupMember.Rank,
                TotalScore = groupMember.TotalScore,
                JoinedAt = groupMember.JoinedAt,
                Status = groupMember.Status
            };
        }

        private GroupMember MapDtoToGroupMember(GroupMemberDTO groupMemberDto)
        {
            return new GroupMember
            {
                GroupId = groupMemberDto.GroupId,
                PlayerId = groupMemberDto.PlayerId,
                Rank = groupMemberDto.Rank,
                TotalScore = groupMemberDto.TotalScore,
                JoinedAt = groupMemberDto.JoinedAt,
                Status = groupMemberDto.Status
            };
        }

        private void UpdateGroupMemberFromDto(GroupMemberDTO groupMemberDto, GroupMember groupMember)
        {
            groupMember.Rank = groupMemberDto.Rank;
            groupMember.TotalScore = groupMemberDto.TotalScore;
            groupMember.Status = groupMemberDto.Status;
            groupMember.JoinedAt = groupMemberDto.JoinedAt; //Potentially remove this
        }
      
        public async Task<ResponseDTO> SelectGroup(CreateGroupMemberDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return new ResponseDTO(400, "Invalid request data.");
                }

                // Check if the group member already exists
                var existingGroupMember = await _groupMemberRepository.GetGroupMember(dto.GroupId, dto.PlayerId);
                if (existingGroupMember != null)
                {
                    return new ResponseDTO(409, "Player is already a member of this group.");
                }

                // Create a new GroupMember
                var newGroupMember = new GroupMember
                {
                    GroupId = dto.GroupId,
                    PlayerId = dto.PlayerId,
                    Rank = 0, // Default value, you can change this
                    TotalScore = 0, // Default value, you can change this
                    Status = "true", // Assume active by default
                    JoinedAt = DateTime.UtcNow // Set the time of joining
                };

                await _groupMemberRepository.CreateAsync(newGroupMember);
                await _unitOfWork.SaveChangesAsync();

                var groupMemberDto = MapGroupMemberToDto(newGroupMember);

                return new ResponseDTO(201, "Player successfully joined the group.", groupMemberDto);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error while selecting group: {ex.Message}");
            }
        }

    }
}
