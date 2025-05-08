using Microsoft.EntityFrameworkCore.Storage;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IUnitOfWork : IDisposable
    {
        UserRepository UserRepository { get; }
        QuizRepository QuizRepository { get; }
        CategoryRepository CategoryRepository { get; }
        QuestionRepository QuestionRepository { get; }
        PlayerRepository PlayerRepository { get; }
        PlayerAnswerRepository PlayerAnswerRepository { get; }
        GroupRepository GroupRepository { get; }
        GroupMemberRepository GroupMemberRepository { get; }
        ServicePackRepository ServicePackRepository { get; }
        UserServicePackRepository UserServicePackRepository { get; }
        PaymentRepository PaymentRepository { get; }
        IDbContextTransaction BeginTransaction();
        Task<int> SaveChangesAsync();
    }
}
