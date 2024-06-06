using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Leadtools;
using Leadtools.Codecs;
namespace MSRecordsEngine.Imaging
{
    public partial class ImagePage : Page
    {

        internal ImagePage() : base()
        {
        }

        public ImagePage(Attachment attachment, int pageNumber) : this()
        {
            GetPage(attachment, pageNumber);
        }

        internal override void GetPage(Attachment attachment, int pageNumber)
        {
            var offset = default(int);

            _pageNumber = pageNumber;

            foreach (AttachmentPart part in attachment.AttachmentParts)
            {
                if (part.PageCount + offset > pageNumber)
                {
                    LoadData(attachment, part, pageNumber - offset);
                    break;
                }

                offset += part.PageCount;
            }
        }

        private void LoadData(Attachment attachment, AttachmentPart part, int pageNumber)
        {
            if (string.IsNullOrEmpty(attachment.DatabaseName))
                throw new Exception(Permissions.ExceptionString.InvalidDatabase);

            try
            {
                using (SqlConnection conn = Attachments.GetConnection(attachment.DatabaseName))
                {
                    if (string.IsNullOrEmpty(Annotations))
                    {
                        int pointerId = part.GetPointerId(attachment.VersionInfo, _pageNumber + 1, part.FileName, conn);
                        Annotations = Attachments.GetAnnotation(Attachments.AttachmentTypes.tkImage, pageNumber + 1, pointerId, conn);
                    }

                    if (_image is not null)
                        return;

                    if (!string.IsNullOrEmpty(part.ImageTableName))
                    {
                        LoadDatabaseImage(attachment.DatabaseName, part, pageNumber, conn);
                        if (pageNumber == 0)
                            attachment.SaveCachedFlyout(_image, "", pageNumber, Output.Format.Jpg, true, Annotations);
                    }
                    else
                    {
                        LoadImage(part.FullPath, pageNumber);
                        if (pageNumber == 0)
                            attachment.SaveCachedFlyout(_image, part.FullPath, pageNumber, Output.Format.Jpg, true, Annotations);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(part.ImageTableName))
                {
                    throw new Exception(string.Format("Error \"{0}\" in ImagePage.LoadData (TableName: {1}, Id: {2} PageNumber: {3})", ex.Message, part.ImageTableName, part.FileName, pageNumber + 1));
                }
                else
                {
                    throw new Exception(string.Format("Error \"{0}\" in ImagePage.LoadData (FileName: {1}, PageNumber: {2})", ex.Message, part.FullPath, pageNumber + 1));
                }

                throw;
            }
        }

        private void LoadImage(string fileName, int pageNumber)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception(Permissions.ExceptionString.PathIsEmpty);
            if (!File.Exists(fileName))
                throw new Exception(Permissions.ExceptionString.FileNotFound, new Exception(fileName));

            try
            {
                var codec = new RasterCodecs();
                // _image = Page.RasterImageToByteArray(New RasterImage(Attachments.Codec.Load(fileName, 0, Codecs.CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1)))
                _image = new RasterImage(codec.Load(fileName, 0, Leadtools.Codecs.CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in ImagePage.LoadImage (FileName: {1}, PageNumber: {2})", ex.Message, fileName, pageNumber + 1));
            }
        }

        private void LoadDatabaseImage(string databaseName, AttachmentPart part, int pageNumber, SqlConnection conn)
        {
            byte[] bmp = Attachments.GetDatabaseImageStream(databaseName, part.ImageTableName, part.FileName, conn);
            var codec = new RasterCodecs();
            // _image = Page.RasterImageToByteArray(New RasterImage(Attachments.Codec.Load(New MemoryStream(bmp), 0, Codecs.CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1)))
            _image = new RasterImage(codec.Load(new MemoryStream(bmp), 0, CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1));
        }
    }
}
