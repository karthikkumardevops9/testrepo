using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class RequisitionMap : EntityTypeConfiguration<Requisition>
    {
        public RequisitionMap()
        {
            // Primary Key
            this.HasKey(t => t.ReqId);

            // Properties
            this.Property(t => t.ReqId)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("Requisitions");
            this.Property(t => t.ReqId).HasColumnName("ReqId");
            this.Property(t => t.ReqDate).HasColumnName("ReqDate");
            this.Property(t => t.Requestor).HasColumnName("Requestor");
            this.Property(t => t.Department).HasColumnName("Department");
        }
    }
}
