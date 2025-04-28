using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IGroupMemberService
    {
        Task<ResponseDTO> GetGroupMemberByIdAsync(int groupId, int userId); // Composite Key
        Task<ResponseDTO> GetGroupMembersByGroupIdAsync(int groupId);
        Task<ResponseDTO> AddGroupMemberAsync(GroupMemberDTO groupMemberDto);
        Task<ResponseDTO> UpdateGroupMemberAsync(int groupId, int userId, GroupMemberDTO groupMemberDto); // Composite Key
        Task<ResponseDTO> DeleteGroupMemberAsync(int groupId, int userId); // Composite Key
        Task<ResponseDTO> SelectGroup(CreateGroupMemberDTO dto);
        Task<ResponseDTO> GetGroupMemberByGroupId(int groupId);
        Task<ResponseDTO> GetGroupMemberByPlayerId(int playerId);
    }
}
