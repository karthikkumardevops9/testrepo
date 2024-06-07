using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLRetentionCitation
    {
        public int SLRetentionCitationId { get; set; }
        public string Citation { get; set; }
        public string Subject { get; set; }
        public string Notation { get; set; }
        public string LegalPeriod { get; set; }
    }
}
