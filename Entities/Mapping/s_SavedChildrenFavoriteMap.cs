using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class s_SavedChildrenFavoriteMap : EntityTypeConfiguration<s_SavedChildrenFavorite>
    {
        public s_SavedChildrenFavoriteMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("s_SavedChildrenFavorite");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SavedCriteriaId).HasColumnName("SavedCriteriaId");
            this.Property(t => t.TableId).HasColumnName("TableId");
        }
    }
}
