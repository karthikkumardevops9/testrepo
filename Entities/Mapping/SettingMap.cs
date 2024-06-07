using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MSRecordsEngine.Entities;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Settings");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Section).HasColumnName("Section");
            this.Property(t => t.Item).HasColumnName("Item");
            this.Property(t => t.ItemValue).HasColumnName("ItemValue");
        }
    }
}
