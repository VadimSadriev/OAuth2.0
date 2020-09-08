using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OAuth.Server.Data.Entities;

namespace OAuth.Server.Data.EntitityConfigurations
{
    public class TokenConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .HasColumnName("id");

            builder.Property(x => x.AuthCodeId)
                .IsRequired()
                .HasColumnName("auth_code_id");

            builder.Property(x => x.Expiration)
                .IsRequired()
                .HasColumnName("expiration");

            builder.Property(x => x.IsExpired)
                .HasColumnName("is_expired");

            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("typ");
        }
    }
}
