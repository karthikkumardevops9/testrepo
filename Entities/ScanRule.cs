using System;
using System.Collections.Generic;

namespace MSRecordsEngine.Entities
{
    public partial class ScanRule
    {
        public int ScanRulesId { get; set; }
        public string Id { get; set; }
        public string DataBaseList { get; set; }
        private Nullable<int> _DirectoriesId;
        public Nullable<int> DirectoriesId
        {
            get
            {
                if (_DirectoriesId == null)
                    return 0;
                else
                    return _DirectoriesId;
            }
            set { _DirectoriesId = value; }
        }
        public string InQueDiskPattern { get; set; }
        public string OutputSettingsId { get; set; }
        public string FileNamePrefix { get; set; }
        public string FileExtension { get; set; }
        private Nullable<int> _NextDocNum;
        public Nullable<int> NextDocNum
        {
            get
            {
                if (_NextDocNum == null)
                    return 0;
                else
                    return _NextDocNum;
            }
            set { _NextDocNum = value; }
        }
        private Nullable<short> _ActiveDevice;
        public Nullable<short> ActiveDevice
        {
            get
            {
                return _ActiveDevice;
            }
            set { _ActiveDevice = value; }
        }
        private Nullable<short> _AsyncInterval;
        public Nullable<short> AsyncInterval
        {
            get
            {
                return _AsyncInterval;
            }
            set { _AsyncInterval = value; }
        }
        private Nullable<short> _DeviceCache;
        public Nullable<short> DeviceCache
        {
            get
            {
                return _DeviceCache;
            }
            set { _DeviceCache = value; }
        }
        private Nullable<int> _DeviceDeleteBack;
        public Nullable<int> DeviceDeleteBack
        {
            get
            {
                if (_DeviceDeleteBack == null)
                    return 0;
                else
                    return _DeviceDeleteBack;
            }
            set { _DeviceDeleteBack = value; }
        }
        private Nullable<bool> _DeviceDeleteBackChecked;
        public Nullable<bool> DeviceDeleteBackChecked
        {
            get
            {
                if (_DeviceDeleteBackChecked == null)
                    return false;
                else
                    return _DeviceDeleteBackChecked;
            }
            set { _DeviceDeleteBackChecked = value; }
        }
        private Nullable<int> _DeviceDeleteFront;
        public Nullable<int> DeviceDeleteFront
        {
            get
            {
                if (_DeviceDeleteFront == null)
                    return 0;
                else
                    return _DeviceDeleteFront;
            }
            set { _DeviceDeleteFront = value; }
        }
        private Nullable<bool> _DeviceDeleteFrontChecked;
        public Nullable<bool> DeviceDeleteFrontChecked
        {
            get
            {
                if (_DeviceDeleteFrontChecked == null)
                    return false;
                else
                    return _DeviceDeleteFrontChecked;
            }
            set { _DeviceDeleteFrontChecked = value; }
        }
        private Nullable<short> _DeviceTimeout;
        public Nullable<short> DeviceTimeout
        {
            get
            {
                return _DeviceTimeout;
            }
            set { _DeviceTimeout = value; }
        }
        private Nullable<bool> _Display;
        public Nullable<bool> Display
        {
            get
            {
                if (_Display == null)
                    return false;
                else
                    return _Display;
            }
            set { _Display = value; }
        }
        private Nullable<short> _IOCompression;
        public Nullable<short> IOCompression
        {
            get
            {
                return _IOCompression;
            }
            set { _IOCompression = value; }
        }
        public string IOStgFlt { get; set; }
        private Nullable<bool> _ScanContinuous;
        public Nullable<bool> ScanContinuous
        {
            get
            {
                if (_ScanContinuous == null)
                    return false;
                else
                    return _ScanContinuous;
            }
            set { _ScanContinuous = value; }
        }
        private Nullable<short> _ScanContrast;
        public Nullable<short> ScanContrast
        {
            get
            {
                return _ScanContrast;
            }
            set { _ScanContrast = value; }
        }
        public string ScanCustomCmd { get; set; }
        private Nullable<short> _ScanDensity;
        public Nullable<short> ScanDensity
        {
            get
            {
                return _ScanDensity;
            }
            set { _ScanDensity = value; }
        }
        private Nullable<short> _ScanDestination;
        public Nullable<short> ScanDestination
        {
            get
            {
                return _ScanDestination;
            }
            set { _ScanDestination = value; }
        }
        private Nullable<short> _ScanDirection;
        public Nullable<short> ScanDirection
        {
            get
            {
                return _ScanDirection;
            }
            set { _ScanDirection = value; }
        }
        private Nullable<short> _ScanDPI;
        public Nullable<short> ScanDPI
        {
            get
            {
                return _ScanDPI;
            }
            set { _ScanDPI = value; }
        }
        private Nullable<bool> _ScanDuplex;
        public Nullable<bool> ScanDuplex
        {
            get
            {
                if (_ScanDuplex == null)
                    return false;
                else
                    return _ScanDuplex;
            }
            set { _ScanDuplex = value; }
        }
        private Nullable<bool> _ScanManualStart;
        public Nullable<bool> ScanManualStart
        {
            get
            {
                if (_ScanManualStart == null)
                    return false;
                else
                    return _ScanManualStart;
            }
            set { _ScanManualStart = value; }
        }
        private Nullable<short> _ScanMode;
        public Nullable<short> ScanMode
        {
            get
            {
                return _ScanMode;
            }
            set { _ScanMode = value; }
        }
        private Nullable<bool> _ScanPad;
        public Nullable<bool> ScanPad
        {
            get
            {
                if (_ScanPad == null)
                    return false;
                else
                    return _ScanPad;
            }
            set { _ScanPad = value; }
        }
        private Nullable<short> _ScanSource;
        public Nullable<short> ScanSource
        {
            get
            {
                return _ScanSource;
            }
            set { _ScanSource = value; }
        }
        private Nullable<short> _Unit;
        public Nullable<short> Unit
        {
            get
            {
                return _Unit;
            }
            set { _Unit = value; }
        }
        private Nullable<bool> _BarCheckSum;
        public Nullable<bool> BarCheckSum
        {
            get
            {
                if (_BarCheckSum == null)
                    return false;
                else
                    return _BarCheckSum;
            }
            set { _BarCheckSum = value; }
        }
        private Nullable<bool> _BarCode;
        public Nullable<bool> BarCode
        {
            get
            {
                if (_BarCode == null)
                    return false;
                else
                    return _BarCode;
            }
            set { _BarCode = value; }
        }
        private Nullable<short> _BarDensity;
        public Nullable<short> BarDensity
        {
            get
            {
                return _BarDensity;
            }
            set { _BarDensity = value; }
        }
        private Nullable<double> _BarHeight;
        public Nullable<double> BarHeight
        {
            get
            {
                return _BarHeight;
            }
            set { _BarHeight = value; }
        }
        private Nullable<short> _BarHorzMax;
        public Nullable<short> BarHorzMax
        {
            get
            {
                return _BarHorzMax;
            }
            set { _BarHorzMax = value; }
        }
        private Nullable<bool> _BarLearn;
        public Nullable<bool> BarLearn
        {
            get
            {
                if (_BarLearn == null)
                    return false;
                else
                    return _BarLearn;
            }
            set { _BarLearn = value; }
        }
        private Nullable<short> _BarLength;
        public Nullable<short> BarLength
        {
            get
            {
                return _BarLength;
            }
            set { _BarLength = value; }
        }
        private Nullable<short> _BarMax;
        public Nullable<short> BarMax
        {
            get
            {
                return _BarMax;
            }
            set { _BarMax = value; }
        }
        private Nullable<short> _BarOrientation;
        public Nullable<short> BarOrientation
        {
            get
            {
                return _BarOrientation;
            }
            set { _BarOrientation = value; }
        }
        private Nullable<short> _BarQuality;
        public Nullable<short> BarQuality
        {
            get
            {
                return _BarQuality;
            }
            set { _BarQuality = value; }
        }
        private Nullable<bool> _BarRatio;
        public Nullable<bool> BarRatio
        {
            get
            {
                if (_BarRatio == null)
                    return false;
                else
                    return _BarRatio;
            }
            set { _BarRatio = value; }
        }
        private Nullable<short> _BarReturnsPatch;
        public Nullable<short> BarReturnsPatch
        {
            get
            {
                return _BarReturnsPatch;
            }
            set { _BarReturnsPatch = value; }
        }
        private Nullable<short> _BarSkew;
        public Nullable<short> BarSkew
        {
            get
            {
                return _BarSkew;
            }
            set { _BarSkew = value; }
        }
        private Nullable<short> _BarType;
        public Nullable<short> BarType
        {
            get
            {
                return _BarType;
            }
            set { _BarType = value; }
        }
        private Nullable<double> _BarWidth;
        public Nullable<double> BarWidth
        {
            get
            {
                return _BarWidth;
            }
            set { _BarWidth = value; }
        }
        private Nullable<short> _FontBackGround;
        public Nullable<short> FontBackGround
        {
            get
            {
                return _FontBackGround;
            }
            set { _FontBackGround = value; }
        }
        private Nullable<short> _FontDPI;
        public Nullable<short> FontDPI
        {
            get
            {
                return _FontDPI;
            }
            set { _FontDPI = value; }
        }
        private Nullable<short> _FontName;
        public Nullable<short> FontName
        {
            get
            {
                return _FontName;
            }
            set { _FontName = value; }
        }
        private Nullable<short> _FontOrientation;
        public Nullable<short> FontOrientation
        {
            get
            {
                return _FontOrientation;
            }
            set { _FontOrientation = value; }
        }
        private Nullable<short> _FontSize;
        public Nullable<short> FontSize
        {
            get
            {
                return _FontSize;
            }
            set { _FontSize = value; }
        }
        private Nullable<bool> _PatchCode;
        public Nullable<bool> PatchCode
        {
            get
            {
                if (_PatchCode == null)
                    return false;
                else
                    return _PatchCode;
            }
            set { _PatchCode = value; }
        }
        private Nullable<double> _PatchLeft;
        public Nullable<double> PatchLeft
        {
            get
            {
                return _PatchLeft;
            }
            set { _PatchLeft = value; }
        }
        private Nullable<bool> _PSAnnotate;
        public Nullable<bool> PSAnnotate
        {
            get
            {
                if (_PSAnnotate == null)
                    return false;
                else
                    return _PSAnnotate;
            }
            set { _PSAnnotate = value; }
        }
        private Nullable<bool> _PSEndorse;
        public Nullable<bool> PSEndorse
        {
            get
            {
                if (_PSEndorse == null)
                    return false;
                else
                    return _PSEndorse;
            }
            set { _PSEndorse = value; }
        }
        private Nullable<short> _PSAnnotateLeft;
        public Nullable<short> PSAnnotateLeft
        {
            get
            {
                return _PSAnnotateLeft;
            }
            set { _PSAnnotateLeft = value; }
        }
        public string PSAnnotateText { get; set; }
        private Nullable<short> _PSAnnotateTop;
        public Nullable<short> PSAnnotateTop
        {
            get
            {
                return _PSAnnotateTop;
            }
            set { _PSAnnotateTop = value; }
        }
        private Nullable<short> _PSEndorseLeft;
        public Nullable<short> PSEndorseLeft
        {
            get
            {
                return _PSEndorseLeft;
            }
            set { _PSEndorseLeft = value; }
        }
        public string PSEndorseText { get; set; }
        private Nullable<short> _PSEndorseTop;
        public Nullable<short> PSEndorseTop
        {
            get
            {
                return _PSEndorseTop;
            }
            set { _PSEndorseTop = value; }
        }
        private Nullable<bool> _SkewCorrect;
        public Nullable<bool> SkewCorrect
        {
            get
            {
                if (_SkewCorrect == null)
                    return false;
                else
                    return _SkewCorrect;
            }
            set { _SkewCorrect = value; }
        }
        private Nullable<bool> _SkewDetect;
        public Nullable<bool> SkewDetect
        {
            get
            {
                if (_SkewDetect == null)
                    return false;
                else
                    return _SkewDetect;
            }
            set { _SkewDetect = value; }
        }
        private Nullable<double> _SkewMaxAngle;
        public Nullable<double> SkewMaxAngle
        {
            get
            {
                return _SkewMaxAngle;
            }
            set { _SkewMaxAngle = value; }
        }
        private Nullable<double> _SkewMinAngle;
        public Nullable<double> SkewMinAngle
        {
            get
            {
                return _SkewMinAngle;
            }
            set { _SkewMinAngle = value; }
        }
        private Nullable<short> _DocumentChangeRule;
        public Nullable<short> DocumentChangeRule
        {
            get
            {
                return _DocumentChangeRule;
            }
            set { _DocumentChangeRule = value; }
        }
        private Nullable<bool> _DupsNewVersion;
        public Nullable<bool> DupsNewVersion
        {
            get
            {
                if (_DupsNewVersion == null)
                    return false;
                else
                    return _DupsNewVersion;
            }
            set { _DupsNewVersion = value; }
        }
        private Nullable<bool> _DupsNewPage;
        public Nullable<bool> DupsNewPage
        {
            get
            {
                if (_DupsNewPage == null)
                    return false;
                else
                    return _DupsNewPage;
            }
            set { _DupsNewPage = value; }
        }
        private Nullable<int> _ViewGroup;
        public Nullable<int> ViewGroup
        {
            get
            {
                if (_ViewGroup == null)
                    return 0;
                else
                    return _ViewGroup;
            }
            set { _ViewGroup = value; }
        }
        private Nullable<bool> _AutoPrint;
        public Nullable<bool> AutoPrint
        {
            get
            {
                if (_AutoPrint == null)
                    return false;
                else
                    return _AutoPrint;
            }
            set { _AutoPrint = value; }
        }
        private Nullable<short> _BlackBorder;
        public Nullable<short> BlackBorder
        {
            get
            {
                return _BlackBorder;
            }
            set { _BlackBorder = value; }
        }
        private Nullable<bool> _BlackBorderCropImage;
        public Nullable<bool> BlackBorderCropImage
        {
            get
            {
                if (_BlackBorderCropImage == null)
                    return false;
                else
                    return _BlackBorderCropImage;
            }
            set { _BlackBorderCropImage = value; }
        }
        private Nullable<short> _BlackBorderWhiteNoiseGap;
        public Nullable<short> BlackBorderWhiteNoiseGap
        {
            get
            {
                return _BlackBorderWhiteNoiseGap;
            }
            set { _BlackBorderWhiteNoiseGap = value; }
        }
        private Nullable<bool> _DeleteSourceFiles;
        public Nullable<bool> DeleteSourceFiles
        {
            get
            {
                if (_DeleteSourceFiles == null)
                    return false;
                else
                    return _DeleteSourceFiles;
            }
            set { _DeleteSourceFiles = value; }
        }
        private Nullable<short> _DeshadeCorrect;
        public Nullable<short> DeshadeCorrect
        {
            get
            {
                return _DeshadeCorrect;
            }
            set { _DeshadeCorrect = value; }
        }
        private Nullable<short> _DeshadeDetect;
        public Nullable<short> DeshadeDetect
        {
            get
            {
                return _DeshadeDetect;
            }
            set { _DeshadeDetect = value; }
        }
        private Nullable<short> _DeshadeHorzSpeckleAdj;
        public Nullable<short> DeshadeHorzSpeckleAdj
        {
            get
            {
                return _DeshadeHorzSpeckleAdj;
            }
            set { _DeshadeHorzSpeckleAdj = value; }
        }
        private Nullable<float> _DeshadeHorzSpeckleMax;
        public Nullable<float> DeshadeHorzSpeckleMax
        {
            get
            {
                return _DeshadeHorzSpeckleMax;
            }
            set { _DeshadeHorzSpeckleMax = value; }
        }
        private Nullable<float> _DeshadeMinHeight;
        public Nullable<float> DeshadeMinHeight
        {
            get
            {
                return _DeshadeMinHeight;
            }
            set { _DeshadeMinHeight = value; }
        }
        private Nullable<float> _DeshadeMinWidth;
        public Nullable<float> DeshadeMinWidth
        {
            get
            {
                return _DeshadeMinWidth;
            }
            set { _DeshadeMinWidth = value; }
        }
        private Nullable<short> _DeshadeUnit;
        public Nullable<short> DeshadeUnit
        {
            get
            {
                return _DeshadeUnit;
            }
            set { _DeshadeUnit = value; }
        }
        private Nullable<short> _DeshadeVertSpeckleAdj;
        public Nullable<short> DeshadeVertSpeckleAdj
        {
            get
            {
                return _DeshadeVertSpeckleAdj;
            }
            set { _DeshadeVertSpeckleAdj = value; }
        }
        private Nullable<float> _DeshadeVertSpeckleMax;
        public Nullable<float> DeshadeVertSpeckleMax
        {
            get
            {
                return _DeshadeVertSpeckleMax;
            }
            set { _DeshadeVertSpeckleMax = value; }
        }
        private Nullable<short> _Despeckle;
        public Nullable<short> Despeckle
        {
            get
            {
                return _Despeckle;
            }
            set { _Despeckle = value; }
        }
        private Nullable<float> _DespeckleHeight;
        public Nullable<float> DespeckleHeight
        {
            get
            {
                return _DespeckleHeight;
            }
            set { _DespeckleHeight = value; }
        }
        private Nullable<short> _DespeckleUnit;
        public Nullable<short> DespeckleUnit
        {
            get
            {
                return _DespeckleUnit;
            }
            set { _DespeckleUnit = value; }
        }
        private Nullable<float> _DespeckleWidth;
        public Nullable<float> DespeckleWidth
        {
            get
            {
                return _DespeckleWidth;
            }
            set { _DespeckleWidth = value; }
        }
        public string DeviceAlias { get; set; }
        private Nullable<double> _DeviceBackPickingHeight;
        public Nullable<double> DeviceBackPickingHeight
        {
            get
            {
                return _DeviceBackPickingHeight;
            }
            set { _DeviceBackPickingHeight = value; }
        }
        private Nullable<double> _DeviceBackPickingLeft;
        public Nullable<double> DeviceBackPickingLeft
        {
            get
            {
                return _DeviceBackPickingLeft;
            }
            set { _DeviceBackPickingLeft = value; }
        }
        private Nullable<bool> _DeviceBackPickingPixels;
        public Nullable<bool> DeviceBackPickingPixels
        {
            get
            {
                if (_DeviceBackPickingPixels == null)
                    return false;
                else
                    return _DeviceBackPickingPixels;
            }
            set { _DeviceBackPickingPixels = value; }
        }
        private Nullable<bool> _DeviceBackPickingRect;
        public Nullable<bool> DeviceBackPickingRect
        {
            get
            {
                if (_DeviceBackPickingRect == null)
                    return false;
                else
                    return _DeviceBackPickingRect;
            }
            set { _DeviceBackPickingRect = value; }
        }
        private Nullable<double> _DeviceBackPickingTop;
        public Nullable<double> DeviceBackPickingTop
        {
            get
            {
                return _DeviceBackPickingTop;
            }
            set { _DeviceBackPickingTop = value; }
        }
        private Nullable<double> _DeviceBackPickingWidth;
        public Nullable<double> DeviceBackPickingWidth
        {
            get
            {
                return _DeviceBackPickingWidth;
            }
            set { _DeviceBackPickingWidth = value; }
        }
        private Nullable<short> _DeviceBackRotate;
        public Nullable<short> DeviceBackRotate
        {
            get
            {
                return _DeviceBackRotate;
            }
            set { _DeviceBackRotate = value; }
        }
        private Nullable<double> _DeviceFrontPickingHeight;
        public Nullable<double> DeviceFrontPickingHeight
        {
            get
            {
                return _DeviceFrontPickingHeight;
            }
            set { _DeviceFrontPickingHeight = value; }
        }
        private Nullable<double> _DeviceFrontPickingLeft;
        public Nullable<double> DeviceFrontPickingLeft
        {
            get
            {
                return _DeviceFrontPickingLeft;
            }
            set { _DeviceFrontPickingLeft = value; }
        }
        private Nullable<bool> _DeviceFrontPickingPixels;
        public Nullable<bool> DeviceFrontPickingPixels
        {
            get
            {
                if (_DeviceFrontPickingPixels == null)
                    return false;
                else
                    return _DeviceFrontPickingPixels;
            }
            set { _DeviceFrontPickingPixels = value; }
        }
        private Nullable<bool> _DeviceFrontPickingRect;
        public Nullable<bool> DeviceFrontPickingRect
        {
            get
            {
                if (_DeviceFrontPickingRect == null)
                    return false;
                else
                    return _DeviceFrontPickingRect;
            }
            set { _DeviceFrontPickingRect = value; }
        }
        private Nullable<double> _DeviceFrontPickingTop;
        public Nullable<double> DeviceFrontPickingTop
        {
            get
            {
                return _DeviceFrontPickingTop;
            }
            set { _DeviceFrontPickingTop = value; }
        }
        private Nullable<double> _DeviceFrontPickingWidth;
        public Nullable<double> DeviceFrontPickingWidth
        {
            get
            {
                return _DeviceFrontPickingWidth;
            }
            set { _DeviceFrontPickingWidth = value; }
        }
        private Nullable<short> _DeviceFrontRotate;
        public Nullable<short> DeviceFrontRotate
        {
            get
            {
                return _DeviceFrontRotate;
            }
            set { _DeviceFrontRotate = value; }
        }
        private Nullable<short> _EdgeEnhancement;
        public Nullable<short> EdgeEnhancement
        {
            get
            {
                return _EdgeEnhancement;
            }
            set { _EdgeEnhancement = value; }
        }
        private Nullable<int> _EdgeEnhancementAlgorithm;
        public Nullable<int> EdgeEnhancementAlgorithm
        {
            get
            {
                if (_EdgeEnhancementAlgorithm == null)
                    return 0;
                else
                    return _EdgeEnhancementAlgorithm;
            }
            set { _EdgeEnhancementAlgorithm = value; }
        }
        private Nullable<short> _HorzLineEdgeCleanFactor;
        public Nullable<short> HorzLineEdgeCleanFactor
        {
            get
            {
                return _HorzLineEdgeCleanFactor;
            }
            set { _HorzLineEdgeCleanFactor = value; }
        }
        private Nullable<float> _HorzLineMaxGap;
        public Nullable<float> HorzLineMaxGap
        {
            get
            {
                return _HorzLineMaxGap;
            }
            set { _HorzLineMaxGap = value; }
        }
        private Nullable<float> _HorzLineMaxHeight;
        public Nullable<float> HorzLineMaxHeight
        {
            get
            {
                return _HorzLineMaxHeight;
            }
            set { _HorzLineMaxHeight = value; }
        }
        private Nullable<float> _HorzLineMinLength;
        public Nullable<float> HorzLineMinLength
        {
            get
            {
                return _HorzLineMinLength;
            }
            set { _HorzLineMinLength = value; }
        }
        private Nullable<short> _HorzLineReconstruct;
        public Nullable<short> HorzLineReconstruct
        {
            get
            {
                return _HorzLineReconstruct;
            }
            set { _HorzLineReconstruct = value; }
        }
        private Nullable<float> _HorzLineReconstructionHeight;
        public Nullable<float> HorzLineReconstructionHeight
        {
            get
            {
                return _HorzLineReconstructionHeight;
            }
            set { _HorzLineReconstructionHeight = value; }
        }
        private Nullable<float> _HorzLineReconstructionWidth;
        public Nullable<float> HorzLineReconstructionWidth
        {
            get
            {
                return _HorzLineReconstructionWidth;
            }
            set { _HorzLineReconstructionWidth = value; }
        }
        private Nullable<short> _HorzLineRemoval;
        public Nullable<short> HorzLineRemoval
        {
            get
            {
                return _HorzLineRemoval;
            }
            set { _HorzLineRemoval = value; }
        }
        private Nullable<short> _HorzLineUnit;
        public Nullable<short> HorzLineUnit
        {
            get
            {
                return _HorzLineUnit;
            }
            set { _HorzLineUnit = value; }
        }
        private Nullable<short> _MultiPage;
        public Nullable<short> MultiPage
        {
            get
            {
                return _MultiPage;
            }
            set { _MultiPage = value; }
        }
        private Nullable<short> _PatchTrigger;
        public Nullable<short> PatchTrigger
        {
            get
            {
                return _PatchTrigger;
            }
            set { _PatchTrigger = value; }
        }
        private Nullable<short> _PatchTriggers;
        public Nullable<short> PatchTriggers
        {
            get
            {
                return _PatchTriggers;
            }
            set { _PatchTriggers = value; }
        }
        private Nullable<bool> _ReadSubDirectories;
        public Nullable<bool> ReadSubDirectories
        {
            get
            {
                if (_ReadSubDirectories == null)
                    return false;
                else
                    return _ReadSubDirectories;
            }
            set { _ReadSubDirectories = value; }
        }
        private Nullable<int> _ScanBackSize;
        public Nullable<int> ScanBackSize
        {
            get
            {
                if (_ScanBackSize == null)
                    return 0;
                else
                    return _ScanBackSize;
            }
            set { _ScanBackSize = value; }
        }
        private Nullable<int> _ScanColorMode;
        public Nullable<int> ScanColorMode
        {
            get
            {
                if (_ScanColorMode == null)
                    return 0;
                else
                    return _ScanColorMode;
            }
            set { _ScanColorMode = value; }
        }
        private Nullable<short> _ScanDither;
        public Nullable<short> ScanDither
        {
            get
            {
                return _ScanDither;
            }
            set { _ScanDither = value; }
        }
        private Nullable<int> _ScanFrontSize;
        public Nullable<int> ScanFrontSize
        {
            get
            {
                if (_ScanFrontSize == null)
                    return 0;
                else
                    return _ScanFrontSize;
            }
            set { _ScanFrontSize = value; }
        }
        private Nullable<short> _ScanStartTimeOut;
        public Nullable<short> ScanStartTimeOut
        {
            get
            {
                return _ScanStartTimeOut;
            }
            set { _ScanStartTimeOut = value; }
        }
        private Nullable<short> _SliceCols;
        public Nullable<short> SliceCols
        {
            get
            {
                return _SliceCols;
            }
            set { _SliceCols = value; }
        }
        private Nullable<int> _SliceHeight;
        public Nullable<int> SliceHeight
        {
            get
            {
                if (_SliceHeight == null)
                    return 0;
                else
                    return _SliceHeight;
            }
            set { _SliceHeight = value; }
        }
        private Nullable<bool> _SliceKeepOriginal;
        public Nullable<bool> SliceKeepOriginal
        {
            get
            {
                if (_SliceKeepOriginal == null)
                    return false;
                else
                    return _SliceKeepOriginal;
            }
            set { _SliceKeepOriginal = value; }
        }
        private Nullable<int> _SliceOffsetCol;
        public Nullable<int> SliceOffsetCol
        {
            get
            {
                if (_SliceOffsetCol == null)
                    return 0;
                else
                    return _SliceOffsetCol;
            }
            set { _SliceOffsetCol = value; }
        }
        private Nullable<int> _SliceOffsetRow;
        public Nullable<int> SliceOffsetRow
        {
            get
            {
                if (_SliceOffsetRow == null)
                    return 0;
                else
                    return _SliceOffsetRow;
            }
            set { _SliceOffsetRow = value; }
        }
        private Nullable<int> _SliceOffsetX;
        public Nullable<int> SliceOffsetX
        {
            get
            {
                if (_SliceOffsetX == null)
                    return 0;
                else
                    return _SliceOffsetX;
            }
            set { _SliceOffsetX = value; }
        }
        private Nullable<int> _SliceOffsetY;
        public Nullable<int> SliceOffsetY
        {
            get
            {
                if (_SliceOffsetY == null)
                    return 0;
                else
                    return _SliceOffsetY;
            }
            set { _SliceOffsetY = value; }
        }
        private Nullable<short> _SliceRows;
        public Nullable<short> SliceRows
        {
            get
            {
                return _SliceRows;
            }
            set { _SliceRows = value; }
        }
        private Nullable<short> _SliceSkipBarcodes;
        public Nullable<short> SliceSkipBarcodes
        {
            get
            {
                return _SliceSkipBarcodes;
            }
            set { _SliceSkipBarcodes = value; }
        }
        private Nullable<bool> _SliceUseBarCodes;
        public Nullable<bool> SliceUseBarCodes
        {
            get
            {
                if (_SliceUseBarCodes == null)
                    return false;
                else
                    return _SliceUseBarCodes;
            }
            set { _SliceUseBarCodes = value; }
        }
        private Nullable<int> _SliceWidth;
        public Nullable<int> SliceWidth
        {
            get
            {
                if (_SliceWidth == null)
                    return 0;
                else
                    return _SliceWidth;
            }
            set { _SliceWidth = value; }
        }
        private Nullable<bool> _SlicingOn;
        public Nullable<bool> SlicingOn
        {
            get
            {
                if (_SlicingOn == null)
                    return false;
                else
                    return _SlicingOn;
            }
            set { _SlicingOn = value; }
        }
        private Nullable<short> _StreakRemoval;
        public Nullable<short> StreakRemoval
        {
            get
            {
                return _StreakRemoval;
            }
            set { _StreakRemoval = value; }
        }
        private Nullable<short> _StreakWidth;
        public Nullable<short> StreakWidth
        {
            get
            {
                return _StreakWidth;
            }
            set { _StreakWidth = value; }
        }
        private Nullable<bool> _TestPage;
        public Nullable<bool> TestPage
        {
            get
            {
                if (_TestPage == null)
                    return false;
                else
                    return _TestPage;
            }
            set { _TestPage = value; }
        }
        private Nullable<short> _VertLineEdgeCleanFactor;
        public Nullable<short> VertLineEdgeCleanFactor
        {
            get
            {
                return _VertLineEdgeCleanFactor;
            }
            set { _VertLineEdgeCleanFactor = value; }
        }
        private Nullable<float> _VertLineMaxGap;
        public Nullable<float> VertLineMaxGap
        {
            get
            {
                return _VertLineMaxGap;
            }
            set { _VertLineMaxGap = value; }
        }
        private Nullable<float> _VertLineMaxWidth;
        public Nullable<float> VertLineMaxWidth
        {
            get
            {
                return _VertLineMaxWidth;
            }
            set { _VertLineMaxWidth = value; }
        }
        private Nullable<float> _VertLineMinHeight;
        public Nullable<float> VertLineMinHeight
        {
            get
            {
                return _VertLineMinHeight;
            }
            set { _VertLineMinHeight = value; }
        }
        private Nullable<short> _VertLineReconstruct;
        public Nullable<short> VertLineReconstruct
        {
            get
            {
                return _VertLineReconstruct;
            }
            set { _VertLineReconstruct = value; }
        }
        private Nullable<float> _VertLineReconstructionHeight;
        public Nullable<float> VertLineReconstructionHeight
        {
            get
            {
                return _VertLineReconstructionHeight;
            }
            set { _VertLineReconstructionHeight = value; }
        }
        private Nullable<float> _VertLineReconstructionWidth;
        public Nullable<float> VertLineReconstructionWidth
        {
            get
            {
                return _VertLineReconstructionWidth;
            }
            set { _VertLineReconstructionWidth = value; }
        }
        private Nullable<short> _VertLineRemoval;
        public Nullable<short> VertLineRemoval
        {
            get
            {
                return _VertLineRemoval;
            }
            set { _VertLineRemoval = value; }
        }
        private Nullable<short> _VertLineUnit;
        public Nullable<short> VertLineUnit
        {
            get
            {
                return _VertLineUnit;
            }
            set { _VertLineUnit = value; }
        }
        private Nullable<bool> _MultiPageWriteWhenComplete;
        public Nullable<bool> MultiPageWriteWhenComplete
        {
            get
            {
                if (_MultiPageWriteWhenComplete == null)
                    return false;
                else
                    return _MultiPageWriteWhenComplete;
            }
            set { _MultiPageWriteWhenComplete = value; }
        }
    }
}
