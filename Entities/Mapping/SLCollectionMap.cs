using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLCollectionMap : EntityTypeConfiguration<SLCollection>
    {
        public SLCollectionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLCollections");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Operator).HasColumnName("Operator");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.SecurityGroupId).HasColumnName("SecurityGroupId");
        }
    }
}
