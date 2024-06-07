using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class MobileDetailMap : EntityTypeConfiguration<MobileDetail>
    {
        public MobileDetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Token);

            // Properties
            this.Property(t => t.UserId)
                .IsRequired();

            this.Property(t => t.Password)
                .IsRequired();

            this.Property(t => t.DeviceId)
                .IsRequired();

            this.Property(t => t.DBName)
                .IsRequired();

            this.Property(t => t.Token)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("MobileDetails");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.DeviceId).HasColumnName("DeviceId");
            this.Property(t => t.DBName).HasColumnName("DBName");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.IsSignOut).HasColumnName("IsSignOut");
            this.Property(t => t.Token).HasColumnName("Token");
        }
    }
}
