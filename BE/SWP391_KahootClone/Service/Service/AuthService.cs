using AutoMapper;

using Repository.DTO;
using Repository.Models;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJWTService _jWTService;
        public AuthService(IUnitOfWork unitOfWork , IJWTService jWTService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jWTService = jWTService;
        }
        public async Task<ResponseDTO> Login(LoginRequestDTO request)
        {
            try
            {
                var account = _unitOfWork.UserRepository.GetAll()
                                .FirstOrDefault(x => x.Username!.ToLower() == request.userName.ToLower()
                                && x.PasswordHash == request.password);

                if (account == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Invalid credentials.");
                }

               
                //var loginResponse = _mapper.Map<LoginResponse>(account);
                var jwt = _jWTService.GenerateToken(account);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, jwt);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }



        public async Task<ResponseDTO> Register(RegisterRequestDTO request)
        {
            try
            {
                if (await _unitOfWork.UserRepository.ExistsByNameAsync(request.Username))
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "The username is already taken. Please choose a different username.");
                }

                // AutoMapper from RegisterRequestDTO => User
                var user = _mapper.Map<User>(request);
                // Hash the password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;
                user.Role = "User";

                await _unitOfWork.UserRepository.CreateAsync(user);
                await _unitOfWork.SaveChangesAsync(); // Ensure changes are saved to the database

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "User registered successfully", request);
            }
            catch (Exception ex)
            {
                // Consider logging the exception for debugging purposes
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
