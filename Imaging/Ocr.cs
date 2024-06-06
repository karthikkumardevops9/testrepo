using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.ImageProcessing.Core;
using Leadtools.Ocr;

namespace MSRecordsEngine.Imaging
{


    public class Ocr
    {
        private IOcrLanguageManager _ocrLanguages;

        private bool _started = false;
        private bool _enableCorrection;
        private bool _enableSubSystem;
        private int _pageNumber;
        // Private _ocrLanguages As RasterOcrLanguage() = {_test.SupportsEnablingMultipleLanguages}
        private string _RDFFile = Path.GetTempFileName();
        private RasterCodecs _codecs;
        private List<RecognizedWord> _words;

        private IOcrEngine __rasterOcr;

        private IOcrEngine _rasterOcr
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return __rasterOcr;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                __rasterOcr = value;
            }
        }
        // Private WithEvents _rasterOcrZoneData As RasterOcrZoneData

        private bool _deskewEnabled = false;
        private bool _despeckleEnabled = false;
        private bool _holeRemoveEnabled = true;
        private bool _borderRemoveEnabled = true;
        private bool _lineRemoveHorzEnabled = true;
        private bool _lineRemoveVertEnabled = true;
        private bool _smoothCharactersEnabled = true;
        private bool _invertTextEnabled = true;

        private DeskewCommand _deskew;
        private DespeckleCommand _despeckle;
        private HolePunchRemoveCommand _holeRemove;
        private BorderRemoveCommand _borderRemove;
        private LineRemoveCommand _lineRemove;
        private SmoothCommand _smoothCharacters;
        private InvertedTextCommand _invertText;

        public event ProgressStatusEventHandler ProgressStatus;

        public delegate void ProgressStatusEventHandler(string Message, int PageNumber, int Percentage);

        public Ocr() : this(false, false)
        {
        }

        public Ocr(bool enableCorrection, bool enableSubSystem)
        {
            SetupCodec();

            _enableCorrection = enableCorrection;
            _enableSubSystem = enableSubSystem;
            // HeyReggie, we want to set it up in config file(thinking about) 
            var lng = new[] { "en", "fr" };
            try
            {
                // Dim value As String = ConfigurationManager.AppSettings.Item("OCRLanguage")
                // If String.IsNullOrEmpty(value) Then Return

                // If Not value.Contains(",") Then
                // _ocrLanguages = New RasterOcrLanguage() {DirectCast([Enum].Parse(GetType(RasterOcrLanguage), value.Trim, True), RasterOcrLanguage)}
                // Return
                // End If


                // Dim values As String() = Split(value, ",")
                // Dim languages As New List(Of RasterOcrLanguage)

                // For Each language As String In values
                // languages.Add(DirectCast([Enum].Parse(GetType(RasterOcrLanguage), language.Trim, True), RasterOcrLanguage))
                // Next
                _ocrLanguages.EnableLanguages(lng);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private class CompareWords : IComparer<RecognizedWord>
        {

            internal int Compare(RecognizedWord word1, RecognizedWord word2)
            {
                int pixelDelta = 2;

                try
                {
                    if (word1.WordTop <= word2.WordTop + pixelDelta & word1.WordTop >= word2.WordTop - pixelDelta)
                    {
                        if (word1.WordLeft <= word2.WordLeft + pixelDelta & word1.WordLeft >= word2.WordLeft - pixelDelta)
                        {
                            return 0;
                        }
                        return word1.WordLeft - word2.WordLeft;
                    }
                    return word1.WordTop - word2.WordTop;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return 0;
                }
            }

            int IComparer<RecognizedWord>.Compare(RecognizedWord word1, RecognizedWord word2) => Compare(word1, word2);
        }

        private void SetupCodec()
        {

            RasterSupport.SetLicense(@"D:\\Codes\\FS11.1\\TabFusionRMS.WebCS\\LEADTOOLS\\LICENSE.lic", "IxKjEexxS");
            _codecs = new RasterCodecs();
            _rasterOcr = OcrEngineManager.CreateEngine(OcrEngineType.LEAD);
            // _rasterOcrZoneData = New RasterOcrZoneData
        }

        public bool DeskewEnabled
        {
            get
            {
                return _deskewEnabled;
            }
            set
            {
                _deskewEnabled = value;
            }
        }

        public bool DespeckleEnabled
        {
            get
            {
                return _despeckleEnabled;
            }
            set
            {
                _despeckleEnabled = value;
            }
        }

        public bool HoleRemoveEnabled
        {
            get
            {
                return _holeRemoveEnabled;
            }
            set
            {
                _holeRemoveEnabled = value;
            }
        }

        public bool BorderRemoveEnabled
        {
            get
            {
                return _borderRemoveEnabled;
            }
            set
            {
                _borderRemoveEnabled = value;
            }
        }

        public bool LineRemoveHorzEnabled
        {
            get
            {
                return _lineRemoveHorzEnabled;
            }
            set
            {
                _lineRemoveHorzEnabled = value;
            }
        }

        public bool LineRemoveVertEnabled
        {
            get
            {
                return _lineRemoveVertEnabled;
            }
            set
            {
                _lineRemoveVertEnabled = value;
            }
        }

        public bool SmoothCharactersEnabled
        {
            get
            {
                return _smoothCharactersEnabled;
            }
            set
            {
                _smoothCharactersEnabled = value;
            }
        }

        public bool InvertTextEnabled
        {
            get
            {
                return _invertTextEnabled;
            }
            set
            {
                _invertTextEnabled = value;
            }
        }

        public List<RecognizedWord> RecognizedWords
        {
            get
            {
                return _words;
            }
        }

        private List<RecognizedWord> GetRecognizedWords(bool sorted)
        {
            return default;
            // Dim words As New List(Of RecognizedWord)
            // Dim word As RasterOcrRecognizedWords() = _rasterOcr.GetRecognizedWords(0)

            // For i As Integer = 0 To word.Length - 1
            // words.Add(New RecognizedWord(word(i).Word, word(i).WordArea, word(i).ZoneIndex))
            // Next

            // If sorted Then words.Sort(New CompareWords)
            // Return words
        }

        private void CleanupImage(RasterImage img, bool returnError)
        {
            try
            {
                // Clean image before adding to OCR object
                if (_deskewEnabled)
                    _deskew.Run(img);
                if (_despeckleEnabled)
                    _despeckle.Run(img);
                if (img.BitsPerPixel > 1)
                    return;

                if (_holeRemoveEnabled)
                    _holeRemove.Run(img);
                if (_invertTextEnabled)
                    _invertText.Run(img);
                if (_borderRemoveEnabled)
                    _borderRemove.Run(img);
                if (_smoothCharactersEnabled)
                    _smoothCharacters.Run(img);

                if (_lineRemoveVertEnabled)
                {
                    _lineRemove.Type = LineRemoveCommandType.Vertical;
                    _lineRemove.Run(img);
                }
                if (_lineRemoveHorzEnabled)
                {
                    _lineRemove.Type = LineRemoveCommandType.Horizontal;
                    _lineRemove.Run(img);
                }
            }
            catch (Exception ex)
            {
                if (returnError)
                    throw;
                //Logs.LoginError(string.Format("Error \"{0}\" in Ocr.CleanupImage", ex.Message));
            }
        }

        private void CleanupImage2(ref RasterImage img, bool returnError)
        {
            // Try
            // Dim grayScale As New GrayscaleCommand()
            // grayScale.BitsPerPixel = 8
            // grayScale.Run(img)
            // 'Run the Intensity Detect command 
            // Dim intensity As New Color.IntensityDetectCommand()
            // intensity.LowThreshold = 180
            // intensity.HighThreshold = 255
            // intensity.Channel = Color.IntensityDetectCommandFlags.Master
            // intensity.Run(img)
            // 'Commented out lines below because it can cause errors on 64bit machines.  Also redundant; original intent was to increase size of image
            // ' for better recognition but cause invalid size errors so switched to same size as original, thus redundant.  RVW 10/13/2014
            // 'Dim resize As New ResizeCommand()
            // 'resize.Flags = RasterSizeFlags.Normal
            // 'resize.DestinationImage = New RasterImage(RasterMemoryFlags.Managed, img.Width, img.Height, img.BitsPerPixel, img.Order, img.ViewPerspective, img.Palette, Nothing)
            // 'resize.Run(img)
            // 'img = resize.DestinationImage
            // Catch ex As Exception
            // If returnError Then Throw
            // slimShared.logWarning(String.Format("Error ""{0}"" in Ocr.CleanupImage2", ex.Message))
            // End Try
        }

        private void InitializeObjects()
        {
            InitializeDeskew();
            InitializeDespeckle();
            InitializeHoleRemove();
            InitializeBorderRemove();
            InitializeBorderRemove();
            InitializeLineRemove();
            InitializeSmoothCharacters();
            InitializeInvertText();
        }

        private void InitializeDeskew()
        {
            // _deskew = New DeskewCommand()
            // _deskew.FillColor = New Leadtools.RasterColor(Drawing.Color.White)
            // _deskew.Flags = DeskewCommandFlags.DeskewImage Or DeskewCommandFlags.DocumentAndPictures Or DeskewCommandFlags.RotateBicubic
        }

        private void InitializeDespeckle()
        {
            _despeckle = new DespeckleCommand();
        }

        private void InitializeHoleRemove()
        {
            _holeRemove = new HolePunchRemoveCommand();
            _holeRemove.Flags = HolePunchRemoveCommandFlags.UseCount | HolePunchRemoveCommandFlags.UseLocation | HolePunchRemoveCommandFlags.UseDpi;
            _holeRemove.Location = HolePunchRemoveCommandLocation.Left;
            _holeRemove.MaximumHoleCount = 5;
            _holeRemove.MinimumHoleCount = 2;
        }

        private void InitializeBorderRemove()
        {
            _borderRemove = new BorderRemoveCommand();
            _borderRemove.Border = BorderRemoveBorderFlags.All;
            _borderRemove.Percent = 20;
            _borderRemove.WhiteNoiseLength = 9;
            _borderRemove.Variance = 3;
        }

        private void InitializeLineRemove()
        {
            _lineRemove = new LineRemoveCommand();
            _lineRemove.Flags = LineRemoveCommandFlags.UseGap | LineRemoveCommandFlags.UseVariance;
            _lineRemove.GapLength = 2;
            _lineRemove.MaximumLineWidth = 8;
            _lineRemove.MaximumWallPercent = 10;
            _lineRemove.MinimumLineLength = 200;
            _lineRemove.Variance = 2;
            _lineRemove.Wall = 14;
        }

        private void InitializeSmoothCharacters()
        {
            _smoothCharacters = new SmoothCommand();
            _smoothCharacters.Flags = SmoothCommandFlags.None;
            _smoothCharacters.Length = 1;
        }

        private void InitializeInvertText()
        {
            _invertText = new InvertedTextCommand();
            _invertText.Flags = InvertedTextCommandFlags.UseDpi;
            _invertText.MinimumInvertWidth = 6000;
            _invertText.MinimumInvertHeight = 186;
            _invertText.MaximumBlackPercent = 95;
            _invertText.MinimumBlackPercent = 75;
        }

        public bool Recognize(object Image, bool ShowGridLines, bool Sorted)
        {
            return Recognize(new MemoryStream(((byte[])Image).ToArray()), ShowGridLines, Sorted);
        }

        public bool Recognize(string fileName, bool ShowZoneLines, bool Sorted)
        {
            RasterImage img;

            try
            {
                img = _codecs.Load(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Ocr.Recognize(FileName) (fileName: {1})", ex.Message, fileName));
                throw;
            }

            return Recognize(img, ShowZoneLines, Sorted);
        }

        public bool Recognize(MemoryStream Stream, bool ShowZoneLines, bool Sorted)
        {
            RasterImage img;

            try
            {
                img = _codecs.Load(Stream);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Ocr.Recognize(Stream)", ex.Message));
                throw;
            }

            return Recognize(img, ShowZoneLines, Sorted);
        }

        public bool Recognize(RasterImage Image, bool ShowZoneLines, bool Sorted)
        {
            bool hadError = false;
            // Start up the Ocr engine
            // Try
            // If Not _started Then _rasterOcr.StartUp()
            // _started = True
            // Catch ex As Exception
            // slimShared.logWarning(String.Format("Error ""{0}"" during Startup in Ocr.Recognize(RasterImage)", ex.Message))
            // End Try

            if (!_started)
                return false;

            try
            {
                InitializeObjects();
                CleanupImage(Image, false);
                CleanupImage2(ref Image, false);
            }
            catch (Exception ex)
            {
               throw new Exception(string.Format("Error \"{0}\" during Cleanup in Ocr.Recognize(RasterImage)", ex.Message));
            }
            finally
            {
                if (_started & hadError)
                {
                    _rasterOcr.Shutdown();
                    _started = false;
                    if (File.Exists(_RDFFile))
                        File.Delete(_RDFFile);
                }
            }

            if (hadError)
                return false;

            // Try
            // SetOptions(False)
            // _rasterOcr.EnableEvents()

            // _rasterOcr.AddPage(Image, -1)
            // 'Find all the zones of the first page of RasterOcr object
            // _rasterOcr.EnableZoneForceSingleColumn = False
            // _rasterOcr.ShowZoneGridLines = ShowZoneLines
            // _rasterOcr.FindZones(0)

            // _rasterOcr.AutoOrientPage(0)
            // _rasterOcr.Recognize(0, 1)
            // _words = GetRecognizedWords(Sorted)
            // Catch ex As Exception
            // slimShared.logWarning(String.Format("Error ""{0}"" in Ocr.Recognize(RasterImage)", ex.Message))
            // Return False
            // Finally
            // _rasterOcr.ShutDown()
            // _started = False
            // If (System.IO.File.Exists(_RDFFile)) Then System.IO.File.Delete(_RDFFile)
            // End Try

            return true;
        }

        public string Recognize(byte[] ByteArray)
        {
            bool hadError = false;
            var image = default(RasterImage);
            // Start up the Ocr engine
            // Try
            // image = _codecs.Load(New IO.MemoryStream(ByteArray))
            // If Not _started Then _rasterOcr.Startup(_codecs, Nothing, "", "")
            // _started = True
            // Catch ex As Exception
            // slimShared.logWarning(String.Format("Error ""{0}"" during Startup in Ocr.Recognize(ByteArray)", ex.Message))
            // Throw
            // End Try

            try
            {
                InitializeObjects();
                CleanupImage(image, true);
                CleanupImage2(ref image, true);
            }
            catch (Exception ex)
            {
               throw new Exception(string.Format("Error \"{0}\" during Cleanup in Ocr.Recognize(ByteArray)", ex.Message));
            }
            finally
            {
                if (_started & hadError)
                {
                    _rasterOcr.Shutdown();
                    _started = false;
                    if (File.Exists(_RDFFile))
                        File.Delete(_RDFFile);
                }
            }

            return default;

            // Try
            // SetOptions(True)
            // _rasterOcr.EnableEvents()
            // _rasterOcr.AddPage(image, -1)
            // 'Find all the zones of the first page of RasterOcr object
            // _rasterOcr.EnableZoneForceSingleColumn = False
            // _rasterOcr.ShowZoneGridLines = False
            // _rasterOcr.FindZones(0)
            // _rasterOcr.AutoOrientPage(0)
            // _rasterOcr.Recognize(0, 1)
            // Return _rasterOcr.SaveResultsToMemory()
            // Catch ex As Exception
            // slimShared.logWarning(String.Format("Error ""{0}"" in Ocr.Recognize(ByteArray)", ex.Message))
            // Throw
            // Finally
            // _rasterOcr.ShutDown()
            // _started = False
            // If (System.IO.File.Exists(_RDFFile)) Then System.IO.File.Delete(_RDFFile)
            // End Try
        }

        private void SetOptions(bool returnError)
        {
            // Try
            // If (File.Exists(_RDFFile)) Then File.Delete(_RDFFile)
            // _rasterOcr.RecognitionDataFileName = _RDFFile
            // Catch ex As Exception
            // If returnError Then Throw
            // slimShared.logWarning(String.Format("Error ""{0}"" in Ocr.SetOptions", ex.Message))
            // End Try

            // Try
            // _rasterOcr.EnableCorrection = _enableCorrection
            // _rasterOcr.EnableSubSystem = _enableSubSystem
            // Catch ex As Exception
            // If returnError Then Throw
            // slimShared.logWarning(String.Format("Error ""{0}"" in Ocr.SetOptions", ex.Message))
            // End Try

            // _rasterOcr.SelectLanguages(_ocrLanguages)
            // _rasterOcr.SpellLanguageID = _ocrLanguages(0)
        }

        // Private Sub _rasterOcr_Progress(ByVal sender As Object, ByVal e As RasterOcrProgressEventArgs) Handles _rasterOcr.Progress
        // Dim message As String = String.Empty

        // Try
        // If (e.Percent = 100) Then
        // slimShared.logInformation("Ocr Complete")
        // RaiseEvent ProgressStatus("Ocr complete", -1, e.Percent)
        // Return
        // End If

        // Select Case e.Id
        // Case RasterOcrProcessId.FindZones
        // message = "Finding zones"
        // Case RasterOcrProcessId.LoadingImage
        // message = "Loading"
        // Case RasterOcrProcessId.ProcessingImage
        // message = "Preprocessing"
        // Case RasterOcrProcessId.Recognition
        // message = "Second recognition pass"
        // Case RasterOcrProcessId.Recognition3
        // message = "Third recognition pass"
        // Case RasterOcrProcessId.RecognizeMor
        // message = "Multi-lingual recognition pass"
        // Case RasterOcrProcessId.SaveRecognizeResult
        // message = "Reading recognition data"
        // Case RasterOcrProcessId.Spelling
        // message = "Spell checking"
        // Case RasterOcrProcessId.SavingImage, RasterOcrProcessId.WriteOutputDocument, RasterOcrProcessId.WriteOutputImage
        // message = "Saving text"
        // End Select

        // Dim iProgress As Integer = Math.Max(0, Math.Min(e.Percent, 100))
        // slimShared.logInformation(String.Format("{0} on {1}", message, _pageNumber + 1))
        // RaiseEvent ProgressStatus(message, _pageNumber + 1, iProgress)
        // Catch ex As Exception
        // slimShared.logInformation(String.Format("Error ""{0}"" on {1} in _rasterOcr.Progress", ex.Message, _pageNumber + 1))
        // RaiseEvent ProgressStatus(ex.Message, _pageNumber + 1, -1)
        // End Try
        // End Sub

        // Private Sub _rasterOcr_RecognizeStatus(ByVal sender As Object, ByVal e As RasterOcrRecognizeStatusEventArgs) Handles _rasterOcr.RecognizeStatus
        // '
        // End Sub

        // Private Sub _rasterOcrZoneData_Verification(ByVal sender As Object, ByVal e As RasterOcrVerificationEventArgs) Handles _rasterOcrZoneData.Verification
        // _rasterOcrZoneData.VerifyCode = RasterOcrVerificationCode.Accept
        // _rasterOcrZoneData.Flags = RasterOcrZoneFlags.DontUseVerificationEvent
        // End Sub
    }
}