using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IGroupService
    {
        Task<ResponseDTO> GetGroupByIdAsync(int groupId);
        Task<ResponseDTO> GetGroupsAsync();
        Task<ResponseDTO> CreateGroupAsync(GroupDTO groupDto);
        Task<ResponseDTO> UpdateGroupAsync(int groupId, GroupDTO groupDto);
        Task<ResponseDTO> DeleteGroupAsync(int groupId);
    }
}
