using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.Service
{
    public class PlayerService :  IPlayerService
    {
        private readonly PlayerRepository _playerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly GameSessionRepository _gameSessionRepository; // Inject GameSessionRepository

        public PlayerService(PlayerRepository playerRepository, IUnitOfWork unitOfWork, GameSessionRepository gameSessionRepository)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _gameSessionRepository = gameSessionRepository ?? throw new ArgumentNullException(nameof(gameSessionRepository)); // Initialize
        }



        public async Task<ResponseDTO> CreatePlayerAsync(PlayerDTO playerDto)
        {
            //  Validate SessionId
            if (playerDto.SessionId.HasValue)
            {
                var sessionExists = await _gameSessionRepository.GetByIdAsync(playerDto.SessionId.Value);
                if (sessionExists == null)
                {
                    return new ResponseDTO(400, "Invalid SessionId");
                }
            }

            // Map DTO to Model
            var player = new Player
            {
                UserId = playerDto.UserId,
                Nickname = playerDto.Nickname,
                PlayerCode = playerDto.PlayerCode,
                AvatarUrl = playerDto.AvatarUrl,
                Score = playerDto.Score,
                SessionId = playerDto.SessionId
            };

            try
            {
                await _playerRepository.CreateAsync(player);
                await _unitOfWork.SaveChangesAsync();

                playerDto.Id = player.Id; // Update DTO with generated ID
                return new ResponseDTO(201, "Player created successfully", playerDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error creating player: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetPlayerByIdAsync(int id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
            {
                return new ResponseDTO(404, "Player not found");
            }

            // Map Model to DTO
            var playerDto = new PlayerDTO
            {
                Id = player.Id,
                UserId = player.UserId,
                Nickname = player.Nickname,
                PlayerCode = player.PlayerCode,
                AvatarUrl = player.AvatarUrl,
                Score = player.Score,
                SessionId = player.SessionId
            };

            return new ResponseDTO(200, "Player retrieved successfully", playerDto);
        }

        public async Task<ResponseDTO> GetAllPlayersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            if (players == null)
            {
                return new ResponseDTO(200, "No players found", new List<PlayerDTO>()); //changed from 404 to 200
            }

            // Map Models to DTOs
            var playerDtos = new List<PlayerDTO>();
            foreach (var player in players)
            {
                playerDtos.Add(new PlayerDTO
                {
                    Id = player.Id,
                    UserId = player.UserId,
                    Nickname = player.Nickname,
                    PlayerCode = player.PlayerCode,
                    AvatarUrl = player.AvatarUrl,
                    Score = player.Score,
                    SessionId = player.SessionId
                });
            }
            return new ResponseDTO(200, "Players retrieved successfully", playerDtos);
        }

        public async Task<ResponseDTO> UpdatePlayerAsync(int id, PlayerDTO playerDto)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
            {
                return new ResponseDTO(404, "Player not found");
            }

            // Validate SessionId
            if (playerDto.SessionId.HasValue)
            {
                var sessionExists = await _gameSessionRepository.GetByIdAsync(playerDto.SessionId.Value);
                if (sessionExists == null)
                {
                    return new ResponseDTO(400, "Invalid SessionId");
                }
            }

            // Update the properties
            existingPlayer.UserId = playerDto.UserId;
            existingPlayer.Nickname = playerDto.Nickname;
            existingPlayer.PlayerCode = playerDto.PlayerCode;
            existingPlayer.AvatarUrl = playerDto.AvatarUrl;
            existingPlayer.Score = playerDto.Score;
            existingPlayer.SessionId = playerDto.SessionId;

            try
            {
                await _playerRepository.UpdateAsync(existingPlayer);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(200, "Player updated successfully", playerDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error updating player: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> DeletePlayerAsync(int id)
        {
            var existingPlayer = await _playerRepository.GetByIdAsync(id);
            if (existingPlayer == null)
            {
                return new ResponseDTO(404, "Player not found");
            }

            try
            {
                await _playerRepository.RemoveAsync(existingPlayer);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(200, "Player deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error deleting player: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetPlayersBySessionIdAsync(int sessionId)
        {
            var players = await _playerRepository.GetPlayersBySessionIdAsync(sessionId);
            if (players == null || !players.Any())
            {
                return new ResponseDTO(200, "No players found for this session.", new List<PlayerDTO>());
            }

            // Map Player entities to PlayerDTOs
            var playerDtos = players.Select(player => new PlayerDTO
            {
                Id = player.Id,
                UserId = player.UserId,
                Nickname = player.Nickname,
                PlayerCode = player.PlayerCode,
                AvatarUrl = player.AvatarUrl,
                Score = player.Score,
                SessionId = player.SessionId,
            }).ToList();

            return new ResponseDTO(200, "Players retrieved successfully.", playerDtos);
        }
    }
}
