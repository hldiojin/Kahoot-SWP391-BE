using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTO
{

    public class RequestDTO
    {
        public List<UserDTO> Users { get; set; } = new List<UserDTO>();
        public List<QuizDTO> Quizzes { get; set; } = new List<QuizDTO>();
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public List<QuestionDTO> Questions { get; set; } = new List<QuestionDTO>();
        public List<GameSessionDTO> GameSessions { get; set; } = new List<GameSessionDTO>();
        public List<PlayerDTO> Players { get; set; } = new List<PlayerDTO>();
        public List<PlayerAnswerDTO> PlayerAnswers { get; set; } = new List<PlayerAnswerDTO>();
        public List<GroupDTO> Groups { get; set; } = new List<GroupDTO>();
        public List<GroupMemberDTO> GroupMembers { get; set; } = new List<GroupMemberDTO>();
        public List<ServicePackDTO> ServicePacks { get; set; } = new List<ServicePackDTO>();
        public List<UserServicePackDTO> UserServicePacks { get; set; } = new List<UserServicePackDTO>();
        public List<PaymentDTO> Payments { get; set; } = new List<PaymentDTO>();

        public class LoginRequestDTO
        {
            public string email { get; set; }
            public string password { get; set; }
        }

        public class RegisterRequestDTO
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class UserDTO
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; } // Formerly password_hash
            public string Role { get; set; }
            public string? AvatarUrl { get; set; }
            public bool IsActive { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class QuizDTO
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int QuizCode { get; set; }
            public string? Description { get; set; }
            public int CreatedBy { get; set; }
            public int CategoryId { get; set; }
            public bool IsPublic { get; set; }
            public string? ThumbnailUrl { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class CategoryDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
        }

        public class QuestionDTO
        {
            public int Id { get; set; }
            public int QuizId { get; set; }
            public string Text { get; set; }
            public string Type { get; set; }
            public string? OptionA { get; set; }
            public string? OptionB { get; set; }
            public string? OptionC { get; set; }
            public string? OptionD { get; set; }
            public string? IsCorrect { get; set; }
            public int Score { get; set; }
            public bool Flag { get; set; }
            public int? TimeLimit { get; set; }
            public int? Arrange { get; set; }
        }

        public class GameSessionDTO
        {
            public int Id { get; set; }
            public int QuizId { get; set; }
            public int HostId { get; set; }
            public string PinCode { get; set; }
            public string GameType { get; set; }
            public string Status { get; set; }
            public int MinPlayer { get; set; }
            public int MaxPlayer { get; set; }
            public DateTime? StartedAt { get; set; }
            public DateTime? EndedAt { get; set; }
        }

        public class PlayerDTO
        {
            public int Id { get; set; }
            public int? UserId { get; set; }
            public string Nickname { get; set; }
            public int PlayerCode { get; set; }
            public string? AvatarUrl { get; set; }
            public int Score { get; set; }
            public int? SessionId { get; set; }
        }

        public class PlayerAnswerDTO
        {
            public int Id { get; set; }
            public int PlayerId { get; set; }
            public int QuestionId { get; set; }
            public DateTime AnsweredAt { get; set; }
            public bool IsCorrect { get; set; }
            public int ResponseTime { get; set; }
            public string? Answer { get; set; }
        }

        public class GroupDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public int Rank { get; set; }
            public int MaxMembers { get; set; }
            public int TotalPoint { get; set; }
            public int CreatedBy { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class GroupMemberDTO
        {
            public int GroupId { get; set; }
            public int UserId { get; set; }
            public int Rank { get; set; }
            public int TotalScore { get; set; }
            public DateTime JoinedAt { get; set; }
            public string Status { get; set; }
        }

        public class ServicePackDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public int DurationDays { get; set; }
            public string? Features { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class UserServicePackDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int ServicePackId { get; set; }
            public DateTime ActivatedAt { get; set; }
            public DateTime? ExpiredAt { get; set; }
            public bool IsActive { get; set; }
        }

        public class PaymentDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int ServicePackId { get; set; }
            public decimal Amount { get; set; }
            public string PaymentMethod { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? PaidAt { get; set; }
        }

        public class CreateServicePackRequestDTO
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int DurationDays { get; set; }
            public string Features { get; set; }
        }

        public class CreateUserServicePackRequestDTO
        {

            public int UserId { get; set; }

            public int ServicePackId { get; set; }
        }
        public class CreatePaymentByPayOSRequestDTO
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public int ServicePackId { get; set; }
        }
        public class PaymentCallbackPayOSRequestDTO
        {
            public string Code { get; set; }

            public string Id { get; set; }

            public bool Cancel { get; set; }

            public string Status { get; set; }

            public long OrderCode { get; set; }

        }

    }
}
