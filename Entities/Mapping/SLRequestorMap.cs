using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLRequestorMap : EntityTypeConfiguration<SLRequestor>
    {
        public SLRequestorMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLRequestor");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.EmployeeId).HasColumnName("EmployeeId");
            this.Property(t => t.DateRequested).HasColumnName("DateRequested");
            this.Property(t => t.Priority).HasColumnName("Priority");
            this.Property(t => t.DateReceived).HasColumnName("DateReceived");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ExceptionComments).HasColumnName("ExceptionComments");
            this.Property(t => t.SLPullListsId).HasColumnName("SLPullListsId");
            this.Property(t => t.DatePulled).HasColumnName("DatePulled");
            this.Property(t => t.DateExceptioned).HasColumnName("DateExceptioned");
            this.Property(t => t.DateDeleted).HasColumnName("DateDeleted");
            this.Property(t => t.DeleteOperatorId).HasColumnName("DeleteOperatorId");
            this.Property(t => t.FileRoomOrder).HasColumnName("FileRoomOrder");
            this.Property(t => t.Instructions).HasColumnName("Instructions");
            this.Property(t => t.DateNeeded).HasColumnName("DateNeeded");
            this.Property(t => t.PriorityOrder).HasColumnName("PriorityOrder");
            this.Property(t => t.RequestedBy).HasColumnName("RequestedBy");
        }
    }
}
