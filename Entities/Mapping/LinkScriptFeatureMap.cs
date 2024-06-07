using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class LinkScriptFeatureMap : EntityTypeConfiguration<LinkScriptFeature>
    {
        public LinkScriptFeatureMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("LinkScriptFeatures");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InstallLinkScript).HasColumnName("InstallLinkScript");
            this.Property(t => t.UninstallLinkScript).HasColumnName("UninstallLinkScript");
            this.Property(t => t.Installed).HasColumnName("Installed");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.InstallInfo).HasColumnName("InstallInfo");
        }
    }
}
