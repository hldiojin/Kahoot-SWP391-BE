using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IPlayerAnswerService
    {
        Task<ResponseDTO> CreatePlayerAnswerAsync(PlayerAnswerDTO playerAnswerDto);
        Task<ResponseDTO> GetPlayerAnswerByIdAsync(int id);
        Task<ResponseDTO> GetAllPlayerAnswersAsync();
        Task<ResponseDTO> UpdatePlayerAnswerAsync(int id, PlayerAnswerDTO playerAnswerDto);
        Task<ResponseDTO> DeletePlayerAnswerAsync(int id);
        Task<ResponseDTO> GetPlayerAnswersByPlayerIdAsync(int playerId); 
        Task<ResponseDTO> GetPlayerAnswersByQuestionIdAsync(int questionId);
    }
}
