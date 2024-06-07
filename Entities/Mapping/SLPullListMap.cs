using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLPullListMap : EntityTypeConfiguration<SLPullList>
    {
        public SLPullListMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLPullLists");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OperatorsId).HasColumnName("OperatorsId");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.SLBatchRequestComment).HasColumnName("SLBatchRequestComment");
            this.Property(t => t.BatchPullList).HasColumnName("BatchPullList");
            this.Property(t => t.BatchPrinted).HasColumnName("BatchPrinted");
            this.Property(t => t.PriorityOrder).HasColumnName("PriorityOrder");
        }
    }
}
