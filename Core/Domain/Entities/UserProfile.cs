namespace Domain.Entities
{
    public abstract class UserProfile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? PhotoId { get; set; }
    }
}
