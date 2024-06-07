using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SecureUserGroupMap : EntityTypeConfiguration<SecureUserGroup>
    {
        public SecureUserGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("SecureUserGroup");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");

            // Relationships
            this.HasRequired(t => t.SecureGroup)
                .WithMany(t => t.SecureUserGroups)
                .HasForeignKey(d => d.GroupID);
            this.HasRequired(t => t.SecureUser)
                .WithMany(t => t.SecureUserGroups)
                .HasForeignKey(d => d.UserID);

        }
    }
}
