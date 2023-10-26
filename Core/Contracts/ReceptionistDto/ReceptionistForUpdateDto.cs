using Contracts.ProfileDto;

namespace Contracts.ReceptionistDto
{
    public class ReceptionistForUpdateDto : ProfileForUpdateDto
    {
        public Guid OfficeId { get; set; }
    }
}
