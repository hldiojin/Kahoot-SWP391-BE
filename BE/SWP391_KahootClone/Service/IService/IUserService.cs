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
        //Task<ResponseDTO> CreatePaymentByPayOS(CreatePaymentByPayOSRequestDTO request);

        Task<ResponseDTO> CreateUserAsync(UserDTO userDTO);
        Task<ResponseDTO> UpadteUserAsync(UserDTO userDTO, int userId);
        Task<bool> RemoveUserAsync(int userId);
        Task<ResponseDTO> GetByIdAsync(int userId);
        Task<ResponseDTO> GetAllUserAsync();
    }
}
