using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IGameSessionService
    {
        Task<IEnumerable<GameSession>> GetAllGameSessionsAsync();
        Task<GameSession?> GetGameSessionByIdAsync(int id);
        Task<bool> CreateGameSessionAsync(GameSession session);
        Task<bool> UpdateGameSessionAsync(GameSession session);
        Task<bool> DeleteGameSessionAsync(int id);
    }
}
