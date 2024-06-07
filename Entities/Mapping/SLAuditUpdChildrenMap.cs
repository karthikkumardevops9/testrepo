using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLAuditUpdChildrenMap : EntityTypeConfiguration<SLAuditUpdChildren>
    {
        public SLAuditUpdChildrenMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLAuditUpdChildren");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SLAuditUpdatesId).HasColumnName("SLAuditUpdatesId");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.TableId).HasColumnName("TableId");
        }
    }
}
