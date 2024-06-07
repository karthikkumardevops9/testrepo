using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class OfficeDocTypeMap : EntityTypeConfiguration<OfficeDocType>
    {
        public OfficeDocTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("OfficeDocTypes");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DefaultFormat).HasColumnName("DefaultFormat");
            this.Property(t => t.SaveFormat).HasColumnName("SaveFormat");
            this.Property(t => t.AppType).HasColumnName("AppType");
            this.Property(t => t.DefaultExtension).HasColumnName("DefaultExtension");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.EditorType).HasColumnName("EditorType");
            this.Property(t => t.HardDelete).HasColumnName("HardDelete");
        }
    }
}
