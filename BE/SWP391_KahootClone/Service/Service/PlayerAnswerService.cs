using Microsoft.AspNetCore.SignalR;
using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using Service.SignalR.Server;
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
        private readonly QuizRepository _quizRepository; // Inject QuestionRepository
        private readonly IHubContext<KahootSignalR> _hubContext;
        private readonly GroupRepository _groupRepository;
        private readonly GroupMemberRepository _groupMemberRepository;

        public PlayerAnswerService(PlayerAnswerRepository playerAnswerRepository, IUnitOfWork unitOfWork, PlayerRepository playerRepository, 
                                   QuestionRepository questionRepository, QuizRepository quizRepository, IHubContext<KahootSignalR> hubContext,
                                   GroupRepository groupRepository, GroupMemberRepository groupMemberRepository)
        {
            _playerAnswerRepository = playerAnswerRepository ?? throw new ArgumentNullException(nameof(playerAnswerRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository)); // Initialize
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository)); // Initialize
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));
            _groupMemberRepository = groupMemberRepository ?? throw new ArgumentNullException(nameof(groupMemberRepository));
        }

        public async Task<ResponseDTO> CreatePlayerAnswerAsync(PlayerAnswerDTO playerAnswerDto)
        {
            try
            {
                // Validate PlayerId and QuestionId
                var playerExists = await _playerRepository.GetPlayerByIdAsync(playerAnswerDto.PlayerId);
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

                QuestionDTO question = MapToDTO(questionExists);
                var score = CalculateSoloScore(playerAnswerDto, question);
                playerExists.Score += score; // Update player score based on the answer

                if(playerExists.GroupMembers != null && playerExists.GroupMembers.Any())
                {
                    var groupMember = playerExists.GroupMembers.FirstOrDefault(x => x.PlayerId.Equals(playerExists.Id));
                    var group = groupMember.Group;

                    groupMember.TotalScore += score;
                    group.TotalPoint += score;

                    await _groupRepository.UpdateAsync(group);
                    await _groupMemberRepository.UpdateAsync(groupMember);
                }

                await _playerAnswerRepository.CreateAsync(playerAnswer);
                await _playerRepository.UpdateAsync(playerExists);
                await _unitOfWork.SaveChangesAsync();
                playerAnswerDto.Id = playerAnswer.Id;

                //check finished quiz
                var quiz = await _quizRepository.GetQuizById(question.QuizId);
                var notFinishedQuestions = quiz.Questions.Where(x => x.PlayerAnswers == null ||
                                                                    (x.PlayerAnswers != null && (!x.PlayerAnswers.Any() || 
                                                                                                  x.PlayerAnswers.DistinctBy(pa => pa.PlayerId).Count() < quiz.NumberOfJoinedPlayers)))
                                                         .ToList();

                if (!notFinishedQuestions.Any())
                {
                    //send signal to clients -> end quiz
                    await _hubContext.Clients.All.SendAsync("EndQuiz", quiz.QuizCode, true);
                }

                return new ResponseDTO(201, "Player answer created successfully", playerExists);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"Error creating player answer: {ex.Message}");
            }
        }
        public int CalculateSoloScore(PlayerAnswerDTO playerAnswer, QuestionDTO question)
        {
            // Validate input
            if (playerAnswer == null)
            {
                throw new ArgumentNullException(nameof(playerAnswer), "Player answer cannot be null.");
            }
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question), "Question cannot be null.");
            }

            // Initialize score
            int score = 0;

            // Check if the answer is correct
            if (playerAnswer.IsCorrect)
            {
                score = question.Score; // Award the question's score if correct.

                // Consider time limit and response time.
                if (question.TimeLimit.HasValue)
                {
                    // Calculate a bonus based on how quickly the player answered.
                    int timeBonus = CalculateTimeBonus(playerAnswer.ResponseTime, question.TimeLimit.Value);
                    score += timeBonus;
                }
            }

            return score;
        }


        private int CalculateTimeBonus(int responseTime, int timeLimit)
        {
            if (responseTime <= 0)
                return 0;

            if (responseTime >= timeLimit)
                return 0;

            // Linear bonus calculation (can be adjusted)
            // The faster the response, the higher the bonus.
            double timeRatio = (double)responseTime / timeLimit;
            int bonus = (int)((1 - timeRatio) * 20); // Example: Max bonus of 20, decreasing linearly.

            return Math.Max(0, bonus); // Ensure bonus is not negative.
        }


        public (Dictionary<int, int> playerScores, int totalGroupScore) CalculateGroupScore(
            List<GroupMemberDTO> groupMembers,
            List<PlayerAnswerDTO> playerAnswers,
            List<QuestionDTO> questions)
        {
            // Validate input
            if (groupMembers == null)
            {
                throw new ArgumentNullException(nameof(groupMembers), "Group members cannot be null.");
            }
            if (playerAnswers == null)
            {
                throw new ArgumentNullException(nameof(playerAnswers), "Player answers cannot be null.");
            }
            if (questions == null)
            {
                throw new ArgumentNullException(nameof(questions), "Questions cannot be null.");
            }

            // Initialize the dictionary to store each player's score.
            Dictionary<int, int> playerScores = new Dictionary<int, int>();
            int totalGroupScore = 0;

            // Populate the playerScores dictionary with initial scores of 0 for each member.
            foreach (var member in groupMembers)
            {
                playerScores[member.PlayerId] = 0;
            }

            // Iterate through each player's answer to calculate scores.
            foreach (var playerAnswer in playerAnswers)
            {
                // Find the corresponding question.
                QuestionDTO question = questions.FirstOrDefault(q => q.Id == playerAnswer.QuestionId);
                if (question != null)
                {
                    // Calculate the score for the player for this question.
                    int playerScore = CalculateSoloScore(playerAnswer, question); // Reuse the solo score calculation

                    // Add the score to the player's total.
                    playerScores[playerAnswer.PlayerId] += playerScore;
                    totalGroupScore += playerScore; // Accumulate the total group score.
                }
                // else:  Question not found.  This might indicate an error, but we'll just skip it.
            }
            return (playerScores, totalGroupScore);
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

        public static QuestionDTO MapToDTO(Question question)
        {
            if (question == null)
                return null;

            return new QuestionDTO
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Text = question.Text, // Changed from QuestionText to Text
                Type = question.Type,
                OptionA = question.OptionA,
                OptionB = question.OptionB,
                OptionC = question.OptionC,
                OptionD = question.OptionD,
                IsCorrect = question.IsCorrect,
                Score = question.Score,
                Flag = question.Flag,
                TimeLimit = question.TimeLimit,
                Arrange = question.Arrange
            };
        }

        public static Question MapToModel(QuestionDTO dto)
        {
            if (dto == null)
                return null;

            return new Question
            {
                Id = dto.Id,
                QuizId = dto.QuizId,
                Text = dto.Text, // Changed from QuestionText to Text
                Type = dto.Type,
                OptionA = dto.OptionA,
                OptionB = dto.OptionB,
                OptionC = dto.OptionC,
                OptionD = dto.OptionD,
                IsCorrect = dto.IsCorrect,
                Score = dto.Score,
                Flag = dto.Flag,
                TimeLimit = dto.TimeLimit,
                Arrange = dto.Arrange
            };
        }

    }
}

