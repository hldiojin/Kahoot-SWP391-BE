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
    public class UserServicePackRepository : GenericRepository<UserServicePack>
    {
        public UserServicePackRepository() { }

        public UserServicePackRepository(SWP_KahootContext context) => _context = context;
        public async Task<bool> CheckUserFreePack(int userID)
        {
            return await _context.UserServicePacks
                .AnyAsync(x => x.UserId == userID && x.ServicePackId == 1);
        }

        public async Task<UserServicePack> GetUserServicePackPremium(int userID)
        {
            return await _context.UserServicePacks
                .FirstOrDefaultAsync(x => x.UserId == userID && x.ServicePackId == 2);
        }
    }
}
