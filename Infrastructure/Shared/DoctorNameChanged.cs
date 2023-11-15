namespace Shared
{
    public interface DoctorNameChanged
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Middlename { get; set; }
    }
}
