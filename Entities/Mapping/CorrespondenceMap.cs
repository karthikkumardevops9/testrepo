using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class CorrespondenceMap : EntityTypeConfiguration<Correspondence>
    {
        public CorrespondenceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Correspondence");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.To).HasColumnName("To");
            this.Property(t => t.From).HasColumnName("From");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.RetentionCodesId).HasColumnName("RetentionCodesId");
            this.Property(t => t.DateClosed).HasColumnName("DateClosed");
            this.Property(t => t.DateOpened).HasColumnName("DateOpened");
            this.Property(t => t.DateOther).HasColumnName("DateOther");
            this.Property(t => t.C_slRetentionInactive).HasColumnName("%slRetentionInactive");
            this.Property(t => t.C_slRetentionInactiveFinal).HasColumnName("%slRetentionInactiveFinal");
            this.Property(t => t.C_slRetentionDispositionStatus).HasColumnName("%slRetentionDispositionStatus");
        }
    }
}
