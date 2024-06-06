using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Models.Mapping
{
    public class SLIndexerMap : EntityTypeConfiguration<SLIndexer>
    {
        public SLIndexerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLIndexer");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IndexType).HasColumnName("IndexType");
            this.Property(t => t.IndexTableName).HasColumnName("IndexTableName");
            this.Property(t => t.IndexFieldName).HasColumnName("IndexFieldName");
            this.Property(t => t.IndexTableId).HasColumnName("IndexTableId");
            this.Property(t => t.IndexData).HasColumnName("IndexData");
            this.Property(t => t.OrphanType).HasColumnName("OrphanType");
            this.Property(t => t.RecordVersion).HasColumnName("RecordVersion");
            this.Property(t => t.PageNumber).HasColumnName("PageNumber");
            this.Property(t => t.AttachmentNumber).HasColumnName("AttachmentNumber");
        }
    }
}
