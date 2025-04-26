using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IUserServicePackService
    {
        Task<ResponseDTO> CreateUserServicePackFree(CreateUserServicePackRequestDTO request);
        Task<ResponseDTO> GetUserServicePackListByUserId(int userID);
    }
}
