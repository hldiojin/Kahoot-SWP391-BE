using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IServices
{
    public interface IUserService
    {
        Task<ResponseDTO> CreatePaymentByPayOS(CreatePaymentByPayOSRequestDTO request);
    }
}
