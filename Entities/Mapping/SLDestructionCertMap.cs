using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLDestructionCertMap : EntityTypeConfiguration<SLDestructionCert>
    {
        public SLDestructionCertMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLDestructionCerts");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.ApprovedBy).HasColumnName("ApprovedBy");
            this.Property(t => t.DateDestroyed).HasColumnName("DateDestroyed");
            this.Property(t => t.ImagesDeleted).HasColumnName("ImagesDeleted");
            this.Property(t => t.NetworkLoginName).HasColumnName("NetworkLoginName");
            this.Property(t => t.Domain).HasColumnName("Domain");
            this.Property(t => t.ComputerName).HasColumnName("ComputerName");
            this.Property(t => t.MacAddress).HasColumnName("MacAddress");
            this.Property(t => t.IP).HasColumnName("IP");
            this.Property(t => t.RetentionDispositionType).HasColumnName("RetentionDispositionType");
        }
    }
}
