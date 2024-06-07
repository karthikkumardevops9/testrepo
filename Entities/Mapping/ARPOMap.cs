using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ARPOMap : EntityTypeConfiguration<ARPO>
    {
        public ARPOMap()
        {
            // Primary Key
            this.HasKey(t => t.POId);

            // Properties
            this.Property(t => t.POId)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("ARPO");
            this.Property(t => t.POId).HasColumnName("POId");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.Customer).HasColumnName("Customer");
        }
    }
}
