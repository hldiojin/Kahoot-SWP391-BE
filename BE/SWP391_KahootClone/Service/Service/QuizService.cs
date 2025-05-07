using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using Service.SignalR.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;
using PlayerDTO = Repository.DTO.PlayerDTO;

public class QuizService : IQuizService
{
    private readonly QuizRepository _quizRepository; // Assuming you have a repository interface
    private readonly PlayerRepository _playerRepository;
    private readonly IHubContext<KahootSignalR> _hubContext;
    private readonly ICommonService _commonService;

    public QuizService(QuizRepository quizRepository, PlayerRepository playerRepository, IHubContext<KahootSignalR> hubContext, ICommonService commonService)
    {
        _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
        _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
    }

    public async Task<ResponseDTO> ChangeQuizStatusAsync(QuizDTO request, int quizId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetByIdAsync(quizId);
            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            existingQuiz.IsPublic = request.IsPublic;
            await _quizRepository.UpdateAsync(existingQuiz);

            return new ResponseDTO(200, "Quiz status updated successfully.", new QuizResponseDTO
            {
                Id = existingQuiz.Id,
                Title = existingQuiz.Title,
                QuizCode = existingQuiz.QuizCode,
                Description = existingQuiz.Description,
                CreatedBy = existingQuiz.CreatedBy,
                CategoryId = existingQuiz.CategoryId,
                IsPublic = existingQuiz.IsPublic,
                ThumbnailUrl = existingQuiz.ThumbnailUrl,
                CreatedAt = existingQuiz.CreatedAt,
                MaxPlayer = existingQuiz.MaxPlayer, // Include new properties
                MinPlayer = existingQuiz.MinPlayer,
                Favorite = existingQuiz.Favorite,
                GameMode = existingQuiz.GameMode
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while updating quiz status: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> CreateQuizAsync(CreateQuizDTO request)
    {
        try
        {
            var quiz = new Quiz
            {
                Title = request.Title,
                QuizCode = GenerateSessionQuizCode(),
                Description = request.Description,
                CreatedBy = request.CreatedBy,
                CategoryId = request.CategoryId,
                IsPublic = true,
                ThumbnailUrl = request.ThumbnailUrl,
                CreatedAt = DateTime.UtcNow,
                MaxPlayer = request.MaxPlayer, // Include new properties from request
                MinPlayer = request.MinPlayer,
                Favorite = request.Favorite,
                GameMode = request.GameMode,
                NumberOfJoinedPlayers = 0
            };

            await _quizRepository.CreateAsync(quiz);

            return new ResponseDTO(201, "Quiz created successfully.", new QuizResponseDTO
            {
                Id = quiz.Id,
                Title = quiz.Title,
                QuizCode = quiz.QuizCode,
                Description = quiz.Description,
                CreatedBy = quiz.CreatedBy,
                CategoryId = quiz.CategoryId,
                IsPublic = quiz.IsPublic,
                ThumbnailUrl = quiz.ThumbnailUrl,
                CreatedAt = quiz.CreatedAt,
                MaxPlayer = quiz.MaxPlayer, // Include new properties from created entity
                MinPlayer = quiz.MinPlayer,
                Favorite = quiz.Favorite,
                GameMode = quiz.GameMode
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while creating quiz: {ex.Message}");
        }
    }
    public int GenerateSessionQuizCode()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[4]; // 4 bytes for a full Int32
            rng.GetBytes(bytes);
            int randomNumber = Math.Abs(BitConverter.ToInt32(bytes, 0));
            int pin = randomNumber % 1000000; // ensures it's between 0 and 999999
            return pin;
        }
    }

    public async Task<ResponseDTO> GetListQuizAsync()
    {
        try
        {
            var quizzes = await _quizRepository.GetAllAsync();
            var quizResponses = quizzes.Select(q => new QuizResponseDTO
            {
                Id = q.Id,
                Title = q.Title,
                QuizCode = q.QuizCode,
                Description = q.Description,
                CreatedBy = q.CreatedBy,
                CategoryId = q.CategoryId,
                IsPublic = q.IsPublic,
                ThumbnailUrl = q.ThumbnailUrl,
                CreatedAt = q.CreatedAt,
                MaxPlayer = q.MaxPlayer,  
                MinPlayer = q.MinPlayer,
                Favorite = q.Favorite,
                GameMode = q.GameMode
            }).ToList();

            return new ResponseDTO(200, "Quizzes retrieved successfully.", quizResponses);
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while retrieving quizzes: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> GetQuizById(int quizId)
    {
        try
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            return new ResponseDTO(200, "Quiz retrieved successfully.", new QuizResponseDTO
            {
                Id = quiz.Id,
                Title = quiz.Title,
                QuizCode = quiz.QuizCode,
                Description = quiz.Description,
                CreatedBy = quiz.CreatedBy,
                CategoryId = quiz.CategoryId,
                IsPublic = quiz.IsPublic,
                ThumbnailUrl = quiz.ThumbnailUrl,
                CreatedAt = quiz.CreatedAt,
                MaxPlayer = quiz.MaxPlayer, // Include new properties
                MinPlayer = quiz.MinPlayer,
                Favorite = quiz.Favorite,
                GameMode = quiz.GameMode
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while retrieving quiz: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> UpdateQuizAsync(QuizDTO request, int quizId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetByIdAsync(quizId);
            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            existingQuiz.Title = request.Title ?? existingQuiz.Title;
            existingQuiz.QuizCode = request.QuizCode;
            existingQuiz.Description = request.Description ?? existingQuiz.Description;
            existingQuiz.CategoryId = request.CategoryId;
            existingQuiz.IsPublic = request.IsPublic;
            existingQuiz.ThumbnailUrl = request.ThumbnailUrl ?? existingQuiz.ThumbnailUrl;
            existingQuiz.MaxPlayer = request.MaxPlayer ?? existingQuiz.MaxPlayer; // Update new properties
            existingQuiz.MinPlayer = request.MinPlayer ?? existingQuiz.MinPlayer;
            existingQuiz.Favorite = request.Favorite ?? existingQuiz.Favorite;
            existingQuiz.GameMode = request.GameMode ?? existingQuiz.GameMode;

            await _quizRepository.UpdateAsync(existingQuiz);

            return new ResponseDTO(200, "Quiz updated successfully.", new QuizResponseDTO
            {
                Id = existingQuiz.Id,
                Title = existingQuiz.Title,
                QuizCode = existingQuiz.QuizCode,
                Description = existingQuiz.Description,
                CreatedBy = existingQuiz.CreatedBy,
                CategoryId = existingQuiz.CategoryId,
                IsPublic = existingQuiz.IsPublic,
                ThumbnailUrl = existingQuiz.ThumbnailUrl,
                CreatedAt = existingQuiz.CreatedAt,
                MaxPlayer = existingQuiz.MaxPlayer, // Include updated properties in response
                MinPlayer = existingQuiz.MinPlayer,
                Favorite = existingQuiz.Favorite,
                GameMode = existingQuiz.GameMode
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while updating quiz: {ex.Message}");
        }
    }
    public async Task<ResponseDTO> GetByUserId(int userId)
    {
        try
        {
            var quizzes = await _quizRepository.GetMySet(userId);
            var quizResponses = quizzes.Select(q => new QuizResponseDTO
            {
                Id = q.Id,
                Title = q.Title,
                QuizCode = q.QuizCode,
                Description = q.Description,
                CreatedBy = q.CreatedBy,
                CategoryId = q.CategoryId,
                IsPublic = q.IsPublic,
                ThumbnailUrl = q.ThumbnailUrl,
                CreatedAt = q.CreatedAt,
                MaxPlayer = q.MaxPlayer, // Include new properties
                MinPlayer = q.MinPlayer,
                Favorite = q.Favorite,
                GameMode = q.GameMode
            }).ToList();

            return new ResponseDTO(200, "Quizzes retrieved successfully.", quizResponses);
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while retrieving quizzes: {ex.Message}");
        }
    }

    public async Task<bool> checkQuizCode(int quizCode)
    {
        bool result = await _quizRepository.checkCode(quizCode);
        return result;
    }
    public async Task<ResponseDTO> GetFavorite(int userId)
    {
        try
        {
            var quizzes = await _quizRepository.GetFavoriteQUizs(userId);
            var quizResponses = quizzes.Select(q => new QuizResponseDTO
            {
                Id = q.Id,
                Title = q.Title,
                QuizCode = q.QuizCode,
                Description = q.Description,
                CreatedBy = q.CreatedBy,
                CategoryId = q.CategoryId,
                IsPublic = q.IsPublic,
                ThumbnailUrl = q.ThumbnailUrl,
                CreatedAt = q.CreatedAt,
                MaxPlayer = q.MaxPlayer, // Include new properties
                MinPlayer = q.MinPlayer,
                Favorite = q.Favorite,
                GameMode = q.GameMode
            }).ToList();

            return new ResponseDTO(200, "Quizzes retrieved successfully.", quizResponses);
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while retrieving quizzes: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> GetQuizByQuizCode(int quizCode)
    {
        try
        {
            var quiz = await _quizRepository.GetByQuizCode(quizCode);
            if (quiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            return new ResponseDTO(200, "Quiz retrieved successfully.", new QuizResponseDTO
            {
                Id = quiz.Id,
                Title = quiz.Title,
                QuizCode = quiz.QuizCode,
                Description = quiz.Description,
                CreatedBy = quiz.CreatedBy,
                CategoryId = quiz.CategoryId,
                IsPublic = quiz.IsPublic,
                ThumbnailUrl = quiz.ThumbnailUrl,
                CreatedAt = quiz.CreatedAt,
                MaxPlayer = quiz.MaxPlayer,
                MinPlayer = quiz.MinPlayer,
                Favorite = quiz.Favorite,
                GameMode = quiz.GameMode
            }
            );
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while retrieving quiz: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> DeleteQuizAsync(int quizId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetByIdAsync(quizId);
            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            await _quizRepository.RemoveAsync(existingQuiz); // Assuming RemoveAsync is the correct method for deletion
            return new ResponseDTO(200, "Quiz deleted successfully.", existingQuiz); // You might not want to return the deleted entity
        }
        catch (Exception ex)
        {
            // Log the exception properly here (e.g., using ILogger)
            return new ResponseDTO(500, $"An error occurred while deleting the quiz: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> FavoriteQuiz(int quizId, int userId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetByIdAsync(quizId);
            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

           
            existingQuiz.Favorite = true;
           

            await _quizRepository.UpdateAsync(existingQuiz);

            return new ResponseDTO(200, "Quiz updated successfully.", new QuizResponseDTO
            {
                Id = existingQuiz.Id,
                Title = existingQuiz.Title,
                QuizCode = existingQuiz.QuizCode,
                Description = existingQuiz.Description,
                CreatedBy = existingQuiz.CreatedBy,
                CategoryId = existingQuiz.CategoryId,
                IsPublic = existingQuiz.IsPublic,
                ThumbnailUrl = existingQuiz.ThumbnailUrl,
                CreatedAt = existingQuiz.CreatedAt,
                MaxPlayer = existingQuiz.MaxPlayer, // Include updated properties in response
                MinPlayer = existingQuiz.MinPlayer,
                Favorite = existingQuiz.Favorite,
                GameMode = existingQuiz.GameMode
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while updating quiz: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> JoinQuizAsync(int quizId, int playerId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetByIdAsync(quizId);

            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(playerId);

            if (existingPlayer == null)
            {
                return new ResponseDTO(404, "Player not found");
            }

            await _quizRepository.UpdateAsync(existingQuiz);

            // increase number of joined players of quiz
            existingQuiz.NumberOfJoinedPlayers += 1;

            var playerDTO = new PlayerDTO(playerId, existingPlayer.Nickname, existingPlayer.AvatarUrl, null, null, null);

            if (existingQuiz.GameMode.ToUpper().Equals("TEAM"))
            {
                playerDTO.GroupId = existingPlayer.GroupMembers.First().GroupId;
                playerDTO.GroupName = existingPlayer.GroupMembers.First().Group.Name;
                playerDTO.GroupDescription = existingPlayer.GroupMembers.First().Group.Description;
            }

            //send signalR to clients join to quiz
            await _hubContext.Clients.All.SendAsync("JoinToQuiz", existingQuiz.QuizCode, playerDTO);

            await _quizRepository.UpdateAsync(existingQuiz);

            return new ResponseDTO(200, "Joined Quiz Successfully.", playerDTO);

        } catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while joining quiz: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> StartQuizAsync(int quizId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetByIdAsync(quizId);

            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            //send signalR to clients join to quiz
            await _hubContext.Clients.All.SendAsync("StartQuiz", existingQuiz.QuizCode, true);

            return new ResponseDTO(200, "Start Quiz Successfully.");
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while starting quiz: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> GetResultQuizAsync(int quizId, int playerId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetQuizById(quizId);

            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            var existingPlayer = await _playerRepository.GetPlayerByIdAsync(playerId);

            if (existingPlayer == null)
            {
                return new ResponseDTO(404, "Player not found");
            }

            var playerResult = new PlayerResultDetailDTO(existingPlayer, existingQuiz.Questions.ToList());

            return new ResponseDTO(200, "Get quiz result successfully.", playerResult);
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while getting quiz result: {ex.Message}");
        }
    }
    
    public async Task<ResponseDTO> GetResultQuizAsync(int quizId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetQuizById(quizId);

            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            var userId = _commonService.GetRequestUser();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new ResponseDTO(401, $"Unauthorize.");
            }

            var players = existingQuiz.Questions.SelectMany(x => x.PlayerAnswers)
                                                .Select(x => x.Player)
                                                .DistinctBy(x => x.Id)
                                                .ToList();

            if (existingQuiz.GameMode.ToUpper().Equals("SOLO"))
            {
                players = players.OrderByDescending(x => x.Score).ToList();
                var topPlayers = players.Take(3).ToList();
                var normalPlayers = players.Skip(3).Take(players.Count()).ToList();

                return new ResponseDTO(200, "Get quiz result successfully.", new ResultQuizForSoloModeDTO(topPlayers, normalPlayers));
            }

            var groups = players.SelectMany(x => x.GroupMembers)
                                .Select(x => x.Group)
                                .DistinctBy(x => x.Id)
                                .OrderByDescending(x => x.TotalPoint);
            var topGroups = groups.Take(3).ToList();
            var normalGroups = groups.Skip(3).Take(groups.Count()).ToList();


            return new ResponseDTO(200, "Get quiz result successfully.", new ResultQuizForTeamModeDTO(topGroups, normalGroups, players));
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while getting quiz result: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> GetJoinedPlayersAsync(int quizId)
    {
        try
        {
            var existingQuiz = await _quizRepository.GetQuizById(quizId);

            if (existingQuiz == null)
            {
                return new ResponseDTO(404, "Quiz not found.");
            }

            var players = await _playerRepository.GetJoinedPlayersAsync(quizId);

            var playerDTOs = players.Select(x => new PlayerDTO(x.Id, x.Nickname, x.AvatarUrl, null, null, null)).ToList();

            if (existingQuiz.GameMode.ToUpper().Equals("TEAM"))
            {
                foreach (var playerDTO in playerDTOs)
                {
                    var existingPlayer = players.FirstOrDefault(x => x.Id.Equals(playerDTO.Id));
                    playerDTO.GroupId = existingPlayer.GroupMembers.First().GroupId;
                    playerDTO.GroupName = existingPlayer.GroupMembers.First().Group.Name;
                    playerDTO.GroupDescription = existingPlayer.GroupMembers.First().Group.Description;
                }
            }

            return new ResponseDTO(200, "Get quiz result successfully.", new JoinedPlayerDTO(playerDTOs));
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while getting joined players: {ex.Message}");
        }
    }
}

