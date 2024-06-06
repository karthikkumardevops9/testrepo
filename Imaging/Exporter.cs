using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Properties;


namespace MSRecordsEngine.Imaging
{

    [CLSCompliant(true)]
    public class Exporter : IDisposable
    {

        private const string _delimiter = ",";
        private string _excludeList;

        private int _count;
        private int _startPage = 0;
        private int _endPage = 0;
        private int _dpi = 96;
        private Output.Values _antialiasing = Output.Values.AliasGrayscaleOnly;
        private string _filePrefix = string.Empty;
        private Output.Format _outputFormat = Output.Format.Jpg;

        public event RaiseErrorEventEventHandler RaiseErrorEvent;

        public delegate void RaiseErrorEventEventHandler(string message, int errorNumber);
        public event RaiseStatusEventEventHandler RaiseStatusEvent;

        public delegate void RaiseStatusEventEventHandler(string status, int pageNumber);

        [DllImport("sccex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IO.Err EXCloseExport(IntPtr hExport);
        [DllImport("sccex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IO.Err EXOpenExport(IntPtr hDoc, Output.Format dwOutputId, DataAccess.IOType dwSpecType, byte[] spec, int dwFlags, int dwReserved, int callback, int commandData, ref IntPtr phExport);
        [DllImport("sccex.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IO.Err EXRunExport(IntPtr hExport);

        #region  IDisposable Support 
        private bool disposedValue = false;        // To detect redundant calls
                                                   // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
            }
            disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public Exporter() : base()
        {
            string value;

            try
            {
                value = ConfigurationManager.AppSettings["Exclusions"];
                if(string.IsNullOrEmpty(value))
                    value = ".wma,.exe,.com,.dll,.mp3,.mpg,.wav,.m4p,.m4a,.aac,.mp4,.m4v,.mov,.avi,.3gp,.dvx,." +
            "divx,.xps,.lnk";//Smead.RecordsManagement.Imaging.ExportCS.Settings.Exclusions; // check raju 2
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                value = ".wma,.exe,.com,.dll,.mp3,.mpg,.wav,.m4p,.m4a,.aac,.mp4,.m4v,.mov,.avi,.3gp,.dvx,." +
            "divx,.xps,.lnk";//Smead.RecordsManagement.Imaging.ExportCS.Settings.Exclusions; // check raju 2
            }

            _excludeList = string.Format("{0}{1}{0}", _delimiter, value);

            try
            {
                value = ConfigurationManager.AppSettings["OutputFormat"];
                if (!string.IsNullOrEmpty(value))
                    _outputFormat = (Output.Format)Enum.Parse(typeof(Output.Format), value, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _outputFormat = Output.Format.Jpg;
            }
        }

        public Exporter(string filePrefix) : this()
        {
            if (string.IsNullOrEmpty(filePrefix))
                throw new NullReferenceException("File prefix is required");
            _filePrefix = filePrefix;
        }

        public Exporter(string filePrefix, Output.Format outputFormat) : this(filePrefix)
        {
            _outputFormat = outputFormat;
        }

        public string ExcludeList
        {
            get
            {
                return _excludeList;
            }
        }

        public Output.Values AntiAliasing
        {
            get
            {
                return _antialiasing;
            }
            set
            {
                switch (value)
                {
                    case Output.Values.PagesRange:
                        {
                            _antialiasing = Output.Values.AliasGrayscaleOnly;
                            break;
                        }

                    default:
                        {
                            _antialiasing = value;
                            break;
                        }
                }
            }
        }

        public int DPI
        {
            get
            {
                return _dpi;
            }
            set
            {
                _dpi = value;
            }
        }

        public int StartPage
        {
            get
            {
                return _startPage;
            }
            set
            {
                _startPage = value;
            }
        }

        public int EndPage
        {
            get
            {
                return _endPage;
            }
            set
            {
                _endPage = value;
            }
        }

        public string FilePrefix
        {
            get
            {
                return _filePrefix;
            }
            set
            {
                _filePrefix = value;
            }
        }

        public Output.Format OutputFormat
        {
            get
            {
                return _outputFormat;
            }
            set
            {
                _outputFormat = value;
            }
        }

        private DataAccess.ReturnType CallbackHandler(IntPtr eHandle, string cData, DataAccess.CommandType commandType, [MarshalAs(UnmanagedType.LPStruct)] CallBackData ncI)
        {
            try
            {
                if (commandType == DataAccess.CommandType.NewFileInfo)
                {
                    _count += 1;
                    this.RaiseStatus(ncI.pSpec.Trim(), _count);
                }
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Error \"{0}\" in Exporter.CallbackHandler", ex.Message), -1, string.Empty, default);
            }

            return DataAccess.ReturnType.NotHandled;
        }

        private void CreateDirectory(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
        }

        public static void DeleteFile(string fileName)
        {
            if (fileName.Contains("*"))
            {
                DeleteFiles(fileName);
                return;
            }

            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName))
                return;

            try
            {
                System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(fileName);
                //(string.Format("The file \"{0}\" was deleted successfully in Exporter.DeleteFile", fileName));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Exporter.DeleteFile (fileName: {1})", ex.Message, fileName));
            }
        }

        public static void DeleteFiles(string fileNameWithWildcard)
        {
            if (string.IsNullOrEmpty(fileNameWithWildcard))
                return;
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fileNameWithWildcard)))
                return;
            string[] files = null;

