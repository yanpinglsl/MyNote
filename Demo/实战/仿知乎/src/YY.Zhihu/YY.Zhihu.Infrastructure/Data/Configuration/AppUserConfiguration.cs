using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YY.Zhihu.Domain;
using YY.Zhihu.Domain.AppUserAggerate.Entites;

namespace YY.Zhihu.Infrastructure.Configuration
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Nickname)
                .HasMaxLength(DataSchemaConstants.DefaultAppUserNickNameLength);

            builder.Property(p => p.Bio)
                .HasMaxLength(DataSchemaConstants.DefaultAppUserBioLength);
        }
    }
}
