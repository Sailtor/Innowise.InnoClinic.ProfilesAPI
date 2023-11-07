using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal sealed class PatientProfileConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");
            builder.HasBaseType<UserProfile>();
            builder.Property(profile => profile.DateOfBirth).IsRequired();
            builder.Property(profile => profile.IsLinkedToAccount).IsRequired();
        }
    }
}
