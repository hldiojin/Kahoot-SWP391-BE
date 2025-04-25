using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.IServices;
using Service.Service;
using static Repository.DTO.RequestDTO;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        var response = await _authService.Register(request);

        if (response.Status == Const.SUCCESS_CREATE_CODE)
        {
            return Ok(response);
        }
        else if (response.Status == Const.FAIL_CREATE_CODE)
        {
            return BadRequest(response);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var response = await _authService.Login(request);

        if (response.Status == Const.SUCCESS_READ_CODE)
        {
            return Ok(response);
        }
        else if (response.Status == Const.FAIL_READ_CODE)
        {
            return Unauthorized(response);
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}