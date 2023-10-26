using Contracts.ProfileDto;

namespace Contracts.DoctorDto
{
    public class DoctorForUpdateDto : ProfileForUpdateDto
    {
        public DateOnly DateOfBirth { get; set; }
        public Guid SpecializationId { get; set; }
        public Guid OfficeId { get; set; }
        public int CareerStartYear { get; set; }
    }
}
