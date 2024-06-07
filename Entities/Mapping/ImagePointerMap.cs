using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class ImagePointerMap : EntityTypeConfiguration<ImagePointer>
    {
        public ImagePointerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("ImagePointers");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TrackablesId).HasColumnName("TrackablesId");
            this.Property(t => t.TrackablesRecordVersion).HasColumnName("TrackablesRecordVersion");
            this.Property(t => t.ScanDirectoriesId).HasColumnName("ScanDirectoriesId");
            this.Property(t => t.ScanBatchesId).HasColumnName("ScanBatchesId");
            this.Property(t => t.ScanSequence).HasColumnName("ScanSequence");
            this.Property(t => t.FileName).HasColumnName("FileName");
            this.Property(t => t.ScanDateTime).HasColumnName("ScanDateTime");
            this.Property(t => t.PageNumber).HasColumnName("PageNumber");
            this.Property(t => t.CRC).HasColumnName("CRC");
            this.Property(t => t.Orientation).HasColumnName("Orientation");
            this.Property(t => t.Skew).HasColumnName("Skew");
            this.Property(t => t.Front).HasColumnName("Front");
            this.Property(t => t.ImageHeight).HasColumnName("ImageHeight");
            this.Property(t => t.ImageWidth).HasColumnName("ImageWidth");
            this.Property(t => t.ImageSize).HasColumnName("ImageSize");
            this.Property(t => t.BarCodeCount).HasColumnName("BarCodeCount");
            this.Property(t => t.BarCodes).HasColumnName("BarCodes");
            this.Property(t => t.PageCount).HasColumnName("PageCount");
            this.Property(t => t.OrgDirectoriesId).HasColumnName("OrgDirectoriesId");
            this.Property(t => t.OrgFileName).HasColumnName("OrgFileName");
            this.Property(t => t.OrgFullPath).HasColumnName("OrgFullPath");
            this.Property(t => t.AddedToFTS).HasColumnName("AddedToFTS");
            this.Property(t => t.AddedToOCR).HasColumnName("AddedToOCR");
            this.Property(t => t.ScanMessage).HasColumnName("ScanMessage");
        }
    }
}
