using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class GameSessionService : IGameSessionService
    {
        private readonly GameSessionRepository _gameSessionRepository;
        public GameSessionService(GameSessionRepository gameSessionRepository)
        {
            _gameSessionRepository = gameSessionRepository;
        }

        public async Task<IEnumerable<GameSession>> GetAllGameSessionsAsync()
        {
            return await _gameSessionRepository.GetAllAsync();
        }

        public async Task<GameSession?> GetGameSessionByIdAsync(int id)
        {
            return await _gameSessionRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateGameSessionAsync(GameSession session)
        {
            await _gameSessionRepository.AddAsync(session);
            return await _gameSessionRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateGameSessionAsync(GameSession session)
        {
            _gameSessionRepository.Update(session);
            return await _gameSessionRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteGameSessionAsync(int id)
        {
            var session = await _gameSessionRepository.GetByIdAsync(id);
            if (session == null)
                return false;

            _gameSessionRepository.Remove(session);
            return await _gameSessionRepository.SaveChangesAsync();
        }
    }
}

