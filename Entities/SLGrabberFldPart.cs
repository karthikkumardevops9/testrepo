using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class SLGrabberFldPart
    {
        public int Id { get; set; }
        private Nullable<int> _SLGrabberFieldsId;
        public Nullable<int> SLGrabberFieldsId
        {
            get
            {
                if (_SLGrabberFieldsId == null)
                    return 0;
                else
                    return _SLGrabberFieldsId;
            }
            set { _SLGrabberFieldsId = value; }
        }
        private Nullable<int> _SLGrabberControlsId;
        public Nullable<int> SLGrabberControlsId
        {
            get
            {
                if (_SLGrabberControlsId == null)
                    return 0;
                else
                    return _SLGrabberControlsId;
            }
            set { _SLGrabberControlsId = value; }
        }
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
