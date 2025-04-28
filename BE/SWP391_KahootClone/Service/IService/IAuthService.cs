using Repository.DTO;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO> Login(LoginRequestDTO request);
        Task<ResponseDTO> Register(RegisterRequestDTO request);

        Task<User> GetCurrentUserAsync();
        Task<ResponseDTO> LogoutAsync();
    }
}
