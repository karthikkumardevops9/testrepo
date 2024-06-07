using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class LocationMap : EntityTypeConfiguration<Location>
    {
        public LocationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Locations");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Out).HasColumnName("Out");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.Requestable).HasColumnName("Requestable");
            this.Property(t => t.InactiveStorage).HasColumnName("InactiveStorage");
            this.Property(t => t.ArchiveStorage).HasColumnName("ArchiveStorage");
        }
    }
}
