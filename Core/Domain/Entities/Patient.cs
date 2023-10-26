namespace Domain.Entities
{
    public class Patient : UserProfile
    {
        public bool IsLinkedToAccount { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}
