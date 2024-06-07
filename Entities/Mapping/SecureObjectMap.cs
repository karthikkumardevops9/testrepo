using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SecureObjectMap : EntityTypeConfiguration<SecureObject>
    {
        public SecureObjectMap()
        {
            // Primary Key
            this.HasKey(t => t.SecureObjectID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SecureObject");
            this.Property(t => t.SecureObjectID).HasColumnName("SecureObjectID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SecureObjectTypeID).HasColumnName("SecureObjectTypeID");
            this.Property(t => t.BaseID).HasColumnName("BaseID");

            // Relationships
            this.HasRequired(t => t.SecureObject2)
                .WithMany(t => t.SecureObject1)
                .HasForeignKey(d => d.BaseID);

        }
    }
}
