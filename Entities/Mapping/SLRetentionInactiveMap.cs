using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLRetentionInactiveMap : EntityTypeConfiguration<SLRetentionInactive>
    {
        public SLRetentionInactiveMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLRetentionInactive");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.Batch).HasColumnName("Batch");
            this.Property(t => t.RetentionCode).HasColumnName("RetentionCode");
            this.Property(t => t.ScheduledInactivity).HasColumnName("ScheduledInactivity");
            this.Property(t => t.EventDate).HasColumnName("EventDate");
            this.Property(t => t.DeptOfRecord).HasColumnName("DeptOfRecord");
            this.Property(t => t.HoldReason).HasColumnName("HoldReason");
            this.Property(t => t.LegalHold).HasColumnName("LegalHold");
            this.Property(t => t.RetentionHold).HasColumnName("RetentionHold");
            this.Property(t => t.RetentionCodeHold).HasColumnName("RetentionCodeHold");
            this.Property(t => t.SLDestructCertItemId).HasColumnName("SLDestructCertItemId");
            this.Property(t => t.FileRoomOrder).HasColumnName("FileRoomOrder");
            this.Property(t => t.Selected).HasColumnName("Selected");
        }
    }
}
