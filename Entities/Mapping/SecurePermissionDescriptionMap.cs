using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SecurePermissionDescriptionMap : EntityTypeConfiguration<SecurePermissionDescription>
    {
        public SecurePermissionDescriptionMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SecureObjectID, t.PermissionID });

            // Properties
            this.Property(t => t.SecureObjectID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PermissionID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Name)
                .IsRequired();

            this.Property(t => t.Permission)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SecurePermissionDescription");
            this.Property(t => t.SecureObjectID).HasColumnName("SecureObjectID");
            this.Property(t => t.PermissionID).HasColumnName("PermissionID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Permission).HasColumnName("Permission");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
