using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SecureUserMap : EntityTypeConfiguration<SecureUser>
    {
        public SecureUserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserID);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired();

            this.Property(t => t.PasswordHash)
                .IsRequired();

            this.Property(t => t.FullName)
                .IsRequired();

            this.Property(t => t.Email)
                .IsRequired();

            this.Property(t => t.AccountType)
                .IsRequired()
                .IsFixedLength();

            // Table & Column Mappings
            this.ToTable("SecureUser");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            this.Property(t => t.FullName).HasColumnName("FullName");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.AccountDisabled).HasColumnName("AccountDisabled");
            this.Property(t => t.AccountType).HasColumnName("AccountType");
            this.Property(t => t.PasswordUpdate).HasColumnName("PasswordUpdate");
            this.Property(t => t.C_oldPassword).HasColumnName("_oldPassword");
            this.Property(t => t.MustChangePassword).HasColumnName("MustChangePassword");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Misc1).HasColumnName("Misc1");
            this.Property(t => t.Misc2).HasColumnName("Misc2");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
        }
    }
}
