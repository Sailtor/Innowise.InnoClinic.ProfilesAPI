using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal sealed class PatientRepository : IPatientRepository
    {
        private readonly RepositoryDbContext _dbContext;
        public PatientRepository(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Patient>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Profiles.OfType<Patient>().ToListAsync(cancellationToken);
        }

        public async Task<Patient> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Profiles.OfType<Patient>().FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);
        }

        public void Update(Patient account, CancellationToken cancellationToken = default)
        {
            _dbContext.Profiles.Update(account);
        }

        public async Task AddAsync(Patient account)
        {
            await _dbContext.Profiles.AddAsync(account);
        }

        public void Remove(Patient profile)
        {
            _dbContext.Profiles.Remove(profile);
        }
    }
}
