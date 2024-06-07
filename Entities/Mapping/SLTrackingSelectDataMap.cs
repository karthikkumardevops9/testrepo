using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLTrackingSelectDataMap : EntityTypeConfiguration<SLTrackingSelectData>
    {
        public SLTrackingSelectDataMap()
        {
            // Primary Key
            this.HasKey(t => t.SLTrackingSelectDataId);

            // Properties
            this.Property(t => t.Id)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("SLTrackingSelectData");
            this.Property(t => t.SLTrackingSelectDataId).HasColumnName("SLTrackingSelectDataId");
            this.Property(t => t.Id).HasColumnName("Id");
        }
    }
}
