using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class vwGetOutputSettingMap : EntityTypeConfiguration<vwGetOutputSetting>
    {
        public vwGetOutputSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.VolumesId);

            // Properties
            this.Property(t => t.VolumesId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("vwGetOutputSetting");
            this.Property(t => t.VolumesId).HasColumnName("VolumesId");
            this.Property(t => t.OutputPath).HasColumnName("OutputPath");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SystemAddressesId).HasColumnName("SystemAddressesId");
            this.Property(t => t.DirDiskMBLimitation).HasColumnName("DirDiskMBLimitation");
            this.Property(t => t.DirCountLimitation).HasColumnName("DirCountLimitation");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Online).HasColumnName("Online");
            this.Property(t => t.OfflineLocation).HasColumnName("OfflineLocation");
            this.Property(t => t.ImageTableName).HasColumnName("ImageTableName");
            this.Property(t => t.DeviceName).HasColumnName("DeviceName");
            this.Property(t => t.PhysicalDriveLetter).HasColumnName("PhysicalDriveLetter");
            this.Property(t => t.ActiveStorage).HasColumnName("ActiveStorage");
            this.Property(t => t.BlockingSize).HasColumnName("BlockingSize");
            this.Property(t => t.DirectorySizeLimits).HasColumnName("DirectorySizeLimits");
            this.Property(t => t.DirectoryMaxEntries).HasColumnName("DirectoryMaxEntries");
        }
    }
}
