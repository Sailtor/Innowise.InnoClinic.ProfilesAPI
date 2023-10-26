using Contracts.ReceptionistDto;

namespace Services.Abstractions
{
    public interface IReceptionistService
    {
        Task<IEnumerable<ReceptionistForResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ReceptionistForResponseDto> GetByIdAsync(Guid receptionistId, CancellationToken cancellationToken);
        Task<ReceptionistForResponseDto> CreateAsync(ReceptionistForCreationDto receptionistForCreationDto, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid receptionistId, ReceptionistForUpdateDto receptionistForUpdateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid receptionistId, CancellationToken cancellationToken = default);
    }
}
