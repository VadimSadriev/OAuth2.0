using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OAuth.Server.Data.Entities;

namespace OAuth.Server.Data.EntitityConfigurations
{
    public class AuthCodeConfiguration : IEntityTypeConfiguration<AuthCode>
    {
        public void Configure(EntityTypeBuilder<AuthCode> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.ClientId)
                .IsRequired()
                .HasColumnName("client_id");
            builder.Property(x => x.Expiration)
                .IsRequired()
                .HasColumnName("expiration");

            builder.Property(x => x.RedirectUri)
                .IsRequired()
                .HasColumnName("redirect_uri");

            builder.Property(x => x.AccountId)
                .IsRequired()
                .HasColumnName("account_id");

            builder.HasOne(x => x.Account)
                .WithMany()
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
