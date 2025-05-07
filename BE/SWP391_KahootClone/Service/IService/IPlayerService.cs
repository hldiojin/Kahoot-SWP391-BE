using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;
using PlayerDTO = Repository.DTO.RequestDTO.PlayerDTO;

namespace Service.IService
{
    public interface IPlayerService
    {
        Task<ResponseDTO> CreatePlayerAsync(PlayerDTO playerDto);
        Task<ResponseDTO> GetPlayerByIdAsync(int id);
        Task<ResponseDTO> GetAllPlayersAsync();
        Task<ResponseDTO> UpdatePlayerAsync(int id, PlayerDTO playerDto);
        Task<ResponseDTO> DeletePlayerAsync(int id);
       
    }
}
