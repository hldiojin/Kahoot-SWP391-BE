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
        // Inject GameSessionRepository

        public PlayerService(PlayerRepository playerRepository, IUnitOfWork unitOfWork)
        {
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            
        }



        public async Task<ResponseDTO> CreatePlayerAsync(PlayerDTO playerDto)
        {
            //  Validate SessionId
            

            // Map DTO to Model
            var player = new Player
            {
                
                Nickname = playerDto.Nickname,
                AvatarUrl = playerDto.AvatarUrl,
                Score = playerDto.Score,    
            };

            try
            {
                await _playerRepository.CreateAsync(player);
                await _unitOfWork.SaveChangesAsync();

                playerDto.playerId = player.Id; // Update DTO with generated ID
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
                playerId = player.Id,
               
                Nickname = player.Nickname,
       
                AvatarUrl = player.AvatarUrl,
                Score = player.Score,
               
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
                    playerId = player.Id,
                  
                    Nickname = player.Nickname,
                   
                    AvatarUrl = player.AvatarUrl,
                    Score = player.Score,
                    
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

           
            

            // Update the properties
            
            existingPlayer.Nickname = playerDto.Nickname;
           
            existingPlayer.AvatarUrl = playerDto.AvatarUrl;
            existingPlayer.Score = playerDto.Score;
            

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

     
    }
}
