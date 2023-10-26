namespace Contracts.ProfileDto
{
    public class ProfileForUpdateDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public Guid? PhotoId { get; set; }

    }
}
