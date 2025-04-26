//using Repository.DTO;
//using Repository.Models;
//using Repository.Repositories;
//using Service.IService;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static Repository.DTO.RequestDTO;

//namespace Service.Service
//{
//    public class GroupMemberService : IGroupMemberService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly GroupMemberRepository _groupMemberRepository; // Use the repository

//        public GroupMemberService(IUnitOfWork unitOfWork, GroupMemberRepository groupMemberRepository)
//        {
//            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//            _groupMemberRepository = groupMemberRepository ?? throw new ArgumentNullException(nameof(groupMemberRepository));
//        }

//        public async Task<ResponseDTO> GetGroupMemberByIdAsync(int groupId, int userId)
//        {
//            try
//            {
//                var groupMember = await _groupMemberRepository.GetByIdAsync(groupId, userId); // Use repository method
//                if (groupMember == null)
//                {
//                    return new ResponseDTO(404, "Group member not found.");
//                }

//                var groupMemberDto = MapGroupMemberToDto(groupMember);
//                return new ResponseDTO(200, "Group member retrieved successfully.", groupMemberDto);
//            }
//            catch (Exception ex)
//            {
//                // Log exception
//                return new ResponseDTO(500, $"Error retrieving group member: {ex.Message}");
//            }
//        }

//        public async Task<ResponseDTO> GetGroupMembersByGroupIdAsync(int groupId)
//        {
//            try
//            {
//                var groupMembers = await _groupMemberRepository.GetByGroupIdAsync(groupId); // Custom repo method
//                if (groupMembers == null || !groupMembers.Any())
//                {
//                    return new ResponseDTO(200, "No group members found for this group.", new List<GroupMemberDTO>()); // return empty list
//                }

//                var groupMemberDtos = groupMembers.Select(MapGroupMemberToDto).ToList();
//                return new ResponseDTO(200, "Group members retrieved successfully.", groupMemberDtos);
//            }
//            catch (Exception ex)
//            {
//                // Log exception
//                return new ResponseDTO(500, $"Error retrieving group members: {ex.Message}");
//            }
//        }

//        public async Task<ResponseDTO> AddGroupMemberAsync(GroupMemberDTO groupMemberDto)
//        {
//            try
//            {
//                var groupMember = MapDtoToGroupMember(groupMemberDto);
//                await _groupMemberRepository.CreateAsync(groupMember); // Use repo
//                await _unitOfWork.SaveChangesAsync();

//                return new ResponseDTO(201, "Group member added successfully.", groupMemberDto);
//            }
//            catch (Exception ex)
//            {
//                // Log exception
//                return new ResponseDTO(500, $"Error adding group member: {ex.Message}");
//            }
//        }

//        public async Task<ResponseDTO> UpdateGroupMemberAsync(int groupId, int userId, GroupMemberDTO groupMemberDto)
//        {
//            try
//            {
//                var existingGroupMember = await _groupMemberRepository.GetByIdAsync(groupId, userId); // Use repo method
//                if (existingGroupMember == null)
//                {
//                    return new ResponseDTO(404, "Group member not found.");
//                }

//                UpdateGroupMemberFromDto(groupMemberDto, existingGroupMember); // Update entity
//                await _groupMemberRepository.UpdateAsync(existingGroupMember); // Use repo
//                await _unitOfWork.SaveChangesAsync();

//                return new ResponseDTO(200, "Group member updated successfully.", groupMemberDto);
//            }
//            catch (Exception ex)
//            {
//                // Log exception
//                return new ResponseDTO(500, $"Error updating group member: {ex.Message}");
//            }
//        }

//        public async Task<ResponseDTO> DeleteGroupMemberAsync(int groupId, int userId)
//        {
//            try
//            {
//                var existingGroupMember = await _groupMemberRepository.GetByIdAsync(groupId, userId); // Use composite key
//                if (existingGroupMember == null)
//                {
//                    return new ResponseDTO(404, "Group member not found.");
//                }
//                await _groupMemberRepository.RemoveAsync(existingGroupMember); // Use repo
//                await _unitOfWork.SaveChangesAsync();
//                return new ResponseDTO(200, "Group member deleted successfully.");
//            }
//            catch (Exception ex)
//            {
//                // Log exception
//                return new ResponseDTO(500, $"Error deleting group member: {ex.Message}");
//            }
//        }
//        private GroupMemberDTO MapGroupMemberToDto(GroupMember groupMember)
//        {
//            return new GroupMemberDTO
//            {
//                GroupId = groupMember.GroupId,
//                UserId = groupMember.UserId,
//                Rank = groupMember.Rank,
//                TotalScore = groupMember.TotalScore,
//                JoinedAt = groupMember.JoinedAt,
//                Status = groupMember.Status
//            };
//        }

//        private GroupMember MapDtoToGroupMember(GroupMemberDTO groupMemberDto)
//        {
//            return new GroupMember
//            {
//                GroupId = groupMemberDto.GroupId,
//                UserId = groupMemberDto.UserId,
//                Rank = groupMemberDto.Rank,
//                TotalScore = groupMemberDto.TotalScore,
//                JoinedAt = groupMemberDto.JoinedAt,
//                Status = groupMemberDto.Status
//            };
//        }

//        private void UpdateGroupMemberFromDto(GroupMemberDTO groupMemberDto, GroupMember groupMember)
//        {
//            groupMember.Rank = groupMemberDto.Rank;
//            groupMember.TotalScore = groupMemberDto.TotalScore;
//            groupMember.Status = groupMemberDto.Status;
//            groupMember.JoinedAt = groupMemberDto.JoinedAt; // added joined at
//        }
//    }
//}
