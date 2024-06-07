using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class LinkScriptHeaderMap : EntityTypeConfiguration<LinkScriptHeader>
    {
        public LinkScriptHeaderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("LinkScriptHeader");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FormPageAction).HasColumnName("FormPageAction");
            this.Property(t => t.DirectoryName).HasColumnName("DirectoryName");
            this.Property(t => t.OutputSettingsId).HasColumnName("OutputSettingsId");
            this.Property(t => t.CallingType).HasColumnName("CallingType");
            this.Property(t => t.PCFilesDeleteAfterCopy).HasColumnName("PCFilesDeleteAfterCopy");
            this.Property(t => t.ViewGroup).HasColumnName("ViewGroup");
            this.Property(t => t.UIType).HasColumnName("UIType");
        }
    }
}
