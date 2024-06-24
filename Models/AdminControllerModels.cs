using MSRecordsEngine.Entities;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MSRecordsEngine.Models
{
    public class CheckChildTableExistParam
    {
        public string ConnectionString { get; set; }
        public int TableId { get; set; }
    }
    public class SetAuditPropertiesDataParam
    {
        public Passport Passport { get; set; }
        public string ConnectionString { get; set; }
        public int TableId { get; set; }
        public bool AuditConfidentialData { get; set; }
        public bool AuditUpdate { get; set; }
        public bool AuditAttachments { get; set; }
        public bool IsChild { get; set; }

    }

    public class RemoveTableFromListParam
    {
        public string ConnectionString { get; set; }
        public string[] TableId { get; set; }
    }

    public class SetBackgroundDataParam
    {
        public string ConnectionString { get; set; }
        public string Id { get; set; }
        public string Section { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }

    public class GetReportInformationParams
    {
        public Passport passport { get; set; }
        public string ConnectionString { get; set; }
        public int pReportID { get; set; }
        public int bIsAdd { get; set; }
    }

    public class ReturnReportInfo
    {
        public string lstTblNamesList { get; set; }
        public string lstReportStylesList { get; set; }
        public string lstChildTablesObjStr { get; set; }
        public string sReportName { get; set; }
        public string tblName { get; set; }
        public string sReportStyleId { get; set; }
        public int subViewId2 { get; set; }
        public int subViewId3 { get; set; }
    }

    public class BarCodeList_TrackingFieldListParams
    {
        public string ConnectionString { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
    }

    public class ReturnRetentionPeriodTablesList
    {
        public string jsonObject { get; set; }
        public string systemJsonObject { get; set; }
        public string serviceJsonObject { get; set; }
    }

    public class ReturnEditAttachmentSettingsEntity
    {
        public string DefaultSettingId { get; set; }
        public bool PrintingFooter { get; set; }
        public bool RenameOnScan { get; set; }
    }

    public class ReturnGetTablesView
    {
        public string lstViewStr { get; set; }
        public string lstChildTablesObjStr { get; set; }
    }

    public class GetTablesViewParams
    {
        public Passport passport { get; set; }
        public string ConnectionString { get; set; }
        public string pTableName { get; set; }
    }

    public class SetOutputSettingsEntityParams
    {
        public Passport passport { get; set; }
        public string ConnectionString { get; set; }
        public OutputSetting outputSetting { get; set; }
        public string DirName { get; set; }
        public bool pInActive { get; set; }
    }

    public class ReturnErrorTypeErrorMsg
    {
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public string stringValue1 { get; set; }
        public string stringValue2 { get; set; }
        public bool boolValue { get; set; }
        public List<int> intLst { get; set; }
    }

    public class EditRemoveOutputSettingsEntityParams
    {
        public Passport passport { get; set; }
        public string ConnectionString { get; set; }
        public string[] pRowSelected { get; set; }
    }

    public class SetAttachmentSettingsEntityParam
    {
        public string ConnectionString { get; set; }
        public string pDefaultOpSettingsId { get; set; }
        public bool pPrintImageFooter { get; set; }
        public bool pRenameOnScan { get; set; }
    }

    public class GetAuditPropertiesDataParams
    {
        public string ConnectionString { get; set; }
        public int TableId { get; set; }
    }

    public class ReturnGetAuditPropertiesData
    {
        public bool? AuditConfidentialData { get; set; }
        public bool? AuditUpdate { get; set; }
        public bool? AuditAttachments { get; set; }
        public bool? confenabled { get; set; }
        public bool? attachenabled { get; set; }
    }

    public class RemoveBarCodeSearchEntityParams
    {
        public string ConnectionString { get; set; }
        public int pId { get; set; }
        public int scan { get; set; }
    }

    public class RemoveRequestorEntityParam
    {
        public string ConnectionString { get; set; }
        public string RequestStatus { get; set; }
    }

    public class PurgeAuditDataParams
    {
        public string ConnectionString { get; set; }
        public DateTime PurgeDate { get; set; }
        public bool UpdateData { get; set; }
        public bool ConfData { get; set; }
        public bool SuccessLoginData { get; set; }
        public bool FailLoginData { get; set; }
    }

    public class GetBackgroundProcessParams
    {
        public string ConnectionString { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
    }

    public class DeleteBackgroundProcessTasksParams
    {
        public string ConnectionString { get; set; }
        public DateTime BGEndDate { get; set; }
        public bool CheckkBGStatusCompleted { get; set; }
        public bool CheckBGStatusError { get; set; }
    }

    public class RemoveBackgroundSectionParams
    {
        public string ConnectionString { get; set;}
        public string SectionArrayObject { get; set; }
    }

    public class SetRequestorSystemEntityParams
    {
        public string ConnectionString { get; set; }
        public bool AllowList { get; set; }
        public bool PopupList { get; set; }
    }

    public class SetTrackingHistoryDataParams
    {
        public string ConnectionString { get; set; }
        public int MaxHistoryDays { get; set; }
        public int MaxHistoryItems { get; set;}
    }

    public class HelperTrackingHistory
    {
        public bool Success { get; set; }
        public string KeysType { get; set; }
    }

    public class SetTrackingSystemEntityParam
    {
        public string ConnectionString { get; set; }
        public bool DateDueOn {  get; set; }
        public bool TrackingOutOn { get; set ; }
        public string TrackingAdditionalField1Desc { get; set; }
        public string TrackingAdditionalField2Desc { get; set; }
        public int TrackingAdditionalField1Type { get; set; }
        public short SystemTrackingDefaultDueBackDays { get; set; }
        public int SystemTrackingMaxHistoryItems { get; set;}
        public int SystemTrackingMaxHistoryDays { get; set;}
    }

    public class RemoveTrackingFieldParams
    {
        public string ConnectionString { get; set; }
        public int RowId { get; set; }
    }

    public class SLTrackingSelectDataParam
    {
        public string ConnectionString { get; set; }
        public int SLTrackingSelectDataId { get; set; }
        public string Id { get; set; }
    }

    public class ResetRequestorLabelParam
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }

    public class SetEmailDetailsParams
    {
        public string ConnectionString { get; set; }
        public bool EMailDeliveryEnabled { get; set; }
        public bool EMailWaitListEnabled { get; set; }
        public bool EMailExceptionEnabled { get; set; }
        public bool EMailBackgroundEnabled { get; set; }
        public bool SMTPAuthentication { get; set; }
        public string SystemEmailSMTPServer { get; set; }
        public int SystemEmailSMTPPort { get; set; }
        public int SystemEmailEMailConfirmationType { get; set; }
        public string SystemEmailSMTPUserPassword { get; set; }
        public string SystemEmailSMTPUserAddress { get; set; }
    }

    public class GetSMTPDetailsParams
    {
        public string ConnectionString { get; set;}
        public bool FlagSMPT { get; set; }
    }

    public class SetWarningMessageParams
    {
        public string WebRootPath { get; set; }
        public string WarningMessage { get; set; }
        public string ShowMessage { get; set; }
    }

    public class RemoveRetentionTableFromListParam
    {
        public string ConnectionString { get; set;}
        public string[] TableIds { get; set; }
    }

    public class GetRetentionPropertiesDataParams
    {
        public Passport Passport { get; set;}
        public int TableId { get; set; }
    }

    public class ReturnGetRetentionPropertiesData
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public string TableEntity { get; set; }
        public bool Trackable { get; set; }
        public string RetCodeFieldsObject { get; set; }
        public string DateFields {  get; set; }
        public string ListRetentionCode {  get; set; }
        public string ListDateCreated { get; set;}
        public string ListDateClosed { get; set;}
        public string ListDateOpened { get; set;}
        public string ListDateOther { get; set;}
        public bool FootNote { get; set;}
        public string RelatedTblObj { get; set;}
        public string RetentionCodesJSON { get; set; }
        public string ArchiveLocationField { get; set; }
        public Dictionary<string, string> IsThereLocation { get; set ; }
    }

    public class SetRetentionParametersParam
    {
        public string ConnectionString { get; set; }
        public bool IsUseCitaions { get; set; }
        public int YearEnd { get; set; }
        public int InactivityPeriod { get; set; }
    }

    public class SetRetentionTblPropDataParam
    {
        public string ConnectionString { get; set; }
        public int TableId { get; set; }
        public bool InActivity { get; set; }
        public int Assignment { get; set; }
        public int Disposition { get; set; }
        public string DefaultRetentionId { get; set; }
        public string RelatedTable { get; set; }
        public string RetentionCode { get; set; }
        public string DateOpened { get; set; }
        public string DateClosed { get; set; }
        public string DateCreated { get; set; }
        public string OtherDate { get; set; }
    }

    public class SetbarCodeSearchEntityParams
    {
        public string ConnectionString { set; get; }
        public int Id { get; set; }
        public string FieldName { get; set; }
        public int ScanOrder { get; set; }
        public string TableName { get; set; }
        public string IdStripChars { get; set; }
        public string IdMask { get; set; }
    }
}
