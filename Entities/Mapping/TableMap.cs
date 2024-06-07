using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class TableMap : EntityTypeConfiguration<Table>
    {
        public TableMap()
        {
            // Primary Key
            this.HasKey(t => t.TableId);

            // Properties
            this.Property(t => t.TableName)
                .IsRequired();
            
            // Table & Column Mappings
            this.ToTable("Tables");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.TableName).HasColumnName("TableName");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.DBName).HasColumnName("DBName");
            this.Property(t => t.DatabaseAccessType).HasColumnName("DatabaseAccessType");
            this.Property(t => t.Attachments).HasColumnName("Attachments");
            this.Property(t => t.AliasImagingTableName).HasColumnName("AliasImagingTableName");
            this.Property(t => t.TrackingTable).HasColumnName("TrackingTable");
            this.Property(t => t.Trackable).HasColumnName("Trackable");
            this.Property(t => t.TrackingStatusFieldName).HasColumnName("TrackingStatusFieldName");
            this.Property(t => t.CounterFieldName).HasColumnName("CounterFieldName");
            this.Property(t => t.ViewGroup).HasColumnName("ViewGroup");
            this.Property(t => t.AddGroup).HasColumnName("AddGroup");
            this.Property(t => t.EditGroup).HasColumnName("EditGroup");
            this.Property(t => t.DelGroup).HasColumnName("DelGroup");
            this.Property(t => t.MgrGroup).HasColumnName("MgrGroup");
            this.Property(t => t.DeleteAttachedGroup).HasColumnName("DeleteAttachedGroup");
            this.Property(t => t.AttributesID).HasColumnName("AttributesID");
            this.Property(t => t.Picture).HasColumnName("Picture");
            this.Property(t => t.IdFieldName).HasColumnName("IdFieldName");
            this.Property(t => t.IdFieldName2).HasColumnName("IdFieldName2");
            this.Property(t => t.IdStripChars).HasColumnName("IdStripChars");
            this.Property(t => t.IdMask).HasColumnName("IdMask");
            this.Property(t => t.BarCodePrefix).HasColumnName("BarCodePrefix");
            this.Property(t => t.DescFieldPrefixOne).HasColumnName("DescFieldPrefixOne");
            this.Property(t => t.DescFieldNameOne).HasColumnName("DescFieldNameOne");
            this.Property(t => t.DescFieldPrefixTwo).HasColumnName("DescFieldPrefixTwo");
            this.Property(t => t.DescFieldNameTwo).HasColumnName("DescFieldNameTwo");
            this.Property(t => t.MaxRecsOnDropDown).HasColumnName("MaxRecsOnDropDown");
            this.Property(t => t.ADOServerCursor).HasColumnName("ADOServerCursor");
            this.Property(t => t.ADOQueryTimeout).HasColumnName("ADOQueryTimeout");
            this.Property(t => t.ADOCacheSize).HasColumnName("ADOCacheSize");
            this.Property(t => t.LSBeforeAddRecord).HasColumnName("LSBeforeAddRecord");
            this.Property(t => t.LSAfterAddRecord).HasColumnName("LSAfterAddRecord");
            this.Property(t => t.LSBeforeEditRecord).HasColumnName("LSBeforeEditRecord");
            this.Property(t => t.LSAfterEditRecord).HasColumnName("LSAfterEditRecord");
            this.Property(t => t.LSBeforeDeleteRecord).HasColumnName("LSBeforeDeleteRecord");
            this.Property(t => t.LSAfterDeleteRecord).HasColumnName("LSAfterDeleteRecord");
            this.Property(t => t.DefaultTrackingTable).HasColumnName("DefaultTrackingTable");
            this.Property(t => t.DefaultTrackingId).HasColumnName("DefaultTrackingId");
            this.Property(t => t.RetentionPeriodActive).HasColumnName("RetentionPeriodActive");
            this.Property(t => t.RetentionInactivityActive).HasColumnName("RetentionInactivityActive");
            this.Property(t => t.RetentionDateOpenedField).HasColumnName("RetentionDateOpenedField");
            this.Property(t => t.RetentionDateCreateField).HasColumnName("RetentionDateCreateField");
            this.Property(t => t.RetentionDateClosedField).HasColumnName("RetentionDateClosedField");
            this.Property(t => t.RetentionDateOtherField).HasColumnName("RetentionDateOtherField");
            this.Property(t => t.RetentionFieldName).HasColumnName("RetentionFieldName");
            this.Property(t => t.TrackingPhoneFieldName).HasColumnName("TrackingPhoneFieldName");
            this.Property(t => t.TrackingMailStopFieldName).HasColumnName("TrackingMailStopFieldName");
            this.Property(t => t.TrackingRequestableFieldName).HasColumnName("TrackingRequestableFieldName");
            this.Property(t => t.OperatorsIdField).HasColumnName("OperatorsIdField");
            this.Property(t => t.InactiveLocationField).HasColumnName("InactiveLocationField");
            this.Property(t => t.DefaultDescriptionField).HasColumnName("DefaultDescriptionField");
            this.Property(t => t.DefaultDescriptionText).HasColumnName("DefaultDescriptionText");
            this.Property(t => t.DefaultRetentionId).HasColumnName("DefaultRetentionId");
            this.Property(t => t.DescFieldPrefixOneTable).HasColumnName("DescFieldPrefixOneTable");
            this.Property(t => t.DescFieldPrefixOneWidth).HasColumnName("DescFieldPrefixOneWidth");
            this.Property(t => t.DescRelateTable1).HasColumnName("DescRelateTable1");
            this.Property(t => t.DescFieldPrefixTwoTable).HasColumnName("DescFieldPrefixTwoTable");
            this.Property(t => t.DescFieldPrefixTwoWidth).HasColumnName("DescFieldPrefixTwoWidth");
            this.Property(t => t.DescRelateTable2).HasColumnName("DescRelateTable2");
            this.Property(t => t.MaxRecordsAllowed).HasColumnName("MaxRecordsAllowed");
            this.Property(t => t.OutTable).HasColumnName("OutTable");
            this.Property(t => t.PCFilesEditGrp).HasColumnName("PCFilesEditGrp");
            this.Property(t => t.PCFilesNVerGrp).HasColumnName("PCFilesNVerGrp");
            this.Property(t => t.RestrictAddToTable).HasColumnName("RestrictAddToTable");
            this.Property(t => t.RuleDateField).HasColumnName("RuleDateField");
            this.Property(t => t.TrackingACTIVEFieldName).HasColumnName("TrackingACTIVEFieldName");
            this.Property(t => t.TrackingOUTFieldName).HasColumnName("TrackingOUTFieldName");
            this.Property(t => t.TrackingType).HasColumnName("TrackingType");
            this.Property(t => t.AuditConfidentialData).HasColumnName("AuditConfidentialData");
            this.Property(t => t.AuditUpdate).HasColumnName("AuditUpdate");
            this.Property(t => t.AllowBatchRequesting).HasColumnName("AllowBatchRequesting");
            this.Property(t => t.ParentFolderTableName).HasColumnName("ParentFolderTableName");
            this.Property(t => t.ParentDocTypeTableName).HasColumnName("ParentDocTypeTableName");
            this.Property(t => t.RecordManageMgmtType).HasColumnName("RecordManageMgmtType");
            this.Property(t => t.TrackingEmailFieldName).HasColumnName("TrackingEmailFieldName");
            this.Property(t => t.AutoAddNotification).HasColumnName("AutoAddNotification");
            this.Property(t => t.TrackingDueBackDaysFieldName).HasColumnName("TrackingDueBackDaysFieldName");
            this.Property(t => t.ImageCaptureFlagFieldName).HasColumnName("ImageCaptureFlagFieldName");
            this.Property(t => t.SignatureRequiredFieldName).HasColumnName("SignatureRequiredFieldName");
            this.Property(t => t.AuditAttachments).HasColumnName("AuditAttachments");
            this.Property(t => t.RetentionFinalDisposition).HasColumnName("RetentionFinalDisposition");
            this.Property(t => t.RetentionAssignmentMethod).HasColumnName("RetentionAssignmentMethod");
            this.Property(t => t.RetentionRelatedTable).HasColumnName("RetentionRelatedTable");
            this.Property(t => t.ArchiveLocationField).HasColumnName("ArchiveLocationField");
            this.Property(t => t.OfficialRecordHandling).HasColumnName("OfficialRecordHandling");
            this.Property(t => t.DescriptionFieldName).HasColumnName("DescriptionFieldName");
            this.Property(t => t.SearchOrder).HasColumnName("SearchOrder");
            this.Property(t => t.CanAttachToNewRow).HasColumnName("CanAttachToNewRow");
            this.Property(t => t.DefaultChildLayoutId).HasColumnName("DefaultChildLayoutId");
        }
    }
}
