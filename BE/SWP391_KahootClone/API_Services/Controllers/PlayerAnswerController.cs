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

        public PlayerAnswerController(IPlayerAnswerService playerAnswerService)
        {
            _playerAnswerService = playerAnswerService;
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
                return CreatedAtAction(nameof(GetPlayerAnswerById), new { id = ((PlayerAnswerDTO)response.Data).Id }, response.Data);
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
    }
}
