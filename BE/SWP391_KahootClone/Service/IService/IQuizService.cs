using Repository.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.IService
{
    public  interface IQuizService
    {
        Task<ResponseDTO> ChangeQuizStatusAsync(QuizDTO request, int quizId);
        Task<ResponseDTO> CreateQuizAsync(CreateQuizDTO request);
        Task<ResponseDTO> GetListQuizAsync();
        Task<ResponseDTO> GetQuizById(int quizId);
        Task<ResponseDTO> UpdateQuizAsync(QuizDTO request, int quizId);
        Task<bool> checkQuizCode(int quizCode);
        Task<ResponseDTO> GetByUserId(int userId);

    }
}
