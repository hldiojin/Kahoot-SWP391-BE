using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.IService;
using Service.Service;

namespace API_Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UserServicePackController : ControllerBase
    {
        private readonly IUserServicePackService _userServicePackService;
        private readonly IJWTService _jWTService;

        public UserServicePackController(IUserServicePackService userServicePackService, IJWTService jWTService)
        {
            _userServicePackService = userServicePackService;
            _jWTService = jWTService;
        }
         
        [Authorize]
        [HttpGet]   
        public async Task<IActionResult> GetUserServicePackListByUserId()
        {
            var user = await _jWTService.GetCurrentUserAsync();
            var response = await _userServicePackService.GetUserServicePackListByUserId(user.Id);
            return StatusCode(response.Status, response);
        }

        [Authorize]
        [HttpPost("free-pack")]
        public async Task<IActionResult> CreateUserServicePackFree()
        {
            var user = await _jWTService.GetCurrentUserAsync();
            var response = await _userServicePackService.CreateUserServicePackFree(new Repository.DTO.RequestDTO.CreateUserServicePackRequestDTO { ServicePackId = 1, UserId = user.Id });
            return StatusCode(response.Status, response);
        }
    }
}

//GetUserServicePackListByUserId