using MSRecordsEngine.Entities;
using Smead.Security;

namespace MSRecordsEngine.Models.FusionModels
{
    public class SetRetentionCodeParam
    {
        public SLRetentionCode PRetentionCode { get; set; }
        public Passport passport { get; set; }
        public bool PRetentionLegalHold { get; set; }
        public bool PRetentionPeriodForceToEndOfYear { get; set; }
        public bool PInactivityForceToEndOfYear { get; set; }
        public string InactivityEventType { get; set; }
        public string RetentionEventType { get; set; }
      
    }

    public class ReturnErrorTypeWithMsg
    {
        public string JsonRetCodeObj { get; set; }
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public bool InUse { get; set; }
    }

    public class AssignCitationToRetentionParam
    {
        public string RetentionCodeId { get; set; }
        public string CitationCodeId { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ReplicationCitationParam
    {
        public string pCopyFromRetCode { get; set; }
        public string pCopyToRetCode { get; set; }
        public string ConnectionString { get; set; }
    }

    public class RetentionYearEndReturn
    {
        public string lblRetentionYrEnd { get; set; }
        public bool citaStatus { get; set; }
    }
    public class SetCitationCodeParam
    {
        public SLRetentionCitation PCitationCode { get; set; }
        public string ConnectionString { get; set; }
    }
    public class RemoveCitationCodeParam
    {
        public string CitationCodeId { get; set; }
        public string ConnectionString { get; set; }
    }
    public class ReplaceRetentionParam
    {
        public int TableId { get; set; }
        public string NewRetentionCode { get; set; }
        public string OldRetentionCode { get; set; }
        public bool updateDisposedRecords { get; set; }
        public Passport Passport { get; set; }

    }
}
