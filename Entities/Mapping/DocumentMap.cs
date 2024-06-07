using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class DocumentMap : EntityTypeConfiguration<Document>
    {
        public DocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DocumentTypeId)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("Documents");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FolderId).HasColumnName("FolderId");
            this.Property(t => t.DocumentTypeId).HasColumnName("DocumentTypeId");
            this.Property(t => t.DocumentDescription).HasColumnName("DocumentDescription");
            this.Property(t => t.Received).HasColumnName("Received");
            this.Property(t => t.ExceptionNote).HasColumnName("ExceptionNote");
            this.Property(t => t.DocumentDate).HasColumnName("DocumentDate");
        }
    }
}
