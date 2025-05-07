using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.DTO
{

    public class ResponseDTO
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public ResponseDTO(int status, string? message, object? data = null)
        {
            Status = status;
            Message = message;
            Data = data;
        }


    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

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
        public int? MaxPlayer { get; set; }
        public int? MinPlayer { get; set; }
        public bool? Favorite { get; set; }
        public string? GameMode { get; set; }
    }
    public class QuizHistoryResponseDTO
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
        public int? MaxPlayer { get; set; }
        public int? MinPlayer { get; set; }
        public bool? Favorite { get; set; }
        public string? GameMode { get; set; }
        public int PlayedCount { get; set; }
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
            public int playerId { get; set; }
            public int? UserId { get; set; }
            public string Nickname { get; set; }
            public int PlayerCode { get; set; }
            public string? AvatarUrl { get; set; }
            public int Score { get; set; }
         
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

            public int Score { get; set; }
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

    public class ServicePackageList
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public string Features { get; set; }

    }

    public class UserServicePackList
    {
        public int Id { get; set; }

        public DateTime ActivatedAt { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public bool IsActive { get; set; }

        public virtual ServicePack ServicePack { get; set; }
    }
    public class PayOSLink
    {
        public string PayOSUrl { get; set; }
    }

    public class GroupScoreResponse
    {
        public Dictionary<int, int> PlayerScores { get; set; }
        public int TotalGroupScore { get; set; }
    }
}

