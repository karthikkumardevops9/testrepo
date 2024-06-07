using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLIndexWizardMap : EntityTypeConfiguration<SLIndexWizard>
    {
        public SLIndexWizardMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("SLIndexWizard");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FolderTableName).HasColumnName("FolderTableName");
            this.Property(t => t.FolderPrompt).HasColumnName("FolderPrompt");
            this.Property(t => t.FolderSearchField).HasColumnName("FolderSearchField");
            this.Property(t => t.AutoCreateFolder).HasColumnName("AutoCreateFolder");
            this.Property(t => t.FolderLevelIndexing).HasColumnName("FolderLevelIndexing");
            this.Property(t => t.DocumemtTableName).HasColumnName("DocumemtTableName");
            this.Property(t => t.IncludeDocTypes).HasColumnName("IncludeDocTypes");
            this.Property(t => t.DocTypeTableName).HasColumnName("DocTypeTableName");
            this.Property(t => t.DocTypePrompt).HasColumnName("DocTypePrompt");
            this.Property(t => t.DocTypeSearchField).HasColumnName("DocTypeSearchField");
            this.Property(t => t.DocumentAutoCreate).HasColumnName("DocumentAutoCreate");
            this.Property(t => t.IndexingType).HasColumnName("IndexingType");
            this.Property(t => t.UseHeaderSheets).HasColumnName("UseHeaderSheets");
            this.Property(t => t.HeaderLabelName).HasColumnName("HeaderLabelName");
            this.Property(t => t.DiscardHeaderSheet).HasColumnName("DiscardHeaderSheet");
            this.Property(t => t.UseSeperatorSheets).HasColumnName("UseSeperatorSheets");
            this.Property(t => t.SeperatorLabelName).HasColumnName("SeperatorLabelName");
            this.Property(t => t.DiscardSeperatorSheet).HasColumnName("DiscardSeperatorSheet");
            this.Property(t => t.OutputSettingsId).HasColumnName("OutputSettingsId");
            this.Property(t => t.LinkScriptHeaderId).HasColumnName("LinkScriptHeaderId");
            this.Property(t => t.VerifyManualEntry).HasColumnName("VerifyManualEntry");
            this.Property(t => t.AddFolderHelpComment).HasColumnName("AddFolderHelpComment");
            this.Property(t => t.AddDocumentHelpComment).HasColumnName("AddDocumentHelpComment");
            this.Property(t => t.AlwaysCreateFolder).HasColumnName("AlwaysCreateFolder");
            this.Property(t => t.LeaveDocTypesGlobal).HasColumnName("LeaveDocTypesGlobal");
        }
    }
}
