using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal sealed class DoctorProfileConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");
            builder.HasBaseType<UserProfile>();
            builder.Property(profile => profile.DateOfBirth).IsRequired();
            builder.Property(profile => profile.SpecializationId).IsRequired();
            builder.Property(profile => profile.OfficeId).IsRequired();
            builder.Property(profile => profile.CareerStartYear).IsRequired();
            builder.Property(profile => profile.Status).IsRequired().HasDefaultValue(DoctorStatus.AtWork);
        }
    }
}
