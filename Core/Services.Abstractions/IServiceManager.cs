namespace Services.Abstractions
{
    public interface IServiceManager
    {
        IPatientService PatientService { get; }
        IDoctorService DoctorService { get; }
        IReceptionistService ReceptionistService { get; }
    }
}
