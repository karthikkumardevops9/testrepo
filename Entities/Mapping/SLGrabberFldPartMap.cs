using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLGrabberFldPartMap : EntityTypeConfiguration<SLGrabberFldPart>
    {
        public SLGrabberFldPartMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLGrabberFldParts");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SLGrabberFieldsId).HasColumnName("SLGrabberFieldsId");
            this.Property(t => t.SLGrabberControlsId).HasColumnName("SLGrabberControlsId");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.StartFromFront).HasColumnName("StartFromFront");
            this.Property(t => t.StartingPosition).HasColumnName("StartingPosition");
            this.Property(t => t.NumberofCharacters).HasColumnName("NumberofCharacters");
            this.Property(t => t.FieldFormat).HasColumnName("FieldFormat");
        }
    }
}
