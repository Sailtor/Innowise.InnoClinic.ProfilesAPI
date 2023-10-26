using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    internal sealed class ReceptionistRepository : IReceptionistRepository
    {
        private readonly RepositoryDbContext _dbContext;
        public ReceptionistRepository(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Receptionist>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Profiles.OfType<Receptionist>().ToListAsync(cancellationToken);
        }
        public async Task<Receptionist> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Profiles.OfType<Receptionist>().FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);
        }
        public void Update(Receptionist account, CancellationToken cancellationToken = default)
        {
            _dbContext.Profiles.Update(account);
        }

        public async Task AddAsync(Receptionist account)
        {
            await _dbContext.Profiles.AddAsync(account);
        }

        public void Remove(Receptionist profile)
        {
            _dbContext.Profiles.Remove(profile);
        }
    }
}
