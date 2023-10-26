using Contracts.PatientDto;

namespace Services.Abstractions
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientForResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<PatientForResponseDto> GetByIdAsync(Guid patientId, CancellationToken cancellationToken);
        Task<PatientForResponseDto> CreateAsync(PatientForCreationDto patientForCreationDto, CancellationToken cancellationToken = default);
        Task LinkUserProfileToAccountAsync(Guid patientId, Guid userAccountId, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid patientId, PatientForUpdateDto patientForUpdateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid patientId, CancellationToken cancellationToken = default);
    }
}
