using AutoMapper;
using BLL.Infrastructure.Validators;
using Contracts.PatientDto;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Services.Abstractions;

namespace Services
{
    internal sealed class PatientService : IPatientService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public PatientService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }
        public async Task<IEnumerable<PatientForResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var patients = await _repositoryManager.PatientRepository.GetAllAsync(cancellationToken);
            var patientsDto = _mapper.Map<IEnumerable<PatientForResponseDto>>(patients);
            return patientsDto;
        }
        public async Task<PatientForResponseDto> GetByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _repositoryManager.PatientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient is null)
            {
                throw new ProfileNotFoundException(patientId);
            }
            var patientDto = _mapper.Map<PatientForResponseDto>(patient);
            return patientDto;
        }
        public async Task<PatientForResponseDto> CreateAsync(PatientForCreationDto PatientForCreationDto, CancellationToken cancellationToken = default)
        {
            _validatorManager.PatientCreationValidator.ValidateAndThrowCustom(PatientForCreationDto);
            var patient = _mapper.Map<Patient>(PatientForCreationDto);
            await _repositoryManager.PatientRepository.AddAsync(patient);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PatientForResponseDto>(patient);
        }
        public async Task UpdateAsync(Guid patientId, PatientForUpdateDto patientForUpdateDto, CancellationToken cancellationToken = default)
        {
            _validatorManager.PatientUpdateValidator.ValidateAndThrowCustom(patientForUpdateDto);
            var patient = await _repositoryManager.PatientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient is null)
            {
                throw new ProfileNotFoundException(patientId);
            }
            _mapper.Map(patientForUpdateDto, patient);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        public async Task LinkUserProfileToAccountAsync(Guid patientId, Guid userAccountId, CancellationToken cancellationToken = default)
        {
            var patient = await _repositoryManager.PatientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient is null)
            {
                throw new ProfileNotFoundException(patientId);
            }
            patient.AccountId = userAccountId;
            patient.IsLinkedToAccount = true;
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(Guid patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _repositoryManager.PatientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient is null)
            {
                throw new ProfileNotFoundException(patientId);
            }
            _repositoryManager.PatientRepository.Remove(patient);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
