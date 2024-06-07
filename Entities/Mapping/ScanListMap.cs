using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ScanListMap : EntityTypeConfiguration<ScanList>
    {
        public ScanListMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("ScanList");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ScanOrder).HasColumnName("ScanOrder");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.FieldType).HasColumnName("FieldType");
            this.Property(t => t.IdStripChars).HasColumnName("IdStripChars");
            this.Property(t => t.IdMask).HasColumnName("IdMask");
        }
    }
}
