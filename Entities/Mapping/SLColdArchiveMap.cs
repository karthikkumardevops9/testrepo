using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLColdArchiveMap : EntityTypeConfiguration<SLColdArchive>
    {
        public SLColdArchiveMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("SLColdArchives");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ArchiveName).HasColumnName("ArchiveName");
            this.Property(t => t.SLCOLDSetupFormsId).HasColumnName("SLCOLDSetupFormsId");
            this.Property(t => t.DirectoriesId).HasColumnName("DirectoriesId");
            this.Property(t => t.OrgDirectoriesId).HasColumnName("OrgDirectoriesId");
            this.Property(t => t.OrgFileName).HasColumnName("OrgFileName");
            this.Property(t => t.OrgFullPath).HasColumnName("OrgFullPath");
            this.Property(t => t.AddedToFTS).HasColumnName("AddedToFTS");
        }
    }
}
