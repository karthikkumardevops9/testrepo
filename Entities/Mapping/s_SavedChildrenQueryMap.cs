using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class s_SavedChildrenQueryMap : EntityTypeConfiguration<s_SavedChildrenQuery>
    {
        public s_SavedChildrenQueryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("s_SavedChildrenQuery");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SavedCriteriaId).HasColumnName("SavedCriteriaId");
            this.Property(t => t.Sequence).HasColumnName("Sequence");
            this.Property(t => t.ColumnName).HasColumnName("ColumnName");
            this.Property(t => t.Operator).HasColumnName("Operator");
            this.Property(t => t.CriteriaValue).HasColumnName("CriteriaValue");
            this.Property(t => t.Active).HasColumnName("Active");
        }
    }
}
