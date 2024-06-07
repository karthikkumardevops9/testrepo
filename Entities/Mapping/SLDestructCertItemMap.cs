using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLDestructCertItemMap : EntityTypeConfiguration<SLDestructCertItem>
    {
        public SLDestructCertItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLDestructCertItems");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.SLDestructionCertsId).HasColumnName("SLDestructionCertsId");
            this.Property(t => t.LegalHold).HasColumnName("LegalHold");
            this.Property(t => t.RetentionHold).HasColumnName("RetentionHold");
            this.Property(t => t.HoldReason).HasColumnName("HoldReason");
            this.Property(t => t.RetentionCode).HasColumnName("RetentionCode");
            this.Property(t => t.ScheduledInactivity).HasColumnName("ScheduledInactivity");
            this.Property(t => t.ScheduledDestruction).HasColumnName("ScheduledDestruction");
            this.Property(t => t.SnoozeUntil).HasColumnName("SnoozeUntil");
            this.Property(t => t.EventDate).HasColumnName("EventDate");
            this.Property(t => t.RetentionUpdated).HasColumnName("RetentionUpdated");
            this.Property(t => t.DispositionFlag).HasColumnName("DispositionFlag");
            this.Property(t => t.DispositionDate).HasColumnName("DispositionDate");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(t => t.DispositionType).HasColumnName("DispositionType");
        }
    }
}
