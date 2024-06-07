using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class GridColumnMap : EntityTypeConfiguration<GridColumn>
    {
        public GridColumnMap()
        {
            // Primary Key
            this.HasKey(t => t.GridColumnId);

            // Properties
            this.Property(t => t.GridColumnName)
                .IsRequired();

            this.Property(t => t.GridColumnDisplayName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("GridColumn");
            this.Property(t => t.GridColumnId).HasColumnName("GridColumnId");
            this.Property(t => t.GridSettingsId).HasColumnName("GridSettingsId");
            this.Property(t => t.GridColumnSrNo).HasColumnName("GridColumnSrNo");
            this.Property(t => t.GridColumnName).HasColumnName("GridColumnName");
            this.Property(t => t.GridColumnDisplayName).HasColumnName("GridColumnDisplayName");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsSortable).HasColumnName("IsSortable");
            this.Property(t => t.IsCheckbox).HasColumnName("IsCheckbox");

            // Relationships
            this.HasRequired(t => t.GridSetting)
                .WithMany(t => t.GridColumns)
                .HasForeignKey(d => d.GridSettingsId);

        }
    }
}
