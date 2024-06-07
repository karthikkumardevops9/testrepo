using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLTextSearchItemMap : EntityTypeConfiguration<SLTextSearchItem>
    {
        public SLTextSearchItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.IndexTableName)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SLTextSearchItems");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IndexType).HasColumnName("IndexType");
            this.Property(t => t.IndexTableName).HasColumnName("IndexTableName");
            this.Property(t => t.IndexFieldName).HasColumnName("IndexFieldName");
            this.Property(t => t.IndexTableId).HasColumnName("IndexTableId");
        }
    }
}
