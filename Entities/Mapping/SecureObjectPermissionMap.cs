using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SecureObjectPermissionMap : EntityTypeConfiguration<SecureObjectPermission>
    {
        public SecureObjectPermissionMap()
        {
            // Primary Key
            this.HasKey(t => t.SecureObjectPermissionID);

            // Properties
            // Table & Column Mappings
            this.ToTable("SecureObjectPermission");
            this.Property(t => t.SecureObjectPermissionID).HasColumnName("SecureObjectPermissionID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.SecureObjectID).HasColumnName("SecureObjectID");
            this.Property(t => t.PermissionID).HasColumnName("PermissionID");

            // Relationships
            this.HasRequired(t => t.SecureGroup)
                .WithMany(t => t.SecureObjectPermissions)
                .HasForeignKey(d => d.GroupID);
            this.HasRequired(t => t.SecureObject)
                .WithMany(t => t.SecureObjectPermissions)
                .HasForeignKey(d => d.SecureObjectID);
            this.HasRequired(t => t.SecurePermission)
                .WithMany(t => t.SecureObjectPermissions)
                .HasForeignKey(d => d.PermissionID);

        }
    }
}
