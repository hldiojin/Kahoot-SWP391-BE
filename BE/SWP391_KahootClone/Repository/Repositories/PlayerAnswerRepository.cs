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
    public class PlayerAnswerRepository : GenericRepository<PlayerAnswer>
    {
        public PlayerAnswerRepository() { }

        public PlayerAnswerRepository(SWP_KahootContext context) => _context = context;

        public async Task<IEnumerable<PlayerAnswer>> GetPlayerAnswersByPlayerIdAsync(int playerId)
        {
            return await _context.PlayerAnswers
                .Where(q => q.PlayerId == playerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PlayerAnswer>> GetPlayerAnswersByQuestionIdAsync(int questionId)
        {
            return await _context.PlayerAnswers
                .Where(q => q.QuestionId == questionId)
                .ToListAsync();
        }

    }
}
