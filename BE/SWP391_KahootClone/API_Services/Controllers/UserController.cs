
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

﻿using Microsoft.AspNetCore.Mvc;

using Repository.DTO;
using Service.IServices;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace API_Services.Controllers
{
    [ApiController]
    [Route("api/v1/users")]

    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
            public UserController(IUserService userService)
            {
                _userService = userService;
            }

            [HttpPost]

            [Authorize(Roles = "admin")]

            public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
            {
                var response = await _userService.CreateUserAsync(userDTO);
                return StatusCode(response.Status, response);
            }

            [HttpPut("{userId}")]

            [Authorize(Roles = "admin")]

            public async Task<IActionResult> UpdateUser([FromRoute] int userId, [FromBody] UserDTO userDTO)
            {
                var response = await _userService.UpadteUserAsync(userDTO, userId);
                return StatusCode(response.Status, response);
            }

            [HttpDelete("{userId}")]

            [Authorize(Roles = "admin")]

            public async Task<IActionResult> DeleteUser([FromRoute] int userId)
            {
                var success = await _userService.RemoveUserAsync(userId);
                if (!success)
                {
                    return NotFound(new ResponseDTO(404, "User not found"));
                }
                return Ok(new ResponseDTO(200, "User deleted successfully"));
            }

            [HttpGet("{userId}")]
            public async Task<IActionResult> GetUserById([FromRoute] int userId)
            {
                var response = await _userService.GetByIdAsync(userId);
                return StatusCode(response.Status, response);
            }

            [HttpGet]
            public async Task<IActionResult> GetAllUsers()
            {
                var response = await _userService.GetAllUserAsync();
                return StatusCode(response.Status, response);
            }
        }
    }

