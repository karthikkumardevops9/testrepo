using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLColdSetupColMap : EntityTypeConfiguration<SLColdSetupCol>
    {
        public SLColdSetupColMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ColName, t.SLCOLDSetupRowsId });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.ColName)
                .IsRequired();

            this.Property(t => t.SLCOLDSetupRowsId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            
            // Table & Column Mappings
            this.ToTable("SLColdSetupCols");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ColName).HasColumnName("ColName");
            this.Property(t => t.SeqNo).HasColumnName("SeqNo");
            this.Property(t => t.SLCOLDSetupRowsId).HasColumnName("SLCOLDSetupRowsId");
            this.Property(t => t.LineOffset).HasColumnName("LineOffset");
            this.Property(t => t.Position).HasColumnName("Position");
            this.Property(t => t.Length).HasColumnName("Length");
            this.Property(t => t.DefaultValue).HasColumnName("DefaultValue");
        }
    }
}
