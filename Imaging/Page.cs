using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using Leadtools;
using Leadtools.Annotations;
using Leadtools.Codecs;
using Leadtools.Controls;



namespace MSRecordsEngine.Imaging
{
    [DataContract()]
    public abstract partial class Page : IDisposable
    {

        protected Page()
        {
            _pageNumber = -1;
        }

        internal abstract void GetPage(Attachment attachment, int pageNumber);

        public Bitmap get_CachedImage(Attachment attachment, string cachedFolder)
        {
            return default;
            // If Not String.IsNullOrEmpty(attachment.AttachmentParts(0).ImageTableName) Then
            // If _image Is Nothing Then Return Nothing
            // Dim format As Output.Format = Output.Format.Jpg
            // Using ms As New System.IO.MemoryStream
            // Dim rc As New Drawing.Rectangle(0, 0, attachment.FlyoutSize.Width, attachment.FlyoutSize.Height)

            // If _image.BitsPerPixel <= 2 Then format = Output.Format.Tif
            // If _image.Width < attachment.FlyoutSize.Width OrElse _image.Height < attachment.FlyoutSize.Height Then
            // rc.Width = _image.Width
            // rc.Height = _image.Height
            // End If

            // rc = RasterImageList.GetFixedAspectRatioImageRectangle(_image.Width, _image.Height, rc)

            // Dim command As New ImageProcessing.ResizeCommand
            // command.Flags = RasterSizeFlags.None
            // command.DestinationImage = New RasterImage(RasterMemoryFlags.Conventional, rc.Width, rc.Height, _image.BitsPerPixel, _image.Order, _image.ViewPerspective, _image.GetPalette, Nothing, Nothing)
            // command.Run(_image)

            // Dim codec As New RasterCodecs
            // codec.Save(command.DestinationImage, ms, attachment.TranslateToLeadToolsFormat(format, _image.BitsPerPixel), attachment.ConvertBitsPerPixel(format, _image.BitsPerPixel))
            // Return New Bitmap(ms)
            // End Using
            // Else
            // Dim img As RasterImage = attachment.GetCachedImage(attachment.AttachmentParts(0).FullPath, 0, cachedFolder, Output.Format.Jpg, Annotations, True)

            // If img Is Nothing Then
            // attachment.SaveCachedFlyout(_image, attachment.AttachmentParts(0).FullPath, 0, Output.Format.Jpg, TypeOf attachment Is ImageAttachment, Annotations)
            // img = attachment.GetCachedImage(attachment.AttachmentParts(0).FullPath, 0, cachedFolder, Output.Format.Jpg, Annotations, True)
            // End If

            // If img Is Nothing Then Return Nothing
            // Return DirectCast(img.bit, Bitmap)
            // End If
        }

