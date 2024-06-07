using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLBatchRequestMap : EntityTypeConfiguration<SLBatchRequest>
    {
        public SLBatchRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLBatchRequests");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OperatorsId).HasColumnName("OperatorsId");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.RequestedIds).HasColumnName("RequestedIds");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.StayOnTop).HasColumnName("StayOnTop");
            this.Property(t => t.RequestedTable).HasColumnName("RequestedTable");
            this.Property(t => t.PriorityOrder).HasColumnName("PriorityOrder");
        }
    }
}
