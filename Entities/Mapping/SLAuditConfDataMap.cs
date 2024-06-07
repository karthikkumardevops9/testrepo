using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLAuditConfDataMap : EntityTypeConfiguration<SLAuditConfData>
    {
        public SLAuditConfDataMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLAuditConfData");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.OperatorsId).HasColumnName("OperatorsId");
            this.Property(t => t.NetworkLoginName).HasColumnName("NetworkLoginName");
            this.Property(t => t.Domain).HasColumnName("Domain");
            this.Property(t => t.ComputerName).HasColumnName("ComputerName");
            this.Property(t => t.MacAddress).HasColumnName("MacAddress");
            this.Property(t => t.IP).HasColumnName("IP");
            this.Property(t => t.AccessDateTime).HasColumnName("AccessDateTime");
            this.Property(t => t.ModuleName).HasColumnName("ModuleName");
            this.Property(t => t.Action).HasColumnName("Action");
            this.Property(t => t.DataBefore).HasColumnName("DataBefore");
            this.Property(t => t.DataAfter).HasColumnName("DataAfter");
            this.Property(t => t.Module).HasColumnName("Module");
        }
    }
}
