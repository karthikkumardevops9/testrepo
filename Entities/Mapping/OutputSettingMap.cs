using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class OutputSettingMap : EntityTypeConfiguration<OutputSetting>
    {
        public OutputSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.DefaultOutputSettingsId);

            // Properties
            this.Property(t => t.Id)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("OutputSettings");
            this.Property(t => t.DefaultOutputSettingsId).HasColumnName("DefaultOutputSettingsId");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DirectoriesId).HasColumnName("DirectoriesId");
            this.Property(t => t.FileNamePrefix).HasColumnName("FileNamePrefix");
            this.Property(t => t.FileExtension).HasColumnName("FileExtension");
            this.Property(t => t.NextDocNum).HasColumnName("NextDocNum");
            this.Property(t => t.ViewGroup).HasColumnName("ViewGroup");
            this.Property(t => t.InActive).HasColumnName("InActive");
            this.Property(t => t.VolumesId).HasColumnName("VolumesId");
        }
    }
}
