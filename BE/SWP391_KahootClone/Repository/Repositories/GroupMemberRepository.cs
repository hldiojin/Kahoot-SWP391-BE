using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Base;
using Repository.DBContext;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class GroupMemberRepository : GenericRepository<GroupMember>
    {
        public GroupMemberRepository() { }

        public GroupMemberRepository(SWP_KahootContext context) => _context = context;
        public async Task<IEnumerable<GroupMember>> GetGroupMemberByGroupIdAsync(int groupId)
        {
            return await _context.GroupMembers
                .Where(q => q.GroupId == groupId)
                .ToListAsync();
        }
        public async Task<GroupMember> GetGroupMember(int groupId, int playerId)
        {
           

            var groupMember = await _context.GroupMembers
                .FirstOrDefaultAsync(u => u.GroupId == groupId && u.PlayerId == playerId);

            return groupMember;
        }
        public async Task<IEnumerable<GroupMember>> GetGroupMemberByPlayerIdAsync(int playerId)
        {
            return await _context.GroupMembers
                .Where(q => q.PlayerId == playerId)
                .ToListAsync();
        }
        public async Task<List<GroupMember>> GetGroupsByPlayerIdAsync(int playerId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.PlayerId == playerId)
                .ToListAsync();
        }
    }
}
