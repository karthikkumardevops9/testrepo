using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Leadtools;
using Leadtools.Codecs;

namespace MSRecordsEngine.Imaging
{
    public partial class FilePage : Page
    {

        private int _ocrDpiForDocuments = 150;

        public FilePage() : base()
        {
        }

        public FilePage(Output.Format format) : this()
        {
            OutputFormat = format;
        }

        public FilePage(Attachment attachment, int pageNumber, int OcrDpiForDocuments, Output.Format format) : this(format)
        {
            if (OcrDpiForDocuments > 0)
                _ocrDpiForDocuments = OcrDpiForDocuments;
            _displayNative = attachment.DisplayNative;
            GetPage(attachment, pageNumber);
        }

        internal override void GetPage(Attachment attachment, int pageNumber)
        {
            _pageNumber = pageNumber;
            if (attachment.AttachmentParts.Count > 0)
                LoadImage(attachment, pageNumber, Attachment.CachedPages);
        }

        private void LoadImage(Attachment attachment, int pageNumber, string cachedFolder)
        {
            string fileName = attachment.AttachmentParts[0].FullPath;
            if (string.IsNullOrEmpty(fileName))
                throw new Exception(Permissions.ExceptionString.PathIsEmpty);
            if (!File.Exists(fileName))
                throw new Exception(Permissions.ExceptionString.FileNotFound, new Exception(fileName));
            if (_image is not null)
                return;

            _image = attachment.GetCachedImage(fileName, pageNumber, cachedFolder, OutputFormat, Annotations, false);
            if (_image is not null)
                return;

            string destinationPath = Path.Combine(Path.GetDirectoryName(fileName), cachedFolder);

            try
            {
                byte[] byteArray = null;

                using (var export = new Exporter())
                {
                    byteArray = export.ToByteArray(fileName, destinationPath, OutputFormat, pageNumber + 1, pageNumber + 1, _ocrDpiForDocuments, false, _displayNative, false, false);
                }

                if (byteArray is not null)
                {
                    using (var ms = new MemoryStream(Output.ImageToByteArray(Output.NotAvailableImage())))
                    {
                        _image = new RasterCodecs().Load(ms);
                    }
                }

                else
                {
                    _image = attachment.GetCachedImage(fileName, pageNumber, cachedFolder, OutputFormat, Annotations, false);
                    if (pageNumber == 0)
                        attachment.SaveCachedFlyout(_image, fileName, pageNumber, OutputFormat, false, Annotations);
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
               //Logs.LoginError(string.Format("\"{0}\" in FilePage.LoadImage", ex.Message));
            }
        }
    }
}
