using Contracts.ProfileDto;

namespace Contracts.ReceptionistDto
{
    public class ReceptionistForCreationDto : ProfileForCreationDto
    {
        public Guid OfficeId { get; set; }
    }
}
