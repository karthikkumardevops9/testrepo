using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Table
    {
        public int TableId { get; set; }
        public string TableName { get; set; }
        public string UserName { get; set; }
        public string DBName { get; set; }
        private Nullable<short> _DatabaseAccessType;
        public Nullable<short> DatabaseAccessType
        {
            get
            {
                return _DatabaseAccessType;
            }
            set { _DatabaseAccessType = value; }
        }
        private Nullable<bool> _Attachments;
        public Nullable<bool> Attachments
        {
            get
            {
                if (_Attachments == null)
                    return false;
                else
                    return _Attachments;
            }
            set { _Attachments = value; }
        }
        public string AliasImagingTableName { get; set; }
        private Nullable<short> _TrackingTable;
        public Nullable<short> TrackingTable
        {
            get
            {
                return _TrackingTable;
            }
            set { _TrackingTable = value; }
        }
        private Nullable<bool> _Trackable;
        public Nullable<bool> Trackable
        {
            get
            {
                if (_Trackable == null)
                    return false;
                else
                    return _Trackable;
            }
            set { _Trackable = value; }
        }
        public string TrackingStatusFieldName { get; set; }
        public string CounterFieldName { get; set; }
        private Nullable<int> _ViewGroup;
        public Nullable<int> ViewGroup
        {
            get
            {
                if (_ViewGroup == null)
                    return 0;
                else
                    return _ViewGroup;
            }
            set { _ViewGroup = value; }
        }
        private Nullable<int> _AddGroup;
        public Nullable<int> AddGroup
        {
            get
            {
                if (_AddGroup == null)
                    return 0;
                else
                    return _AddGroup;
            }
            set { _AddGroup = value; }
        }
        private Nullable<int> _EditGroup;
        public Nullable<int> EditGroup
        {
            get
            {
                if (_EditGroup == null)
                    return 0;
                else
                    return _EditGroup;
            }
            set { _EditGroup = value; }
        }
        private Nullable<int> _DelGroup;
        public Nullable<int> DelGroup
        {
            get
            {
                if (_DelGroup == null)
                    return 0;
                else
                    return _DelGroup;
            }
            set { _DelGroup = value; }
        }
        private Nullable<int> _MgrGroup;
        public Nullable<int> MgrGroup
        {
            get
            {
                if (_MgrGroup == null)
                    return 0;
                else
                    return _MgrGroup;
            }
            set { _MgrGroup = value; }
        }
        private Nullable<int> _DeleteAttachedGroup;
        public Nullable<int> DeleteAttachedGroup
        {
            get
            {
                if (_DeleteAttachedGroup == null)
                    return 0;
                else
                    return _DeleteAttachedGroup;
            }
            set { _DeleteAttachedGroup = value; }
        }
        private Nullable<int> _AttributesID;
        public Nullable<int> AttributesID
        {
            get
            {
                if (_AttributesID == null)
                    return 0;
                else
                    return _AttributesID;
            }
            set { _AttributesID = value; }
        }
        public string Picture { get; set; }
        public string IdFieldName { get; set; }
        public string IdFieldName2 { get; set; }
        public string IdStripChars { get; set; }
        public string IdMask { get; set; }
        public string BarCodePrefix { get; set; }
        public string DescFieldPrefixOne { get; set; }
        public string DescFieldNameOne { get; set; }
        public string DescFieldPrefixTwo { get; set; }
        public string DescFieldNameTwo { get; set; }
        private Nullable<int> _MaxRecsOnDropDown;
        public Nullable<int> MaxRecsOnDropDown
        {
            get
            {
                if (_MaxRecsOnDropDown == null)
                    return 0;
                else
                    return _MaxRecsOnDropDown;
            }
            set { _MaxRecsOnDropDown = value; }
        }
        private Nullable<bool> _ADOServerCursor;
        public Nullable<bool> ADOServerCursor
        {
            get
            {
                if (_ADOServerCursor == null)
                    return false;
                else
                    return _ADOServerCursor;
            }
            set { _ADOServerCursor = value; }
        }
        private Nullable<int> _ADOQueryTimeout;
        public Nullable<int> ADOQueryTimeout
        {
            get
            {
                if (_ADOQueryTimeout == null)
                    return 0;
                else
                    return _ADOQueryTimeout;
            }
            set { _ADOQueryTimeout = value; }
        }
        private Nullable<int> _ADOCacheSize;
        public Nullable<int> ADOCacheSize
        {
            get
            {
                if (_ADOCacheSize == null)
                    return 0;
                else
                    return _ADOCacheSize;
            }
            set { _ADOCacheSize = value; }
        }
        public string LSBeforeAddRecord { get; set; }
        public string LSAfterAddRecord { get; set; }
        public string LSBeforeEditRecord { get; set; }
        public string LSAfterEditRecord { get; set; }
        public string LSBeforeDeleteRecord { get; set; }
        public string LSAfterDeleteRecord { get; set; }
        public string DefaultTrackingTable { get; set; }
        public string DefaultTrackingId { get; set; }
        private Nullable<bool> _RetentionPeriodActive;
        public Nullable<bool> RetentionPeriodActive
        {
            get
            {
                if (_RetentionPeriodActive == null)
                    return false;
                else
                    return _RetentionPeriodActive;
            }
            set { _RetentionPeriodActive = value; }
        }
        private Nullable<bool> _RetentionInactivityActive;
        public Nullable<bool> RetentionInactivityActive
        {
            get
            {
                if (_RetentionInactivityActive == null)
                    return false;
                else
                    return _RetentionInactivityActive;
            }
            set { _RetentionInactivityActive = value; }
        }
        public string RetentionDateOpenedField { get; set; }
        public string RetentionDateCreateField { get; set; }
        public string RetentionDateClosedField { get; set; }
        public string RetentionDateOtherField { get; set; }
        public string RetentionFieldName { get; set; }
        public string TrackingPhoneFieldName { get; set; }
        public string TrackingMailStopFieldName { get; set; }
        public string TrackingRequestableFieldName { get; set; }
        public string OperatorsIdField { get; set; }
        public string InactiveLocationField { get; set; }
        public string DefaultDescriptionField { get; set; }
        public string DefaultDescriptionText { get; set; }
        public string DefaultRetentionId { get; set; }
        public string DescFieldPrefixOneTable { get; set; }
        private Nullable<short> _DescFieldPrefixOneWidth;
        public Nullable<short> DescFieldPrefixOneWidth
        {
            get
            {
                return _DescFieldPrefixOneWidth;
            }
            set { _DescFieldPrefixOneWidth = value; }
        }
        public string DescRelateTable1 { get; set; }
        public string DescFieldPrefixTwoTable { get; set; }
        private Nullable<short> _DescFieldPrefixTwoWidth;
        public Nullable<short> DescFieldPrefixTwoWidth
        {
            get
            {
                return _DescFieldPrefixTwoWidth;
            }
            set { _DescFieldPrefixTwoWidth = value; }
        }
        public string DescRelateTable2 { get; set; }
        private Nullable<short> _MaxRecordsAllowed;
        public Nullable<short> MaxRecordsAllowed
        {
            get
            {
                return _MaxRecordsAllowed;
            }
            set { _MaxRecordsAllowed = value; }
        }
        private Nullable<short> _OutTable;
        public Nullable<short> OutTable
        {
            get
            {
                return _OutTable;
            }
            set { _OutTable = value; }
        }
        private Nullable<int> _PCFilesEditGrp;
        public Nullable<int> PCFilesEditGrp
        {
            get
            {
                if (_PCFilesEditGrp == null)
                    return 0;
                else
                    return _PCFilesEditGrp;
            }
            set { _PCFilesEditGrp = value; }
        }
        private Nullable<int> _PCFilesNVerGrp;
        public Nullable<int> PCFilesNVerGrp
        {
            get
            {
                if (_PCFilesNVerGrp == null)
                    return 0;
                else
                    return _PCFilesNVerGrp;
            }
            set { _PCFilesNVerGrp = value; }
        }
        private Nullable<short> _RestrictAddToTable;
        public Nullable<short> RestrictAddToTable
        {
            get
            {
                return _RestrictAddToTable;
            }
            set { _RestrictAddToTable = value; }
        }
        public string RuleDateField { get; set; }
        public string TrackingACTIVEFieldName { get; set; }
        public string TrackingOUTFieldName { get; set; }
        private Nullable<short> _TrackingType;
        public Nullable<short> TrackingType
        {
            get
            {
                return _TrackingType;
            }
            set { _TrackingType = value; }
        }
        private Nullable<bool> _AuditConfidentialData;
        public Nullable<bool> AuditConfidentialData
        {
            get
            {
                if (_AuditConfidentialData == null)
                    return false;
                else
                    return _AuditConfidentialData;
            }
            set { _AuditConfidentialData = value; }
        }
        private Nullable<bool> _AuditUpdate;
        public Nullable<bool> AuditUpdate
        {
            get
            {
                if (_AuditUpdate == null)
                    return false;
                else
                    return _AuditUpdate;
            }
            set { _AuditUpdate = value; }
        }
        private Nullable<bool> _AllowBatchRequesting;
        public Nullable<bool> AllowBatchRequesting
        {
            get
            {
                if (_AllowBatchRequesting == null)
                    return false;
                else
                    return _AllowBatchRequesting;
            }
            set { _AllowBatchRequesting = value; }
        }
        public string ParentFolderTableName { get; set; }
        public string ParentDocTypeTableName { get; set; }
        private Nullable<int> _RecordManageMgmtType;
        public Nullable<int> RecordManageMgmtType
        {
            get
            {
                if (_RecordManageMgmtType == null)
                    return 0;
                else
                    return _RecordManageMgmtType;
            }
            set { _RecordManageMgmtType = value; }
        }
        public string TrackingEmailFieldName { get; set; }
        private Nullable<bool> _AutoAddNotification;
        public Nullable<bool> AutoAddNotification
        {
            get
            {
                if (_AutoAddNotification == null)
                    return false;
                else
                    return _AutoAddNotification;
            }
            set { _AutoAddNotification = value; }
        }
        public string TrackingDueBackDaysFieldName { get; set; }
        public string ImageCaptureFlagFieldName { get; set; }
        public string SignatureRequiredFieldName { get; set; }
        private Nullable<bool> _AuditAttachments;
        public Nullable<bool> AuditAttachments
        {
            get
            {
                if (_AuditAttachments == null)
                    return false;
                else
                    return _AuditAttachments;
            }
            set { _AuditAttachments = value; }
        }
        private Nullable<int> _RetentionFinalDisposition;
        public Nullable<int> RetentionFinalDisposition
        {
            get
            {
                if (_RetentionFinalDisposition == null)
                    return 0;
                else
                    return _RetentionFinalDisposition;
            }
            set { _RetentionFinalDisposition = value; }
        }
        private Nullable<int> _RetentionAssignmentMethod;
        public Nullable<int> RetentionAssignmentMethod
        {
            get
            {
                if (_RetentionAssignmentMethod == null)
                    return 0;
                else
                    return _RetentionAssignmentMethod;
            }
            set { _RetentionAssignmentMethod = value; }
        }
        public string RetentionRelatedTable { get; set; }
        public string ArchiveLocationField { get; set; }
        private Nullable<bool> _OfficialRecordHandling;
        public Nullable<bool> OfficialRecordHandling
        {
            get
            {
                if (_OfficialRecordHandling == null)
                    return false;
                else
                    return _OfficialRecordHandling;
            }
            set { _OfficialRecordHandling = value; }
        }
        public string DescriptionFieldName { get; set; }
        private Nullable<int> _SearchOrder;
        public Nullable<int> SearchOrder
        {
            get
            {
                if (_SearchOrder == null)
                    return 0;
                else
                    return _SearchOrder;
            }
            set { _SearchOrder = value; }
        }
        private Nullable<bool> _CanAttachToNewRow;
        public Nullable<bool> CanAttachToNewRow
        {
            get
            {
                if (_CanAttachToNewRow == null)
                    return false;
                else
                    return _CanAttachToNewRow;
            }
            set { _CanAttachToNewRow = value; }
        }
        private Nullable<int> _DefaultChildLayoutId;
        public Nullable<int> DefaultChildLayoutId
        {
            get
            {
                if (_DefaultChildLayoutId == null)
                    return 0;
                else
                    return _DefaultChildLayoutId;
            }
            set { _DefaultChildLayoutId = value; }
        }
    }
}
