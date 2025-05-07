using AutoMapper;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;


namespace Service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaymentRepository _paymentRepository;
        private readonly ServicePackRepository _servicePackRepository;
        private readonly UserServicePackRepository _userServicePackRepository;

        public PaymentService(IConfiguration configuration,
            PaymentRepository paymentRepository,
            UserServicePackRepository userServicePackRepository,
            ServicePackRepository servicePackRepository,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _config = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userServicePackRepository = userServicePackRepository ?? throw new ArgumentNullException(nameof(userServicePackRepository));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _servicePackRepository = servicePackRepository ?? throw new ArgumentNullException(nameof(servicePackRepository));
        }

        public async Task<ResponseDTO> CreatePaymentByPayOS(CreatePaymentByPayOSRequestDTO request)     //tao thanh toan voi ServicePackId = 2 (Premium), neu no chua co Premium hoac Premium het han moi cho tao thanh toan 
        {
            try
            {                                                                                              //luu xuong db 2 bang Payment - status: Pending va UserServicePack - isActive = false
                decimal price = 0;
                var servicePack = await _servicePackRepository.GetByIdAsync(request.ServicePackId);
                if (servicePack is null)
                    return new ResponseDTO(404, "Cannot find service pack, try again");
                price = servicePack.Price;
                if (price == 0)
                    return new ResponseDTO(400, "Price is 0, cannot create payment link");

                var userPackPremiun = await _userServicePackRepository.GetUserServicePackPremium(request.UserId);
                if (userPackPremiun != null && userPackPremiun.IsActive == true)
                    return new ResponseDTO(400, "You already have a premium service pack, cannot create payment link");
                var clientId = _config["PayOSSettings:ClientId"];
                var apiKey = _config["PayOSSettings:ApiKey"];
                var checksumKey = _config["PayOSSettings:ChecksumKey"];
                PayOS payOS = new PayOS(clientId, apiKey, checksumKey);

                var domain = "http://localhost:3000/login";

                string orderCode = DateTimeOffset.Now.ToString("ffffff");
                List<ItemData> items = new List<ItemData>();
                PaymentData paymentData = new PaymentData(
                    orderCode: int.Parse(orderCode),
                    amount: (int)price,
                    description: $"Nap: {price} VND",
                    items: items,
                    cancelUrl: domain,
                    returnUrl: domain,
                    buyerName: request.Username);
                  
                var payment = new Payment
                {
                    UserId = request.UserId,
                    ServicePackId = request.ServicePackId,
                    Amount = price,
                    PaymentMethod = orderCode,          //don nap tien ben payOS o day
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                var userPack = new UserServicePack
                {
                    UserId = request.UserId,
                    ServicePackId = request.ServicePackId,
                    ActivatedAt = DateTime.Now,
                    ExpiredAt = DateTime.Now.AddDays(servicePack.DurationDays),
                    IsActive = false
                };
                await _userServicePackRepository.CreateAsync(userPack);
                await _paymentRepository.CreateAsync(payment);
                CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);
                PaymentLinkInformation paymentLinkInformation = await payOS.getPaymentLinkInformation(paymentData.orderCode);
                return new ResponseDTO(200, $"Create payment link succress", new PayOSLink { PayOSUrl = createPayment.checkoutUrl });
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Create payment link failed: {ex.Message}");
            }
        }
        public async Task<ResponseDTO> PaymentCallbackPayOS(PaymentCallbackPayOSRequestDTO request)         //thanh toan xong thi doi Payment - status: PAID va UserServicePack - isActive = true   => test roi
        {                                                                                                   //thanh toan FAIL thi doi Payment - status: CANCELLED va UserServicePack - isActive = false giu nguyen => chua test 
            var orderCode = request.OrderCode.ToString();
            var payment = await _paymentRepository.GetPaymentByPaymentMethod(orderCode);
            var userPack = await _userServicePackRepository.GetUserServicePackPremium(payment.UserId);
            if (userPack is null)
                return new ResponseDTO(404, "Cannot find user pack, try again");
            if (request.Status == "PAID")
            {
                userPack.IsActive = true;
                await _userServicePackRepository.UpdateAsync(userPack);
            }

            payment.PaidAt = DateTime.Now;
            if (payment is null)
                return new ResponseDTO(404, "Cannot find payment, try again");
            payment.Status = request.Status switch
            {
                "PAID" => "PAID",
                "CANCELLED" => "CANCELLED",
                "PENDING" => "PENDING",
                "PROCESSING" => "PROCESSING",
                _ => "FAILED"
            };
            await _paymentRepository.UpdateAsync(payment);
            return new ResponseDTO(200, "Update payment status success", payment);
        }

        public async Task<ResponseDTO> CancelPayment(int paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                return new ResponseDTO(404, "Payment not found");
            }

            if (payment.Status != "Pending")
            {
                return new ResponseDTO(400, "Cannot cancel payment that is not in Pending status");
            }
            var clientId = _config["PayOSSettings:ClientId"];
            var apiKey = _config["PayOSSettings:ApiKey"];
            var checksumKey = _config["PayOSSettings:ChecksumKey"];
            PayOS payOS = new PayOS(clientId, apiKey, checksumKey);
            try
            {
                var cancelResult = await payOS.cancelPaymentLink(int.Parse(payment.PaymentMethod)); // Use PaymentMethod (which stores orderCode)
                if (cancelResult.status.Equals("True"))
                {
                    payment.Status = "CANCELLED";
                    await _paymentRepository.UpdateAsync(payment);
                    return new ResponseDTO(200, "Payment cancelled successfully", payment);
                }
                else
                {
                    return new ResponseDTO(500, $"Failed to cancel payment in PayOS: {cancelResult.cancellationReason}");
                }

            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Failed to cancel payment in PayOS");
            }
        }
    }
}