using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class PackingSlipMap : EntityTypeConfiguration<PackingSlip>
    {
        public PackingSlipMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("PackingSlips");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.POId).HasColumnName("POId");
            this.Property(t => t.PSDate).HasColumnName("PSDate");
        }
    }
}
