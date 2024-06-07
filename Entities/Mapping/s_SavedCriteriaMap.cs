using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class s_SavedCriteriaMap : EntityTypeConfiguration<s_SavedCriteria>
    {
        public s_SavedCriteriaMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("s_SavedCriteria");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.SavedName).HasColumnName("SavedName");
            this.Property(t => t.SavedType).HasColumnName("SavedType");
            this.Property(t => t.ViewId).HasColumnName("ViewId");
        }
    }
}
