using Domain.Entities;

namespace Domain.Repositories
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Patient> GetByIdAsync(Guid patientId, CancellationToken cancellationToken = default);
        void Update(Patient profile, CancellationToken cancellationToken = default);
        Task AddAsync(Patient profile);
        void Remove(Patient profile);
    }
}
