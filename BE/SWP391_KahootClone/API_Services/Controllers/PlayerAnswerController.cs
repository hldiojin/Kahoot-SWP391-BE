using Microsoft.AspNetCore.Mvc;
using Service.Service; // Adjust the namespace if needed
using Repository.DTO; // Adjust the namespace if needed
using System.Threading.Tasks;
using Service.IService;
using static Repository.DTO.RequestDTO;

namespace API_Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerAnswerController : ControllerBase
    {
        private readonly IPlayerAnswerService _playerAnswerService;
        private readonly ScoreCalculatorService _scoreCalculator;
        public PlayerAnswerController(IPlayerAnswerService playerAnswerService, ScoreCalculatorService scoreCalculator)
        {
            _playerAnswerService = playerAnswerService;
            _scoreCalculator = scoreCalculator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayerAnswer([FromBody] PlayerAnswerDTO playerAnswerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _playerAnswerService.CreatePlayerAnswerAsync(playerAnswerDto);
            if (response.Status == 201)
            {
                return Ok( response.Data);
            }
            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerAnswerById(int id)
        {
            var response = await _playerAnswerService.GetPlayerAnswerByIdAsync(id);
            if (response.Status == 200)
            {
                return Ok(response.Data);
            }
            return StatusCode(response.Status, response.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlayerAnswers()
        {
            var response = await _playerAnswerService.GetAllPlayerAnswersAsync();
            if (response.Status == 200)
            {
                return Ok(response.Data);
            }
            return StatusCode(response.Status, response.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayerAnswer(int id, [FromBody] PlayerAnswerDTO playerAnswerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != playerAnswerDto.Id)
            {
                return BadRequest("Id in body does not match Id in route.");
            }

            var response = await _playerAnswerService.UpdatePlayerAnswerAsync(id, playerAnswerDto);
            if (response.Status == 200)
            {
                return Ok(response.Data);
            }
            return StatusCode(response.Status, response.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerAnswer(int id)
        {
            var response = await _playerAnswerService.DeletePlayerAnswerAsync(id);
            if (response.Status == 200)
            {
                return Ok(response.Message);
            }
            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("Player/{playerId}")]
        public async Task<IActionResult> GetPlayerAnswersByPlayerId(int playerId)
        {
            var response = await _playerAnswerService.GetPlayerAnswersByPlayerIdAsync(playerId);
            if (response.Status == 200)
            {
                return Ok(response.Data);
            }
            return StatusCode(response.Status, response.Message);
        }

        [HttpGet("Question/{questionId}")]
        public async Task<IActionResult> GetPlayerAnswersByQuestionId(int questionId)
        {
            var response = await _playerAnswerService.GetPlayerAnswersByQuestionIdAsync(questionId);
            if (response.Status == 200)
            {
                return Ok(response.Data);
            }
            return StatusCode(response.Status, response.Message);
        }

        [HttpPost("SoloScore")]
        public ActionResult<int> CalculateSoloScore([FromBody] SoloScoreRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int score = _scoreCalculator.CalculateSoloScore(request.PlayerAnswer, request.Question);
                return Ok(score);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message); // Return a 400 Bad Request with the error message
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while calculating the score: " + ex.Message); // Return 500
            }
        }

        /// <summary>
        /// Calculates the score for a group based on the individual player answers.
        /// </summary>
        /// <param name="groupMembers">The members of the group.</param>
        /// <param name="playerAnswers">The answers given by the players in the group.</param>
        /// <param name="questions">The questions that were answered.</param>
        /// <returns>A dictionary of player scores, and the total group score.</returns>
        [HttpPost("GroupScore")]
        public ActionResult<GroupScoreResponse> CalculateGroupScore([FromBody] GroupScoreRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                (Dictionary<int, int> playerScores, int totalGroupScore) = _scoreCalculator.CalculateGroupScore(request.GroupMembers, request.PlayerAnswers, request.Questions);
                var response = new GroupScoreResponse
                {
                    PlayerScores = playerScores,
                    TotalGroupScore = totalGroupScore
                };
                return Ok(response);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while calculating the group score: " + ex.Message);
            }
        }
    }
}
