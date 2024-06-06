using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace MSRecordsEngine.RecordsManager
{
    public class Winform
    {
        public enum FormWindowState
        {
            /// <summary>A default sized window.</summary>
            Normal,
            /// <summary>A minimized window.</summary>
            Minimized,
            /// <summary>A maximized window.</summary>
            Maximized,
        }
        public class DataFormats
        {
            /// <summary>Specifies the standard ANSI text format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Text = nameof(Text);
            /// <summary>Specifies the standard Windows Unicode text format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string UnicodeText = nameof(UnicodeText);
            /// <summary>Specifies the Windows device-independent bitmap (DIB) format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Dib = "DeviceIndependentBitmap";
            /// <summary>Specifies a Windows bitmap format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Bitmap = nameof(Bitmap);
            /// <summary>Specifies the Windows enhanced metafile format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string EnhancedMetafile = nameof(EnhancedMetafile);
            /// <summary>Specifies the Windows metafile format, which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string MetafilePict = "MetaFilePict";
            /// <summary>Specifies the Windows symbolic link format, which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string SymbolicLink = nameof(SymbolicLink);
            /// <summary>Specifies the Windows Data Interchange Format (DIF), which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Dif = "DataInterchangeFormat";
            /// <summary>Specifies the Tagged Image File Format (TIFF), which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Tiff = "TaggedImageFileFormat";
            /// <summary>Specifies the standard Windows original equipment manufacturer (OEM) text format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string OemText = "OEMText";
            /// <summary>Specifies the Windows palette format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Palette = nameof(Palette);
            /// <summary>Specifies the Windows pen data format, which consists of pen strokes for handwriting software; Windows Forms does not use this format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string PenData = nameof(PenData);
            /// <summary>Specifies the Resource Interchange File Format (RIFF) audio format, which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Riff = "RiffAudio";
            /// <summary>Specifies the wave audio format, which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string WaveAudio = nameof(WaveAudio);
            /// <summary>Specifies the Windows file drop format, which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string FileDrop = nameof(FileDrop);
            /// <summary>Specifies the Windows culture format, which Windows Forms does not directly use. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Locale = nameof(Locale);
            /// <summary>Specifies text in the HTML Clipboard format. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Html = "HTML Format";
            /// <summary>Specifies text consisting of Rich Text Format (RTF) data. This <see langword="static" /> field is read-only.</summary>
            public static readonly string Rtf = "Rich Text Format";
            /// <summary>Specifies a comma-separated value (CSV) format, which is a common interchange format used by spreadsheets. This format is not used directly by Windows Forms. This <see langword="static" /> field is read-only.</summary>
            public static readonly string CommaSeparatedValue = "Csv";
            /// <summary>Specifies the Windows Forms string class format, which Windows Forms uses to store string objects. This <see langword="static" /> field is read-only.</summary>
            public static readonly string StringFormat = typeof(string).FullName;
            private static int formatCount = 0;
            private static object internalSyncObject = new object();
        }
    }
}
