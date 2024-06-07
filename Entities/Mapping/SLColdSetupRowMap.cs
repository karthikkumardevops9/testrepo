using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLColdSetupRowMap : EntityTypeConfiguration<SLColdSetupRow>
    {
        public SLColdSetupRowMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLColdSetupRows");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RowName).HasColumnName("RowName");
            this.Property(t => t.SeqNo).HasColumnName("SeqNo");
            this.Property(t => t.SLCOLDSetupFormsId).HasColumnName("SLCOLDSetupFormsId");
            this.Property(t => t.RowMask).HasColumnName("RowMask");
            this.Property(t => t.SendBreak).HasColumnName("SendBreak");
            this.Property(t => t.ScanFormsId).HasColumnName("ScanFormsId");
            this.Property(t => t.LineOnPage).HasColumnName("LineOnPage");
            this.Property(t => t.NewRowLine).HasColumnName("NewRowLine");
            this.Property(t => t.ErrorOnMissingFromPage).HasColumnName("ErrorOnMissingFromPage");
        }
    }
}
