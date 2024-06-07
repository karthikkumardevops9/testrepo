using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class UserlinkMap : EntityTypeConfiguration<Userlink>
    {
        public UserlinkMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Userlinks");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TrackablesId).HasColumnName("TrackablesId");
            this.Property(t => t.RecordVersion).HasColumnName("RecordVersion");
            this.Property(t => t.IndexTableId).HasColumnName("IndexTableId");
            this.Property(t => t.IndexTable).HasColumnName("IndexTable");
            this.Property(t => t.AttachmentNumber).HasColumnName("AttachmentNumber");
            this.Property(t => t.VersionUpdated).HasColumnName("VersionUpdated");
        }
    }
}
