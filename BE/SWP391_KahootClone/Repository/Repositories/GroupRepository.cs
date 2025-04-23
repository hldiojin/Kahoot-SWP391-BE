using Repository.Base;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class GroupRepository    : GenericRepository<Models.Group>
    {
        public GroupRepository() { }

        public GroupRepository(SWP_KahootContext context) => _context = context;
    }
}
