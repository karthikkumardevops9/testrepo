using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class DocumentTypeMap : EntityTypeConfiguration<DocumentType>
    {
        public DocumentTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.DocumentTypeId);

            // Properties
            this.Property(t => t.DocumentTypeId)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("DocumentTypes");
            this.Property(t => t.DocumentTypeId).HasColumnName("DocumentTypeId");
            this.Property(t => t.FolderTypeId).HasColumnName("FolderTypeId");
            this.Property(t => t.DateIncrement).HasColumnName("DateIncrement");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
