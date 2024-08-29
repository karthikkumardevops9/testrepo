using Microsoft.AspNetCore.Mvc.Rendering;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models.FusionModels;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Data;

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

    public class ReturnGetTablesView : ReturnErrorTypeErrorMsg
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

    public class SetSystemAddressDetailsParam
    {
        public SystemAddress SystemAddress { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SetVolumeDetailsParam
    {
        public Volume Volume { get; set; }
        public Passport Passport { get; set; }
        public bool Active { get; set; }
    }

    public class GetVolumesListParams
    {
        public string ConnectionString { get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public string pId { get; set; }
        public int rows { get; set; }
    }

    public class ReturnGetGeneralDetails : ReturnErrorTypeErrorMsg
    {
        public string CursorFlagJSON { get; set; }
        public string AuditflagJSON { get; set; }
        public string SelectTableJSON { get; set; }
        public string DisplayFieldListJSON { get; set; }
        public string ServerPathJSON { get; set; }
        public string DBUserNameJSON { get; set; }
        public string UserTableIconJSON { get; set; }
        public string AttachmentLicenseJSON { get; set; }
    }

    public class GetGeneralDetailsParam
    {
        public string ConnectionString { get; set; }
        public bool AttachmentPermission { get; set; }
        public string TableName { get; set; }
        public string ServerPath { get; set; }
    }

    public class SetGeneralDetailsParam
    {
        public Table Table { get; set; }
        public bool Attachments { get; set; }
        public int OfficialRecord { get; set; }
        public Passport Passport { get; set; }
    }

    public class ReturnSetGeneralDetails : ReturnErrorTypeErrorMsg
    {
        public string SearchValueJSON { get; set; }
        public string WarnMsgJSON { get; set; }
    }

    public class LoadFieldDataParam
    {
        public string TableName { get; set; }
        public string sidx {  get; set; }
        public string sord { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public string ConnectionString { get; set;}
    }

    public class ReturnGetFieldTypeList : ReturnErrorTypeErrorMsg
    {
        public string FieldTypesList { get; set; }
    }

    public class ReturnCheckBeforeRemoveFieldFromTable : ReturnErrorTypeErrorMsg
    {
        public bool DeleteIndexes { get; set; }
    }

    public class RemoveFieldFromTableParam
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public bool DeleteIndexes { get; set; }
        public string ConnectionString { get; set;}
    }

    public class ReturnCheckFieldBeforeEdit
    {
        public string Message { get; set; }
        public string IndexMessage { get; set; }
    }

    public class CheckBeforeUpdatepParam
    {
        public string FieldName { get; set;}
        public int NewFieldSize { get; set; }
        public int NewFieldType { get; set; }
        public int OrigFieldSize { get; set; }
        public int OrigFieldType { get; set; }
    }

    public class AddEditFieldParam
    {
        public string OperationName { get; set; }
        public string TableName { get; set; }
        public string NewInternalName { get; set; }
        public string OriginalInternalName { get; set; }
        public string FieldType { get; set; }
        public string OriginalFieldType { get; set; }
        public string FieldSize { get; set; }
        public string OriginalFieldSize { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReturnGetTableTrackingProperties : ReturnErrorTypeErrorMsg
    {
        public string ContainerList { get; set; }
        public string SystemEntities { get; set; }
        public string SelectTable { get; set; }
        public string OutFieldList { get; set; }
        public string DueBackFieldList { get; set; }
        public string ActiveFieldList { get; set; }
        public string EmailAddressList { get; set; }
        public string RequesFieldList { get; set; }
        public string InactiveFieldList { get; set; }
        public string ArchiveFieldList { get; set; }
        public string UserIdFieldList { get; set; }
        public string PhoneFieldList { get; set; }
        public string MailSTopFieldList { get; set; }
        public string SignatureFieldList { get; set; }
        public string LabelDestination { get; set; }
    }

    public class SetTableTrackingDetailsParams
    {
        public Table TrackingForm { get; set; }
        public bool FieldFlag { get; set; }
        public bool AutoAddNotification { get; set; }
        public bool AllowBatchRequesting { get; set; }
        public bool Trackable { get; set; }
        public Passport Passport { get; set; }
    }

    public class ReturnSetTableTrackingDetails : ReturnErrorTypeErrorMsg
    {
        public string WarningMessage { set; get;}
    }

    public class GetTableEntityParam
    {
        public string TableName { get; set; }
        public int ContainerInfo { get; set; }
        public string StatusFieldText { get; set; }
        public string ConnectionString {  get; set; }
    }

    public class ReturnGetTrackingDestination : ReturnErrorTypeErrorMsg
    {
        public string RecordJSON { set; get; }
        public string ColVisibleJSON { get; set; }
        public string Col1VisibleJSON { get; set; }
        public string Col2VisibleJSON { get; set; }
        public string ColDataFieldJSON { get; set; }
        public string Col1DataFieldJSON { get; set; }
        public string Col2DataFieldJSON { get; set; }
        public string BRequestPermissionJSON { get; set; }
        public string BTransferPermissionJSON { get; set; }
        public string TableObjectJSON { get; set;}
        public string NoRecordMsgJSON { get; set; }
    }

    public class GetTrackingDestinationParam
    {
        public string TableName { get; set; }
        public Passport Passport { get; set; }
        public bool ConfigureTransfer { get; set; }
        public bool TransferValue { get; set; }
        public bool RequestVal { get; set; }
    }

    public class SetFileRoomOrderRecordParam
    {
        public SLTableFileRoomOrder SLTableFileRoomOrder { get; set; }
        public string TableName { get; set; }
        public bool StartFromFront { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReturnGetAdvanceDetails : ReturnErrorTypeErrorMsg
    {
        public string TableEntity { get; set; }
        public string ParentFolderList { get; set; }
        public string ParentDocList { get; set; }
        public string Flag { get; set; }
    }

    public class SetAdvanceDetailsParam
    {
        public Table Table { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReturnSetAdvanceDetails: ReturnErrorTypeErrorMsg
    {
        public string WarningMsg { get; set; }
    }

    public class GetDataListParams
    {
        public string ConnectionString { get; set; }
        public string TableName {  get; set; }
        public string sidx { get; set;}
        public string sord { get; set;}
        public int page { get; set;}
        public int rows { get; set;}
    }

    public class DeleteSelectedRowsParam
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string rows { get; set; }
        public string col { get; set; }
    }

    public class ProcessRequestParam
    {
        public string Data {  get; set; }
        public string TableName { get; set; }
        public string ColName { get; set; }
        public string ColType { get; set; }
        public string ColumnName { get; set; }
        public string PkValue { get; set; }
        public string ColumnValue { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SetNewWorkgroupParam
    {
        public Passport Passport { get; set; }
        public string WorkGroupName { get; set; }
        public int TabsetsId { get; set; }
    }

    public class ReturnSetNewWorkgroup : ReturnErrorTypeErrorMsg
    {
        public string TabSetId { get; set; }
    }

    public class RemoveNewWorkgroupParam
    {
        public Passport Passport { get; set; }
        public int TabSetsId { get; set; }
    }

    public class SetNewTableParam
    {
        public int ParentNodeId { get; set; }
        public int FieldType { get; set; }
        public int FieldSize { get; set; }
        public int NodeLevel { get; set; }
        public string TableName { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string IdFieldName { get; set; }
        public Passport Passport { get; set; }
    }

    public class ReturnSetNewTable : ReturnErrorTypeErrorMsg
    {
        public string NewNodeId { get; set; }
        public int ViewIdTemp { get; set; }
    }

    public class RtnSetTablesEntity
    {
        public bool Success { get; set; }
        public int TableId { get; set; }
    }

    public class RtnSetViewsEntity
    {
        public bool Success { get; set; }
        public int ViewId { get; set; }
    }

    public class RtnSetRelationshipsEntity
    {
        public bool Success { get; set; }
        public int RelationshipId { get; set;}
    }

    public class RtnSetTabSetEntity
    {
        public bool Success { get; set; }
        public int TableTabId { get; set; }
    }

    public class ReturnGetDeleteTableNames : ReturnErrorTypeErrorMsg
    {
        public string ParentTable { get; set; }
        public string ChildTable { get; set; }
        public string ChildTableId { get; set; }
    }

    public class ReturnDeleteTable : ReturnErrorTypeErrorMsg
    {
        public bool IsUnattached { get; set; }
    }

    public class ReturnLoadAttachExistingTableScreen
    {
        public string TableName { get; set; }
        public string IdFieldName { get; set; }
        public string TableIdColumnList { get; set;}
        public string TablesList { get; set;}
    }

    public class RenameTreeNodeParam
    {
        public string PrevNodeName {  get; set; }
        public string NewNodeName {  get; set; }
        public int Id {  get; set; }
        public string RenameOperation {  get; set; }
        public Passport Passport {  get; set; }
    }

    public class ChangeNodeOrderParam
    {
        public int UpperTableId { get; set; }
        public string TableName { get; set; }
        public int TableId { get; set; }
        public char Action { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReturnGetDatabaseList : ReturnErrorTypeErrorMsg
    {
        public string DatabaseList { get; set; }
    }

    public class GetDatabaseListParam
    {
        public string Instance { get; set; }
        public string UserID { get; set; }
        public string Pass { get; set; }
    }

    public class AddNewDBParam
    {
        public Databas Databas { get; set; }
        public string Connectionstring { get; set; }
        public string DBProvider { get; set; }
    }

    public class CheckIfDateChangedParam
    {
        public Databas Databas { get; set; }
        public string Connectionstring { get; set; }
    }

    public class ReturnLoadRegisterList : ReturnErrorTypeErrorMsg
    {
        public string RegisterBlock { get; set; }
    }

    public class ReturnGetAvailableDatabase : ReturnErrorTypeErrorMsg
    {
        public string ExternalDB { get; set; }
        public string SystemDB { get; set; }
    }

    public class ReturnGetAvailableTable : ReturnErrorTypeErrorMsg
    {
        public bool Flag { get; set; }
        public string UnregisterList { get; set; }
    }

    public class ReturnGetPrimaryField : ReturnErrorTypeErrorMsg
    {
        public string PrimaryFieldList { get; set; }
    }

    public class GetAvailableTableParam
    {
        public string DbName { get; set; }
        public string Server { get; set; }
        public string ConnectionString { get; set; }
    }

    public class SetRegisterTableParam
    {
        public string DbName { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public Passport Passport { get; set; }
    }

    public class UnRegisterTableParam
    {
        public string DbName { get; set; }
        public string TableName { get; set; }
        public Passport Passport { get; set; }
    }

    public class DropTableParam
    {
        public string DbName { get; set; }
        public string TableName { get; set; }
        public Passport Passport { get; set; }
    }

    public class ReturnLoadMapView
    {
        public List<SystemEntity> SystemEntities { get; set; }
        public List<RelationShip> RelationShips { get; set; }
        public List<Table> Tables { get; set; }
        public List<TableTab> TableTabs { get; set; }
        public List<TabSet> TabSets { get; set; }
        public IEnumerable<SelectListItem> DatabaseList { get; set; }
        public SelectList FieldTypeList { get; set; }
    }

    public class ReturnGetAttachTableFieldsList : ReturnErrorTypeErrorMsg
    {
        public string lstColumnNames { get; set; }
    }

    public class ReportsTreePartialParam
    {
        public string Root { get; set; }
        public int iCntRpt { get; set; }
        public Passport Passport { get; set; }
    }

    public class DeleteReportsPrintingColumnParam
    {
        public List<ViewColumn> LstViewColumns { get; set; }
        public int ColumnId { get; set; }
        public int Index { get; set; }
    }

    public class ReturnDeleteReportsPrintingColumn : ReturnErrorTypeErrorMsg
    {
        public List<ViewColumn> LstViewColumns { get; set; }
    }

    public class GetColumnsForPrintingParam
    {
        public string TableName { get; set; }
        public int ViewId { get; set; }
        public int Index { get; set; }
        public Passport Passport { get; set;}
    }

    public class ReturnGetColumnsForPrinting : ReturnErrorTypeErrorMsg
    {
        public string ViewColumnEntity { get; set; }
        public string ViewsEntity { get; set; }
        public string LstTrackedHistory { get; set; }
        public string TableEntity { get; set; }
        public List<ViewColumn> TempViewColumns { get; set; }
        public bool Trackable { get; set; }
    }

    public class SetReportDefinitionValuesParam
    {
        public string OldReportName { get; set; }
        public string ReportStyle { get; set; }
        public Dictionary<string, string> FormData { get; set; }
        public Passport Passport { get; set; }
        public List<ViewColumn> TempViewColumns_1 { get; set; }
        public List<ViewColumn> TempViewColumns_2 { get; set; }
        public List<ViewColumn> TempViewColumns_3 { get; set; }
    }

    public class ReturnSetReportDefinitionValues : ReturnErrorTypeErrorMsg
    {
        public int Level1Id { get; set; }
    }

    public class RemoveActiveLevelParam
    {
        public Array ViewIds { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReturnSetColumnInTempViewCol : ReturnErrorTypeErrorMsg
    {
        public string LstViewColumn { get; set; }
        public List<ViewColumn> TempViewColumns { get; set; }
    }

    public class RtnFillViewColField
    {
        public List<KeyValuePair<string, string>> LstKeyValuePair { get; set; }
        public Dictionary<string, string> mcFieldName { get; set; }
        public Dictionary<string, RelationShip> mcRelationships { get; set; }
        public Dictionary<string, int> mcLevel { get; set; }
    }

    public class SetColumnInTempViewColParam
    {
        public ViewColumn formEntity{ get; set; }
        public int DisplayStyleData{ get; set; }
        public int DuplicateType{ get; set; }
        public string TableName{ get; set; }
        public string reportNameOrId{ get; set; }
        public int LevelNum{ get; set; }
        public string SQLString{ get; set; }
        public string FieldNameTB { get; set; }
        public bool IsReportColumn { get; set; }
        public bool DropDownFlagBool { get; set; }
        public bool pPageBreakField { get; set; }
        public bool pSuppressDuplicates { get; set; }
        public bool pCountColumn { get; set; }
        public bool pSubtotalColumn { get; set; }
        public bool pRestartPageNumber { get; set; }
        public bool pUseAsPrintId { get; set; }
        public bool pSortableField { get; set; }
        public bool pFilterField { get; set; }
        public bool pEditAllowed { get; set; }
        public bool pColumnVisible { get; set; }
        public bool pDropDownSuggestionOnly { get; set; }
        public bool pMaskInclude { get; set; }
        public Enums.geViewColumnsLookupType LookupNumber { get; set; }
        public string ConnectionString { get; set; }
        public bool tempExist { get; set; }
        public List<ViewColumn> tempViewColumns { get; set; }
    }

    public class DropDownValidationParam
    {
        public bool CurrentStatus {  get; set; }
        public bool EditStatus {  get; set; }
        public string TableName {  get; set; }
        public string FieldNameVar {  get; set; }
        public string ConnectionString {  get; set; }
        public Enums.geViewColumnsLookupType LookUpVar {  get; set; }
    }

    public class ReturnDropDownValidation : ReturnErrorTypeErrorMsg
    {
        public string ChkEditable {  get; set; }
        public string ChkDropDownSuggest {  get; set; }
    }

    public class ReturnIsFLDNamesExists : ReturnErrorTypeErrorMsg
    {
        public bool FldNamesExists { get; set; }
    }

    public class ReturnGetTableFieldsListAndParentTableFields : ReturnErrorTypeErrorMsg
    {
        public List<string> LstAllColumnNames { get; set; }
    }

    public class ValidateTABQUIKSQLStatmentsParams
    {
        public TabQuickSQL TabQuickSQL { get; set; }
        public string ConnectionString { get; set; }
    }

    public class TabQuickSQL
    {
        public string SQLStatement { get; set; }
        public string sTableName { get; set; }
        public bool IsSelectStatement { get; set; }
    }

    public class FormTABQUIKSelectSQLStatementParam
    {
        public TabQuikUI TabQuikUI { get; set; }
        public string ConnectionString { get; set; }
        public List<string> lstAllColumnNames { get; set; }
    }

    public class TabQuikUI
    {
        public string pTableName { get; set; }
        public string dtTABQUIKData { get; set; }
    }

    public class ReturnFormTABQUIKSelectSQLStatement : ReturnErrorTypeErrorMsg
    {
        public string SQLStatement { get; set; }
    }

    public class TabQuickSaveTABQUIKFields
    {
        public string sOperation { get; set; }
        public string JobName { get; set; }
        public string TableName { get; set; }
        public string SQLSelectString { get; set; }
        public string SQLUpdateString { get; set; }
        public string dtTABQUIKData { get; set; }

    }

    public class SaveTABQUIKFieldsParam
    {
        public TabQuickSaveTABQUIKFields TabQuickSaveTABQUIKFields { get; set; }
        public string ConnectionString { get; set; }
        public string FLDColumns { get; set; }
        public List<string> lstAllColumnNames { get; set; }
    }

    public class GetTABQUIKMappingGridParam
    {
        public string JobName { get; set; }
        public string TableName { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReturnGetTABQUIKMappingGrid
    {
        public OneStripJob OneStripJob { get; set; }
        public OneStripJob OneStripJobGeneral { get; set; }
        public List<OneStripJobField> OneStripJobField { get; set; }
    }

    public class SystemEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
