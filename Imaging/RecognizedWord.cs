using System;

namespace MSRecordsEngine.Imaging
{
    [CLSCompliant(true)]
    [Serializable()]
    public class RecognizedWord : object
    {

        private string _word = string.Empty;
        private System.Drawing.Rectangle _wordArea;
        private int _wordZone;

        public RecognizedWord() : base()
        {
        }

        public RecognizedWord(string word, System.Drawing.Rectangle wordarea, int zone) : base()
        {
            _word = word;
            _wordArea = wordarea;
            _wordZone = zone;
        }

        public string Word
        {
            get
            {
                return _word;
            }
        }

        public System.Drawing.Rectangle WordArea
        {
            get
            {
                return _wordArea;
            }
        }

        public int WordLeft
        {
            get
            {
                return _wordArea.Left;
            }
        }

        public int WordTop
        {
            get
            {
                return _wordArea.Top;
            }
        }

        public int WordWidth
        {
            get
            {
                return _wordArea.Width;
            }
        }

        public int WordHeight
        {
            get
            {
                return _wordArea.Height;
            }
        }

        public int WordZone
        {
            get
            {
                return _wordZone;
            }
        }
    }
}