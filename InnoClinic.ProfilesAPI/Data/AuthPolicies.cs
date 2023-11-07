namespace InnoClinic.ProfilesAPI.Data
{
    public static class AuthPolicies
    {
        public const string OwnerOrReceptionist = "OwnerOrReceptionist";
        public const string OwnerOrReceptionistOrDoctor = "OwnerOrDoctorOrReceptionist";
    }
}
