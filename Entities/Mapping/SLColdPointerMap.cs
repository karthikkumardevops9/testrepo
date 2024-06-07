using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLColdPointerMap : EntityTypeConfiguration<SLColdPointer>
    {
        public SLColdPointerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLColdPointers");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TrackablesId).HasColumnName("TrackablesId");
            this.Property(t => t.TrackablesRecordVersion).HasColumnName("TrackablesRecordVersion");
            this.Property(t => t.ScanBatchesId).HasColumnName("ScanBatchesId");
            this.Property(t => t.ScanSequence).HasColumnName("ScanSequence");
            this.Property(t => t.ArchiveId).HasColumnName("ArchiveId");
            this.Property(t => t.StartingPage).HasColumnName("StartingPage");
            this.Property(t => t.Pages).HasColumnName("Pages");
            this.Property(t => t.ScanDateTime).HasColumnName("ScanDateTime");
        }
    }
}
