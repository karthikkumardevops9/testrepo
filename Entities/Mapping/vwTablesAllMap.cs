using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class vwTablesAllMap : EntityTypeConfiguration<vwTablesAll>
    {
        public vwTablesAllMap()
        {
            // Primary Key
            this.HasKey(t => t.TABLE_NAME);

            // Properties
            this.Property(t => t.TABLE_NAME)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("vwTablesAll");
            this.Property(t => t.TABLE_CATALOG).HasColumnName("TABLE_CATALOG");
            this.Property(t => t.TABLE_SCHEMA).HasColumnName("TABLE_SCHEMA");
            this.Property(t => t.TABLE_NAME).HasColumnName("TABLE_NAME");
            this.Property(t => t.TABLE_TYPE).HasColumnName("TABLE_TYPE");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Expr1).HasColumnName("Expr1");
            this.Property(t => t.RowRank).HasColumnName("RowRank");
        }
    }
}
