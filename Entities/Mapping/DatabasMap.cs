using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class DatabasMap : EntityTypeConfiguration<Databas>
    {
        public DatabasMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DBName)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("Databases");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DBName).HasColumnName("DBName");
            this.Property(t => t.DBType).HasColumnName("DBType");
            this.Property(t => t.DBConnectionText).HasColumnName("DBConnectionText");
            this.Property(t => t.DBConnectionTimeout).HasColumnName("DBConnectionTimeout");
            this.Property(t => t.DBDatabase).HasColumnName("DBDatabase");
            this.Property(t => t.DBPassword).HasColumnName("DBPassword");
            this.Property(t => t.DBProvider).HasColumnName("DBProvider");
            this.Property(t => t.DBServer).HasColumnName("DBServer");
            this.Property(t => t.DBUseDBEngineUIDPWD).HasColumnName("DBUseDBEngineUIDPWD");
            this.Property(t => t.DBUserId).HasColumnName("DBUserId");
        }
    }
}
