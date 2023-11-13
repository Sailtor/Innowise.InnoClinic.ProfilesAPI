using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations
{
    internal sealed class ReceptionistProfileConfiguration : IEntityTypeConfiguration<Receptionist>
    {
        public void Configure(EntityTypeBuilder<Receptionist> builder)
        {
            builder.ToTable("Receptionists");
            builder.HasBaseType<UserProfile>();
            builder.Property(profile => profile.OfficeId).IsRequired();
        }
    }
}
