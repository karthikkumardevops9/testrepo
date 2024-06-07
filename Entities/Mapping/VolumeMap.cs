using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class VolumeMap : EntityTypeConfiguration<Volume>
    {
        public VolumeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Volumes");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PathName).HasColumnName("PathName");
            this.Property(t => t.SystemAddressesId).HasColumnName("SystemAddressesId");
            this.Property(t => t.OSN_Volume).HasColumnName("OSN_Volume");
            this.Property(t => t.DeviceTypeId).HasColumnName("DeviceTypeId");
            this.Property(t => t.DirDiskMBLimitation).HasColumnName("DirDiskMBLimitation");
            this.Property(t => t.DirCountLimitation).HasColumnName("DirCountLimitation");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Online).HasColumnName("Online");
            this.Property(t => t.OfflineLocation).HasColumnName("OfflineLocation");
            this.Property(t => t.ImageTableName).HasColumnName("ImageTableName");
            this.Property(t => t.ViewGroup).HasColumnName("ViewGroup");
        }
    }
}
