using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class LookupTypeMap : EntityTypeConfiguration<LookupType>
    {
        public LookupTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.LookupTypeId);

            // Properties
            // Table & Column Mappings
            this.ToTable("LookupType");
            this.Property(t => t.LookupTypeId).HasColumnName("LookupTypeId");
            this.Property(t => t.LookupTypeCode).HasColumnName("LookupTypeCode");
            this.Property(t => t.LookupTypeValue).HasColumnName("LookupTypeValue");
            this.Property(t => t.LookupTypeDesc).HasColumnName("LookupTypeDesc");
            this.Property(t => t.LookupTypeForCode).HasColumnName("LookupTypeForCode");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
            this.Property(t => t.LookupTypeParentCode).HasColumnName("LookupTypeParentCode");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
