using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace MSRecordsEngine.Imaging
{

    [CLSCompliant(true)]
    public class IO
    {
        internal class ExceptionString
        {
            internal static string StreamIsInvalid = "Stream is invalid";
        }

        public enum Err
        {
            Unknown = -0x1,
            InvalidSpec = -0x2,              // The file spec given (IOSPEC) cannot be used in this environment 
            AllocFail = -0x3,                // The IO system could not allocate memory 
            BadParam = -0x4,                 // One of the parameters contained bad or insufficient info 
            NoFile = -0x5,                   // File not found 
            NoCreate = -0x6,                 // File could not be created 
            BadInfoId = -0x7,                // dwInfoId parameter to IOGetInfo was invalid 
            SeekOutOfRange = -0x8,           // Seeking out of range in a range type file 
            EOF = -0x9,                      // End of file reached 
            False = -0xA,
            LockFail = -0xB,                 // Failed to lock allocated memory (Windows) 
            BadFileSize = -0xC,
            InvalidState = -0xD,
            ShortRead = -0xE,                // Failed to read the expected bytecount 
            ShortWrite = -0xF,               // Failed to write the expected bytecount 
            UnableToExpand = -0x10,          // Tried to expand an OLE2 file and failed
            BadFileName = -0x11,             // Full file name length exceeds the OS limits 
            OK = 0x0,
            ChunkerInitFailed = 0x6,         // The chunker (SCCCH.DLL) could not be initialized
            FileOpenFailed = 0x7,            // The file could not be opened
            BadFile = 0x9,                   // The file is corrupt
            EmptyFile = 0xA,                 // The file is empty
            ProtectedFile = 0xB,             // The file is password protected or encrypted
            OutOfMemory = 0x1E,              // Insufficient memory to perform the operation
            DataNotAvailable = 0x2D,         // The requested data is not yet available
            InsufficientBuffer = 0x2E,       // The provided buffer is not large enough to contain the result
            MemoryLeak = 0x30,               // A memory leak was detected
            DRMFile = 0x31,                  // The file is protected by DRM tools.
            SystemError = 0x80,              // Operating system generated error
            ProcCreate = 0x81,               // Process creation error
            ProcDestroy = 0x82,              // Process destruction error
            ProcTimeout = 0x83,              // Process exceeded its allotted run time.
            AssertFailed = 0x84,             // Assertion failure
            BadDataFormat = 0x88,            // Data sent was recognized, but in an unexpected format
            IndexOutofBounds = 0x89,         // Specified index is outside the legal range
            ProcessDestroy = 0x90,           // Error destroying a process
            BlockingCall = 0x91,             // Satisfying the call would require blocking the thread
            BadFileHandle = 0x140,           // File handle is invalid
            BadPath = 0x145,                 // The specified path does not exist
            BadSpec = 0x146,                 // The specified spec is not valid.
            BadInfoId2 = 0x147,              // dwInfoID parameter to IOGetInfo() was invalid
            BadURLReference = 0x150,         // The locator refers to an external URL that can not be accessed
            BadFileSize2 = 0x151,            // File size exceeds the maximum size that can be processed
            SeqAccessViolation = 0x240,      // An expired data point has been requested in a sequential access situation
            ChunkerFailure = 0x241,          // Out Of Memory
            AccessViolation = 0x3C1,         // Access violation                                   
            Breakpoint = 0x3C2,              // Breakpoint encountered                             
            DataMisalignment = 0x3C3,        // Data misalignment                                  
            SingleStep = 0x3C4,              // Trace trap single step indicated                   
            BoundsExceeded = 0x3C5,          // Out of bounds array element referenced             
            FloatAbnormal = 0x3C6,           // Floating point value is abnormal                   
            FloatDivideByZero = 0x3C7,       // Floating point divide by zero                      
            FloatInexactResult = 0x3C8,      // Result cannot be represented as decimal fraction   
            FloatInvalidOperation = 0x3C9,   // General floating point exception                   
            FloatOverflow = 0x3CA,           // Floating point exponent overflow                   
            FloatStackCheck = 0x3CB,         // Floating point stack underflow or overflow         
            FloatUnderFlow = 0x3CC,          // Floating point exponent underflow                  
            IntegerDivideByZero = 0x3CD,     // Integer divide by zero                             
            IntegerOverflow = 0x3CE,         // Integer overflow                                   
            PrivilegedInstruction = 0x3CF,   // Priveleged instruction                             
            NonContinuable = 0x3D0,          // A noncontinuable exception has occurred            
            BadResponse = 0x500,             // Received an unexpected response
            CommTimeout = 0x501,             // Communications timeout
            CommUnknown = 0x502,             // General communications error
            ConnectionRefused = 0x503,       // Connection refused
            CommFault = 0x504,               // Communications fault
            ConnectionDown = 0x505,          // Host or network is down
            ConnectionUnreachable = 0x506,   // Host or network is unreachable
            Disconnected = 0x507,            // Unexpected disconnection
            Synchronize = 0x508,             // Synchronization error
            StartPageError = 0x700,          // The start page value is larger than the number of pages in the document
            EndPageError = 0x701            // The start page value is larger than the end page value
        }

        public enum Seek
        {
            Top,
            Current,
            Bottom
        }

        public enum Info
        {
            OsHandle = 0x1,
            hSpec,
            FileName,
            IsOle2Storage,
            Ole2ClsId,
            PathName,                        // Human readable path name to the file (includes file name) 
            IsOle2RootStorage,
            IsOle2SubStorage,
            IsOle2SubStream,
            ParentHandle,
            FileSize,
            IsReadOnly,
            TimeDate,                        // # seconds since Jan.1, 1970 
            PathType                        // OITYPE of the path returned by PATHNAME,
        }

        public static string GetErrorStringFromEnum(Err errCode)
        {
            switch (errCode)
            {
                case Err.InvalidSpec:
                    {
                        return "The file spec given (IOSPEC) cannot be used in this environment";
                    }
                case Err.AllocFail:
                    {
                        return "The IO system could not allocate memory";
                    }
                case Err.BadParam:
                    {
                        return "One of the parameters contained bad or insufficient info";
                    }
                case Err.NoFile:
                    {
                        return "The file was not found";
                    }
                case Err.NoCreate:
                    {
                        return "The file could not be created";
                    }
                case Err.BadInfoId:
                case Err.BadInfoId2:
                    {
                        return "The dwInfoId parameter to IOGetInfo was invalid";
                    }
                case Err.SeekOutOfRange:
                    {
                        return "Seeking out of range in a range type file";
                    }
                case Err.EOF:
                    {
                        return "End of file reached";
                    }
                case Err.False:
                    {
                        return "Returned FALSE";
                    }
                case Err.LockFail:
                    {
                        return "Failed to lock allocated memory";
                    }
                case Err.BadFileSize:
                    {
                        return errCode.ToString();
                    }
                case Err.InvalidState:
                    {
                        return errCode.ToString();
                    }
                case Err.ShortRead:
                    {
                        return "Failed to read the expected byte count";
                    }
                case Err.ShortWrite:
                    {
                        return "Failed to write the expected byte count";
                    }
                case Err.UnableToExpand:
                    {
                        return "Tried to expand an OLE2 file and failed";
                    }
                case Err.BadFileName:
                    {
                        return "The full file name length exceed the exceeds OS limits";
                    }
                case Err.OK:
                    {
                        return errCode.ToString();
                    }
                case Err.ChunkerInitFailed:
                    {
                        return "The chunker (SCCCH.DLL) could not be initialized";
                    }
                case Err.FileOpenFailed:
                    {
                        return "The file could not be opened";
                    }
                case Err.BadFile:
                    {
                        return "The file is corrupt";
                    }
                case Err.EmptyFile:
                    {
                        return "The file is empty";
                    }
                case Err.ProtectedFile:
                    {
                        return "The file is password protected or encrypted";
                    }
                case Err.OutOfMemory:
                    {
                        return "Insufficient memory to perform the operation";
                    }
                case Err.DataNotAvailable:
                    {
                        return "The requested data is not yet available";
                    }
                case Err.InsufficientBuffer:
                    {
                        return "The provided buffer is not large enough to contain the result";
                    }
                case Err.MemoryLeak:
                    {
                        return "A memory leak was detected";
                    }
                case Err.DRMFile:
                    {
                        return "The file is protected by DRM tools";
                    }
                case Err.SystemError:
                    {
                        return "Operating system generated error";
                    }
                case Err.ProcCreate:
                    {
                        return "Process creation error";
                    }
                case Err.ProcDestroy:
                    {
                        return "Process destruction error";
                    }
                case Err.ProcTimeout:
                    {
                        return "Process exceeded its allotted run time";
                    }
                case Err.AssertFailed:
                    {
                        return "Assertion failure";
                    }
                case Err.BadDataFormat:
                    {
                        return "Data sent was recognized, but in an unexpected format";
                    }
                case Err.IndexOutofBounds:
                    {
                        return "Specified index is outside the legal range";
                    }
                case Err.ProcessDestroy:
                    {
                        return "Error destroying a process";
                    }
                case Err.BlockingCall:
                    {
                        return "Satisfying the call would require blocking the thread";
                    }
                case Err.BadFileHandle:
                    {
                        return "The file handle is invalid";
                    }
                case Err.BadPath:
                    {
                        return "The specified path does not exist";
                    }
                case Err.BadSpec:
                    {
                        return "The specified spec is not valid";
                    }
                case Err.BadURLReference:
                    {
                        return "The locator refers to an external URL that can not be accessed";
                    }
                case Err.BadFileSize2:
                    {
                        return "File size exceeds the maximum size that can be processed";
                    }
                case Err.SeqAccessViolation:
                    {
                        return "An expired data point has been requested in a sequential access situation";
                    }
                case Err.ChunkerFailure:
                    {
                        return "Out of memory";
                    }
                case Err.AccessViolation:
                    {
                        return "Access violation";
                    }
                case Err.Breakpoint:
                    {
                        return "Breakpoint encountered";
                    }
                case Err.DataMisalignment:
                    {
                        return "Data misalignment";
                    }
                case Err.SingleStep:
                    {
                        return "Trace trap single step indicated";
                    }
                case Err.BoundsExceeded:
                    {
                        return "Out of bounds array element referenced";
                    }
                case Err.FloatAbnormal:
                    {
                        return "Floating point value is abnormal";
                    }
                case Err.FloatDivideByZero:
                    {
                        return "Floating point divide by zero";
                    }
                case Err.FloatInexactResult:
                    {
                        return "Result cannot be represented as decimal fraction";
                    }
                case Err.FloatInvalidOperation:
                    {
                        return "General floating point exception";
                    }
                case Err.FloatOverflow:
                    {
                        return "Floating point exponent overflow";
                    }
                case Err.FloatStackCheck:
                    {
                        return "Floating point stack underflow or overflow";
                    }
                case Err.FloatUnderFlow:
                    {
                        return "Floating point exponent underflow";
                    }
                case Err.IntegerDivideByZero:
                    {
                        return "Integer divide by zero";
                    }
                case Err.IntegerOverflow:
                    {
                        return "Integer overflow";
                    }
                case Err.PrivilegedInstruction:
                    {
                        return "Privileged instruction";
                    }
                case Err.NonContinuable:
                    {
                        return "A non-continuable exception has occurred";
                    }
                case Err.BadResponse:
                    {
                        return "Received an unexpected response";
                    }
                case Err.CommTimeout:
                    {
                        return "Communications timeout";
                    }
                case Err.CommUnknown:
                    {
                        return "General communications error";
                    }
                case Err.ConnectionRefused:
                    {
                        return "Connection refused";
                    }
                case Err.CommFault:
                    {
                        return "Communications fault";
                    }
                case Err.ConnectionDown:
                    {
                        return "Host or network is down";
                    }
                case Err.ConnectionUnreachable:
                    {
                        return "Host or network is unreachable";
                    }
                case Err.Disconnected:
                    {
                        return "Unexpected disconnection";
                    }
                case Err.Synchronize:
                    {
                        return "Synchronization error";
                    }
                case Err.StartPageError:
                    {
                        return "The start page value is larger than the number of pages in the document";
                    }
                case Err.EndPageError:
                    {
                        return "The end page value is less than the start page value";
                    }

                default:
                    {
                        return GetErrorString((int)errCode);
                    }
            }
        }

        public static string GetErrorString(int errCode)
        {
            string rtn = string.Empty;
            var buffer = new byte[1024];

            try
            {
                int count = 1;
                DataAccess.GetErrorString(errCode, buffer, 1024);
                for (int i = 0; i <= 1023; i++)
                {
                    if (buffer[i] == 0)
                    {
                        count = i;
                        break;
                    }
                }

                rtn = new System.Text.UTF8Encoding().GetString(buffer, 0, count);
                if (rtn.IndexOf(Constants.vbNullChar) > -1)
                    return rtn.Substring(0, rtn.IndexOf(Constants.vbNullChar));
                return rtn;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class CallBackData
    {
        // handle of the first output file that the new file is associated with 
        public IntPtr hParentFile;
        // FI ID indicating type of data parent file will contain 
        public int dwParentOutputId;
        // information on how new file should relate to the file passed in hFile 
        public int dwAssociation;
        // FI ID indicating type of data that this file will contain 
        public int dwOutputId;
        // Reserved; set to 0 
        public int dwFlags;
        public int dwSpecType;
        public string pSpec;
        // more file info based on the dwParentOutput ID; 
        public int pExportData;
        // pointer to the name of the template 
        public int pTemplateName;
    }
}