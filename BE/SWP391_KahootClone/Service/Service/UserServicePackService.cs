using AutoMapper;
using Azure.Core;
using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using static Repository.DTO.RequestDTO;

namespace Service.Service
{
    public class UserServicePackService : IUserServicePackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserServicePackRepository _userServicePackRepository;
        public UserServicePackService(UserServicePackRepository userServicePackRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userServicePackRepository = userServicePackRepository ?? throw new ArgumentNullException(nameof(userServicePackRepository));
        }

        public async Task<ResponseDTO> CreateUserServicePackFree(CreateUserServicePackRequestDTO request)
        {
            try
            {
                var checkExistingServicePackFree = await _userServicePackRepository.CheckUserFreePack(request.UserId);
                if (!checkExistingServicePackFree)
                {
                    var userServicePack = new UserServicePack
                    {
                        UserId = request.UserId,
                        ServicePackId = request.ServicePackId,
                        ActivatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _userServicePackRepository.CreateAsync(userServicePack);
                    await _unitOfWork.SaveChangesAsync();
                    return new ResponseDTO(200, "User Service Pack free created successfully.", userServicePack);
                }
                else
                    return new ResponseDTO(400, "User Service Pack free already created.");

            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error creating game session: {ex.Message}");
            }
        }
        public async Task<ResponseDTO> GetUserServicePackListByUserId(int userID)
        {
            try
            {
                var servicePackList = await _userServicePackRepository.GetAllAsync();
                var response = servicePackList.Where(x => x.UserId == userID).Select(x => new UserServicePackList
                {
                    Id = x.Id,
                    ActivatedAt = x.ActivatedAt,
                    ExpiredAt = x.ExpiredAt,
                    IsActive = x.IsActive,
                    ServicePack = x.ServicePack,
                }).ToList();

                return new ResponseDTO(200, "Get all service pack successfully.", response);

            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error creating game session: {ex.Message}");
            }
        }
    }
}
