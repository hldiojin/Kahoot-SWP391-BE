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
    public class QuizRepository : GenericRepository<Quiz>
    {
        public QuizRepository() { }

        public QuizRepository(SWP_KahootContext context) => _context = context;

        public async Task<IEnumerable<Quiz>> GetMySet(int userId)
        {
            return await _context.Quizzes
                .Where(q => q.CreatedBy == userId)
                .ToListAsync();
        }
            public async Task<Boolean> checkCode(int quizCode)
            {
            return await _context.Quizzes
                            .AnyAsync(q => q.QuizCode == quizCode);
        }

        public async Task<IEnumerable<Quiz>> GetFavoriteQUizs(int userId)
        {
            return await _context.Quizzes
                .Where(q => q.CreatedBy == userId && q.Favorite== true)
                .ToListAsync();
        }
        public async Task<Quiz> GetByQuizCode(int quizCode)
        {
            return await _context.Quizzes
                .FirstOrDefaultAsync(q => q.QuizCode == quizCode);
        }

        
    }
}
