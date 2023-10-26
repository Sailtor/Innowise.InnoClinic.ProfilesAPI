using Contracts.ProfileDto;

namespace Contracts.ReceptionistDto
{
    public class ReceptionistForResponseDto : ProfileForResponseDto
    {
        public Guid OfficeId { get; set; }
    }
}
