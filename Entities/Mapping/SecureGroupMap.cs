using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SecureGroupMap : EntityTypeConfiguration<SecureGroup>
    {
        public SecureGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.GroupID);

            // Properties
            this.Property(t => t.GroupName)
                .IsRequired();

            this.Property(t => t.Description)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SecureGroup");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.GroupType).HasColumnName("GroupType");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.ActiveDirectoryGroup).HasColumnName("ActiveDirectoryGroup");
            this.Property(t => t.ActiveDirectoryPath).HasColumnName("ActiveDirectoryPath");
            this.Property(t => t.AutoLogOffSeconds).HasColumnName("AutoLogOffSeconds");
            this.Property(t => t.AutoLockSeconds).HasColumnName("AutoLockSeconds");
            this.Property(t => t.C_oldGroupID).HasColumnName("_oldGroupID");
        }
    }
}
