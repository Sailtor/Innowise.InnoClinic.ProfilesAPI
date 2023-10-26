using Contracts.ProfileDto;

namespace Contracts.PatientDto
{
    public class PatientForResponseDto : ProfileForResponseDto
    {
        public bool IsLinkedToAccount { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
