using Microsoft.EntityFrameworkCore.Storage;
using Repository.DBContext;
using Repository.Models;
using Repository.Repositories;

namespace Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SWP_KahootContext _context;
        private UserRepository _userRepository;
        private QuizRepository _quizRepository;
        private CategoryRepository _categoryRepository;
        private QuestionRepository _questionRepository;
       
        private PlayerRepository _playerRepository;
        private PlayerAnswerRepository _playerAnswerRepository;
        private GroupRepository _groupRepository;
        private GroupMemberRepository _groupMemberRepository;
        private ServicePackRepository _servicePackRepository;
        private UserServicePackRepository _userServicePackRepository;
        private PaymentRepository _paymentRepository;

        public UnitOfWork(SWP_KahootContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IDbContextTransaction BeginTransaction()
        {
            
            return _context.Database.BeginTransaction();
        }
        public UserRepository UserRepository
        {
            get => _userRepository ??= new UserRepository(_context);
        }

        public QuizRepository QuizRepository
        {
            get => _quizRepository ??= new QuizRepository(_context);
        }

        public CategoryRepository CategoryRepository
        {
            get => _categoryRepository ??= new CategoryRepository(_context);
        }

        public QuestionRepository QuestionRepository
        {
            get => _questionRepository ??= new QuestionRepository(_context);
        }

      
        public PlayerRepository PlayerRepository
        {
            get => _playerRepository ??= new PlayerRepository(_context);
        }

        public PlayerAnswerRepository PlayerAnswerRepository
        {
            get => _playerAnswerRepository ??= new PlayerAnswerRepository(_context);
        }

        public GroupRepository GroupRepository
        {
            get => _groupRepository ??= new GroupRepository(_context);
        }

        public GroupMemberRepository GroupMemberRepository
        {
            get => _groupMemberRepository ??= new GroupMemberRepository(_context);
        }

        public ServicePackRepository ServicePackRepository
        {
            get => _servicePackRepository ??= new ServicePackRepository(_context);
        }

        public UserServicePackRepository UserServicePackRepository
        {
            get => _userServicePackRepository ??= new UserServicePackRepository(_context);
        }

        public PaymentRepository PaymentRepository
        {
            get => _paymentRepository ??= new PaymentRepository(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}