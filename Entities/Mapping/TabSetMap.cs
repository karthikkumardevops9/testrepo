using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class TabSetMap : EntityTypeConfiguration<TabSet>
    {
        public TabSetMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            
            // Table & Column Mappings
            this.ToTable("TabSets");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.TabFontName).HasColumnName("TabFontName");
            this.Property(t => t.TabFontSize).HasColumnName("TabFontSize");
            this.Property(t => t.TabFontBold).HasColumnName("TabFontBold");
            this.Property(t => t.TabMaxWidth).HasColumnName("TabMaxWidth");
            this.Property(t => t.TabCutSize).HasColumnName("TabCutSize");
            this.Property(t => t.TabHeight).HasColumnName("TabHeight");
            this.Property(t => t.TabRowOffset).HasColumnName("TabRowOffset");
            this.Property(t => t.TabShape).HasColumnName("TabShape");
            this.Property(t => t.ViewGroup).HasColumnName("ViewGroup");
            this.Property(t => t.StartupTabset).HasColumnName("StartupTabset");
            this.Property(t => t.Picture).HasColumnName("Picture");
        }
    }
}
