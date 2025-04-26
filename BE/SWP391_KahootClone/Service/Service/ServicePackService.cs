using AutoMapper;
using AutoMapper.Features;
using Azure.Core;
using Repository.DTO;
using Repository.Models;
using Repository.Repositories;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.DTO.RequestDTO;

namespace Service.Service
{
    public class ServicePackService : IServicePackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ServicePackRepository _servicePackRepository;
        public ServicePackService(ServicePackRepository servicePackRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _servicePackRepository = servicePackRepository ?? throw new ArgumentNullException(nameof(servicePackRepository));

        }
        public async Task<ResponseDTO> CreeateServicePack(CreateServicePackRequestDTO request)
        {
            try
            {
                var servicePack = new ServicePack
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    DurationDays = request.DurationDays,
                    Features = request.Features,
                    CreatedAt = DateTime.UtcNow
                };
                await _servicePackRepository.CreateAsync(servicePack);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseDTO(201, "Service pack created successfully", servicePack);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error creating game session: {ex.Message}");
            }
        }
        public async Task<ResponseDTO> GetServicePackList()
        {
            try
            {
                var servicePackList = await _servicePackRepository.GetAllAsync();
                var response = servicePackList.Select(x => new ServicePackageList
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    DurationDays = x.DurationDays,
                    Features = x.Features,
                    Price = x.Price
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
