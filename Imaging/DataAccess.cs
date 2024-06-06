using System;
using System.Runtime.InteropServices;

namespace MSRecordsEngine.Imaging
{

    [CLSCompliant(true)]
    public class DataAccess
    {
        public enum Err
        {
            OK = 0,
            FileOpenFailed = 7,                  // The file could not be opened                                  
            BadFile = 9,                         // The file is corrupt                                           
            EmptyFile = 10,                      // The file is empty                                             
            ProtectedFile = 11,                  // The file is password protected or encrypted                   
            SupplementaryFileOpenFailed = 12,    // The filter could not open additional files needed to view     
            NoFile = 16,                         // No file is currently open                                     
            EOF = 31,                            // Unexpected EOF in file                                        
            FileCreate = 34,                     // Error while creating file                                     
            FileChanged = 38,                    // File has changed unexpectedly                                 
            FileWriteFailed = 47,                // Error writing to file                                         
            DRMFile = 49                        // The file is protected by DRM tools.                           
        }

        public enum IOType
        {
            AnsiPath = 2,
            Redirect = 13
        }

        public enum CommandType
        {
            CreateNewFile = 0x101,
            NewFileInfo = 0x102
        }

        public enum ReturnType
        {
            NotHandled = 50
        }

        public enum ThreadType
        {
            PThreads, // This value or NativeThreads are valid in Windows; NativeThreads is preferred for readability
            NoThreads,
            NativeThreads
        }

        public enum ThreadReturnType
        {
            Success,
            Failed,
            AlreadyCalled
        }

        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DAGetErrorString")]
        public static extern void GetErrorString(int errorNumber, byte[] pBuffer, int bufSize);

        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DAInit")]
        public static extern Err Init();
        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DADeInit")]
        public static extern Err DeInit();

        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DAOpenDocument")]
        public static extern Err OpenDocument(ref IntPtr hDoc, IOType specType, byte[] spec, int flags);
        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DACloseDocument")]
        public static extern Err CloseDocument(IntPtr hDoc);

        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DASetOption")]
        public static extern Err SetOption(IntPtr hDoc, Output.Options OptionId, ref Output.Values newValue, ref int valueSize);
        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DASetOption")]
        public static extern Err SetOption(IntPtr hDoc, Output.Options OptionId, ref Output.Format newValue, ref int valueSize);
        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DASetOption")]
        public static extern Err SetOption(IntPtr hDoc, Output.Options OptionId, ref int newValue, ref int valueSize);
        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DASetOption")]
        public static extern Err SetOption(IntPtr hDoc, Output.Options OptionId, [MarshalAs(UnmanagedType.LPStr)] ref string szValue, ref int dwSize);

        [DllImport("sccda.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DAThreadInit")]
        private static extern ThreadReturnType ThreadInit(ThreadType ThreadOption);

        public static void MultiThreadOn()
        {
            try
            {
                ThreadInit(ThreadType.NativeThreads);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in DataAccess.MultiThreadOn", ex.Message));
            }
        }

        public static void MultiThreadOff()
        {
            try
            {
                ThreadInit(ThreadType.NoThreads);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in DataAccess.MultiThreadOff", ex.Message));
            }
        }
    }
}