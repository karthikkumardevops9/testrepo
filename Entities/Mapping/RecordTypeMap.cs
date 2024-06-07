using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class RecordTypeMap : EntityTypeConfiguration<RecordType>
    {
        public RecordTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            
            // Table & Column Mappings
            this.ToTable("RecordTypes");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.TrackHistory).HasColumnName("TrackHistory");
        }
    }
}
