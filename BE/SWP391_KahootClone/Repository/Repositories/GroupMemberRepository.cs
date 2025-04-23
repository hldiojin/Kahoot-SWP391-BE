using Repository.Base;
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
    }
}
