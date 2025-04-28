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
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository() { }

        public UserRepository(SWP_KahootContext context) => _context = context;
        public async Task<User> GetUserAccount(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null; // or throw an exception, depending on your requirements

            var userAccount = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);

            return userAccount;
        }


        public async Task<User> GetUserByCurrentId(int userID)
        {
            if (userID <= 0) // Corrected null check and added check for non-positive ID
            {
                return null; // Or throw an ArgumentException("User ID cannot be null or zero.");
            }

            var userAccount = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userID); // Corrected the lambda expression

            return userAccount;
        }

        public async Task<bool> ExistsByNameAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == userName.ToLower());
        }
    }
    }
