using Microsoft.EntityFrameworkCore;
using Repository.Base;
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

        public async Task<IEnumerable<Player>> GetPlayersBySessionIdAsync(int sessionId)
        {
            return await _context.Players
                .Where(q => q.SessionId == sessionId)
                .ToListAsync();
        }
    }
}
