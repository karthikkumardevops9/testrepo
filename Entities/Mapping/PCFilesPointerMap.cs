using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class PCFilesPointerMap : EntityTypeConfiguration<PCFilesPointer>
    {
        public PCFilesPointerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("PCFilesPointers");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TrackablesId).HasColumnName("TrackablesId");
            this.Property(t => t.TrackablesRecordVersion).HasColumnName("TrackablesRecordVersion");
            this.Property(t => t.Pages).HasColumnName("Pages");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.ScanDirectoriesId).HasColumnName("ScanDirectoriesId");
            this.Property(t => t.OrgDirectoriesId).HasColumnName("OrgDirectoriesId");
            this.Property(t => t.OrgFileName).HasColumnName("OrgFileName");
            this.Property(t => t.PCFilesEditGrp).HasColumnName("PCFilesEditGrp");
            this.Property(t => t.PCFilesNVerGrp).HasColumnName("PCFilesNVerGrp");
            this.Property(t => t.OrgFullPath).HasColumnName("OrgFullPath");
            this.Property(t => t.AddedToFTS).HasColumnName("AddedToFTS");
            this.Property(t => t.AddedToOCR).HasColumnName("AddedToOCR");
            this.Property(t => t.ScanBatchesId).HasColumnName("ScanBatchesId");
            this.Property(t => t.ScanSequence).HasColumnName("ScanSequence");
            this.Property(t => t.ScanDateTime).HasColumnName("ScanDateTime");
            this.Property(t => t.BarCodeCount).HasColumnName("BarCodeCount");
            this.Property(t => t.BarCodes).HasColumnName("BarCodes");
            this.Property(t => t.ScanMessage).HasColumnName("ScanMessage");
            this.Property(t => t.PageNumber).HasColumnName("PageNumber");
        }
    }
}
