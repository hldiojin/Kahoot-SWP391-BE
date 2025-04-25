using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public interface IQuestionService
    {
        Task<ResponseDTO> CreateQuestionAsync(QuestionDTO questionDto);
        Task<ResponseDTO> GetQuestionByIdAsync(int questionId);
        Task<ResponseDTO> UpdateQuestionAsync(int questionId, QuestionDTO questionDto);
        Task<ResponseDTO> DeleteQuestionAsync(int questionId);
        Task<ResponseDTO> GetQuestionsByQuizIdAsync(int quizId);
    }
}
