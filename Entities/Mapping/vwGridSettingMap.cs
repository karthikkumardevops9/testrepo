using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class vwGridSettingMap : EntityTypeConfiguration<vwGridSetting>
    {
        public vwGridSettingMap()
        {
            // Primary Key
            this.HasKey(t => new { t.GridSettingsId, t.GridSettingsName, t.GridColumnId, t.GridColumnSrNo, t.GridColumnName, t.GridColumnDisplayName, t.IsActive, t.IsSortable, t.IsCheckbox });

            // Properties
            this.Property(t => t.GridSettingsId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.GridSettingsName)
                .IsRequired();

            this.Property(t => t.GridColumnId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.GridColumnSrNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.GridColumnName)
                .IsRequired();

            this.Property(t => t.GridColumnDisplayName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("vwGridSettings");
            this.Property(t => t.GridSettingsId).HasColumnName("GridSettingsId");
            this.Property(t => t.GridSettingsName).HasColumnName("GridSettingsName");
            this.Property(t => t.GridColumnId).HasColumnName("GridColumnId");
            this.Property(t => t.GridColumnSrNo).HasColumnName("GridColumnSrNo");
            this.Property(t => t.GridColumnName).HasColumnName("GridColumnName");
            this.Property(t => t.GridColumnDisplayName).HasColumnName("GridColumnDisplayName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsSortable).HasColumnName("IsSortable");
            this.Property(t => t.IsCheckbox).HasColumnName("IsCheckbox");
        }
    }
}
