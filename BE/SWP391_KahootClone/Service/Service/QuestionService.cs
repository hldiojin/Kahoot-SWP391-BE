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
    public class QuestionService : IQuestionService
    {
        private readonly QuestionRepository _questionRepository;
        private readonly QuizRepository _quizRepository; // Inject QuizRepository if you need to validate QuizId
        private readonly IUnitOfWork _unitOfWork;

        // Constructor
        public QuestionService(QuestionRepository questionRepository, QuizRepository quizRepository, IUnitOfWork unitOfWork)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // Create a new question
        public async Task<ResponseDTO> CreateQuestionAsync(QuestionDTO questionDto)
        {
            // Validate QuizId
            var quizExists = await _quizRepository.GetByIdAsync(questionDto.QuizId); // Use QuizRepository
            if (quizExists == null)
            {
                return new ResponseDTO(400, "QuizId is invalid.");
            }

            // Map QuestionDTO to Question entity
            var question = new Question
            {
                QuizId = questionDto.QuizId,
                Text = questionDto.Text,
                Type = questionDto.Type,
                OptionA = questionDto.OptionA,
                OptionB = questionDto.OptionB,
                OptionC = questionDto.OptionC,
                OptionD = questionDto.OptionD,
                IsCorrect = questionDto.IsCorrect,
                Score = questionDto.Score,
                Flag = questionDto.Flag,
                TimeLimit = questionDto.TimeLimit,
                Arrange = questionDto.Arrange
            };

            try
            {
                await _questionRepository.CreateAsync(question);
                await _unitOfWork.SaveChangesAsync(); // Use UnitOfWork
                questionDto.Id = question.Id; // set the id back to the dto
                return new ResponseDTO(201, "Question created successfully.", questionDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"An error occurred while creating the question: {ex.Message}");
            }
        }

        // Get a question by its ID
        public async Task<ResponseDTO> GetQuestionByIdAsync(int questionId)
        {
            var question = await _questionRepository.GetByIdAsync(questionId);
            if (question == null)
            {
                return new ResponseDTO(404, "Question not found.");
            }

            // Map Question entity to QuestionDTO
            var questionDto = new QuestionDTO
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Text = question.Text,
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

            return new ResponseDTO(200, "Question retrieved successfully.", questionDto);
        }

        // Update an existing question
        public async Task<ResponseDTO> UpdateQuestionAsync(int questionId, QuestionDTO questionDto)
        {
            var existingQuestion = await _questionRepository.GetByIdAsync(questionId);
            if (existingQuestion == null)
            {
                return new ResponseDTO(404, "Question not found.");
            }

            // Validate QuizId
            var quizExists = await _quizRepository.GetByIdAsync(questionDto.QuizId); // Use QuizRepository
            if (quizExists == null)
            {
                return new ResponseDTO(400, "QuizId is invalid.");
            }

            // Update the properties of the existing question
            existingQuestion.QuizId = questionDto.QuizId;
            existingQuestion.Text = questionDto.Text;
            existingQuestion.Type = questionDto.Type;
            existingQuestion.OptionA = questionDto.OptionA;
            existingQuestion.OptionB = questionDto.OptionB;
            existingQuestion.OptionC = questionDto.OptionC;
            existingQuestion.OptionD = questionDto.OptionD;
            existingQuestion.IsCorrect = questionDto.IsCorrect;
            existingQuestion.Score = questionDto.Score;
            existingQuestion.Flag = questionDto.Flag;
            existingQuestion.TimeLimit = questionDto.TimeLimit;
            existingQuestion.Arrange = questionDto.Arrange;

            try
            {
                await _questionRepository.UpdateAsync(existingQuestion);
                await _unitOfWork.SaveChangesAsync(); // Use UnitOfWork
                return new ResponseDTO(200, "Question updated successfully.", questionDto);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"An error occurred while updating the question: {ex.Message}");
            }
        }

        // Delete a question
        public async Task<ResponseDTO> DeleteQuestionAsync(int questionId)
        {
            var existingQuestion = await _questionRepository.GetByIdAsync(questionId);
            if (existingQuestion == null)
            {
                return new ResponseDTO(404, "Question not found.");
            }

            try
            {
                await _questionRepository.RemoveAsync(existingQuestion);
                await _unitOfWork.SaveChangesAsync(); // Use UnitOfWork
                return new ResponseDTO(200, "Question deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(500, $"An error occurred while deleting the question: {ex.Message}");
            }
        }

        public async Task<ResponseDTO> GetQuestionsByQuizIdAsync(int quizId)
        {
            var questions = await _questionRepository.GetByQuizIdAsync(quizId);
            if (questions == null || !questions.Any())
            {
                return new ResponseDTO(200, "No questions found for this quiz.", new List<QuestionDTO>());
            }

            // Map Question entity to QuestionDTO
            var questionDtos = questions.Select(question => new QuestionDTO
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Text = question.Text,
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
            }).ToList();

            return new ResponseDTO(200, "Questions retrieved successfully.", questionDtos);
        }
        public async Task<ResponseDTO> GetAllQuestionsAsync()
        {
            var questions = await _questionRepository.GetAllAsync();
            if (questions == null || !questions.Any())
            {
                return new ResponseDTO(200, "No questions found.", new List<QuestionDTO>());
            }

            var questionDtos = questions.Select(question => new QuestionDTO
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Text = question.Text,
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
            }).ToList();
            return new ResponseDTO(200, "All questions retrieved successfully.", questionDtos);
        }
    }
}
