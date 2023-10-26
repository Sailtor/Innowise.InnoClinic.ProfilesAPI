using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence
{
    internal sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.UseTpcMappingStrategy();
            builder.HasKey(profile => profile.Id);
            builder.Property(profile => profile.Id).ValueGeneratedOnAdd();
            builder.Property(profile => profile.Name).IsRequired().HasMaxLength(1024);
            builder.Property(profile => profile.LastName).IsRequired().HasMaxLength(1024);
            builder.Property(profile => profile.MiddleName).HasMaxLength(1024);
            builder.HasIndex(profile => profile.AccountId)
                .HasDatabaseName("UX_UserProfile_AccountId")
                .IsUnique();
            builder.HasIndex(profile => profile.PhotoId)
                .HasDatabaseName("UX_UserProfile_PhotoId")
                .IsUnique();
        }
    }
}
