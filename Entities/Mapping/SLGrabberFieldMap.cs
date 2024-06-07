using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLGrabberFieldMap : EntityTypeConfiguration<SLGrabberField>
    {
        public SLGrabberFieldMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLGrabberFields");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SLGrabberFunctionsId).HasColumnName("SLGrabberFunctionsId");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.SmeadlinkFieldName).HasColumnName("SmeadlinkFieldName");
            this.Property(t => t.IsIdField).HasColumnName("IsIdField");
            this.Property(t => t.IsOkayIfMissing).HasColumnName("IsOkayIfMissing");
            this.Property(t => t.SmeadlinkTableName).HasColumnName("SmeadlinkTableName");
        }
    }
}
