namespace API_Services.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Service.Service; // Adjust the namespace if needed
    using Repository.DTO; // Adjust the namespace if needed
    using System.Threading.Tasks;
    using Service.IService;
    using static Repository.DTO.RequestDTO;

    namespace API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class GameSessionController : ControllerBase
        {
            private readonly IGameSessionService _gameSessionService;

            public GameSessionController(IGameSessionService gameSessionService)
            {
                _gameSessionService = gameSessionService;
            }

            [HttpPost]
            public async Task<IActionResult> CreateGameSession([FromBody] GameSessionDTO gameSessionDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _gameSessionService.CreateGameSessionAsync(gameSessionDto);
                if (response.Status == 201)
                {
                    return CreatedAtAction(nameof(GetGameSessionById), new { id = ((GameSessionDTO)response.Data).Id }, response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetGameSessionById(int id)
            {
                var response = await _gameSessionService.GetGameSessionByIdAsync(id);
                if (response.Status == 200)
                {
                    return Ok(response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }

            [HttpGet]
            public async Task<IActionResult> GetAllGameSessions()
            {
                var response = await _gameSessionService.GetAllGameSessionsAsync();
                if (response.Status == 200)
                {
                    return Ok(response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }   

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdateGameSession(int id, [FromBody] GameSessionDTO gameSessionDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (id != gameSessionDto.Id) //check dto
                {
                    return BadRequest("Id in body does not match Id in route.");
                }

                var response = await _gameSessionService.UpdateGameSessionAsync(id, gameSessionDto);
                if (response.Status == 200)
                {
                    return Ok(response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteGameSession(int id)
            {
                var response = await _gameSessionService.DeleteGameSessionAsync(id);
                if (response.Status == 200)
                {
                    return Ok(response.Message);
                }
                return StatusCode(response.Status, response.Message);
            }
            // Add other actions as needed (e.g., StartGame, EndGame)
        }
    }


}
