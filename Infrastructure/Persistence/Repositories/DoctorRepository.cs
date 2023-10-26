using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal sealed class DoctorRepository : IDoctorRepository
    {
        private readonly RepositoryDbContext _dbContext;
        public DoctorRepository(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Profiles.OfType<Doctor>().ToListAsync(cancellationToken);
        }

        public async Task<Doctor> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Profiles.OfType<Doctor>().FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);
        }

        public void Update(Doctor account, CancellationToken cancellationToken = default)
        {
            _dbContext.Profiles.Update(account);
        }

        public async Task AddAsync(Doctor account)
        {
            await _dbContext.Profiles.AddAsync(account);
        }
    }
}
