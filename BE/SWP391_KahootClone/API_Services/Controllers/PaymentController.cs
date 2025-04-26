using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service;
using Service.Service;
using static Repository.DTO.RequestDTO;
using Microsoft.AspNetCore.Authorization;

namespace API_Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IJWTService _jWTService;

        public PaymentController(IPaymentService paymentService, IJWTService jWTService)
        {
            _paymentService = paymentService;
            _jWTService = jWTService;
        }

        [Authorize]
        [HttpPost]              //nhap cai id cua pack premium vao = 2
        public async Task<IActionResult> CreatePayment(int servicePackId)
        {
            var user = await _jWTService.GetCurrentUserAsync();
            var response = await _paymentService.CreatePaymentByPayOS(new CreatePaymentByPayOSRequestDTO
            {
                ServicePackId = servicePackId,
                Username = user.Username,
                UserId = user.Id
            });
            return StatusCode(response.Status, response);
        }

        [HttpGet]
        public async Task<IActionResult> PayOSCallback()
        {
            var query = HttpContext.Request.Query;
            var request = new PaymentCallbackPayOSRequestDTO
            {
                Code = query["code"],
                Id = query["id"],
                Cancel = bool.TryParse(query["cancel"], out var cancel) && cancel,
                Status = query["status"],
                OrderCode = long.TryParse(query["orderCode"], out var orderCode) ? orderCode : 0,
            };
            var response = await _paymentService.PaymentCallbackPayOS(request);
            return StatusCode(response.Status, response);
        }

    }

    public class PaymentCallbackPayOSRequest
    {
        [FromQuery(Name = "code")]
        public string Code { get; set; }

        [FromQuery(Name = "id")]
        public string Id { get; set; }

        [FromQuery(Name = "cancel")]
        public bool Cancel { get; set; }

        [FromQuery(Name = "status")]
        public string Status { get; set; }

        [FromQuery(Name = "orderCode")]
        public long OrderCode { get; set; }

        public IQueryCollection RawQueryCollection { get; set; }
    }
}
