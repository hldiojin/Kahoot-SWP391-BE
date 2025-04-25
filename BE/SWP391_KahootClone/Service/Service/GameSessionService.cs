using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.Service
{
    public class GameSessionService : IGameSessionService
    {
        private readonly GameSessionRepository _gameSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
       

        public GameSessionService(GameSessionRepository gameSessionRepository, IUnitOfWork unitOfWork)
        {
            _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
          
        }

        public async Task<ResponseDTO> CreateGameSessionAsync(GameSessionDTO gameSessionDto)
        {
          
            
            // Map DTO to Model
            var gameSession = new GameSession
            {
                QuizId = gameSessionDto.QuizId,
                HostId = gameSessionDto.HostId,
                PinCode = gameSessionDto.PinCode,
                GameType = gameSessionDto.GameType,
                Status = gameSessionDto.Status,
                MinPlayer = gameSessionDto.MinPlayer,
                MaxPlayer = gameSessionDto.MaxPlayer,
                StartedAt = gameSessionDto.StartedAt,
                EndedAt = gameSessionDto.EndedAt
            };

            try
            {
                await _gameSessionRepository.CreateAsync(gameSession);
                await _unitOfWork.SaveChangesAsync();

                gameSessionDto.Id = gameSession.Id; // Update DTO with generated ID
                return new ResponseDTO(201, "Game session created successfully", gameSessionDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error creating game session: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetGameSessionByIdAsync(int id)
        {
            var gameSession = await _gameSessionRepository.GetByIdAsync(id);
            if (gameSession == null)
            {
                return new ResponseDTO(404, "Game session not found");
            }

            // Map Model to DTO
            var gameSessionDto = new GameSessionDTO
            {
                Id = gameSession.Id,
                QuizId = gameSession.QuizId,
                HostId = gameSession.HostId,
                PinCode = GenerateSessionPinCode(),
                GameType = gameSession.GameType,
                Status = gameSession.Status,
                MinPlayer = gameSession.MinPlayer,
                MaxPlayer = gameSession.MaxPlayer,
                StartedAt = gameSession.StartedAt,
                EndedAt = gameSession.EndedAt
            };

            return new ResponseDTO(200, "Game session retrieved successfully", gameSessionDto);
        }
        public  string GenerateSessionPinCode()
        {
            // Use a cryptographically secure random number generator
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[3]; // 3 bytes will give us a number up to 2^24 - 1
                rng.GetBytes(bytes);

                // Convert the byte array to an integer
                int randomNumber = BitConverter.ToInt32(new byte[] { bytes[0], bytes[1], bytes[2], 0 }, 0);

                // Ensure the number is within the 6-digit range (000000 - 999999)
                int pin = Math.Abs(randomNumber % 1000000);

                // Format the integer as a 6-digit string with leading zeros if necessary
                return pin.ToString("D6");
            }
        }
            public async Task<ResponseDTO> GetAllGameSessionsAsync()
        {
            var gameSessions = await _gameSessionRepository.GetAllAsync();
            if (gameSessions == null)
            {
                return new ResponseDTO(200, "No game sessions found", new List<GameSessionDTO>());
            }

            // Map Models to DTOs
            var gameSessionDtos = new List<GameSessionDTO>();
            foreach (var gameSession in gameSessions)
            {
                gameSessionDtos.Add(new GameSessionDTO
                {
                    Id = gameSession.Id,
                    QuizId = gameSession.QuizId,
                    HostId = gameSession.HostId,
                    PinCode = gameSession.PinCode,
                    GameType = gameSession.GameType,
                    Status = gameSession.Status,
                    MinPlayer = gameSession.MinPlayer,
                    MaxPlayer = gameSession.MaxPlayer,
                    StartedAt = gameSession.StartedAt,
                    EndedAt = gameSession.EndedAt
                });
            }
            return new ResponseDTO(200, "Game sessions retrieved successfully", gameSessionDtos);
        }

        public async Task<ResponseDTO> UpdateGameSessionAsync(int id, GameSessionDTO gameSessionDto)
        {
            var existingGameSession = await _gameSessionRepository.GetByIdAsync(id);
            if (existingGameSession == null)
            {
                return new ResponseDTO(404, "Game session not found");
            }

            // Update the properties
            existingGameSession.QuizId = gameSessionDto.QuizId;
            existingGameSession.HostId = gameSessionDto.HostId;
            existingGameSession.PinCode = gameSessionDto.PinCode;
            existingGameSession.GameType = gameSessionDto.GameType;
            existingGameSession.Status = gameSessionDto.Status;
            existingGameSession.MinPlayer = gameSessionDto.MinPlayer;
            existingGameSession.MaxPlayer = gameSessionDto.MaxPlayer;
            existingGameSession.StartedAt = gameSessionDto.StartedAt;
            existingGameSession.EndedAt = gameSessionDto.EndedAt;

            try
            {
                await _gameSessionRepository.UpdateAsync(existingGameSession);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(200, "Game session updated successfully", gameSessionDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error updating game session: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> DeleteGameSessionAsync(int id)
        {
            var existingGameSession = await _gameSessionRepository.GetByIdAsync(id);
            if (existingGameSession == null)
            {
                return new ResponseDTO(404, "Game session not found");
            }

            try
            {
                await _gameSessionRepository.RemoveAsync(existingGameSession);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(200, "Game session deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error deleting game session: {ex.Message}");
            }
        }
        // Implement other methods as needed (e.g., StartGame, EndGame)
    }
}
