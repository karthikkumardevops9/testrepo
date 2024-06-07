using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLRetentionCodeMap : EntityTypeConfiguration<SLRetentionCode>
    {
        public SLRetentionCodeMap()
        {
            // Primary Key
            this.HasKey(t => t.SLRetentionCodesId);

            // Properties
            this.Property(t => t.Id)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("SLRetentionCodes");
            this.Property(t => t.SLRetentionCodesId).HasColumnName("SLRetentionCodesId");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.DeptOfRecord).HasColumnName("DeptOfRecord");
            this.Property(t => t.Notes).HasColumnName("Notes");
            this.Property(t => t.RetentionLegalHold).HasColumnName("RetentionLegalHold");
            this.Property(t => t.RetentionPeriodLegal).HasColumnName("RetentionPeriodLegal");
            this.Property(t => t.RetentionPeriodUser).HasColumnName("RetentionPeriodUser");
            this.Property(t => t.RetentionPeriodOther).HasColumnName("RetentionPeriodOther");
            this.Property(t => t.RetentionPeriodTotal).HasColumnName("RetentionPeriodTotal");
            this.Property(t => t.RetentionPeriodForceToEndOfYear).HasColumnName("RetentionPeriodForceToEndOfYear");
            this.Property(t => t.RetentionEventType).HasColumnName("RetentionEventType");
            this.Property(t => t.InactivityEventType).HasColumnName("InactivityEventType");
            this.Property(t => t.InactivityPeriod).HasColumnName("InactivityPeriod");
            this.Property(t => t.InactivityForceToEndOfYear).HasColumnName("InactivityForceToEndOfYear");
        }
    }
}
