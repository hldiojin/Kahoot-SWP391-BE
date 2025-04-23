using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTO
{
 
        public class ResponseDTO
        {
            public List<UserResponseDTO> Users { get; set; } = new List<UserResponseDTO>();
            public List<QuizResponseDTO> Quizzes { get; set; } = new List<QuizResponseDTO>();
            public List<CategoryResponseDTO> Categories { get; set; } = new List<CategoryResponseDTO>();
            public List<QuestionResponseDTO> Questions { get; set; } = new List<QuestionResponseDTO>();
            public List<GameSessionResponseDTO> GameSessions { get; set; } = new List<GameSessionResponseDTO>();
            public List<PlayerResponseDTO> Players { get; set; } = new List<PlayerResponseDTO>();
            public List<PlayerAnswerResponseDTO> PlayerAnswers { get; set; } = new List<PlayerAnswerResponseDTO>();
            public List<GroupResponseDTO> Groups { get; set; } = new List<GroupResponseDTO>();
            public List<GroupMemberResponseDTO> GroupMembers { get; set; } = new List<GroupMemberResponseDTO>();
            public List<ServicePackResponseDTO> ServicePacks { get; set; } = new List<ServicePackResponseDTO>();
            public List<UserServicePackResponseDTO> UserServicePacks { get; set; } = new List<UserServicePackResponseDTO>();
            public List<PaymentResponseDTO> Payments { get; set; } = new List<PaymentResponseDTO>();
        

        public class UserResponseDTO
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string? AvatarUrl { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            // Note: Password is excluded for security
        }

        public class QuizResponseDTO
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

        public class CategoryResponseDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
        }

        public class QuestionResponseDTO
        {
            public int Id { get; set; }
            public int QuizId { get; set; }
            public string Text { get; set; }
            public string Type { get; set; }
            public string? OptionA { get; set; }
            public string? OptionB { get; set; }
            public string? OptionC { get; set; }
            public string? OptionD { get; set; }
            public int Score { get; set; }
            public bool Flag { get; set; }
            public int? TimeLimit { get; set; }
            public int? Arrange { get; set; }
            // Note: IsCorrect is excluded to prevent exposing answers
        }

        public class GameSessionResponseDTO
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

        public class PlayerResponseDTO
        {
            public int Id { get; set; }
            public int? UserId { get; set; }
            public string Nickname { get; set; }
            public int PlayerCode { get; set; }
            public string? AvatarUrl { get; set; }
            public int Score { get; set; }
            public int? SessionId { get; set; }
        }

        public class PlayerAnswerResponseDTO
        {
            public int Id { get; set; }
            public int PlayerId { get; set; }
            public int QuestionId { get; set; }
            public DateTime AnsweredAt { get; set; }
            public bool IsCorrect { get; set; }
            public int ResponseTime { get; set; }
            public string? Answer { get; set; }
        }

        public class GroupResponseDTO
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

        public class GroupMemberResponseDTO
        {
            public int GroupId { get; set; }
            public int UserId { get; set; }
            public int Rank { get; set; }
            public int TotalScore { get; set; }
            public DateTime JoinedAt { get; set; }
            public string Status { get; set; }
        }

        public class ServicePackResponseDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public int DurationDays { get; set; }
            public string? Features { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class UserServicePackResponseDTO
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int ServicePackId { get; set; }
            public DateTime ActivatedAt { get; set; }
            public DateTime? ExpiredAt { get; set; }
            public bool IsActive { get; set; }
        }

        public class PaymentResponseDTO
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
    }

}