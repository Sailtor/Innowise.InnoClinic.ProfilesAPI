using Contracts.ProfileDto;

namespace Contracts.PatientDto
{
    public class PatientForCreationDto : ProfileForCreationDto
    {
        public bool IsLinkedToAccount { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
