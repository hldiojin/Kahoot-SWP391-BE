using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.Service;
using static Repository.DTO.RequestDTO;

namespace API_Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicePackController : ControllerBase
    {
        private readonly IServicePackService _servicePackService;

        public ServicePackController(IServicePackService servicePackService)
        {
            _servicePackService = servicePackService;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateServicePack(CreateServicePackRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _servicePackService.CreeateServicePack(request);
            return StatusCode(response.Status, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetListQuiz()
        {
            var response = await _servicePackService.GetServicePackList();
            return StatusCode(response.Status, response);
        }

        [HttpPut]
        [Route("Update/{servicePackId}")]

        [Authorize(Roles = "admin")]

        public async Task<IActionResult> UpdateServicePack(int servicePackId, [FromBody] CreateServicePackRequestDTO request)
        {
            var response = await _servicePackService.Update(request, servicePackId);
            return StatusCode(response.Status, response);
        }
    }
}
