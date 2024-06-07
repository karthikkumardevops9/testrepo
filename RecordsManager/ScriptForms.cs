using System.Drawing;
namespace MSRecordsEngine.RecordsManager
{
    public class ScriptForm
    {
        private bool _visible;
        private bool _raiseEventAllowed;
        private int _height;
        private int _left;
        private int _top;
        private int _width;
        private System.Drawing.Color _backColor;
        private System.Drawing.Color _foreColor;
        private string _caption = string.Empty;
        private string _currentTableName = string.Empty;
        private string _tag = string.Empty;
        private string _text = string.Empty;
        private Winform.FormWindowState _windowState;
        private System.Drawing.Font _font;
        private sfFormSize _size;

        private string _emailAttachments = string.Empty;
        private string _emailFromAddress = string.Empty;
        private string _emailFromName = string.Empty;
        private string _emailSubject = string.Empty;
        private string _emailToAddress = string.Empty;
        private string _emailToName = string.Empty;

        public enum sfFormSize
        {
            Normal,
            Autosize,
            Maximized
        }

        internal ScriptForm(bool raiseEventAllowed) : base()
        {
            _raiseEventAllowed = raiseEventAllowed;
        }

        // ------------------------------------------------------------------------------------------
        // Property: BackColor (System.Drawing.Color)
        // ------------------------------------------------------------------------------------------
        public System.Drawing.Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Caption (String)
        // ------------------------------------------------------------------------------------------
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: CurrentTableName (String)
        // ------------------------------------------------------------------------------------------
        public string CurrentTableName
        {
            get
            {
                return _currentTableName;
            }
            set
            {
                _currentTableName = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: FormFont (System.Drawing.Font)
        // ------------------------------------------------------------------------------------------
        public System.Drawing.Font FormFont
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: ForeColor (System.Drawing.Color)
        // ------------------------------------------------------------------------------------------
        public System.Drawing.Color ForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                _foreColor = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: FormHeight (Integer)
        // ------------------------------------------------------------------------------------------
        public int FormHeight
        {
            get
            {
                return _height;
            }
            set
            {
                if (value <= 0)
                    return;
                _height = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: FormLeft (Integer)
        // ------------------------------------------------------------------------------------------
        public int FormLeft
        {
            get
            {
                return _left;
            }
            set
            {
                if (value <= 0)
                    return;
                _left = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: FormTop (Integer)
        // ------------------------------------------------------------------------------------------
        public int FormTop
        {
            get
            {
                return _top;
            }
            set
            {
                if (value <= 0)
                    return;
                _top = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: FormWidth (Integer)
        // ------------------------------------------------------------------------------------------
        public int FormWidth
        {
            get
            {
                return _width;
            }
            set
            {
                if (value <= 0)
                    return;
                _width = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Tag (String)
        // ------------------------------------------------------------------------------------------
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Text (String)
        // ------------------------------------------------------------------------------------------
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: Visible (Boolean)
        // ------------------------------------------------------------------------------------------
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }
        // ------------------------------------------------------------------------------------------
        // Property: WindowState (System.Windows.Forms.FormWindowState)
        // ------------------------------------------------------------------------------------------
        public Winform.FormWindowState WindowState
        {
            get
            {
                return _windowState;
            }
            set
            {
                _windowState = value;
            }
        }

        public sfFormSize FormSize
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
            }
        }

        public string EmailAttachments
        {
            get
            {
                return _emailAttachments;
            }
            set
            {
                _emailAttachments = value;
            }
        }

        public string EmailFromAddress
        {
            get
            {
                return _emailFromAddress;
            }
            set
            {
                _emailFromAddress = value;
            }
        }

        public string EmailFromName
        {
            get
            {
                return _emailFromName;
            }
            set
            {
                _emailFromName = value;
            }
        }
        public string EmailSubject
        {
            get
            {
                return _emailSubject;
            }
            set
            {
                _emailSubject = value;
            }
        }
        public string EmailToAddress
        {
            get
            {
                return _emailToAddress;
            }
            set
            {
                _emailToAddress = value;
            }
        }
        public string EmailToName
        {
            get
            {
                return _emailToName;
            }
            set
            {
                _emailToName = value;
            }
        }

    }

    public class ScriptWaitForm
    {
        private bool _cancelled;
        private bool _cancelVisible;
        private bool _percentVisible;
        private bool _raiseEventAllowed;
        private int _percentage;
        private string _description;
        private string _displayMessage;
        private string _windowCaption;

        public event CloseEventHandler Close;

        public delegate void CloseEventHandler(ref bool bSuccessful);

        internal ScriptWaitForm(bool raiseEventAllowed) : base()
        {
            _raiseEventAllowed = raiseEventAllowed;
        }

        public bool Cancelled
        {
            get
            {
                return _cancelled;
            }
            set
            {
                _cancelled = value;
            }
        }

        public bool CancelVisible
        {
            get
            {
                return _cancelVisible;
            }
            set
            {
                _cancelVisible = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public string DisplayMessage
        {
            get
            {
                return _displayMessage;
            }
            set
            {
                _displayMessage = value;
            }
        }

        public bool PercentVisible
        {
            get
            {
                return _percentVisible;
            }
            set
            {
                _percentVisible = value;
            }
        }

        public int Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > 100)
                {
                    value = 100;
                }
                _percentage = value;
            }
        }

        public string WindowCaption
        {
            get
            {
                return _windowCaption;
            }
            set
            {
                _windowCaption = value;
            }
        }
    }
}