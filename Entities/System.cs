using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class System
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        private Nullable<float> _HeadingHeight;
        public Nullable<float> HeadingHeight
        {
            get
            {
                return _HeadingHeight;
            }
            set { _HeadingHeight = value; }
        }
        private Nullable<short> _RowHeight;
        public Nullable<short> RowHeight
        {
            get
            {
                return _RowHeight;
            }
            set { _RowHeight = value; }
        }
        public string GridFontName { get; set; }
        private Nullable<float> _GridFontSize;
        public Nullable<float> GridFontSize
        {
            get
            {
                return _GridFontSize;
            }
            set { _GridFontSize = value; }
        }
        private Nullable<bool> _GridFontBold;
        public Nullable<bool> GridFontBold
        {
            get
            {
                if (_GridFontBold == null)
                    return false;
                else
                    return _GridFontBold;
            }
            set { _GridFontBold = value; }
        }
        private Nullable<short> _GridHdrAttributes;
        public Nullable<short> GridHdrAttributes
        {
            get
            {
                return _GridHdrAttributes;
            }
            set { _GridHdrAttributes = value; }
        }
        public string TabFontName { get; set; }
        private Nullable<float> _TabFontSize;
        public Nullable<float> TabFontSize
        {
            get
            {
                return _TabFontSize;
            }
            set { _TabFontSize = value; }
        }
        private Nullable<bool> _TabFontBold;
        public Nullable<bool> TabFontBold
        {
            get
            {
                if (_TabFontBold == null)
                    return false;
                else
                    return _TabFontBold;
            }
            set { _TabFontBold = value; }
        }
        public string BaseFontName { get; set; }
        private Nullable<float> _BaseFontSize;
        public Nullable<float> BaseFontSize
        {
            get
            {
                return _BaseFontSize;
            }
            set { _BaseFontSize = value; }
        }
        private Nullable<bool> _BaseFontBold;
        public Nullable<bool> BaseFontBold
        {
            get
            {
                if (_BaseFontBold == null)
                    return false;
                else
                    return _BaseFontBold;
            }
            set { _BaseFontBold = value; }
        }
        private Nullable<short> _GridOffsetX;
        public Nullable<short> GridOffsetX
        {
            get
            {
                return _GridOffsetX;
            }
            set { _GridOffsetX = value; }
        }
        private Nullable<short> _GridOffsetY;
        public Nullable<short> GridOffsetY
        {
            get
            {
                return _GridOffsetY;
            }
            set { _GridOffsetY = value; }
        }
        private Nullable<short> _TabMaxWidth;
        public Nullable<short> TabMaxWidth
        {
            get
            {
                return _TabMaxWidth;
            }
            set { _TabMaxWidth = value; }
        }
        private Nullable<short> _TabCutSize;
        public Nullable<short> TabCutSize
        {
            get
            {
                return _TabCutSize;
            }
            set { _TabCutSize = value; }
        }
        private Nullable<short> _TabHeight;
        public Nullable<short> TabHeight
        {
            get
            {
                return _TabHeight;
            }
            set { _TabHeight = value; }
        }
        private Nullable<short> _TabRowOffset;
        public Nullable<short> TabRowOffset
        {
            get
            {
                return _TabRowOffset;
            }
            set { _TabRowOffset = value; }
        }
        private Nullable<short> _TabSelectType;
        public Nullable<short> TabSelectType
        {
            get
            {
                return _TabSelectType;
            }
            set { _TabSelectType = value; }
        }
        private Nullable<short> _TabsPerRow;
        public Nullable<short> TabsPerRow
        {
            get
            {
                return _TabsPerRow;
            }
            set { _TabsPerRow = value; }
        }
        private Nullable<short> _FrameWidth;
        public Nullable<short> FrameWidth
        {
            get
            {
                return _FrameWidth;
            }
            set { _FrameWidth = value; }
        }
        private Nullable<int> _RMAGgroup;
        public Nullable<int> RMAGgroup
        {
            get
            {
                if (_RMAGgroup == null)
                    return 0;
                else
                    return _RMAGgroup;
            }
            set { _RMAGgroup = value; }
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
        private Nullable<short> _LibrarianMDIPercent;
        public Nullable<short> LibrarianMDIPercent
        {
            get
            {
                return _LibrarianMDIPercent;
            }
            set { _LibrarianMDIPercent = value; }
        }
        private Nullable<int> _ImageBackColor1;
        public Nullable<int> ImageBackColor1
        {
            get
            {
                if (_ImageBackColor1 == null)
                    return 0;
                else
                    return _ImageBackColor1;
            }
            set { _ImageBackColor1 = value; }
        }
        private Nullable<int> _ImageBackColor2;
        public Nullable<int> ImageBackColor2
        {
            get
            {
                if (_ImageBackColor2 == null)
                    return 0;
                else
                    return _ImageBackColor2;
            }
            set { _ImageBackColor2 = value; }
        }
        private Nullable<int> _ImageBackColor3;
        public Nullable<int> ImageBackColor3
        {
            get
            {
                if (_ImageBackColor3 == null)
                    return 0;
                else
                    return _ImageBackColor3;
            }
            set { _ImageBackColor3 = value; }
        }
        private Nullable<int> _ImageGridBackColor1;
        public Nullable<int> ImageGridBackColor1
        {
            get
            {
                if (_ImageGridBackColor1 == null)
                    return 0;
                else
                    return _ImageGridBackColor1;
            }
            set { _ImageGridBackColor1 = value; }
        }
        private Nullable<int> _ImageGridBackColor2;
        public Nullable<int> ImageGridBackColor2
        {
            get
            {
                if (_ImageGridBackColor2 == null)
                    return 0;
                else
                    return _ImageGridBackColor2;
            }
            set { _ImageGridBackColor2 = value; }
        }
        private Nullable<int> _ImageGridBackColor3;
        public Nullable<int> ImageGridBackColor3
        {
            get
            {
                if (_ImageGridBackColor3 == null)
                    return 0;
                else
                    return _ImageGridBackColor3;
            }
            set { _ImageGridBackColor3 = value; }
        }
        private Nullable<int> _AnnotationAdd;
        public Nullable<int> AnnotationAdd
        {
            get
            {
                if (_AnnotationAdd == null)
                    return 0;
                else
                    return _AnnotationAdd;
            }
            set { _AnnotationAdd = value; }
        }
        private Nullable<int> _AnnotationEdit;
        public Nullable<int> AnnotationEdit
        {
            get
            {
                if (_AnnotationEdit == null)
                    return 0;
                else
                    return _AnnotationEdit;
            }
            set { _AnnotationEdit = value; }
        }
        private Nullable<int> _AnnotationDelete;
        public Nullable<int> AnnotationDelete
        {
            get
            {
                if (_AnnotationDelete == null)
                    return 0;
                else
                    return _AnnotationDelete;
            }
            set { _AnnotationDelete = value; }
        }
        private Nullable<int> _AnnotationView;
        public Nullable<int> AnnotationView
        {
            get
            {
                if (_AnnotationView == null)
                    return 0;
                else
                    return _AnnotationView;
            }
            set { _AnnotationView = value; }
        }
        public string NoGridURL { get; set; }
        private Nullable<int> _ADOConnectionTimeout;
        public Nullable<int> ADOConnectionTimeout
        {
            get
            {
                if (_ADOConnectionTimeout == null)
                    return 0;
                else
                    return _ADOConnectionTimeout;
            }
            set { _ADOConnectionTimeout = value; }
        }
        public string DefaultOutputSettingsId { get; set; }
        public string LSAfterDestinationScanned { get; set; }
        public string LSAfterObjectScanned { get; set; }
        public string LSAfterDestinationAccepted { get; set; }
        public string LSAfterObjectAccepted { get; set; }
        public string LSAfterTrackingComplete { get; set; }
        private Nullable<bool> _UseViewDisplayMode;
        public Nullable<bool> UseViewDisplayMode
        {
            get
            {
                if (_UseViewDisplayMode == null)
                    return false;
                else
                    return _UseViewDisplayMode;
            }
            set { _UseViewDisplayMode = value; }
        }
        private Nullable<int> _FormViewMinLines;
        public Nullable<int> FormViewMinLines
        {
            get
            {
                if (_FormViewMinLines == null)
                    return 0;
                else
                    return _FormViewMinLines;
            }
            set { _FormViewMinLines = value; }
        }
        private Nullable<int> _ImportRunGroup;
        public Nullable<int> ImportRunGroup
        {
            get
            {
                if (_ImportRunGroup == null)
                    return 0;
                else
                    return _ImportRunGroup;
            }
            set { _ImportRunGroup = value; }
        }
        private Nullable<int> _ExpressSetupGroup;
        public Nullable<int> ExpressSetupGroup
        {
            get
            {
                if (_ExpressSetupGroup == null)
                    return 0;
                else
                    return _ExpressSetupGroup;
            }
            set { _ExpressSetupGroup = value; }
        }
        private Nullable<int> _ManualTrackingGroup;
        public Nullable<int> ManualTrackingGroup
        {
            get
            {
                if (_ManualTrackingGroup == null)
                    return 0;
                else
                    return _ManualTrackingGroup;
            }
            set { _ManualTrackingGroup = value; }
        }
        private Nullable<int> _MaxHistoryDays;
        public Nullable<int> MaxHistoryDays
        {
            get
            {
                if (_MaxHistoryDays == null)
                    return 0;
                else
                    return _MaxHistoryDays;
            }
            set { _MaxHistoryDays = value; }
        }
        private Nullable<int> _MaxHistoryItems;
        public Nullable<int> MaxHistoryItems
        {
            get
            {
                if (_MaxHistoryItems == null)
                    return 0;
                else
                    return _MaxHistoryItems;
            }
            set { _MaxHistoryItems = value; }
        }
        public string TrackingAdditionalField1Desc { get; set; }
        private Nullable<int> _TrackingAdditionalField1Type;
        public Nullable<int> TrackingAdditionalField1Type
        {
            get
            {
                if (_TrackingAdditionalField1Type == null)
                    return 0;
                else
                    return _TrackingAdditionalField1Type;
            }
            set { _TrackingAdditionalField1Type = value; }
        }
        public string TrackingAdditionalField2Desc { get; set; }
        private Nullable<bool> _AllowRequests;
        public Nullable<bool> AllowRequests
        {
            get
            {
                if (_AllowRequests == null)
                    return false;
                else
                    return _AllowRequests;
            }
            set { _AllowRequests = value; }
        }
        private Nullable<bool> _AllowWaitList;
        public Nullable<bool> AllowWaitList
        {
            get
            {
                if (_AllowWaitList == null)
                    return false;
                else
                    return _AllowWaitList;
            }
            set { _AllowWaitList = value; }
        }
        private Nullable<bool> _PopupWaitList;
        public Nullable<bool> PopupWaitList
        {
            get
            {
                if (_PopupWaitList == null)
                    return false;
                else
                    return _PopupWaitList;
            }
            set { _PopupWaitList = value; }
        }
        private Nullable<int> _RequestorOperatorGrp;
        public Nullable<int> RequestorOperatorGrp
        {
            get
            {
                if (_RequestorOperatorGrp == null)
                    return 0;
                else
                    return _RequestorOperatorGrp;
            }
            set { _RequestorOperatorGrp = value; }
        }
        private Nullable<int> _RequestorHighPGrp;
        public Nullable<int> RequestorHighPGrp
        {
            get
            {
                if (_RequestorHighPGrp == null)
                    return 0;
                else
                    return _RequestorHighPGrp;
            }
            set { _RequestorHighPGrp = value; }
        }
        private Nullable<bool> _RetentionTurnOffCitations;
        public Nullable<bool> RetentionTurnOffCitations
        {
            get
            {
                if (_RetentionTurnOffCitations == null)
                    return false;
                else
                    return _RetentionTurnOffCitations;
            }
            set { _RetentionTurnOffCitations = value; }
        }
        private Nullable<int> _RetentionYearEnd;
        public Nullable<int> RetentionYearEnd
        {
            get
            {
                if (_RetentionYearEnd == null)
                    return 0;
                else
                    return _RetentionYearEnd;
            }
            set { _RetentionYearEnd = value; }
        }
        private Nullable<int> _RetentionAttachDelGroup;
        public Nullable<int> RetentionAttachDelGroup
        {
            get
            {
                if (_RetentionAttachDelGroup == null)
                    return 0;
                else
                    return _RetentionAttachDelGroup;
            }
            set { _RetentionAttachDelGroup = value; }
        }
        private Nullable<int> _RetentionOperatorGroup;
        public Nullable<int> RetentionOperatorGroup
        {
            get
            {
                if (_RetentionOperatorGroup == null)
                    return 0;
                else
                    return _RetentionOperatorGroup;
            }
            set { _RetentionOperatorGroup = value; }
        }
        private Nullable<int> _GridBackColorEven;
        public Nullable<int> GridBackColorEven
        {
            get
            {
                if (_GridBackColorEven == null)
                    return 0;
                else
                    return _GridBackColorEven;
            }
            set { _GridBackColorEven = value; }
        }
        private Nullable<int> _GridBackColorOdd;
        public Nullable<int> GridBackColorOdd
        {
            get
            {
                if (_GridBackColorOdd == null)
                    return 0;
                else
                    return _GridBackColorOdd;
            }
            set { _GridBackColorOdd = value; }
        }
        private Nullable<int> _GridForeColorEven;
        public Nullable<int> GridForeColorEven
        {
            get
            {
                if (_GridForeColorEven == null)
                    return 0;
                else
                    return _GridForeColorEven;
            }
            set { _GridForeColorEven = value; }
        }
        private Nullable<int> _GridForeColorOdd;
        public Nullable<int> GridForeColorOdd
        {
            get
            {
                if (_GridForeColorOdd == null)
                    return 0;
                else
                    return _GridForeColorOdd;
            }
            set { _GridForeColorOdd = value; }
        }
        private Nullable<int> _ReportGridColor;
        public Nullable<int> ReportGridColor
        {
            get
            {
                if (_ReportGridColor == null)
                    return 0;
                else
                    return _ReportGridColor;
            }
            set { _ReportGridColor = value; }
        }
        private Nullable<bool> _AlternateRowColors;
        public Nullable<bool> AlternateRowColors
        {
            get
            {
                if (_AlternateRowColors == null)
                    return false;
                else
                    return _AlternateRowColors;
            }
            set { _AlternateRowColors = value; }
        }
        private Nullable<int> _ArchGroup;
        public Nullable<int> ArchGroup
        {
            get
            {
                if (_ArchGroup == null)
                    return 0;
                else
                    return _ArchGroup;
            }
            set { _ArchGroup = value; }
        }
        private Nullable<int> _COLDGroup;
        public Nullable<int> COLDGroup
        {
            get
            {
                if (_COLDGroup == null)
                    return 0;
                else
                    return _COLDGroup;
            }
            set { _COLDGroup = value; }
        }
        private Nullable<bool> _DateDueOn;
        public Nullable<bool> DateDueOn
        {
            get
            {
                if (_DateDueOn == null)
                    return false;
                else
                    return _DateDueOn;
            }
            set { _DateDueOn = value; }
        }
        private Nullable<int> _FaxmGroup;
        public Nullable<int> FaxmGroup
        {
            get
            {
                if (_FaxmGroup == null)
                    return 0;
                else
                    return _FaxmGroup;
            }
            set { _FaxmGroup = value; }
        }
        private Nullable<int> _ImportGroup;
        public Nullable<int> ImportGroup
        {
            get
            {
                if (_ImportGroup == null)
                    return 0;
                else
                    return _ImportGroup;
            }
            set { _ImportGroup = value; }
        }
        private Nullable<int> _LabelGroup;
        public Nullable<int> LabelGroup
        {
            get
            {
                if (_LabelGroup == null)
                    return 0;
                else
                    return _LabelGroup;
            }
            set { _LabelGroup = value; }
        }
        private Nullable<bool> _LitigationOn;
        public Nullable<bool> LitigationOn
        {
            get
            {
                if (_LitigationOn == null)
                    return false;
                else
                    return _LitigationOn;
            }
            set { _LitigationOn = value; }
        }
        private Nullable<bool> _NetworkSecurityOn;
        public Nullable<bool> NetworkSecurityOn
        {
            get
            {
                if (_NetworkSecurityOn == null)
                    return false;
                else
                    return _NetworkSecurityOn;
            }
            set { _NetworkSecurityOn = value; }
        }
        private Nullable<int> _OtherGroup;
        public Nullable<int> OtherGroup
        {
            get
            {
                if (_OtherGroup == null)
                    return 0;
                else
                    return _OtherGroup;
            }
            set { _OtherGroup = value; }
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
        public string Picture { get; set; }
        private Nullable<bool> _PrintFast;
        public Nullable<bool> PrintFast
        {
            get
            {
                if (_PrintFast == null)
                    return false;
                else
                    return _PrintFast;
            }
            set { _PrintFast = value; }
        }
        private Nullable<bool> _ReconciliationOn;
        public Nullable<bool> ReconciliationOn
        {
            get
            {
                if (_ReconciliationOn == null)
                    return false;
                else
                    return _ReconciliationOn;
            }
            set { _ReconciliationOn = value; }
        }
        private Nullable<int> _RedactViewGrp;
        public Nullable<int> RedactViewGrp
        {
            get
            {
                if (_RedactViewGrp == null)
                    return 0;
                else
                    return _RedactViewGrp;
            }
            set { _RedactViewGrp = value; }
        }
        private Nullable<bool> _RetentionOn;
        public Nullable<bool> RetentionOn
        {
            get
            {
                if (_RetentionOn == null)
                    return false;
                else
                    return _RetentionOn;
            }
            set { _RetentionOn = value; }
        }
        private Nullable<int> _ScanGroup;
        public Nullable<int> ScanGroup
        {
            get
            {
                if (_ScanGroup == null)
                    return 0;
                else
                    return _ScanGroup;
            }
            set { _ScanGroup = value; }
        }
        private Nullable<int> _SecurityGroup;
        public Nullable<int> SecurityGroup
        {
            get
            {
                if (_SecurityGroup == null)
                    return 0;
                else
                    return _SecurityGroup;
            }
            set { _SecurityGroup = value; }
        }
        private Nullable<int> _SQLGroup;
        public Nullable<int> SQLGroup
        {
            get
            {
                if (_SQLGroup == null)
                    return 0;
                else
                    return _SQLGroup;
            }
            set { _SQLGroup = value; }
        }
        private Nullable<int> _TrackingGroup;
        public Nullable<int> TrackingGroup
        {
            get
            {
                if (_TrackingGroup == null)
                    return 0;
                else
                    return _TrackingGroup;
            }
            set { _TrackingGroup = value; }
        }
        private Nullable<bool> _TrackingOutOn;
        public Nullable<bool> TrackingOutOn
        {
            get
            {
                if (_TrackingOutOn == null)
                    return false;
                else
                    return _TrackingOutOn;
            }
            set { _TrackingOutOn = value; }
        }
        private Nullable<int> _ReqAutoPrintMethod;
        public Nullable<int> ReqAutoPrintMethod
        {
            get
            {
                if (_ReqAutoPrintMethod == null)
                    return 0;
                else
                    return _ReqAutoPrintMethod;
            }
            set { _ReqAutoPrintMethod = value; }
        }
        private Nullable<int> _ReqAutoPrintCopies;
        public Nullable<int> ReqAutoPrintCopies
        {
            get
            {
                if (_ReqAutoPrintCopies == null)
                    return 0;
                else
                    return _ReqAutoPrintCopies;
            }
            set { _ReqAutoPrintCopies = value; }
        }
        private Nullable<int> _ReqAutoPrintInterval;
        public Nullable<int> ReqAutoPrintInterval
        {
            get
            {
                if (_ReqAutoPrintInterval == null)
                    return 0;
                else
                    return _ReqAutoPrintInterval;
            }
            set { _ReqAutoPrintInterval = value; }
        }
        private Nullable<int> _ReqAutoPrintIDType;
        public Nullable<int> ReqAutoPrintIDType
        {
            get
            {
                if (_ReqAutoPrintIDType == null)
                    return 0;
                else
                    return _ReqAutoPrintIDType;
            }
            set { _ReqAutoPrintIDType = value; }
        }
        private Nullable<int> _BatchRequestGroup;
        public Nullable<int> BatchRequestGroup
        {
            get
            {
                if (_BatchRequestGroup == null)
                    return 0;
                else
                    return _BatchRequestGroup;
            }
            set { _BatchRequestGroup = value; }
        }
        private Nullable<int> _AuditingSecurityManagerGrp;
        public Nullable<int> AuditingSecurityManagerGrp
        {
            get
            {
                if (_AuditingSecurityManagerGrp == null)
                    return 0;
                else
                    return _AuditingSecurityManagerGrp;
            }
            set { _AuditingSecurityManagerGrp = value; }
        }
        private Nullable<int> _RequestConfirmation;
        public Nullable<int> RequestConfirmation
        {
            get
            {
                if (_RequestConfirmation == null)
                    return 0;
                else
                    return _RequestConfirmation;
            }
            set { _RequestConfirmation = value; }
        }
        private Nullable<bool> _EMailDeliveryEnabled;
        public Nullable<bool> EMailDeliveryEnabled
        {
            get
            {
                if (_EMailDeliveryEnabled == null)
                    return false;
                else
                    return _EMailDeliveryEnabled;
            }
            set { _EMailDeliveryEnabled = value; }
        }
        private Nullable<bool> _EMailWaitListEnabled;
        public Nullable<bool> EMailWaitListEnabled
        {
            get
            {
                if (_EMailWaitListEnabled == null)
                    return false;
                else
                    return _EMailWaitListEnabled;
            }
            set { _EMailWaitListEnabled = value; }
        }
        private Nullable<int> _EMailSendMethod;
        public Nullable<int> EMailSendMethod
        {
            get
            {
                if (_EMailSendMethod == null)
                    return 0;
                else
                    return _EMailSendMethod;
            }
            set { _EMailSendMethod = value; }
        }
        private Nullable<int> _EMailConfirmationType;
        public Nullable<int> EMailConfirmationType
        {
            get
            {
                if (_EMailConfirmationType == null)
                    return 0;
                else
                    return _EMailConfirmationType;
            }
            set { _EMailConfirmationType = value; }
        }
        private Nullable<bool> _SMTPAuthentication;
        public Nullable<bool> SMTPAuthentication
        {
            get
            {
                if (_SMTPAuthentication == null)
                    return false;
                else
                    return _SMTPAuthentication;
            }
            set { _SMTPAuthentication = value; }
        }
        private Nullable<int> _SMTPPort;
        public Nullable<int> SMTPPort
        {
            get
            {
                if (_SMTPPort == null)
                    return 0;
                else
                    return _SMTPPort;
            }
            set { _SMTPPort = value; }
        }
        public string SMTPServer { get; set; }
        public string SMTPUserAddress { get; set; }
        public string SMTPUserPassword { get; set; }
        private Nullable<DateTime> _LastPastDueEmailTime;
        public Nullable<DateTime> LastPastDueEmailTime
        {
            get
            {
                return _LastPastDueEmailTime;
            }
            set { _LastPastDueEmailTime = value; }
        }
        public string LastPastDueEmailUser { get; set; }
        private Nullable<bool> _EMailExceptionEnabled;
        public Nullable<bool> EMailExceptionEnabled
        {
            get
            {
                if (_EMailExceptionEnabled == null)
                    return false;
                else
                    return _EMailExceptionEnabled;
            }
            set { _EMailExceptionEnabled = value; }
        }
        private Nullable<short> _DefaultDueBackDays;
        public Nullable<short> DefaultDueBackDays
        {
            get
            {
                return _DefaultDueBackDays;
            }
            set { _DefaultDueBackDays = value; }
        }
        private Nullable<int> _ImageCaptureGroup;
        public Nullable<int> ImageCaptureGroup
        {
            get
            {
                if (_ImageCaptureGroup == null)
                    return 0;
                else
                    return _ImageCaptureGroup;
            }
            set { _ImageCaptureGroup = value; }
        }
        private Nullable<int> _ExportGroup;
        public Nullable<int> ExportGroup
        {
            get
            {
                if (_ExportGroup == null)
                    return 0;
                else
                    return _ExportGroup;
            }
            set { _ExportGroup = value; }
        }
        private Nullable<int> _NotificationEnabled;
        public Nullable<int> NotificationEnabled
        {
            get
            {
                if (_NotificationEnabled == null)
                    return 0;
                else
                    return _NotificationEnabled;
            }
            set { _NotificationEnabled = value; }
        }
        private Nullable<int> _AttachmentVersionGroup;
        public Nullable<int> AttachmentVersionGroup
        {
            get
            {
                if (_AttachmentVersionGroup == null)
                    return 0;
                else
                    return _AttachmentVersionGroup;
            }
            set { _AttachmentVersionGroup = value; }
        }
        private Nullable<int> _RedactEditGrp;
        public Nullable<int> RedactEditGrp
        {
            get
            {
                if (_RedactEditGrp == null)
                    return 0;
                else
                    return _RedactEditGrp;
            }
            set { _RedactEditGrp = value; }
        }
        private Nullable<bool> _UseTableIcons;
        public Nullable<bool> UseTableIcons
        {
            get
            {
                if (_UseTableIcons == null)
                    return false;
                else
                    return _UseTableIcons;
            }
            set { _UseTableIcons = value; }
        }
        private Nullable<int> _SignatureCaptureOn;
        public Nullable<int> SignatureCaptureOn
        {
            get
            {
                if (_SignatureCaptureOn == null)
                    return 0;
                else
                    return _SignatureCaptureOn;
            }
            set { _SignatureCaptureOn = value; }
        }
        private Nullable<int> _InactiveRecordGroup;
        public Nullable<int> InactiveRecordGroup
        {
            get
            {
                if (_InactiveRecordGroup == null)
                    return 0;
                else
                    return _InactiveRecordGroup;
            }
            set { _InactiveRecordGroup = value; }
        }
        private Nullable<bool> _PrintImageFooter;
        public Nullable<bool> PrintImageFooter
        {
            get
            {
                if (_PrintImageFooter == null)
                    return false;
                else
                    return _PrintImageFooter;
            }
            set { _PrintImageFooter = value; }
        }
        private Nullable<bool> _RenameOnScan;
        public Nullable<bool> RenameOnScan
        {
            get
            {
                if (_RenameOnScan == null)
                    return false;
                else
                    return _RenameOnScan;
            }
            set { _RenameOnScan = value; }
        }
    }
}
