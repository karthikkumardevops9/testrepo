using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class Annotation
    {
        public int Id { get; set; }
        public string TableId { get; set; }
        public string Table { get; set; }
        public string Annotation1 { get; set; }
        public string DeskOf { get; set; }
        private Nullable<DateTime> _NoteDateTime;
        public Nullable<DateTime> NoteDateTime
        {
            get
            {
                return _NoteDateTime;
            }
            set { _NoteDateTime = value; }
        }
        public string UserName { get; set; }
        public string NewAnnotation { get; set; }
        private Nullable<bool> _NewAnnotationComplete;
        public Nullable<bool> NewAnnotationComplete
        {
            get
            {
                if (_NewAnnotationComplete == null)
                    return false;
                else
                    return _NewAnnotationComplete;
            }
            set { _NewAnnotationComplete = value; }
        }
    }
}
