using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ImportLoadMap : EntityTypeConfiguration<ImportLoad>
    {
        public ImportLoadMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);
            
            // Table & Column Mappings
            this.ToTable("ImportLoads");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.IdFieldName).HasColumnName("IdFieldName");
            this.Property(t => t.InputFile).HasColumnName("InputFile");
            this.Property(t => t.MaxDupCount).HasColumnName("MaxDupCount");
            this.Property(t => t.TableSheetName).HasColumnName("TableSheetName");
            this.Property(t => t.TrackDestinationId).HasColumnName("TrackDestinationId");
            this.Property(t => t.LoadName).HasColumnName("LoadName");
            this.Property(t => t.RecordType).HasColumnName("RecordType");
            this.Property(t => t.RecordLength).HasColumnName("RecordLength");
            this.Property(t => t.Delimiter).HasColumnName("Delimiter");
            this.Property(t => t.Duplicate).HasColumnName("Duplicate");
            this.Property(t => t.UpdateParent).HasColumnName("UpdateParent");
            this.Property(t => t.SQLQuery).HasColumnName("SQLQuery");
            this.Property(t => t.ReverseOrder).HasColumnName("ReverseOrder");
            this.Property(t => t.SaveImageAsNewPage).HasColumnName("SaveImageAsNewPage");
            this.Property(t => t.DifferentImagePath).HasColumnName("DifferentImagePath");
            this.Property(t => t.ScanRule).HasColumnName("ScanRule");
            this.Property(t => t.ReplaceThisPath).HasColumnName("ReplaceThisPath");
            this.Property(t => t.ReplaceWithPath).HasColumnName("ReplaceWithPath");
            this.Property(t => t.DeleteSourceFile).HasColumnName("DeleteSourceFile");
            this.Property(t => t.DeleteSourceImage).HasColumnName("DeleteSourceImage");
            this.Property(t => t.DirectFromHandheld).HasColumnName("DirectFromHandheld");
            this.Property(t => t.OneStripJobsId).HasColumnName("OneStripJobsId");
            this.Property(t => t.ImportPrintType).HasColumnName("ImportPrintType");
            this.Property(t => t.DatabaseName).HasColumnName("DatabaseName");
            this.Property(t => t.DateDue).HasColumnName("DateDue");
            this.Property(t => t.DoReconciliation).HasColumnName("DoReconciliation");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.FirstRowHeader).HasColumnName("FirstRowHeader");
            this.Property(t => t.SaveImageAsNewVersion).HasColumnName("SaveImageAsNewVersion");
            this.Property(t => t.FromHandHeldEnum).HasColumnName("FromHandHeldEnum");
            this.Property(t => t.SaveImageAsNewVersionAsOfficialRecord).HasColumnName("SaveImageAsNewVersionAsOfficialRecord");
            this.Property(t => t.TempInputFile).HasColumnName("TempInputFile");
        }
    }
}
