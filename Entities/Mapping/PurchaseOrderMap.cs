using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class PurchaseOrderMap : EntityTypeConfiguration<PurchaseOrder>
    {
        public PurchaseOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.POId);

            // Properties
            this.Property(t => t.POId)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("PurchaseOrders");
            this.Property(t => t.POId).HasColumnName("POId");
            this.Property(t => t.ReqId).HasColumnName("ReqId");
            this.Property(t => t.PODate).HasColumnName("PODate");
            this.Property(t => t.VendorId).HasColumnName("VendorId");
            this.Property(t => t.VendorName).HasColumnName("VendorName");
            this.Property(t => t.RetentionCodesId).HasColumnName("RetentionCodesId");
            this.Property(t => t.C_slRetentionInactive).HasColumnName("%slRetentionInactive");
            this.Property(t => t.C_slRetentionInactiveFinal).HasColumnName("%slRetentionInactiveFinal");
            this.Property(t => t.C_slRetentionDispositionStatus).HasColumnName("%slRetentionDispositionStatus");
        }
    }
}
