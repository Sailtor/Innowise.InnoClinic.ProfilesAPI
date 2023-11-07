using Domain.Repositories;

namespace Persistence.Repositories
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IPatientRepository> _lazyPatientRepository;
        private readonly Lazy<IDoctorRepository> _lazyDoctorRepository;
        private readonly Lazy<IReceptionistRepository> _lazyReceptionistRepository;
        private readonly Lazy<IUnitOfWork> _lazyUnitOfWork;

        public RepositoryManager(RepositoryDbContext dbContext)
        {
            _lazyPatientRepository = new Lazy<IPatientRepository>(() => new PatientRepository(dbContext));
            _lazyDoctorRepository = new Lazy<IDoctorRepository>(() => new DoctorRepository(dbContext));
            _lazyReceptionistRepository = new Lazy<IReceptionistRepository>(() => new ReceptionistRepository(dbContext));
            _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
        }

        public IPatientRepository PatientRepository => _lazyPatientRepository.Value;
        public IDoctorRepository DoctorRepository => _lazyDoctorRepository.Value;
        public IReceptionistRepository ReceptionistRepository => _lazyReceptionistRepository.Value;
        public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
    }
}
