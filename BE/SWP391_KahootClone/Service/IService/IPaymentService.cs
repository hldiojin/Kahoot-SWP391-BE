using Microsoft.AspNetCore.Http;
using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IPaymentService
    {
        Task<ResponseDTO> CreatePaymentByPayOS(CreatePaymentByPayOSRequestDTO request);
        Task<ResponseDTO> PaymentCallbackPayOS(PaymentCallbackPayOSRequestDTO request);
    }


}
