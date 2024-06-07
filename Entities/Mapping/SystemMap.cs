using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace MSRecordsEngine.Entities.Mapping
{
    public class SystemMap : EntityTypeConfiguration<System>
    {
        public SystemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            
            // Table & Column Mappings
            this.ToTable("System");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.HeadingHeight).HasColumnName("HeadingHeight");
            this.Property(t => t.RowHeight).HasColumnName("RowHeight");
            this.Property(t => t.GridFontName).HasColumnName("GridFontName");
            this.Property(t => t.GridFontSize).HasColumnName("GridFontSize");
            this.Property(t => t.GridFontBold).HasColumnName("GridFontBold");
            this.Property(t => t.GridHdrAttributes).HasColumnName("GridHdrAttributes");
            this.Property(t => t.TabFontName).HasColumnName("TabFontName");
            this.Property(t => t.TabFontSize).HasColumnName("TabFontSize");
            this.Property(t => t.TabFontBold).HasColumnName("TabFontBold");
            this.Property(t => t.BaseFontName).HasColumnName("BaseFontName");
            this.Property(t => t.BaseFontSize).HasColumnName("BaseFontSize");
            this.Property(t => t.BaseFontBold).HasColumnName("BaseFontBold");
            this.Property(t => t.GridOffsetX).HasColumnName("GridOffsetX");
            this.Property(t => t.GridOffsetY).HasColumnName("GridOffsetY");
            this.Property(t => t.TabMaxWidth).HasColumnName("TabMaxWidth");
            this.Property(t => t.TabCutSize).HasColumnName("TabCutSize");
            this.Property(t => t.TabHeight).HasColumnName("TabHeight");
            this.Property(t => t.TabRowOffset).HasColumnName("TabRowOffset");
            this.Property(t => t.TabSelectType).HasColumnName("TabSelectType");
            this.Property(t => t.TabsPerRow).HasColumnName("TabsPerRow");
            this.Property(t => t.FrameWidth).HasColumnName("FrameWidth");
            this.Property(t => t.RMAGgroup).HasColumnName("RMAGgroup");
            this.Property(t => t.MgrGroup).HasColumnName("MgrGroup");
            this.Property(t => t.LibrarianMDIPercent).HasColumnName("LibrarianMDIPercent");
            this.Property(t => t.ImageBackColor1).HasColumnName("ImageBackColor1");
            this.Property(t => t.ImageBackColor2).HasColumnName("ImageBackColor2");
            this.Property(t => t.ImageBackColor3).HasColumnName("ImageBackColor3");
            this.Property(t => t.ImageGridBackColor1).HasColumnName("ImageGridBackColor1");
            this.Property(t => t.ImageGridBackColor2).HasColumnName("ImageGridBackColor2");
            this.Property(t => t.ImageGridBackColor3).HasColumnName("ImageGridBackColor3");
            this.Property(t => t.AnnotationAdd).HasColumnName("AnnotationAdd");
            this.Property(t => t.AnnotationEdit).HasColumnName("AnnotationEdit");
            this.Property(t => t.AnnotationDelete).HasColumnName("AnnotationDelete");
            this.Property(t => t.AnnotationView).HasColumnName("AnnotationView");
            this.Property(t => t.NoGridURL).HasColumnName("NoGridURL");
            this.Property(t => t.ADOConnectionTimeout).HasColumnName("ADOConnectionTimeout");
            this.Property(t => t.DefaultOutputSettingsId).HasColumnName("DefaultOutputSettingsId");
            this.Property(t => t.LSAfterDestinationScanned).HasColumnName("LSAfterDestinationScanned");
            this.Property(t => t.LSAfterObjectScanned).HasColumnName("LSAfterObjectScanned");
            this.Property(t => t.LSAfterDestinationAccepted).HasColumnName("LSAfterDestinationAccepted");
            this.Property(t => t.LSAfterObjectAccepted).HasColumnName("LSAfterObjectAccepted");
            this.Property(t => t.LSAfterTrackingComplete).HasColumnName("LSAfterTrackingComplete");
            this.Property(t => t.UseViewDisplayMode).HasColumnName("UseViewDisplayMode");
            this.Property(t => t.FormViewMinLines).HasColumnName("FormViewMinLines");
            this.Property(t => t.ImportRunGroup).HasColumnName("ImportRunGroup");
            this.Property(t => t.ExpressSetupGroup).HasColumnName("ExpressSetupGroup");
            this.Property(t => t.ManualTrackingGroup).HasColumnName("ManualTrackingGroup");
            this.Property(t => t.MaxHistoryDays).HasColumnName("MaxHistoryDays");
            this.Property(t => t.MaxHistoryItems).HasColumnName("MaxHistoryItems");
            this.Property(t => t.TrackingAdditionalField1Desc).HasColumnName("TrackingAdditionalField1Desc");
            this.Property(t => t.TrackingAdditionalField1Type).HasColumnName("TrackingAdditionalField1Type");
            this.Property(t => t.TrackingAdditionalField2Desc).HasColumnName("TrackingAdditionalField2Desc");
            this.Property(t => t.AllowRequests).HasColumnName("AllowRequests");
            this.Property(t => t.AllowWaitList).HasColumnName("AllowWaitList");
            this.Property(t => t.PopupWaitList).HasColumnName("PopupWaitList");
            this.Property(t => t.RequestorOperatorGrp).HasColumnName("RequestorOperatorGrp");
            this.Property(t => t.RequestorHighPGrp).HasColumnName("RequestorHighPGrp");
            this.Property(t => t.RetentionTurnOffCitations).HasColumnName("RetentionTurnOffCitations");
            this.Property(t => t.RetentionYearEnd).HasColumnName("RetentionYearEnd");
            this.Property(t => t.RetentionAttachDelGroup).HasColumnName("RetentionAttachDelGroup");
            this.Property(t => t.RetentionOperatorGroup).HasColumnName("RetentionOperatorGroup");
            this.Property(t => t.GridBackColorEven).HasColumnName("GridBackColorEven");
            this.Property(t => t.GridBackColorOdd).HasColumnName("GridBackColorOdd");
            this.Property(t => t.GridForeColorEven).HasColumnName("GridForeColorEven");
            this.Property(t => t.GridForeColorOdd).HasColumnName("GridForeColorOdd");
            this.Property(t => t.ReportGridColor).HasColumnName("ReportGridColor");
            this.Property(t => t.AlternateRowColors).HasColumnName("AlternateRowColors");
            this.Property(t => t.ArchGroup).HasColumnName("ArchGroup");
            this.Property(t => t.COLDGroup).HasColumnName("COLDGroup");
            this.Property(t => t.DateDueOn).HasColumnName("DateDueOn");
            this.Property(t => t.FaxmGroup).HasColumnName("FaxmGroup");
            this.Property(t => t.ImportGroup).HasColumnName("ImportGroup");
            this.Property(t => t.LabelGroup).HasColumnName("LabelGroup");
            this.Property(t => t.LitigationOn).HasColumnName("LitigationOn");
            this.Property(t => t.NetworkSecurityOn).HasColumnName("NetworkSecurityOn");
            this.Property(t => t.OtherGroup).HasColumnName("OtherGroup");
            this.Property(t => t.PCFilesEditGrp).HasColumnName("PCFilesEditGrp");
            this.Property(t => t.PCFilesNVerGrp).HasColumnName("PCFilesNVerGrp");
            this.Property(t => t.Picture).HasColumnName("Picture");
            this.Property(t => t.PrintFast).HasColumnName("PrintFast");
            this.Property(t => t.ReconciliationOn).HasColumnName("ReconciliationOn");
            this.Property(t => t.RedactViewGrp).HasColumnName("RedactViewGrp");
            this.Property(t => t.RetentionOn).HasColumnName("RetentionOn");
            this.Property(t => t.ScanGroup).HasColumnName("ScanGroup");
            this.Property(t => t.SecurityGroup).HasColumnName("SecurityGroup");
            this.Property(t => t.SQLGroup).HasColumnName("SQLGroup");
            this.Property(t => t.TrackingGroup).HasColumnName("TrackingGroup");
            this.Property(t => t.TrackingOutOn).HasColumnName("TrackingOutOn");
            this.Property(t => t.ReqAutoPrintMethod).HasColumnName("ReqAutoPrintMethod");
            this.Property(t => t.ReqAutoPrintCopies).HasColumnName("ReqAutoPrintCopies");
            this.Property(t => t.ReqAutoPrintInterval).HasColumnName("ReqAutoPrintInterval");
            this.Property(t => t.ReqAutoPrintIDType).HasColumnName("ReqAutoPrintIDType");
            this.Property(t => t.BatchRequestGroup).HasColumnName("BatchRequestGroup");
            this.Property(t => t.AuditingSecurityManagerGrp).HasColumnName("AuditingSecurityManagerGrp");
            this.Property(t => t.RequestConfirmation).HasColumnName("RequestConfirmation");
            this.Property(t => t.EMailDeliveryEnabled).HasColumnName("EMailDeliveryEnabled");
            this.Property(t => t.EMailWaitListEnabled).HasColumnName("EMailWaitListEnabled");
            this.Property(t => t.EMailSendMethod).HasColumnName("EMailSendMethod");
            this.Property(t => t.EMailConfirmationType).HasColumnName("EMailConfirmationType");
            this.Property(t => t.SMTPAuthentication).HasColumnName("SMTPAuthentication");
            this.Property(t => t.SMTPPort).HasColumnName("SMTPPort");
            this.Property(t => t.SMTPServer).HasColumnName("SMTPServer");
            this.Property(t => t.SMTPUserAddress).HasColumnName("SMTPUserAddress");
            this.Property(t => t.SMTPUserPassword).HasColumnName("SMTPUserPassword");
            this.Property(t => t.LastPastDueEmailTime).HasColumnName("LastPastDueEmailTime");
            this.Property(t => t.LastPastDueEmailUser).HasColumnName("LastPastDueEmailUser");
            this.Property(t => t.EMailExceptionEnabled).HasColumnName("EMailExceptionEnabled");
            this.Property(t => t.DefaultDueBackDays).HasColumnName("DefaultDueBackDays");
            this.Property(t => t.ImageCaptureGroup).HasColumnName("ImageCaptureGroup");
            this.Property(t => t.ExportGroup).HasColumnName("ExportGroup");
            this.Property(t => t.NotificationEnabled).HasColumnName("NotificationEnabled");
            this.Property(t => t.AttachmentVersionGroup).HasColumnName("AttachmentVersionGroup");
            this.Property(t => t.RedactEditGrp).HasColumnName("RedactEditGrp");
            this.Property(t => t.UseTableIcons).HasColumnName("UseTableIcons");
            this.Property(t => t.SignatureCaptureOn).HasColumnName("SignatureCaptureOn");
            this.Property(t => t.InactiveRecordGroup).HasColumnName("InactiveRecordGroup");
            this.Property(t => t.PrintImageFooter).HasColumnName("PrintImageFooter");
            this.Property(t => t.RenameOnScan).HasColumnName("RenameOnScan");
        }
    }
}
