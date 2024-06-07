using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class AddInReport
    {
        public int Id { get; set; }
        private Nullable<int> _Sequence;
        public Nullable<int> Sequence
        {
            get
            {
                if (_Sequence == null)
                    return 0;
                else
                    return _Sequence;
            }
            set { _Sequence = value; }
        }
        public string MenuText { get; set; }
        public string CommandLine { get; set; }
    }
}
