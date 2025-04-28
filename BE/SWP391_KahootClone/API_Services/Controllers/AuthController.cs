using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Repository.DTO; // Assuming your DTOs are in this namespace
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static Repository.DTO.RequestDTO;
using Service.Service; // Import for [Authorize]

namespace API_Services.Controllers // Adjust the namespace as needed
{
    [ApiController]
    [Route("api/auth")] // Consider using [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor; // Add for accessing HttpContext

        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns a 400 with validation errors
            }

            ResponseDTO response = await _authService.Login(request);

            if (response.Status == Const.SUCCESS_READ_CODE)
            {
                return Ok(response); // Returns 200 with the login data (e.g., JWT)
            }
            else if (response.Status == Const.FAIL_READ_CODE)
            {
                return BadRequest(response); // Returns 400 for invalid credentials
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response); // Returns 500 for other errors
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)] // Use 201 Created for successful registration
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns 400 with validation errors
            }

            ResponseDTO response = await _authService.Register(request);

            if (response.Status == Const.SUCCESS_CREATE_CODE)
            {
                //  Ideally, you should return CreatedAtAction, but since Register doesn't
                //  return the newly created resource's URL, we'll just return Ok.
                //  If you modify your Register method to return the new user's ID,
                //  you can use CreatedAtAction.
                return Ok(response); // Returns 200 with the response data
                // return CreatedAtAction(nameof(GetUser), new { id = newUserId }, response); // Preferred
            }
            else if (response.Status == Const.FAIL_CREATE_CODE)
            {
                return BadRequest(response); // Returns 400 for registration failures (e.g., username taken)
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response); // Returns 500 for other errors
            }
        }

        [HttpGet("me")] // Use a more descriptive route like "me"
        [Authorize] // Requires the user to be authenticated
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Call the service method to get the current user
                var user = await _authService.GetCurrentUserAsync();

                if (user != null)
                {
                    //  Map the User object to a DTO before returning it.  This is important
                    //  to avoid exposing sensitive information (like password hashes) to the client.
                    var userDto = new UserDTO
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        // Add other properties as needed.  DO NOT include PasswordHash!
                    };
                    return Ok(userDto); // Return 200 with the user data
                }
                else
                {
                    return Unauthorized(new ResponseDTO(Const.FAIL_READ_CODE, "User not found")); // 401 if no user
                }
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message)); // 500
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await _authService.LogoutAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
            }
        }
    }

    //  Create a UserDTO to define what data you want to return to the client.
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        //  Add other properties as needed, but NEVER include PasswordHash or other
        //  sensitive information.
    }
}