        [DataMember()]
        public byte[] Image
        {
            get
            {
                try
                {
                    return RasterImageToByteArray(_image);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error \"{0}\" in Page.Image", ex.Message));
                }
            }
            set
            {
                _image = ByteArrayToRasterImage(value);
            }
        }
        protected RasterImage _image = default;

        [DataMember()]
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                _pageNumber = value;
            }
        }
        protected int _pageNumber;

        [DataMember()]
        public string Annotations
        {
            get
            {
                return _annotations;
            }
            set
            {
                _annotations = value;
            }
        }
        private string _annotations;

        [DataMember()]
        public bool HideRedactions
        {
            get
            {
                return _hideRedactions;
            }
            set
            {
                _hideRedactions = value;
            }
        }
        private bool _hideRedactions;

        [DataMember()]
        public bool ShowAnnotations
        {
            get
            {
                return _showAnnotations;
            }
            set
            {
                _showAnnotations = value;
            }
        }
        private bool _showAnnotations;

        [IgnoreDataMember()]
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
        private Output.Format _outputFormat = Output.Format.Jpg;

        public List<RecognizedWord> RecognizedWords
        {
            get
            {
                return _recognizedWords;
            }
        }
        [DataMember()]
        private List<RecognizedWord> _recognizedWords;

        [DataMember()]
        public int AnnotationsDrawMode
        {
            get
            {
                return _annotationsDrawMode;
            }
            set
            {
                _annotationsDrawMode = value;
            }
        }
        private int _annotationsDrawMode = -1;

        public bool DisplayNative
        {
            get
            {
                return _displayNative;
            }
        }
        [DataMember()]
        protected bool _displayNative = false;

        internal void FillRecognizedWords(System.Drawing.Bitmap bmp, int annotationDrawMode, bool Sorted)
        {
            using (var ms = new MemoryStream(Output.ImageToByteArray(bmp)))
            {
                _image = new RasterCodecs().Load(ms);
            }
            FillRecognizedWords(annotationDrawMode, Sorted);
        }

        internal void FillRecognizedWords(int annotationDrawMode, bool Sorted)
        {
            _annotationsDrawMode = annotationDrawMode;

            if (_image is null)
                return;
            ImageViewer imageViewer = default;

            try
            {
                if ((Attachments.AnnotationsDrawMode)_annotationsDrawMode != Attachments.AnnotationsDrawMode.None)
                {
                    imageViewer = new ImageViewer();
                    imageViewer.AutoDisposeImages = true;
                    imageViewer.Image = _image;
                    RealizeRedactions(imageViewer, Annotations, (Attachments.AnnotationsDrawMode)_annotationsDrawMode);
                }
            }
            catch (Exception ex)
            {
                imageViewer = default;
                throw new Exception(string.Format("Error \"{0}\" in Page.FillRecognizedWords", ex.Message));
            }

            Ocr ocr = default;

            try
            {
                ocr = new Ocr();
                if (imageViewer is not null)
                {
                    ocr.Recognize(imageViewer.Image, false, Sorted);
                }
                else
                {
                    ocr.Recognize(_image, false, Sorted);
                }

                _recognizedWords = ocr.RecognizedWords;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Page.FillRecognizedWords", ex.Message));
            }
            finally
            {
                ocr = default;
            }

            if (imageViewer is not null)
            {
                if (imageViewer.Image is not null)
                    imageViewer.Image.Dispose();
                imageViewer.Image.Dispose();
                imageViewer = default;
            }
        }

        internal static void RealizeRedactions(ImageViewer imageViewer, string annotations, Attachments.AnnotationsDrawMode annotationsDrawMode)
        {
            // If imageViewer Is Nothing Then Return
            // If annotationsDrawMode = Attachments.AnnotationsDrawMode.None Then Return
            // If String.IsNullOrEmpty(annotations) Then Return

            // Try
            // Dim automationManager As New AnnAutomationManager
            // automationManager.RasterCodecs = New Codecs.RasterCodecs
            // Dim annAuto As New AnnAutomation(automationManager, imageViewer)
            // Dim annotationCodec As New AnnCodecs

            // annAuto.Container.Transform = imageViewer.Transform
            // annotationCodec.Load(New System.IO.MemoryStream(Export.Output.StringToByteArray(annotations)), annAuto.Container, 1)

            // For Each ann As AnnObject In annAuto.Container.Objects
            // ann.Visible = (TypeOf ann Is AnnRedactionObject)
            // Next

            // annAuto.RealizeAllRedactions()
            // Catch ex As Exception
            // End Try
        }

        internal static RasterImage ByteArrayToRasterImage(byte[] byteArray)
        {
            if (byteArray is null || byteArray.Length == 0)
                return default;
            var codec = new RasterCodecs();
            return (RasterImage)codec.Load(new MemoryStream(byteArray));
        }

        internal static byte[] RasterImageToByteArray(RasterImage value)
        {
            if (value is null)
                return null;

            try
            {
                var codec = new RasterCodecs();

                using (var stream = new MemoryStream())
                {
                    if (value.OriginalFormat == RasterImageFormat.Unknown)
                    {
                        if (value.BitsPerPixel > 24)
                        {
                            codec.Save(value, stream, RasterImageFormat.Jpeg, 24);
                        }
                        else
                        {
                            codec.Save(value, stream, RasterImageFormat.Jpeg, value.BitsPerPixel);
                        }
                    }
                    else
                    {
                        codec.Save(value, stream, value.OriginalFormat, value.BitsPerPixel);
                    }

                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        #region  IDisposable Support 
        private bool disposedValue = false;        // To detect redundant calls
                                                   // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_image is not null)
                        _image.Dispose();
                    _image = null;
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
    }
}
