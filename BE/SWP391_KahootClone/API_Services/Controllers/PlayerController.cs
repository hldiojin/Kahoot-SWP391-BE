namespace API_Services.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Service.Service; // Adjust the namespace if needed
    using Repository.DTO; // Adjust the namespace if needed
    using System.Threading.Tasks;
    using Service.IService;
    using static Repository.DTO.RequestDTO;

   
        [ApiController]
        [Route("api/[controller]")]
        public class PlayerController : ControllerBase
        {
            private readonly IPlayerService _playerService;

            public PlayerController(IPlayerService playerService)
            {
                _playerService = playerService;
            }

            [HttpPost]
            public async Task<IActionResult> CreatePlayer([FromBody] PlayerDTO playerDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _playerService.CreatePlayerAsync(playerDto);
                if (response.Status == 201)
                {
                    return CreatedAtAction(nameof(GetPlayerById), new { id = ((PlayerDTO)response.Data).Id }, response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetPlayerById(int id)
            {
                var response = await _playerService.GetPlayerByIdAsync(id);
                if (response.Status == 200)
                {
                    return Ok(response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }

            [HttpGet]
            public async Task<IActionResult> GetAllPlayers()
            {
                var response = await _playerService.GetAllPlayersAsync();
                if (response.Status == 200)
                {
                    return Ok(response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> UpdatePlayer(int id, [FromBody] PlayerDTO playerDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (id != playerDto.Id)
                {
                    return BadRequest("Id in body does not match Id in route.");
                }

                var response = await _playerService.UpdatePlayerAsync(id, playerDto);
                if (response.Status == 200)
                {
                    return Ok(response.Data);
                }
                return StatusCode(response.Status, response.Message);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeletePlayer(int id)
            {
                var response = await _playerService.DeletePlayerAsync(id);
                if (response.Status == 200)
                {
                    return Ok(response.Message);
                }
                return StatusCode(response.Status, response.Message);
            }

           
        }
    }



