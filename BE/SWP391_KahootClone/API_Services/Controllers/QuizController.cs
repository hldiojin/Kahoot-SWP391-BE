using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.DTO;
using Repository.Models;
using Service.IService;
using System.Security.Claims;
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
        [HttpGet("Favorite/{userId}")]
        public async Task<IActionResult> Favorite(int userId)
        {
            var response = await _quizService.GetFavorite(userId);
            return StatusCode(response.Status, response);
        }
        [HttpGet("QuizCode/{quizCode}")]
        public async Task<IActionResult> GetQuizByQuizCode(int quizCode)
        {
            var response = await _quizService.GetQuizByQuizCode(quizCode);
            return StatusCode(response.Status, response);
        }

        [HttpDelete("{id}")]
       
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            try
            {
                var response = await _quizService.DeleteQuizAsync(id);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new ResponseDTO(500, "Internal Server Error", ex.Message));
            }
        }

        [HttpPut("{quizId}/favorite")] // More RESTful route
        
        public async Task<ActionResult<ResponseDTO>> FavoriteQuiz(int quizId)
        {
            try
            {
                // Get the user's ID from the claims.  This is the *correct* way to get the user ID.
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var response = await _quizService.FavoriteQuiz(quizId, userId);

                if (response.Status == 200)
                {
                    return Ok(response); // Use Ok() for 200 responses
                }
                else if (response.Status == 404)
                {
                    return NotFound(response); // Use NotFound() for 404 responses
                }
                else
                {
                    return StatusCode(500, response); // Use StatusCode() for other errors
                }
            }
            catch (Exception ex)
            {
                // Log the exception here using a proper logging library (e.g., Serilog, NLog)
                // _logger.LogError(ex, "Error favoriting quiz {QuizId}", quizId);

                //  Don't return the raw exception message to the client in a production environment.
                return StatusCode(500, new ResponseDTO(500, "An unexpected error occurred."));
            }
        }

        [HttpPost("JoinQuiz/{quizId}")]
        public async Task<IActionResult> JoinQuizAsync([FromRoute] int quizId, [FromBody] int playerId)
        {
            try
            {
                var response = await _quizService.JoinQuizAsync(quizId, playerId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ResponseDTO(500, "Internal Server Error", ex.Message));
            }
        }

        [HttpPost("StartQuiz/{quizId}")]
        [Authorize]
        public async Task<IActionResult> StartQuizAsync([FromRoute] int quizId)
        {
            try
            {
                var response = await _quizService.StartQuizAsync(quizId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ResponseDTO(500, "Internal Server Error", ex.Message));
            }
        }

        [HttpGet("Player/ResultQuiz/{quizId}")]
        public async Task<IActionResult> GetResultQuizAsync([FromRoute] int quizId, [FromQuery] int playerId)
        {
            try
            {
                var response = await _quizService.GetResultQuizAsync(quizId, playerId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ResponseDTO(500, "Internal Server Error", ex.Message));
            }
        }

        [HttpGet("ResultQuiz/{quizId}")]
        [Authorize]
        public async Task<IActionResult> GetResultQuizAsync([FromRoute] int quizId)
        {
            try
            {
                var response = await _quizService.GetResultQuizAsync(quizId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ResponseDTO(500, "Internal Server Error", ex.Message));
            }
        }

        [HttpGet("JoinedPlayers/{quizId}")]
        [Authorize]
        public async Task<IActionResult> GetJoinedPlayersAsync([FromRoute] int quizId)
        {
            try
            {
                var response = await _quizService.GetJoinedPlayersAsync(quizId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ResponseDTO(500, "Internal Server Error", ex.Message));
            }
        }
    }

}
