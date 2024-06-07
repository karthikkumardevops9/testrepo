using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLServiceTaskMap : EntityTypeConfiguration<SLServiceTask>
    {
        public SLServiceTaskMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLServiceTasks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Interval).HasColumnName("Interval");
            this.Property(t => t.Parameters).HasColumnName("Parameters");
            this.Property(t => t.TaskType).HasColumnName("TaskType");
            this.Property(t => t.EMailAddress).HasColumnName("EMailAddress");
            this.Property(t => t.CustomProcess).HasColumnName("CustomProcess");
            this.Property(t => t.NotificationType).HasColumnName("NotificationType");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.ViewId).HasColumnName("ViewId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.RecordCount).HasColumnName("RecordCount");
            this.Property(t => t.DestinationTableName).HasColumnName("DestinationTableName");
            this.Property(t => t.DestinationTableId).HasColumnName("DestinationTableId");
            this.Property(t => t.DueBackDate).HasColumnName("DueBackDate");
            this.Property(t => t.Reconciliation).HasColumnName("Reconciliation");
            this.Property(t => t.ReportLocation).HasColumnName("ReportLocation");
            this.Property(t => t.DownloadLocation).HasColumnName("DownloadLocation");
            this.Property(t => t.IsNotification).HasColumnName("IsNotification");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
        }
    }
}
