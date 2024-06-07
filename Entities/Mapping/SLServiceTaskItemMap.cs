using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLServiceTaskItemMap : EntityTypeConfiguration<SLServiceTaskItem>
    {
        public SLServiceTaskItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLServiceTaskItems");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SLServiceTaskId).HasColumnName("SLServiceTaskId");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.TableId).HasColumnName("TableId");
        }
    }
}
