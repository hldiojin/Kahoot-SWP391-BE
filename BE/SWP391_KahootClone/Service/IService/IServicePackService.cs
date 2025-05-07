using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IServicePackService
    {
        Task<ResponseDTO> CreeateServicePack(CreateServicePackRequestDTO request);
        Task<ResponseDTO> GetServicePackList();
        Task<ResponseDTO> Update(CreateServicePackRequestDTO request, int servicePackId);
    }
}
