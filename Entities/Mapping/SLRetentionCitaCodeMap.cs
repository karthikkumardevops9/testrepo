using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLRetentionCitaCodeMap : EntityTypeConfiguration<SLRetentionCitaCode>
    {
        public SLRetentionCitaCodeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLRetentionCitaCodes");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SLRetentionCitationsCitation).HasColumnName("SLRetentionCitationsCitation");
            this.Property(t => t.SLRetentionCodesId).HasColumnName("SLRetentionCodesId");
        }
    }
}
