using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ImportFieldMap : EntityTypeConfiguration<ImportField>
    {
        public ImportFieldMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);
            
            // Table & Column Mappings
            this.ToTable("ImportFields");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SwingYear).HasColumnName("SwingYear");
            this.Property(t => t.ImportLoad).HasColumnName("ImportLoad");
            this.Property(t => t.ReadOrder).HasColumnName("ReadOrder");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.StartPosition).HasColumnName("StartPosition");
            this.Property(t => t.EndPosition).HasColumnName("EndPosition");
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
            this.Property(t => t.DateFormat).HasColumnName("DateFormat");
        }
    }
}
