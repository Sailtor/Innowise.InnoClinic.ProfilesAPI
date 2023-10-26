namespace Domain.Exceptions
{
    public sealed class ProfileNotFoundException : NotFoundException
    {
        public ProfileNotFoundException(Guid profileId)
            : base($"The profile with the identifier {profileId} was not found.")
        {
        }
    }
}
