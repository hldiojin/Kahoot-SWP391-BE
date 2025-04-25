using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
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

            existingQuiz.IsPublic = request.IsPublic; // Assuming you only want to change the IsPublic status
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
                CreatedAt = existingQuiz.CreatedAt
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while updating quiz status: {ex.Message}");
        }
    }

    public async Task<ResponseDTO> CreateQuizAsync(QuizDTO request)
    {
        try
        {
            // Map QuizDTO to your data model (e.g., Quiz entity)
            var quiz = new Quiz // Assuming you have a Quiz entity
            {
                Title = request.Title,
                QuizCode = request.QuizCode,
                Description = request.Description,
                CreatedBy = request.CreatedBy,
                CategoryId = request.CategoryId,
                IsPublic = request.IsPublic,
                ThumbnailUrl = request.ThumbnailUrl,
                CreatedAt = DateTime.UtcNow // Set creation timestamp
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
                CreatedAt = quiz.CreatedAt
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while creating quiz: {ex.Message}");
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
                CreatedAt = q.CreatedAt
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
                CreatedAt = quiz.CreatedAt
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

            // Update properties if they are provided in the request
            existingQuiz.Title = request.Title ?? existingQuiz.Title;
            existingQuiz.QuizCode = request.QuizCode;
            existingQuiz.Description = request.Description ?? existingQuiz.Description;
            existingQuiz.CategoryId = request.CategoryId;
            existingQuiz.IsPublic = request.IsPublic;
            existingQuiz.ThumbnailUrl = request.ThumbnailUrl ?? existingQuiz.ThumbnailUrl;

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
                CreatedAt = existingQuiz.CreatedAt
            });
        }
        catch (Exception ex)
        {
            // Log the exception
            return new ResponseDTO(500, $"An error occurred while updating quiz: {ex.Message}");
        }
    }
}

