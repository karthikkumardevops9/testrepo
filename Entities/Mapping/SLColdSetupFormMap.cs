using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SLColdSetupFormMap : EntityTypeConfiguration<SLColdSetupForm>
    {
        public SLColdSetupFormMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.FormName });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.FormName)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("SLColdSetupForms");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.LoadName).HasColumnName("LoadName");
            this.Property(t => t.FormName).HasColumnName("FormName");
            this.Property(t => t.LineFormat).HasColumnName("LineFormat");
            this.Property(t => t.PageMask).HasColumnName("PageMask");
            this.Property(t => t.PageStartsOnLine).HasColumnName("PageStartsOnLine");
            this.Property(t => t.MaxNumOfLinesPerPage).HasColumnName("MaxNumOfLinesPerPage");
            this.Property(t => t.NumOfPagesBeforeError).HasColumnName("NumOfPagesBeforeError");
            this.Property(t => t.FirstPageStartsOnLine).HasColumnName("FirstPageStartsOnLine");
            this.Property(t => t.PageMaskFormFeed).HasColumnName("PageMaskFormFeed");
            this.Property(t => t.SkipLinesBeforeStarting).HasColumnName("SkipLinesBeforeStarting");
            this.Property(t => t.ImagePathPageOne).HasColumnName("ImagePathPageOne");
            this.Property(t => t.ImagePathPageTwo).HasColumnName("ImagePathPageTwo");
            this.Property(t => t.CPI).HasColumnName("CPI");
            this.Property(t => t.LPI).HasColumnName("LPI");
            this.Property(t => t.PageHeight).HasColumnName("PageHeight");
            this.Property(t => t.PageWidth).HasColumnName("PageWidth");
            this.Property(t => t.CharsWide).HasColumnName("CharsWide");
            this.Property(t => t.LinesHigh).HasColumnName("LinesHigh");
            this.Property(t => t.LastArchiveId).HasColumnName("LastArchiveId");
            this.Property(t => t.FontName).HasColumnName("FontName");
            this.Property(t => t.GreenBar).HasColumnName("GreenBar");
            this.Property(t => t.OffsetX).HasColumnName("OffsetX");
            this.Property(t => t.OffsetY).HasColumnName("OffsetY");
            this.Property(t => t.SpecialProcessing).HasColumnName("SpecialProcessing");
            this.Property(t => t.ArchiveVolumesId).HasColumnName("ArchiveVolumesId");
            this.Property(t => t.AppendToLastArchive).HasColumnName("AppendToLastArchive");
            this.Property(t => t.ArchiveDirectoriesId).HasColumnName("ArchiveDirectoriesId");
            this.Property(t => t.ArchivePrefix).HasColumnName("ArchivePrefix");
            this.Property(t => t.DebugFile).HasColumnName("DebugFile");
            this.Property(t => t.DebugLevel).HasColumnName("DebugLevel");
            this.Property(t => t.MakeMatchReport).HasColumnName("MakeMatchReport");
            this.Property(t => t.NextArchiveNumber).HasColumnName("NextArchiveNumber");
            this.Property(t => t.NoBreaks).HasColumnName("NoBreaks");
            this.Property(t => t.NumPagesToProcess).HasColumnName("NumPagesToProcess");
        }
    }
}
