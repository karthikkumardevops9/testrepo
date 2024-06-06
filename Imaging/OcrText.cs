
using System.Runtime.Serialization;

namespace MSRecordsEngine.Imaging
{
    [DataContract()]
    public partial class OcrText
    {
        public OcrText()
        {
            // 
        }

        public OcrText(string Text, string ErrorMessage) : this()
        {
            _ocredText = Text;
            _errorMessage = ErrorMessage;
        }

        [DataMember()]
        public string OcredText
        {
            get
            {
                return _ocredText;
            }
            set
            {
                _ocredText = value;
            }
        }
        private string _ocredText = string.Empty;

        [DataMember()]
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }
        private string _errorMessage = string.Empty;
    }
}
