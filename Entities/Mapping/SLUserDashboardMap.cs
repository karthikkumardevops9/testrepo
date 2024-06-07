using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLUserDashboardMap : EntityTypeConfiguration<SLUserDashboard>
    {
        public SLUserDashboardMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            this.Property(t => t.UserID)
                .IsRequired();

            this.Property(t => t.Json)
                .IsMaxLength();

            // Table & Column Mappings
            this.ToTable("SLUserDashboard");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Json).HasColumnName("Json");
        }
    }
}
