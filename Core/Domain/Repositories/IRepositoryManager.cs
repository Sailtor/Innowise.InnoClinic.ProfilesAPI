namespace Domain.Repositories
{
    public interface IRepositoryManager
    {
        IPatientRepository PatientRepository { get; }
        IDoctorRepository DoctorRepository { get; }
        IReceptionistRepository ReceptionistRepository { get; }
        IUnitOfWork UnitOfWork { get; }

    }
}
