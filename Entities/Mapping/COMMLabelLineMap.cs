using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class COMMLabelLineMap : EntityTypeConfiguration<COMMLabelLine>
    {
        public COMMLabelLineMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("COMMLabelLines");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.COMMLabelId).HasColumnName("COMMLabelId");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.TextStartString).HasColumnName("TextStartString");
            this.Property(t => t.TextEndString).HasColumnName("TextEndString");
            this.Property(t => t.FindByField).HasColumnName("FindByField");
        }
    }
}
