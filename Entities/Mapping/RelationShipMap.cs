using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class RelationShipMap : EntityTypeConfiguration<RelationShip>
    {
        public RelationShipMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("RelationShips");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UpperTableName).HasColumnName("UpperTableName");
            this.Property(t => t.UpperTableFieldName).HasColumnName("UpperTableFieldName");
            this.Property(t => t.LowerTableName).HasColumnName("LowerTableName");
            this.Property(t => t.LowerTableFieldName).HasColumnName("LowerTableFieldName");
            this.Property(t => t.TabOrder).HasColumnName("TabOrder");
            this.Property(t => t.IdTypes).HasColumnName("IdTypes");
            this.Property(t => t.DrillDownViewGroup).HasColumnName("DrillDownViewGroup");
        }
    }
}
