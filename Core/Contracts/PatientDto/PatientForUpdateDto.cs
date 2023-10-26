using Contracts.ProfileDto;

namespace Contracts.PatientDto
{
    public class PatientForUpdateDto : ProfileForUpdateDto
    {
        public DateOnly DateOfBirth { get; set; }
    }
}
