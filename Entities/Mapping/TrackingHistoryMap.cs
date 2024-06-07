using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class TrackingHistoryMap : EntityTypeConfiguration<TrackingHistory>
    {
        public TrackingHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("TrackingHistory");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TrackedTableId).HasColumnName("TrackedTableId");
            this.Property(t => t.TrackedTable).HasColumnName("TrackedTable");
            this.Property(t => t.LocationsId).HasColumnName("LocationsId");
            this.Property(t => t.EmployeesId).HasColumnName("EmployeesId");
            this.Property(t => t.BoxesId).HasColumnName("BoxesId");
            this.Property(t => t.TransactionDateTime).HasColumnName("TransactionDateTime");
            this.Property(t => t.IsActualScan).HasColumnName("IsActualScan");
            this.Property(t => t.BatchId).HasColumnName("BatchId");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.TrackingAdditionalField1).HasColumnName("TrackingAdditionalField1");
            this.Property(t => t.TrackingAdditionalField2).HasColumnName("TrackingAdditionalField2");
        }
    }
}
