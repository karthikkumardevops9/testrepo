
namespace MSRecordsEngine.Imaging
{
    public partial class AuditType
    {
        // The AuditType enums are partically duplicated here from Smead.RecordsManagement.Auditing.AuditType enum to avoid circular references
        // ALSO they are duplicated in SRME.SLAuditUpdates.AuditType enum so we don't need to register another assembly for COM
        // IMPORTANT to keep in sync.  RVW 10/27/2021
        public enum AttachmentViewerActionType
        {
            AddAttachment = 301,
            AddVersion,
            AddPage,
            RenameAttachment = 306,
            MoveAttachment,
            AttachOrphan,
            DeleteAttachment,
            DeleteVersion,
            DeletePage,
            DeleteOrphan,
            CheckIn,
            CheckOut,
            UndoCheckOut,
            MarkOfficial,
            RotatePage,
            EditAnnotations,
            RotateAnnotations
        }
        public enum ApiActionType
        {
            AddRecord = 601,
            DeleteRecord,
            UpdateRecord
        }
    }
}
