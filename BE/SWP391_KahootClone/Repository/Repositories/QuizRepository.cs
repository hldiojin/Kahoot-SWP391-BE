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
        public async Task<int> CheckCode(int quizCode)
        {
            var quiz = await _context.Quizzes
                                     .FirstOrDefaultAsync(q => q.QuizCode == quizCode);

            return quiz != null ? quiz.Id : 0; // returns quizId or 0 if not found
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

        public async Task<Quiz> GetQuizById(int id)
        {
            return await _context.Quizzes.Include(x => x.Questions)
                                         .ThenInclude(x => x.PlayerAnswers).ThenInclude(x => x.Player).ThenInclude(x => x.GroupMembers).ThenInclude(x => x.Group)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(x => x.Id.Equals(id));

        }
        public async Task<Quiz> GetQuiz(int id)
        {
            return await _context.Quizzes.Include(x => x.Questions)
                                         .ThenInclude(x => x.PlayerAnswers)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(x => x.Id.Equals(id));

        }




    }
}
