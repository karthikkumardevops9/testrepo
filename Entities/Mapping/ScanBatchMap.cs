using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ScanBatchMap : EntityTypeConfiguration<ScanBatch>
    {
        public ScanBatchMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("ScanBatches");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.BatchStartDateTime).HasColumnName("BatchStartDateTime");
            this.Property(t => t.PageCount).HasColumnName("PageCount");
            this.Property(t => t.DocumentCount).HasColumnName("DocumentCount");
            this.Property(t => t.BelowDeleteSizeCount).HasColumnName("BelowDeleteSizeCount");
            this.Property(t => t.RescannedCount).HasColumnName("RescannedCount");
            this.Property(t => t.AutoIndexedCount).HasColumnName("AutoIndexedCount");
            this.Property(t => t.LastScanSequence).HasColumnName("LastScanSequence");
            this.Property(t => t.ScanRulesIdUsed).HasColumnName("ScanRulesIdUsed");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.RecordType).HasColumnName("RecordType");
        }
    }
}
