using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class InvoiceMap : EntityTypeConfiguration<Invoice>
    {
        public InvoiceMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.InvoiceId)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("Invoices");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.POId).HasColumnName("POId");
            this.Property(t => t.InvoiceId).HasColumnName("InvoiceId");
            this.Property(t => t.InvoiceDate).HasColumnName("InvoiceDate");
        }
    }
}
