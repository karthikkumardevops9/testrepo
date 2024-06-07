using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLSignatureMap : EntityTypeConfiguration<SLSignature>
    {
        public SLSignatureMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLSignature");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Table).HasColumnName("Table");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.Signature).HasColumnName("Signature");
        }
    }
}
