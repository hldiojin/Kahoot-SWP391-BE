using Microsoft.EntityFrameworkCore;
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
    public class GroupRepository    : GenericRepository<Models.Group>
    {
        public GroupRepository() { }

        public GroupRepository(SWP_KahootContext context) => _context = context;

        public async Task<Group> GetGroupById(int id)
        {
            return await _context.Groups.Include(x => x.GroupMembers)
                                        .FirstOrDefaultAsync(x => x.Id.Equals(id));
        }
    }
}
