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
    public class PlayerAnswerService : IPlayerAnswerService
    {
        private readonly PlayerAnswerRepository _playerAnswerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlayerRepository _playerRepository; // Inject PlayerRepository
        private readonly QuestionRepository _questionRepository; // Inject QuestionRepository

        public PlayerAnswerService(PlayerAnswerRepository playerAnswerRepository, IUnitOfWork unitOfWork, PlayerRepository playerRepository, QuestionRepository questionRepository)
        {
            _playerAnswerRepository = playerAnswerRepository ?? throw new ArgumentNullException(nameof(playerAnswerRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository)); // Initialize
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository)); // Initialize
        }

        public async Task<ResponseDTO> CreatePlayerAnswerAsync(PlayerAnswerDTO playerAnswerDto)
        {
            // Validate PlayerId and QuestionId
            var playerExists = await _playerRepository.GetByIdAsync(playerAnswerDto.PlayerId);
            if (playerExists == null)
            {
                return new ResponseDTO(400, "Invalid PlayerId");
            }

            var questionExists = await _questionRepository.GetByIdAsync(playerAnswerDto.QuestionId);
            if (questionExists == null)
            {
                return new ResponseDTO(400, "Invalid QuestionId");
            }

            // Map DTO to Model
            var playerAnswer = new PlayerAnswer
            {
                PlayerId = playerAnswerDto.PlayerId,
                QuestionId = playerAnswerDto.QuestionId,
                AnsweredAt = playerAnswerDto.AnsweredAt,
                IsCorrect = playerAnswerDto.IsCorrect,
                ResponseTime = playerAnswerDto.ResponseTime,
                Answer = playerAnswerDto.Answer
            };

            try
            {
                await _playerAnswerRepository.CreateAsync(playerAnswer);
                await _unitOfWork.SaveChangesAsync();

                playerAnswerDto.Id = playerAnswer.Id; // Update DTO with generated ID
                return new ResponseDTO(201, "Player answer created successfully", playerAnswerDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error creating player answer: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetPlayerAnswerByIdAsync(int id)
        {
            var playerAnswer = await _playerAnswerRepository.GetByIdAsync(id);
            if (playerAnswer == null)
            {
                return new ResponseDTO(404, "Player answer not found");
            }

            // Map Model to DTO
            var playerAnswerDto = new PlayerAnswerDTO
            {
                Id = playerAnswer.Id,
                PlayerId = playerAnswer.PlayerId,
                QuestionId = playerAnswer.QuestionId,
                AnsweredAt = playerAnswer.AnsweredAt,
                IsCorrect = playerAnswer.IsCorrect,
                ResponseTime = playerAnswer.ResponseTime,
                Answer = playerAnswer.Answer
            };

            return new ResponseDTO(200, "Player answer retrieved successfully", playerAnswerDto);
        }

        public async Task<ResponseDTO> GetAllPlayerAnswersAsync()
        {
            var playerAnswers = await _playerAnswerRepository.GetAllAsync();
            if (playerAnswers == null)
            {
                return new ResponseDTO(200, "No player answers found", new List<PlayerAnswerDTO>()); // Changed from 404 to 200
            }

            // Map Models to DTOs
            var playerAnswerDtos = new List<PlayerAnswerDTO>();
            foreach (var playerAnswer in playerAnswers)
            {
                playerAnswerDtos.Add(new PlayerAnswerDTO
                {
                    Id = playerAnswer.Id,
                    PlayerId = playerAnswer.PlayerId,
                    QuestionId = playerAnswer.QuestionId,
                    AnsweredAt = playerAnswer.AnsweredAt,
                    IsCorrect = playerAnswer.IsCorrect,
                    ResponseTime = playerAnswer.ResponseTime,
                    Answer = playerAnswer.Answer
                });
            }
            return new ResponseDTO(200, "Player answers retrieved successfully", playerAnswerDtos);
        }

        public async Task<ResponseDTO> UpdatePlayerAnswerAsync(int id, PlayerAnswerDTO playerAnswerDto)
        {
            var existingPlayerAnswer = await _playerAnswerRepository.GetByIdAsync(id);
            if (existingPlayerAnswer == null)
            {
                return new ResponseDTO(404, "Player answer not found");
            }

            // Validate PlayerId and QuestionId
            var playerExists = await _playerRepository.GetByIdAsync(playerAnswerDto.PlayerId);
            if (playerExists == null)
            {
                return new ResponseDTO(400, "Invalid PlayerId");
            }

            var questionExists = await _questionRepository.GetByIdAsync(playerAnswerDto.QuestionId);
            if (questionExists == null)
            {
                return new ResponseDTO(400, "Invalid QuestionId");
            }

            // Update the properties
            existingPlayerAnswer.PlayerId = playerAnswerDto.PlayerId;
            existingPlayerAnswer.QuestionId = playerAnswerDto.QuestionId;
            existingPlayerAnswer.AnsweredAt = playerAnswerDto.AnsweredAt;
            existingPlayerAnswer.IsCorrect = playerAnswerDto.IsCorrect;
            existingPlayerAnswer.ResponseTime = playerAnswerDto.ResponseTime;
            existingPlayerAnswer.Answer = playerAnswerDto.Answer;

            try
            {
                await _playerAnswerRepository.UpdateAsync(existingPlayerAnswer);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(200, "Player answer updated successfully", playerAnswerDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error updating player answer: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> DeletePlayerAnswerAsync(int id)
        {
            var existingPlayerAnswer = await _playerAnswerRepository.GetByIdAsync(id);
            if (existingPlayerAnswer == null)
            {
                return new ResponseDTO(404, "Player answer not found");
            }

            try
            {
                await _playerAnswerRepository.RemoveAsync(existingPlayerAnswer);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseDTO(200, "Player answer deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error deleting player answer: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetPlayerAnswersByPlayerIdAsync(int playerId)
        {
            var playerAnswers = await _playerAnswerRepository.GetPlayerAnswersByPlayerIdAsync(playerId);
            if (playerAnswers == null || !playerAnswers.Any())
            {
                return new ResponseDTO(200, "No player answers found for this player.", new List<PlayerAnswerDTO>());
            }

            // Map PlayerAnswer entities to PlayerAnswerDTOs
            var playerAnswerDtos = playerAnswers.Select(pa => new PlayerAnswerDTO
            {
                Id = pa.Id,
                PlayerId = pa.PlayerId,
                QuestionId = pa.QuestionId,
                AnsweredAt = pa.AnsweredAt,
                IsCorrect = pa.IsCorrect,
                ResponseTime = pa.ResponseTime,
                Answer = pa.Answer
            }).ToList();

            return new ResponseDTO(200, "Player answers retrieved successfully.", playerAnswerDtos);
        }

        public async Task<ResponseDTO> GetPlayerAnswersByQuestionIdAsync(int questionId)
        {
            var playerAnswers = await _playerAnswerRepository.GetPlayerAnswersByQuestionIdAsync(questionId);
            if (playerAnswers == null || !playerAnswers.Any())
            {
                return new ResponseDTO(200, "No player answers found for this question.", new List<PlayerAnswerDTO>());
            }

            // Map PlayerAnswer entities to PlayerAnswerDTOs
            var playerAnswerDtos = playerAnswers.Select(pa => new PlayerAnswerDTO
            {
                Id = pa.Id,
                PlayerId = pa.PlayerId,
                QuestionId = pa.QuestionId,
                AnsweredAt = pa.AnsweredAt,
                IsCorrect = pa.IsCorrect,
                ResponseTime = pa.ResponseTime,
                Answer = pa.Answer
            }).ToList();

            return new ResponseDTO(200, "Player answers retrieved successfully.", playerAnswerDtos);
        }
    }
}

