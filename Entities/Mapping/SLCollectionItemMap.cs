using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLCollectionItemMap : EntityTypeConfiguration<SLCollectionItem>
    {
        public SLCollectionItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SLCollectionItems");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CollectionId).HasColumnName("CollectionId");
            this.Property(t => t.Index).HasColumnName("Index");
            this.Property(t => t.PointersId).HasColumnName("PointersId");
            this.Property(t => t.Table).HasColumnName("Table");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.AttachmentType).HasColumnName("AttachmentType");
            this.Property(t => t.DisplayText).HasColumnName("DisplayText");
            this.Property(t => t.TableUserName).HasColumnName("TableUserName");
            this.Property(t => t.Ticket).HasColumnName("Ticket");
            this.Property(t => t.Extension).HasColumnName("Extension");
        }
    }
}
