using Azure.Core;
using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

public class QuizService : IQuizService
{
    private readonly QuizRepository _quizRepository; // Assuming you have a repository interface

    public QuizService(QuizRepository quizRepository)
    {
        _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
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
                GameMode = request.GameMode
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
}

