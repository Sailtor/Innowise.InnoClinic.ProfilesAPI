using AutoMapper;
using Domain.Repositories;
using MassTransit;
using Services.Abstractions;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IPatientService> _lazyPatientService;
        private readonly Lazy<IDoctorService> _lazyDoctorService;
        private readonly Lazy<IReceptionistService> _lazyReceptionistService;

        public ServiceManager(
            IRepositoryManager repositoryManager,
            IMapper mapper,
            IValidatorManager validatorManager,
            IPublishEndpoint publishEndpoint)
        {
            _lazyPatientService = new Lazy<IPatientService>(() => new PatientService(repositoryManager, mapper, validatorManager));
            _lazyDoctorService = new Lazy<IDoctorService>(() => new DoctorService(repositoryManager, mapper, validatorManager, publishEndpoint));
            _lazyReceptionistService = new Lazy<IReceptionistService>(() => new ReceptionistService(repositoryManager, mapper, validatorManager));
        }
        public IPatientService PatientService => _lazyPatientService.Value;
        public IDoctorService DoctorService => _lazyDoctorService.Value;
        public IReceptionistService ReceptionistService => _lazyReceptionistService.Value;

    }
}
