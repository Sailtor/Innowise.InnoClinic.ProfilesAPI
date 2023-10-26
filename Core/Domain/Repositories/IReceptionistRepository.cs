using Domain.Entities;

namespace Domain.Repositories
{
    public interface IReceptionistRepository
    {
        Task<IEnumerable<Receptionist>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Receptionist> GetByIdAsync(Guid receptionistId, CancellationToken cancellationToken = default);
        void Update(Receptionist profile, CancellationToken cancellationToken = default);
        Task AddAsync(Receptionist profile);
        void Remove(Receptionist profile);
    }
}
