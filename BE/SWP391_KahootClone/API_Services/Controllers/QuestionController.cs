using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

// Make sure you have the correct namespace for your services and DTOs
using Service; // For IQuestionService
using Repository.DTO; // For QuestionDTO
using Service.IService;
using static Repository.DTO.RequestDTO;
namespace API_Services.Controllers
{
   

    [ApiController]
    [Route("api/questions")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionDTO questionDto)
        {
            var response = await _questionService.CreateQuestionAsync(questionDto);
            return StatusCode(response.Status, response);
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            var response = await _questionService.GetQuestionByIdAsync(questionId);
            return StatusCode(response.Status, response);
        }

        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] QuestionDTO questionDto)
        {
            var response = await _questionService.UpdateQuestionAsync(questionId, questionDto);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var response = await _questionService.DeleteQuestionAsync(questionId);
            return StatusCode(response.Status, response);
        }

        [HttpGet("quiz/{quizId}")] // Corrected route for getting questions by QuizId
        public async Task<IActionResult> GetQuestionsByQuizId(int quizId)
        {
            var response = await _questionService.GetQuestionsByQuizIdAsync(quizId);
            return StatusCode(response.Status, response);
        }
    }


}
