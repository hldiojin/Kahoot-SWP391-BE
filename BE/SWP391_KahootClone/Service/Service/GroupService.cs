using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using static Repository.DTO.RequestDTO;



namespace Service.Service // Correct namespace
{
   
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IMapper _mapper; // Removed IMapper
        private readonly GroupRepository _groupRepository;

        public GroupService(IUnitOfWork unitOfWork, GroupRepository groupRepository) // Removed IMapper and added GroupRepository
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Removed IMapper
        }

        public async Task<ResponseDTO> GetGroupByIdAsync(int groupId)
        {
            try
            {
                var group = await _groupRepository.GetByIdAsync(groupId); // Use GroupRepository
                if (group == null)
                {
                    return new ResponseDTO(404, "Group not found.");
                }

                var groupDto = MapGroupToDto(group); // Use manual mapping
                return new ResponseDTO(200, "Group retrieved successfully.", groupDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error retrieving group: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetGroupsAsync()
        {
            try
            {
                var groups = await _groupRepository.GetAllAsync(); // Use GroupRepository
                var groupDtos = groups.Select(MapGroupToDto).ToList(); // Use manual mapping
                return new ResponseDTO(200, "Groups retrieved successfully.", groupDtos);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error retrieving groups: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> CreateGroupAsync(GroupDTO groupDto)
        {
            try
            {
                var group = MapDtoToGroup(groupDto); // Use manual mapping
                group.CreatedAt = DateTime.UtcNow; // Set CreatedAt
                await _groupRepository.CreateAsync(group); // Use GroupRepository
                await _unitOfWork.SaveChangesAsync();

                groupDto.Id = group.Id;
                return new ResponseDTO(201, "Group created successfully.", groupDto); // Return the input dto
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error creating group: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> UpdateGroupAsync(int groupId, GroupDTO groupDto)
        {
            try
            {
                var existingGroup = await _groupRepository.GetByIdAsync(groupId); // Use GroupRepository
                if (existingGroup == null)
                {
                    return new ResponseDTO(404, "Group not found.");
                }

                UpdateGroupFromDto(groupDto, existingGroup); // Use manual update
                await _groupRepository.UpdateAsync(existingGroup); // Use GroupRepository
                await _unitOfWork.SaveChangesAsync(); // Save changes

                return new ResponseDTO(200, "Group updated successfully.", groupDto); // return input

            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error updating group: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> DeleteGroupAsync(int groupId)
        {
            try
            {
                var existingGroup = await _groupRepository.GetByIdAsync(groupId); // Use GroupRepository
                if (existingGroup == null)
                {
                    return new ResponseDTO(404, "Group not found.");
                }

                await _groupRepository.RemoveAsync(existingGroup); // Use GroupRepository
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(200, "Group deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error deleting group: {ex.Message}");
            }
        }

        private GroupDTO MapGroupToDto(Group group)
        {
            return new GroupDTO
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Rank = group.Rank,
                MaxMembers = group.MaxMembers,
                TotalPoint = group.TotalPoint,
                CreatedBy = group.CreatedBy,
                CreatedAt = group.CreatedAt
            };
        }

        private Group MapDtoToGroup(GroupDTO groupDto)
        {
            return new Group
            {
                Id = groupDto.Id,
                Name = groupDto.Name,
                Description = groupDto.Description,
                Rank = groupDto.Rank,
                MaxMembers = groupDto.MaxMembers,
                TotalPoint = groupDto.TotalPoint,
                CreatedBy = groupDto.CreatedBy,
                CreatedAt = groupDto.CreatedAt
            };
        }

        private void UpdateGroupFromDto(GroupDTO groupDto, Group group)
        {
            group.Name = groupDto.Name;
            group.Description = groupDto.Description;
            group.Rank = groupDto.Rank;
            group.MaxMembers = groupDto.MaxMembers;
            group.TotalPoint = groupDto.TotalPoint;
            group.CreatedBy = groupDto.CreatedBy;
            group.CreatedAt = groupDto.CreatedAt;
        }
    }
}


