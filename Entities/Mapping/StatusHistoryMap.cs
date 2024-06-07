using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class StatusHistoryMap : EntityTypeConfiguration<StatusHistory>
    {
        public StatusHistoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("StatusHistory");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FoldersId).HasColumnName("FoldersId");
            this.Property(t => t.Operator).HasColumnName("Operator");
            this.Property(t => t.StatusChangeDateTime).HasColumnName("StatusChangeDateTime");
            this.Property(t => t.NewStatus).HasColumnName("NewStatus");
        }
    }
}
