using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class GridSettingMap : EntityTypeConfiguration<GridSetting>
    {
        public GridSettingMap()
        {
            // Primary Key
            this.HasKey(t => t.GridSettingsId);

            // Properties
            this.Property(t => t.GridSettingsName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("GridSettings");
            this.Property(t => t.GridSettingsId).HasColumnName("GridSettingsId");
            this.Property(t => t.GridSettingsName).HasColumnName("GridSettingsName");
        }
    }
}
