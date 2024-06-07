using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLTableFileRoomOrderMap : EntityTypeConfiguration<SLTableFileRoomOrder>
    {
        public SLTableFileRoomOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLTableFileRoomOrder");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.StartFromFront).HasColumnName("StartFromFront");
            this.Property(t => t.StartingPosition).HasColumnName("StartingPosition");
            this.Property(t => t.NumberofCharacters).HasColumnName("NumberofCharacters");
            this.Property(t => t.FieldFormat).HasColumnName("FieldFormat");
        }
    }
}
