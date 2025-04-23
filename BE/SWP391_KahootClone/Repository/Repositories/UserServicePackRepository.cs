using Repository.Base;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class UserServicePackRepository : GenericRepository<UserServicePack>
    {
        public UserServicePackRepository() { }

        public UserServicePackRepository(SWP_KahootContext context) => _context = context;
    }
}
