using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

public class UserService : IUserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResponseDTO> CreateUserAsync(UserDTO userDTO)
    {
        var user = new User
        {
            Username = userDTO.Username,
            Email = userDTO.Email,
            PasswordHash = userDTO.Password, // Should hash in production
            Role = userDTO.Role,
            AvatarUrl = userDTO.AvatarUrl,
            IsActive = userDTO.IsActive,
            //Status = userDTO.Status,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        return new ResponseDTO(201, "User created successfully", user);
    }

    public async Task<ResponseDTO> UpadteUserAsync(UserDTO userDTO, int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new ResponseDTO(404, "User not found");
        }

        // Update fields
        user.Username = userDTO.Username;
        user.Email = userDTO.Email;
        user.PasswordHash = userDTO.Password; // Again, consider hashing
        user.Role = userDTO.Role;
        user.AvatarUrl = userDTO.AvatarUrl;
        user.IsActive = userDTO.IsActive;
        //user.Status = userDTO.Status;

        await _userRepository.UpdateAsync(user);

        return new ResponseDTO(200, "User updated successfully", user);
    }

    public async Task<bool> RemoveUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        await _userRepository.RemoveAsync(user);
        return true;
    }

    public async Task<ResponseDTO> GetByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new ResponseDTO(404, "User not found");
        }

        var userDTO = new UserDTO
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Password = user.PasswordHash,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
          //  Status = user.Status,
            CreatedAt = user.CreatedAt
        };

        return new ResponseDTO(200, "User retrieved successfully", userDTO);
    }

    public async Task<ResponseDTO> GetAllUserAsync()
    {
        var users = await _userRepository.GetAllAsync();

        var userDTOs = new List<UserDTO>();
        foreach (var user in users)
        {
            userDTOs.Add(new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Password = user.PasswordHash,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                //Status = user.Status,
                CreatedAt = user.CreatedAt
            });
        }

        return new ResponseDTO(200, "Users retrieved successfully", userDTOs);
    }
}
