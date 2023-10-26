using AutoMapper;
using BLL.Infrastructure.Validators;
using Contracts.DoctorDto;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using FluentValidation;
using Services.Abstractions;

namespace Services
{
    internal sealed class DoctorService : IDoctorService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IValidatorManager _validatorManager;

        public DoctorService(IRepositoryManager repositoryManager, IMapper mapper, IValidatorManager validatorManager)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _validatorManager = validatorManager;
        }

        public async Task<IEnumerable<DoctorForResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var doctors = await _repositoryManager.DoctorRepository.GetAllAsync(cancellationToken);
            var doctorsDto = _mapper.Map<IEnumerable<DoctorForResponseDto>>(doctors);
            return doctorsDto;
        }

        public async Task<DoctorForResponseDto> GetByIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var doctor = await _repositoryManager.DoctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor is null)
            {
                throw new ProfileNotFoundException(doctorId);
            }
            var doctorDto = _mapper.Map<DoctorForResponseDto>(doctor);
            return doctorDto;
        }

        public async Task<DoctorForResponseDto> CreateAsync(DoctorForCreationDto doctorForCreationDto, CancellationToken cancellationToken = default)
        {
            _validatorManager.DoctorCreationValidator.ValidateAndThrowCustom(doctorForCreationDto);
            var doctor = _mapper.Map<Doctor>(doctorForCreationDto);
            await _repositoryManager.DoctorRepository.AddAsync(doctor);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<DoctorForResponseDto>(doctor);
        }

        public async Task UpdateAsync(Guid doctorId, DoctorForUpdateDto doctorForUpdateDto, CancellationToken cancellationToken = default)
        {
            _validatorManager.DoctorUpdateValidator.ValidateAndThrowCustom(doctorForUpdateDto);
            var doctor = await _repositoryManager.DoctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor is null)
            {
                throw new ProfileNotFoundException(doctorId);
            }
            _mapper.Map(doctorForUpdateDto, doctor);
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeDoctorStatusAsync(Guid doctorId, DoctorStatus status, CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(typeof(DoctorStatus), status))
            {
                throw new ValidationException("Invalid doctor status: " + status);
            }
            var doctor = await _repositoryManager.DoctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor is null)
            {
                throw new ProfileNotFoundException(doctorId);
            }
            doctor.Status = status;
            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
