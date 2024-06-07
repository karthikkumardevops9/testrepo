using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class TableTabMap : EntityTypeConfiguration<TableTab>
    {
        public TableTabMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("TableTabs");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.BaseView).HasColumnName("BaseView");
            this.Property(t => t.TabOrder).HasColumnName("TabOrder");
            this.Property(t => t.TopTabViewGroup).HasColumnName("TopTabViewGroup");
            this.Property(t => t.TopTab).HasColumnName("TopTab");
            this.Property(t => t.TabSet).HasColumnName("TabSet");
        }
    }
}
