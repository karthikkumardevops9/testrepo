using MSRecordsEngine.Entities;
using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Data;
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

    public class CheckModuleLevelAccessParams
    {
        public Passport passport { get; set; }
        public string TablePermission { get; set; }
        public string iCntRpt { get; set; }
        public string ViewPermission { get; set; }
    }

    public class LoadSecurityUserGridDataParams
    {
        public string ConnectionString { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
    }

    public class SetUserDetailsParams
    {
        public string ConnectionString { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Misc1 { get; set; }
        public string Misc2 { get; set;}
        public bool AccountDisabled { get; set; }
    }

    public class SetUserPasswordParams
    {
        public string ConnectionString { get; set; }
        public int UserId { get; set; }
        public string UserPassword { get; set; }
        public bool CheckedState { get; set; }
    }
    public class SetGroupsAgainstUserParams
    {
        public string ConnectionString { get; set; }
        public int UserID { get; set; }
        public string[] GroupList { get; set; }
    }

    public class UnlockUserAccountParams
    {
        public Passport passport { get; set; }
        public string OperatorId { get; set; }
    }

    public class SetGroupDetailsParams
    {
        public string ConnectionString { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string ActiveDirectoryGroup { get; set; }
        public int AutoLockSeconds { get; set; }
        public int AutoLogOffSeconds { get; set; }
    }

    public class SetUsersAgainstGroupParams
    {
        public string ConnectionString { get; set; }
        public int GroupId { get; set; }
        public string[] UserList { get; set; }
    }

    public class SetPermissionsToSecurableObjectParams
    {
        public Passport Passport { get; set; }
        public int[] SecurableObjIds { get; set; }
        public List<int> PermisionIds { get; set; }
        public List<int> PermissionRvmed { get; set; }
    }
    public class SetGroupPermissionsParams
    {
        public Passport Passport { get; set; }
        public int[] GroupIds { get; set; }
        public int[] SecurableObjIds { get; set; }
        public List <int> PermisionIds { get; set; }
    }

    public class SecureObjectsReturn
    {
        public int SecureObjectID { get; set; }
        public string Name { get; set; }
        public int SecureObjectTypeID { get; set; }
        public int BaseID { get; set; }
    }

    public class ValidateApplicationLinkReq
    {
        public Passport passport { get; set; }
        public string pModuleNameStr { get; set; }
    }

    public class ViewColumnEntity
    {
        public int Id { get; set; }
        public int ViewsId { get; set; }
        public int ColumnNum { get; set; }
        public string FieldName { get; set; }
        public string Heading { get; set; }
        public int LookupType { get; set; }
        public int ColumnWidth { get; set; }
        public bool ColumnVisible { get; set; }
        public int ColumnOrder { get; set; }
        public int ColumnStyle { get; set; }
        public string EditMask { get; set; }
        public string Picture { get; set; }
        public int LookupIdCol { get; set; }
        public int SortField { get; set; }
        public bool SortableField { get; set; }
        public bool FilterField { get; set; }
        public bool CountColumn { get; set; }
        public bool SubtotalColumn { get; set; }
        public bool PrintColumnAsSubheader { get; set; }
        public bool RestartPageNumber { get; set; }
        public bool UseAsPrintId { get; set; }
        public bool DropDownSuggestionOnly { get; set; }
        public bool SuppressPrinting { get; set; }
        public bool ValueCount { get; set; }
        public string AlternateFieldName { get; set; }
        public string DefaultLookupValue { get; set; }
        public string DropDownFilterIdField { get; set; }
        public string DropDownFilterMatchField { get; set; }
        public int DropDownFlag { get; set; }
        public int DropDownReferenceColNum { get; set; }
        public string DropDownReferenceValue { get; set; }
        public string DropDownTargetField { get; set; }
        public bool EditAllowed { get; set; }
        public int FormColWidth { get; set; }
        public int FreezeOrder { get; set; }
        public string InputMask { get; set; }
        public bool MaskClipMode { get; set; }
        public bool MaskInclude { get; set; }
        public string MaskPromptChar { get; set; }
        public int MaxPrintLines { get; set; }
        public bool PageBreakField { get; set; }
        public int PrinterColWidth { get; set; }
        public int SortOrder { get; set; }
        public bool SortOrderDesc { get; set; }
        public bool SuppressDuplicates { get; set; }
        public bool VisibleOnForm { get; set; }
        public bool VisibleOnPrint { get; set; }
        public int AlternateSortColumn { get; set; }
        public int LabelLeft { get; set; }
        public int LabelTop { get; set; }
        public int LabelWidth { get; set; }
        public int LabelHeight { get; set; }
        public int ControlLeft { get; set; }
        public int ControlTop { get; set; }
        public int ControlWidth { get; set; }
        public int ControlHeight { get; set; }
        public int TabOrder { get; set; }
        public int LabelJustify { get; set; }
    }

    public class ViewEntity
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ViewName { get; set; }
        public string SQLStatement { get; set; }
        public int MaxRecsPerFetch { get; set; }
        public string Picture { get; set; }
        public string ReportStylesId { get; set; }
        public int ViewOrder { get; set; }
        public string WorkFlow1 { get; set; }
        public string WorkFlow1Pic { get; set; }
        public string WorkFlowDesc1 { get; set; }
        public string WorkFlowToolTip1 { get; set; }
        public string WorkFlowHotKey1 { get; set; }
        public string WorkFlow2 { get; set; }
        public string WorkFlow2Pic { get; set; }
        public string WorkFlowDesc2 { get; set; }
        public string WorkFlowToolTip2 { get; set; }
        public string WorkFlowHotKey2 { get; set; }
        public string WorkFlow3 { get; set; }
        public string WorkFlow3Pic { get; set; }
        public string WorkFlowDesc3 { get; set; }
        public string WorkFlowToolTip3 { get; set; }
        public string WorkFlowHotKey3 { get; set; }
        public string WorkFlow4 { get; set; }
        public string WorkFlow4Pic { get; set; }
        public string WorkFlowDesc4 { get; set; }
        public string WorkFlowToolTip4 { get; set; }
        public string WorkFlowHotKey4 { get; set; }
        public string WorkFlow5 { get; set; }
        public string WorkFlow5Pic { get; set; }
        public string WorkFlowDesc5 { get; set; }
        public string WorkFlowToolTip5 { get; set; }
        public string WorkFlowHotKey5 { get; set; }
        public int TablesId { get; set; }
        public int ViewGroup { get; set; }
        public bool Visible { get; set; }
        public bool VariableColWidth { get; set; }
        public bool VariableRowHeight { get; set; }
        public bool VariableFixedCols { get; set; }
        public int RowHeight { get; set; }
        public bool AddAllowed { get; set; }
        public int ViewType { get; set; }
        public bool UseExactRowCount { get; set; }
        public string TablesDown { get; set; }
        public bool Printable { get; set; }
        public bool GrandTotal { get; set; }
        public int LeftIndent { get; set; }
        public int RightIndent { get; set; }
        public string SubTableName { get; set; }
        public int SubViewId { get; set; }
        public bool PrintWithoutChildren { get; set; }
        public bool SuppressHeader { get; set; }
        public bool SuppressFooter { get; set; }
        public bool PrintFrozenOnly { get; set; }
        public bool TrackingEverContained { get; set; }
        public bool PrintImages { get; set; }
        public bool PrintImageFullPage { get; set; }
        public bool PrintImageFirstPageOnly { get; set; }
        public bool PrintImageRedlining { get; set; }
        public int PrintImageLeftMargin { get; set; }
        public int PrintImageRightMargin { get; set; }
        public bool PrintImageAllVersions { get; set; }
        public int ChildColumnHeaders { get; set; }
        public bool SuppressImageDataRow { get; set; }
        public bool SuppressImageFooter { get; set; }
        public int DisplayMode { get; set; }
        public bool AutoRotateImage { get; set; }
        public bool GrandTotalOnSepPage { get; set; }
        public string UserName { get; set; }
        public bool IncludeFileRoomOrder { get; set; }
        public int AltViewId { get; set; }
        public bool DeleteGridAvail { get; set; }
        public bool FiltersActive { get; set; }
        public bool IncludeTrackingLocation { get; set; }
        public bool InTaskList { get; set; }
        public string TaskListDisplayString { get; set; }
        public int PrintAttachments { get; set; }
        public bool MultiParent { get; set; }
        public bool SearchableView { get; set; }
        public bool CustomFormView { get; set; }
        public int MaxRecsPerFetchDesktop { get; set; }
    }

    public class ViewFilterEntity
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public int ViewsId { get; set; }
        public int ColumnNum { get; set; }
        public string OpenParen { get; set; }
        public string Operator { get; set; }
        public string FilterData { get; set; }
        public string CloseParen { get; set; }
        public string JoinOperator { get; set; }
        public bool Active { get; set; }
        public int DisplayColumnNum { get; set; }
        public bool PartOfView { get; set; }
    }

    public class GetDataFromViewColumnParams
    {
        public string ConnectionString { get; set; }
        public Dictionary<string, bool> EditSettingList { get; set; }
        public ViewColumn ViewColumnEntity { get; set; }
        public List<ViewColumn> CurrentViewColumn { get; set; }
        public string TableName { get; set; }
        public View View { get; set; }
    }

    public class ReturnFillFieldTypeAndSize
    {
        public string ErrorType { get; set; }
        public string FiledType { get; set; }
        public string FieldSize { get; set; }
        public string EditMaskLength { get; set; }
        public string InputMaskLength { get; set; }
    }

    public class ReturnLoadViewsSettings
    {
        public ViewsCustomModel ViewsCustomModel { get; set; }
        public List<MSRecordsEngine.Models.GridColumns> GridColumnEntities {  get; set; }
        public List<ViewColumn> ViewColumns { get; set; }   
        public  string TableName { get; set; }
    }

    public class GetViewsRelatedDataParam
    {
        public string TableName { get; set; }
        public int ViewId { get; set; }
        public Passport Passport { get; set; }
    }

    public class ReturnGetViewsRelatedData
    {
        public List<ViewFilter> TempViewFilterList {  get; set; }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public int SLTableFileRoomOrderCount { get; set; }
        public int MaxRecsPerFetch { get; set; }
        public bool btnColumnAdd { get; set; }
        public bool ShouldEnableMoveFilter { get; set; }
        public bool SearchableView { get; set ; }
        public bool Trackable { get; set ; }
        public bool TaskList { get; set ; }
        public bool InFileRoomOrder { get; set ; }
        public bool FilterActive { get; set ; }
        public bool IncludeTrackingLocation { get; set ; }
    }

    public class FillInternalFieldNameParams
    {
        public Enums.geViewColumnsLookupType ColumnTypeVar { get; set; }
        public string TableName { get; set; }
        public bool viewFlag { get; set; }
        public bool IsLocationChecked { get; set; }
        public string msSQL { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ValidateViewColEditSettingParams
    {
        public ViewsCustomModel viewsCustomModel { get; set; }
        public string TableName { get; set; }
        public Enums.geViewColumnsLookupType LookupType { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string ConnectionString { get; set; }
    }


    public class  FiltereOperaterValue
    {
        public List<KeyValuePair<string, string>> KeyValuePairs { get; set; }
        public Dictionary<string, bool> DictionaryResult { get; set; }
    }

    public class FillColumnComboboxData
    {
        public DataTable DataTable { get; set; }
        public string ValueFieldName { get; set; }
        public string ThisFieldHeading { get; set; }
        public string FirstLookupHeading { get; set; }
        public string SecondLookupHeading { get; set; }
    }

    public class ViewTreePartialParam
    {
        public string Root { get; set; }
        public Passport Passport { get; set; }
    }
    public class RefreshViewColGridParam
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public List<ViewColumn> ViewColumns { get; set; }
    }

    public class DeleteViewParams
    {
        public int ViewId { get; set; }
        public Passport Passport { get; set; }
    }

    public class ProcessFilterResult
    {
        public string sSql { get; set; }
        public string Error { get; set; }
    }

    public class ValidateFilterDataParam
    {
        public ViewsCustomModel ViewsCustomModel { get; set; }
        public List<ViewColumn> ViewColumns { get; set; }
        public string ConnectionString {  set; get; }
        public bool EventFlag { get; set; }
    }

    public class ReturnValidateFilterData
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorJson { get; set; }
        public string MoveFilterFlagJson { get; set; }
        public List<ViewFilter> ViewFilters { get; set; }
    }

    public class ValidateEditSettingsOnEditParams
    {
        public ViewColumn ViewColumn { get; set; }
        public List<ViewColumn> ViewColumns { get; set;}
        public string TableName { get; set; }
        public View View {  get; set; }
        public string ConnectionString {  get; set; }
    }

    public class DeleteReportParam
    {
        public int ReportId { get; set; }
        public Passport Passport { get; set; }
    }

    public class ValidateSqlStatementParams
    {
        public string ConnectionString { get; set; }
        public bool IncludeFileRoomOrder { get; set; }
        public bool IncludeTrackingLocation { get; set; }
        public bool InTaskList { get; set; }
        public ViewsCustomModel ViewsCustomModel { get; set; }
    }

    public class MoveFilterInSQLParams
    {
        public ViewsCustomModel ViewsCustomModel { get; set; }
        public List<ViewFilter> ViewFilters { get; set; }
        public List<ViewColumn> viewColumns { get; set; }
        public string ConnectionString { get; set; }
    }

    public class GetOperatorDDLDataParam
    {
        public int ViewId { get; set; }
        public int ColumnNum { get; set; }
        public string TableName { get; set; }
        public string ConnectionString { get; set; }
        public List<ViewColumn> ViewColumns { get; set; }
        public List<ViewFilter> ViewFilters { get; set; }
    }

    public class ReturnGetOperatorDDLData
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string LookupFieldJSON { get; set; }
        public string ValueFieldNameJSON { get; set; }
        public string FirstLookupJSON { get; set; }
        public string SecondLookupJSON { get; set; }
        public string RecordJSON { get; set; }
        public string JsonFilterControls { get; set; }
        public string FilterColumnsJSON { get; set; }
        public string JsonObjectOperator { get; set; }
    }

    public class SetViewsDetailsParam
    {
        public Passport Passport { get; set; }
        public ViewsCustomModel ViewsCustomModel { get; set; }
        public List<ViewColumn> ViewColumns { get; set; }
        public List<ViewFilter> ViewFilters { get; set; }
        public Dictionary<int, int> OrgViewColumnIds { get; set; }
        public Dictionary<int, int> UpViewColumnIds { get; set; }
        public bool IncludeFileRoomOrder { get; set; }
        public bool IncludeTrackingLocation { get; set;}
        public bool InTaskList { get; set;}
        public bool FiltersActive { get; set;}
    }

    public class ReturnSetViewsDetails
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string ViewId { get; set; }
        public List<ViewColumn> ViewColumns { get; set; }
        public View View {  get; set; }
    }

    public class ReturnEditOutputSettingsEntity : ReturnErrorTypeErrorMsg
    {
        public string OutputSettingsEntity { get; set; }
        public string FileName { get; set; }
    }

    public class ReturnGetReportStylesData : ReturnErrorTypeErrorMsg
    {
        public string ReportStyleName { get; set; }
        public string ReportStyleEntity { get; set; }
    }

    public class ReturnFillViewColumnControl : ReturnErrorTypeErrorMsg
    {
        public string ColumnType { get; set; }
        public string Allignment {  get; set; }
        public string VisualAttribute { get; set; }
    }

    public class ReturnSetAuditPropertiesData : ReturnErrorTypeErrorMsg 
    {
        public List<int> TableIds { get; set; }
    }
    public class ReturnCheckModuleLevelAccess : ReturnErrorTypeErrorMsg
    {
        public int iCntRpts { get; set; }
        public Dictionary<string, bool> AccessDictionary { get; set; }
        public bool AtLeastOneTablePermissionSessionValue { get; set; }
        public bool AtLeastOneViewPermissionSessionValue { get; set; }
    }

    public class ReturnViewsOrderChange : ReturnErrorTypeErrorMsg
    {
        public bool LowerLast {  get; set; }
        public bool UpperLast { get; set;}
    }

    public class ReturnCheckChildTableExist: ReturnErrorTypeErrorMsg
    {
        public bool ChildExist { get; set; }
    }
}
