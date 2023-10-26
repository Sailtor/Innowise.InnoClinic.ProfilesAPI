namespace Contracts.ProfileDto
{
    public class ProfileForCreationDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? PhotoId { get; set; }
    }
}
