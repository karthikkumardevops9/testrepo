using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLRetentionCitationMap : EntityTypeConfiguration<SLRetentionCitation>
    {
        public SLRetentionCitationMap()
        {
            // Primary Key
            this.HasKey(t => t.SLRetentionCitationId);

            // Properties
            this.Property(t => t.Citation)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("SLRetentionCitations");
            this.Property(t => t.SLRetentionCitationId).HasColumnName("SLRetentionCitationId");
            this.Property(t => t.Citation).HasColumnName("Citation");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.Notation).HasColumnName("Notation");
            this.Property(t => t.LegalPeriod).HasColumnName("LegalPeriod");
        }
    }
}
