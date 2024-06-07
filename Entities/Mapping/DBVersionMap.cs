using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class DBVersionMap : EntityTypeConfiguration<DBVersion>
    {
        public DBVersionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            
            // Table & Column Mappings
            this.ToTable("DBVersion");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Version).HasColumnName("Version");
            this.Property(t => t.ProductCode).HasColumnName("ProductCode");
            this.Property(t => t.ProductVersion).HasColumnName("ProductVersion");
        }
    }
}
