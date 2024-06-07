using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class vwColumnsAllMap : EntityTypeConfiguration<vwColumnsAll>
    {
        public vwColumnsAllMap()
        {
            // Primary Key
            this.HasKey(t => new { t.ID, t.TABLE_NAME });

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TABLE_NAME)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("vwColumnsAll");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.COLUMN_NAME).HasColumnName("COLUMN_NAME");
            this.Property(t => t.TABLE_NAME).HasColumnName("TABLE_NAME");
            this.Property(t => t.DATA_TYPE).HasColumnName("DATA_TYPE");
        }
    }
}
