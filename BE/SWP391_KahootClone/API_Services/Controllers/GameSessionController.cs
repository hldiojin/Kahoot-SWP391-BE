using Microsoft.AspNetCore.Mvc;
using Repository.Models;
using Service.IService;
using Service.IServices;
using Service.Service;

namespace API_Services.Controllers
{

    public class GameSessionController : ControllerBase
    {
        private readonly IGameSessionService _GameSessionService;

        public GameSessionController(IGameSessionService gameSessionService)
        {
            _GameSessionService = gameSessionService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGameSessions()
        {
            var result = await _GameSessionService.GetAllGameSessionsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameSessionById(int id)
        {
            var session = await _GameSessionService.GetGameSessionByIdAsync(id);
            if (session == null)
                return NotFound(new { Message = "Game session not found." });

            return Ok(session);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGameSession([FromBody] GameSession gameSession)
        {
            var success = await _GameSessionService.CreateGameSessionAsync(gameSession);
            if (success)
                return Ok(new { Message = "Game session created successfully." });

            return BadRequest(new { Message = "Failed to create game session." });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGameSession([FromBody] GameSession gameSession)
        {
            var success = await _GameSessionService.UpdateGameSessionAsync(gameSession);
            if (success)
                return Ok(new { Message = "Game session updated successfully." });

            return BadRequest(new { Message = "Failed to update game session." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGameSession(int id)
        {
            var success = await _GameSessionService.DeleteGameSessionAsync(id);
            if (success)
                return Ok(new { Message = "Game session deleted successfully." });

            return NotFound(new { Message = "Game session not found." });
        }
    }
}
