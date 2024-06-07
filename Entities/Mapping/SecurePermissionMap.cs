using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SecurePermissionMap : EntityTypeConfiguration<SecurePermission>
    {
        public SecurePermissionMap()
        {
            // Primary Key
            this.HasKey(t => t.PermissionID);

            // Properties
            this.Property(t => t.PermissionID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Permission)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SecurePermission");
            this.Property(t => t.PermissionID).HasColumnName("PermissionID");
            this.Property(t => t.Permission).HasColumnName("Permission");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Ordinal).HasColumnName("Ordinal");
            this.Property(t => t.Children).HasColumnName("Children");
            this.Property(t => t.Indent).HasColumnName("Indent");
        }
    }
}
