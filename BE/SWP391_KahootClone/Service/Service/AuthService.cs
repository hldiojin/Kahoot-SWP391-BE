using AutoMapper;
using Repository.Models;
using Service.IService;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Repository.DTO;

namespace Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJWTService _jWTService;
        private readonly IConfiguration _configuration; // Add IConfiguration
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUnitOfWork unitOfWork, IJWTService jWTService, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) // Add to constructor
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jWTService = jWTService;
            _configuration = configuration; // Inject configuration
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResponseDTO> Login(LoginRequestDTO request)
        {
            try
            {
                var account = _unitOfWork.UserRepository.GetAll()
                                                .FirstOrDefault(x => x.Email!.ToLower() == request.email.ToLower()
                                                && BCrypt.Net.BCrypt.Verify(request.password, x.PasswordHash));

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

        public async Task<User> GetCurrentUserAsync()
        {
            try
            {
                // Lấy token từ header Authorization
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return null; // Or throw an exception, depending on your design
                }

                var claimsPrincipal = _jWTService.ValidateToken(token);

                // Lấy UserId từ claims
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return null; // Or throw an exception
                }

                int userId = int.Parse(userIdClaim.Value);

                // Lấy người dùng hiện tại
                var user = await _unitOfWork.UserRepository.GetUserByCurrentId(userId);
                if (user == null)
                {
                    return null; // Or throw an exception
                }
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null; // Or throw an exception
            }
        }
        public async Task<ResponseDTO> LogoutAsync()
        {
            try
            {
                // Get the token from the Authorization header
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No token provided.");
                }

                // Invalidate the token (implementation depends on your JWTService)
                _jWTService.InvalidateToken(token); // You need to implement this in your JWTService

                // Clear the cookie (optional, if you're using cookies)
                _httpContextAccessor.HttpContext?.Response.Cookies.Delete("Authorization");

                return new ResponseDTO(Const.SUCCESS_DELETE_CODE, "Logged out successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ResponseDTO(Const.ERROR_EXCEPTION, $"Error logging out: {ex.Message}");
            }
        }

       
    }
}

