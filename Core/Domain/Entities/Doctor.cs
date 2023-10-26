namespace Domain.Entities
{
    public class Doctor : UserProfile
    {
        public DateOnly DateOfBirth { get; set; }
        public Guid SpecializationId { get; set; }
        public Guid OfficeId { get; set; }
        public int CareerStartYear { get; set; }
        public DoctorStatus Status { get; set; }
    }
}
