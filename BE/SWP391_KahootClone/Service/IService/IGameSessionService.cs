using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IGameSessionService
    {
        Task<ResponseDTO> CreateGameSessionAsync(GameSessionDTO gameSessionDto);
        Task<ResponseDTO> GetGameSessionByIdAsync(int id);
        Task<ResponseDTO> GetAllGameSessionsAsync();
        Task<ResponseDTO> UpdateGameSessionAsync(int id, GameSessionDTO gameSessionDto);
        Task<ResponseDTO> DeleteGameSessionAsync(int id);
    }
}
