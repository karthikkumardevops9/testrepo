using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SystemAddressMap : EntityTypeConfiguration<SystemAddress>
    {
        public SystemAddressMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SystemAddresses");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DeviceName).HasColumnName("DeviceName");
            this.Property(t => t.PhysicalDriveLetter).HasColumnName("PhysicalDriveLetter");
            this.Property(t => t.RequireTemporary).HasColumnName("RequireTemporary");
            this.Property(t => t.ActiveStorage).HasColumnName("ActiveStorage");
            this.Property(t => t.BlockingSize).HasColumnName("BlockingSize");
            this.Property(t => t.DirectorySizeLimits).HasColumnName("DirectorySizeLimits");
            this.Property(t => t.DirectoryMaxEntries).HasColumnName("DirectoryMaxEntries");
        }
    }
}
