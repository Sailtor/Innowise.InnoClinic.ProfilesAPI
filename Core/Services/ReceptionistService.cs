using AutoMapper;
using BLL.Infrastructure.Validators;
using Contracts.ReceptionistDto;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    internal sealed class ReceptionistService : IReceptionistService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public ReceptionistService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async Task<IEnumerable<ReceptionistForResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var receptionists = await _repositoryManager.ReceptionistRepository.GetAllAsync(cancellationToken);
            var receptionistsDto = _mapper.Map<IEnumerable<ReceptionistForResponseDto>>(receptionists);
            return receptionistsDto;
        }

        public async Task<ReceptionistForResponseDto> GetByIdAsync(Guid receptionistId, CancellationToken cancellationToken = default)
        {
            var receptionist = await _repositoryManager.ReceptionistRepository.GetByIdAsync(receptionistId, cancellationToken);
            if (receptionist is null)
            {
                throw new ProfileNotFoundException(receptionistId);
            }
            var receptionistDto = _mapper.Map<ReceptionistForResponseDto>(receptionist);
            return receptionistDto;
        }

        public async Task<ReceptionistForResponseDto> CreateAsync(ReceptionistForCreationDto receptionistForCreationDto, CancellationToken cancellationToken = default)
        {
            _validatorManager.ReceptionistCreationValidator.ValidateAndThrowCustom(receptionistForCreationDto);
            var receptionist = _mapper.Map<Receptionist>(receptionistForCreationDto);
            await _repositoryManager.ReceptionistRepository.AddAsync(receptionist);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ReceptionistForResponseDto>(receptionist);
        }

        public async Task UpdateAsync(Guid receptionistId, ReceptionistForUpdateDto receptionistForUpdateDto, CancellationToken cancellationToken = default)
        {
            _validatorManager.ReceptionistUpdateValidator.ValidateAndThrowCustom(receptionistForUpdateDto);
            var receptionist = await _repositoryManager.ReceptionistRepository.GetByIdAsync(receptionistId, cancellationToken);
            if (receptionist is null)
            {
                throw new ProfileNotFoundException(receptionistId);
            }
            _mapper.Map(receptionistForUpdateDto, receptionist);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid receptionistId, CancellationToken cancellationToken = default)
        {
            var receptionist = await _repositoryManager.ReceptionistRepository.GetByIdAsync(receptionistId, cancellationToken);
            if (receptionist is null)
            {
                throw new ProfileNotFoundException(receptionistId);
            }
            _repositoryManager.ReceptionistRepository.Remove(receptionist);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
