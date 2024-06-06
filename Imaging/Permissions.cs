using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic

namespace MSRecordsEngine.Imaging
{
    public partial class Permissions
    {
        public partial class ExceptionString
        {
            public static string AlreadyCheckedOut = "CheckedOut0";
            public static string AuthenticationExpired = "Authentication Expired";
            public static string CannotAddWhenCheckedOut = "{0} cannot be added when an attachment is checked out.";
            public static string CannotDeleteWhenCheckedOut = "Attachment {0} is checked out and cannot be deleted.";
            public static string CannotDeleteVersionWhenCheckedOut = "Versions cannot be deleted when attachment {0} is checked out.";
            public static string CannotLinkWhenCheckedOut = "{0} cannot be linked when an attachment is checked out.";
            public static string CannotLinkToPCFile = "A new {0} cannot be linked to a non-image attachment.";
            public static string CanOnlyAddWhenCheckedOutToMe = "{0} can only be added when an attachment is checked out to the current user.";
            public static string DatabaseNotFound = "Database not found";
            public static string DocumentIdNotFound = "Document Id not found";
            public static string DoesNotExist = "The {0} does not exist. It may have been deleted by another user. Please try again.";
            public static string FileNotCreated = "Could not create file";
            public static string FileNotFound = "File not found";
            public static string FileTypeExcluded = "File type excluded";
            public static string ImageNotFound = "Image not found";
            public static string InvalidImage = "Invalid Image Id";
            public static string InvalidDatabase = "Invalid Database";
            public static string InvalidParameters = "Invalid Parameter(s)";
            public static string InvalidPermissions = "Insufficient Permissions";
            public static string MissingOutputSetting = "There was a problem using the default output setting." + Constants.vbCrLf + "Please contact the System Administrator.";
            public static string MissingParameters = "Missing Parameter(s)";
            public static string NoRecords = "RecordCount0";
            public static string NotAuthorized = "Not Authorized";
            public static string PageNotFound = "Page not found";
            public static string PathIsEmpty = "File/Path is empty";
            public static string StreamIsInvalid = "Stream is invalid";
            public static string StreamUploadFailed = "Upload of stream unsuccessful";
            public static string Successful = "Successful0";
            public static string UnknownError = "Unknown error has occurred, please verify the Image Service is running correctly";
        }

        internal enum Annotation
        {
            View,
            Add,
            Edit,
            Delete,
            Redact
        }
        internal enum Attachment
        {
            Access,
            View,
            Add,
            Edit,
            Delete,
            Print,
            Scanning,
            Index,
            Versioning,
            Email,
            Move,
            MarkOfficial
        }
        internal enum Orphan
        {
            View,
            Add,
            Delete,
            Scanning,
            Index
        }
        internal enum Volume
        {
            Access,
            View,
            Add,
            Edit,
            Delete
        }
    }
}
