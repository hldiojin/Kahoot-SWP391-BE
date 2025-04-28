using Microsoft.AspNetCore.Mvc;
using Service.IService;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;
namespace API_Services.Controllers
{
  

    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
        }

        [HttpPost("{quizId}/status")]
        public async Task<IActionResult> ChangeQuizStatus(int quizId, [FromBody] QuizDTO request)
        {
            var response = await _quizService.ChangeQuizStatusAsync(request, quizId);
            return StatusCode(response.Status, response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDTO request)
        {
            var response = await _quizService.CreateQuizAsync(request);
            return StatusCode(response.Status, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListQuiz()
        {
            var response = await _quizService.GetListQuizAsync();
            return StatusCode(response.Status, response);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(int quizId)
        {
            var response = await _quizService.GetQuizById(quizId);
            return StatusCode(response.Status, response);
        }

        [HttpPut("{quizId}")]
        public async Task<IActionResult> UpdateQuiz(int quizId, [FromBody] QuizDTO request)
        {
            var response = await _quizService.UpdateQuizAsync(request, quizId);
            return StatusCode(response.Status, response);
        }
        [HttpGet("MySets/{userId}")]
        public async Task<IActionResult> MySets(int userId)
        {
            var response = await _quizService.GetByUserId(userId);
            return StatusCode(response.Status, response);
        }
        [HttpGet("check-quiz-code/{quizCode}")]
        public async Task<IActionResult> CheckQuizCode(int quizCode)
        {
            bool exists = await _quizService.checkQuizCode(quizCode);

            if (exists)
            {
                return Ok(new { message = "Quiz code is valid." });
            }
            else
            {
                return NotFound(new { message = "Quiz code not found." });
            }
        }
    }

}
