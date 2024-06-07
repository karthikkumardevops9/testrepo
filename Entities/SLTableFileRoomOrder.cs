using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLTableFileRoomOrder
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        private Nullable<bool> _StartFromFront;
        public Nullable<bool> StartFromFront
        {
            get
            {
                if (_StartFromFront == null)
                    return false;
                else
                    return _StartFromFront;
            }
            set { _StartFromFront = value; }
        }
        private Nullable<int> _StartingPosition;
        public Nullable<int> StartingPosition
        {
            get
            {
                if (_StartingPosition == null)
                    return 0;
                else
                    return _StartingPosition;
            }
            set { _StartingPosition = value; }
        }
        private Nullable<int> _NumberofCharacters;
        public Nullable<int> NumberofCharacters
        {
            get
            {
                if (_NumberofCharacters == null)
                    return 0;
                else
                    return _NumberofCharacters;
            }
            set { _NumberofCharacters = value; }
        }
        public string FieldFormat { get; set; }
    }
}
