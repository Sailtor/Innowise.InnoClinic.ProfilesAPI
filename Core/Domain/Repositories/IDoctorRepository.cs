using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Doctor> GetByIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Doctor>> FindAsync(Expression<Func<Doctor, bool>> expression, CancellationToken cancellationToken = default);
        void Update(Doctor profile, CancellationToken cancellationToken = default);
        Task AddAsync(Doctor profile);
    }
}