            try
            {
                files = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(fileNameWithWildcard), System.IO.Path.GetFileName(fileNameWithWildcard));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Exporter.DeleteFiles (fileName: {1})", ex.Message, fileNameWithWildcard));
            }

            if (files is not null)
            {
                foreach (string file in files)
                    DeleteFile(file);
            }
        }

        private bool ExcludeExtension(string fileName, bool displayNative)
        {
            if (displayNative)
                return true;
            if (string.IsNullOrEmpty(_excludeList))
                return false;
            if (_excludeList.ToLower().Contains(string.Format("{0}.*{0}", _delimiter)))
                return true;
            string extension = System.IO.Path.GetExtension(fileName).ToLower();
            if (_excludeList.ToLower().Contains(string.Format("{0}{1}{0}", _delimiter, extension)))
                return true;
            return false;
        }

        private IntPtr OpenDocument(string fileName)
        {
            IntPtr hDoc = (IntPtr)0;
            var dataErr = DataAccess.OpenDocument(ref hDoc, DataAccess.IOType.AnsiPath, Output.StringToByteArray(fileName), 0);
            if (dataErr == DataAccess.Err.OK)
                return hDoc;

            ShowError("OpenDocument failed:", (int)dataErr, fileName, "FileName", true);
            return IntPtr.Zero;
        }

        private IntPtr OpenExport(IntPtr hDoc, Output.Format outputFormat, string outputFile, int startPage)
        {
            IntPtr hExport = (IntPtr)0;
            var ioErr = Exporter.EXOpenExport(hDoc, outputFormat, DataAccess.IOType.AnsiPath, Output.StringToByteArray(outputFile), 0, 0, 0, 0, ref hExport);
            if (ioErr == IO.Err.OK)
                return hExport;

            ShowError("OpenExport failed:", (int)ioErr, outputFile, "OutputFile", true);
            // need to have something for the first page so we only throw exception when start page > 1
            if (startPage > 1)
                throw new IndexOutOfRangeException();
            return IntPtr.Zero;
        }

        private string OutputFile(string destinationPath, Output.Format outputFormat)
        {
            string rtn = System.IO.Path.Combine(destinationPath, System.IO.Path.GetFileNameWithoutExtension(_filePrefix));
            return string.Format("{0}.{1}", rtn, Enum.GetName(typeof(Output.Format), outputFormat));
        }

        private void RaiseError(string message, int errorNumber)
        {
            RaiseErrorEvent?.Invoke(message, errorNumber);
        }

        private void RaiseStatus(string status, int pageNumber)
        {
            RaiseStatusEvent?.Invoke(status, pageNumber);
        }

        private void Cleanup(IntPtr hDoc, IntPtr hExport, bool returnValue, string destinationFile)
        {
            if (!hDoc.Equals(IntPtr.Zero))
            {
                try
                {
                    if (!hExport.Equals(IntPtr.Zero))
                        EXCloseExport(hExport);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Do nothing
                }
                try
                {
                    DataAccess.CloseDocument(hDoc);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // Do nothing
                }
            }

            try
            {
                DataAccess.DeInit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Do nothing
            }

            if (returnValue)
                DeleteFile(destinationFile);
        }

        public void RunExport(string fileName, string destinationPath, bool displayNative)
        {
            RunExport(fileName, destinationPath, OutputFormat, displayNative);
        }

        public void RunExport(string fileName, string destinationPath, Output.Format outputFormat, bool displayNative)
        {
            RunExport(fileName, destinationPath, outputFormat, StartPage, EndPage, displayNative);
        }

        public void RunExport(string fileName, string destinationPath, int startPage, int endPage, bool displayNative)
        {
            RunExport(fileName, destinationPath, OutputFormat, startPage, endPage, displayNative);
        }

        public void RunExport(string fileName, string destinationPath, Output.Format outputFormat, int startPage, int endPage, bool displayNative)
        {
            if (string.IsNullOrEmpty(destinationPath))
                throw new NullReferenceException("Destination path is required");

            if (ExcludeExtension(fileName, displayNative))
            {
                //Logs.Loginformation(string.Format("File type excluded - File name: \"{0}\"", fileName));
                RaiseStatus("File type excluded", 1);
                return;
            }

            _count = 0;

            try
            {
                DataAccess.MultiThreadOn();
                var dataErr = DataAccess.Init();
            }
            catch (Exception ex)
            {
                RaiseStatus(ex.Message, 0);
                return;
            }

            SetOptions(startPage, endPage);
            var hExport = IntPtr.Zero;
            var hDoc = OpenDocument(fileName);

            if (!hDoc.Equals(IntPtr.Zero))
            {
                CreateDirectory(destinationPath);
                hExport = OpenExport(hDoc, outputFormat, OutputFile(destinationPath, outputFormat), 0);
                var ioErr = RunExport(hExport, string.Empty);

                if (_count == 1)
                {
                    RaiseStatus(string.Format("{0} page created.", _count), -1);
                }
                else if (_count > 1)
                {
                    RaiseStatus(string.Format("{0} pages created.", _count), -1);
                }
            }

            Cleanup(hDoc, hExport, false, string.Empty);
        }

        private IO.Err RunExport(IntPtr hExport, string destinationFile)
        {
            if (hExport.Equals(IntPtr.Zero))
                return IO.Err.OK;

            var ioErr = EXRunExport(hExport);
            if (ioErr == IO.Err.OK)
                return IO.Err.OK;

            if (ioErr == IO.Err.StartPageError || ioErr == IO.Err.EndPageError)
            {
                return (IO.Err)ShowError("RunExport; No more pages:", (int)ioErr, destinationFile, "DestinationFile", false);
            }

            return (IO.Err)ShowError("RunExport failed:", (int)ioErr, destinationFile, "DestinationFile", true);
        }

        private void SetOptions(int startPage, int endPage)
        {
            SetOptions(IntPtr.Zero, startPage, endPage, _dpi);
        }

        private void SetOptions(IntPtr hDoc, int startPage, int endPage, int dpi)
        {
            int argnewValue = Conversions.ToInteger(true);
            int argvalueSize = 4;
            DataAccess.SetOption(hDoc, Output.Options.UseDocumentPageSettings, ref argnewValue, ref argvalueSize);
            int argvalueSize1 = 4;
            DataAccess.SetOption(hDoc, Output.Options.AntiAliasing, ref _antialiasing, ref argvalueSize1);

            if (dpi != 0)
            {
                int argvalueSize2 = 4;
                DataAccess.SetOption(hDoc, Output.Options.GraphicDPI, ref dpi, ref argvalueSize2);
            }
            if (startPage > 0 | endPage > 0)
            {
                int argvalueSize3 = 4;
                DataAccess.SetOption(hDoc, Output.Options.StartPage, ref startPage, ref argvalueSize3);
                int argvalueSize4 = 4;
                DataAccess.SetOption(hDoc, Output.Options.EndPage, ref endPage, ref argvalueSize4);
                int argvalueSize5 = 0;
                var PagesRange = Output.Values.PagesRange;
                DataAccess.SetOption(hDoc, Output.Options.WhichPages, ref PagesRange, ref argvalueSize5);
            }
            else
            {
                int argvalueSize6 = 0;
                var PagesAll = Output.Values.PagesAll;
                DataAccess.SetOption(hDoc, Output.Options.WhichPages, ref PagesAll, ref argvalueSize6);
            }
        }

        private int ShowError(string str, int errCode, string problemFileName, bool isError)
        {
            return ShowError(str, errCode, problemFileName, string.Empty, isError);
        }

        private int ShowError(string str, int errCode, string problemFileName, string problemFileDescription, bool isError)
        {
            string logStr;

            if (errCode == -1)
            {
                logStr = string.Format("{0} (&H{1})", str, Conversion.Hex(errCode));
                RaiseError(str, 0);
            }
            else
            {
                string message = IO.GetErrorString(errCode);
                logStr = string.Format("{0} {1} (&H{2})", str, message, Conversion.Hex(errCode));
                RaiseError(string.Format("{0} {1}", str, message), errCode);
            }

            if (!string.IsNullOrEmpty(problemFileName))
            {
                if (string.IsNullOrEmpty(problemFileDescription))
                    problemFileDescription = "FileName";
                logStr += string.Format(" {0}{1}: {2}", Constants.vbCrLf, problemFileDescription, problemFileName);
            }

           

            return errCode;
        }

        public Bitmap ToImage(string fileName, string destinationPath, Output.Format outputFormat, int startPage, int endPage, int dpi, bool returnValue, bool displayNative, bool thumbnail, bool pageThumbnail)
        {
            var byteArray = ToByteArray(fileName, destinationPath, outputFormat, startPage, endPage, dpi, returnValue, displayNative, thumbnail, pageThumbnail);
            if (byteArray is null)
                return null;

            var ic = new ImageConverter();
            if (!ic.IsValid(byteArray))
                throw new Exception(IO.ExceptionString.StreamIsInvalid);
            return (Bitmap)ic.ConvertFrom(byteArray);
        }

        public byte[] ToByteArray(string fileName, string destinationPath, Output.Format outputFormat, bool returnValue, bool displayNative, bool thumbnail, bool pageThumbnail)
        {
            return ToByteArray(fileName, destinationPath, outputFormat, StartPage, EndPage, returnValue, displayNative, thumbnail, pageThumbnail);
        }

        public byte[] ToByteArray(string fileName, string destinationPath, Output.Format outputFormat, int startPage, int endPage, bool returnValue, bool displayNative, bool thumbnail, bool pageThumbnail)
        {
            return ToByteArray(fileName, destinationPath, outputFormat, startPage, endPage, _dpi, returnValue, displayNative, thumbnail, pageThumbnail);
        }

        public byte[] ToByteArray(string fileName, string destinationPath, Output.Format outputFormat, int startPage, int endPage, int dpi, bool returnValue, bool displayNative, bool thumbnail, bool pageThumbnail)
        {
            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName))
                throw new System.IO.FileNotFoundException("File not found", fileName);

            if (ExcludeExtension(fileName, displayNative))
            {
               //Logs.Loginformation(string.Format("File type excluded - File name: \"{0}\"", fileName));
                RaiseStatus("File type excluded", 1);
                if (returnValue)
                    return Output.ImageToByteArray(Resources.NotAvailableLarge);
                return Output.StringToByteArray("File type excluded");
            }

            _count = 0;
            var dataErr = default(DataAccess.Err);
            byte[] byteArray = null;
            string destinationFile = string.Empty;

            try
            {
                DataAccess.MultiThreadOn();
                dataErr = DataAccess.Init();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} ({1})", ex.Message, dataErr));
            }

            var hExport = IntPtr.Zero;
            var hDoc = OpenDocument(fileName);

            if (!hDoc.Equals(IntPtr.Zero))
            {
                SetOptions(hDoc, startPage, endPage, dpi);

                if (string.IsNullOrEmpty(destinationPath))
                {
                    destinationFile = System.IO.Path.GetTempFileName();
                    DeleteFile(destinationFile);
                    destinationFile = destinationFile.Replace(".tmp", string.Format(".{0}.{1}", System.IO.Path.GetFileNameWithoutExtension(fileName), outputFormat.ToString()));
                }
                else if (thumbnail)
                {
                    string newFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    if (newFileName.StartsWith("."))
                        newFileName = string.Format("_{0}", newFileName.Substring(1));

                    if (pageThumbnail)
                    {
                        destinationFile = System.IO.Path.Combine(destinationPath, string.Format("p.{0}.{1}", newFileName, outputFormat.ToString()));
                    }
                    else
                    {
                        destinationFile = System.IO.Path.Combine(destinationPath, string.Format("a.{0}.{1}", newFileName, outputFormat.ToString()));
                    }
                }
                else if (startPage != endPage || startPage == endPage && startPage == 1)
                {
                    destinationFile = System.IO.Path.Combine(destinationPath, string.Format("{0}.{1}", System.IO.Path.GetFileNameWithoutExtension(fileName), outputFormat.ToString()));
                }
                else
                {
                    destinationFile = System.IO.Path.Combine(destinationPath, string.Format("{0}{1}.{2}", System.IO.Path.GetFileNameWithoutExtension(fileName), (startPage - 1).ToString("x4"), outputFormat.ToString()));
                }

                hExport = OpenExport(hDoc, outputFormat, destinationFile, startPage);

                if (hExport.Equals(IntPtr.Zero))
                {
                    if (returnValue)
                    {
                        Cleanup(hDoc, hExport, returnValue, destinationFile);
                        return Output.ImageToByteArray(Resources.NotAvailableLarge);
                    }

                    if (string.IsNullOrEmpty(Output.ImageToFile(destinationFile, Resources.NotAvailableLarge)))
                        RaiseStatusEvent?.Invoke(destinationFile, 1);
                }
                else
                {
                    var ioErr = RunExport(hExport, destinationFile);

                    if (ioErr == IO.Err.OK && returnValue)
                    {
                        byteArray = Output.FileToByteArray(destinationFile);
                    }
                    else if (ioErr == IO.Err.StartPageError || ioErr == IO.Err.EndPageError)
                    {
                        // Changed returnValue to True because Outside In 8.5.3+ seems to create a zero byte 
                        // file even though a page range exception has been returned.  RVW 11/29/2018
                        // http://10.138.197.20:8080/browse/FUS-5775
                        Cleanup(hDoc, hExport, true, destinationFile);
                        throw new IndexOutOfRangeException();
                    }
                    else if (ioErr != IO.Err.OK)
                    {
                        Cleanup(hDoc, hExport, returnValue, destinationFile);
                        throw new Exception(string.Format("RunExport failed: {1} ({2}) {0}destinationFile: {3}", Constants.vbCrLf, IO.GetErrorStringFromEnum(ioErr), Conversion.Hex((int)ioErr), destinationFile));
                    }
                }
            }

            Cleanup(hDoc, hExport, returnValue, destinationFile);
            return byteArray;
        }
    }
}