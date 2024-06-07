using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class TrackableMap : EntityTypeConfiguration<Trackable>
    {
        public TrackableMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.RecordVersion });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.RecordVersion)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            
            // Table & Column Mappings
            this.ToTable("Trackables");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RecordVersion).HasColumnName("RecordVersion");
            this.Property(t => t.RecordTypesId).HasColumnName("RecordTypesId");
            this.Property(t => t.PageCount).HasColumnName("PageCount");
            this.Property(t => t.Orphan).HasColumnName("Orphan");
            this.Property(t => t.Verified).HasColumnName("Verified");
            this.Property(t => t.CheckedOut).HasColumnName("CheckedOut");
            this.Property(t => t.CheckedOutDate).HasColumnName("CheckedOutDate");
            this.Property(t => t.CheckedOutUser).HasColumnName("CheckedOutUser");
            this.Property(t => t.CheckedOutIP).HasColumnName("CheckedOutIP");
            this.Property(t => t.CheckedOutMAC).HasColumnName("CheckedOutMAC");
            this.Property(t => t.CheckedOutFolder).HasColumnName("CheckedOutFolder");
            this.Property(t => t.CheckoutLocked).HasColumnName("CheckoutLocked");
            this.Property(t => t.OfficialRecord).HasColumnName("OfficialRecord");
            this.Property(t => t.OfficialRecordReconciliation).HasColumnName("OfficialRecordReconciliation");
            this.Property(t => t.CheckedOutUserId).HasColumnName("CheckedOutUserId");
        }
    }
}
