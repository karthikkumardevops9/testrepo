using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class DirectoryMap : EntityTypeConfiguration<Directory>
    {
        public DirectoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("Directories");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.VolumesId).HasColumnName("VolumesId");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.DirFullFlag).HasColumnName("DirFullFlag");
        }
    }
}
