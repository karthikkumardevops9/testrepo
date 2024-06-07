using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SysNextTrackableMap : EntityTypeConfiguration<SysNextTrackable>
    {
        public SysNextTrackableMap()
        {
            // Primary Key
            this.HasKey(t => t.NextTrackablesId);

            // Table & Column Mappings
            this.ToTable("SysNextTrackable");
            this.Property(t => t.NextTrackablesId).HasColumnName("NextTrackablesId");
        }
    }
}
