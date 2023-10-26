using Contracts.DoctorDto;
using Domain.Entities;

namespace Services.Abstractions
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorForResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<DoctorForResponseDto> GetByIdAsync(Guid doctorId, CancellationToken cancellationToken);
        Task<DoctorForResponseDto> CreateAsync(DoctorForCreationDto doctorForCreationDto, CancellationToken cancellationToken = default);
        Task ChangeDoctorStatusAsync(Guid doctorId, DoctorStatus status, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid doctorId, DoctorForUpdateDto doctorForUpdateDto, CancellationToken cancellationToken = default);
    }
}
