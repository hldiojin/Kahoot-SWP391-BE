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
        GameSessionRepository GameSessionRepository { get; }
        PlayerRepository PlayerRepository { get; }
        PlayerAnswerRepository PlayerAnswerRepository { get; }
        GroupRepository GroupRepository { get; }
        IGroupMemberRepository GroupMemberRepository { get; }
        IServicePackRepository ServicePackRepository { get; }
        IUserServicePackRepository UserServicePackRepository { get; }
        IPaymentRepository PaymentRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
