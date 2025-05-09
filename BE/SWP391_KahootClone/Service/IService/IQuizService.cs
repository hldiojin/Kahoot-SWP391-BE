﻿using Repository.DTO;
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
        Task<int> checkQuizCode(int quizCode);
        Task<ResponseDTO> GetByUserId(int userId);
       // Task<ResponseDTO> GetFavorite(int userId);
        Task<ResponseDTO> GetQuizByQuizCode(int quizCode);
        Task<ResponseDTO> FavoriteQuiz(int quizId, int userId);

        Task<ResponseDTO> DeleteQuizAsync(int quizId);


       

        Task<ResponseDTO> JoinQuizAsync(int quizId, int playerId);
        Task<ResponseDTO> StartQuizAsync(int quizId);
        Task<ResponseDTO> GetResultQuizAsync(int quizId, int playerId);
        Task<ResponseDTO> GetResultQuizAsync(int quizId);
        Task<ResponseDTO> GetJoinedPlayersAsync(int quizId);
    }
}
