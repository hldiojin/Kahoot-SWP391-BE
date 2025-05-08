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
    public class PlayerRepository : GenericRepository<Player>
    {
        public PlayerRepository() { }

        public PlayerRepository(SWP_KahootContext context) => _context = context;

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            return await _context.Players.Include(x => x.GroupMembers)
                                         .ThenInclude(x => x.Group)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<List<Player>> GetJoinedPlayersAsync(int quizId)
        {
            return await _context.Players.Include(x => x.GroupMembers)
                                         .ThenInclude(x => x.Group)
                                         .Where(x => x.QuizId.Equals(quizId)).ToListAsync();
        }
    }
}
