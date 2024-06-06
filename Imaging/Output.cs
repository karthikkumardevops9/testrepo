using MSRecordsEngine.Properties;
using System;
using System.Drawing;

namespace MSRecordsEngine.Imaging
{

    [CLSCompliant(true)]
    public class Output
    {
        public enum Values
        {
            AliasNone = 0x0,
            AliasAlways = 0x1,
            AliasGrayscaleOnly = 0x2,
            PagesAll = 0x1,
            PagesSelection = 0x2,
            PagesRange = 0x3
        }

        public enum Options
        {
            UseTemplate = 0x3,
            GraphicType = 0x6,
            AntiAliasing = 0x1E,
            GraphicDPI = 0x22,
            WhichPages = 0x5277C,
            StartPage = 0x5277D,
            EndPage = 0x5277E,
            UseDocumentPageSettings = 0x52782
        }

        public enum Format
        {
            Htm = 0x44D,
            Xml = 0x47E,
            Text = 0x514,
            Bmp = 0x5DC,
            Tif = 0x5DD,
            Gif = 0x5DF,
            Jpg = 0x5FF,
            Png = 0x626
        }

        public static string ByteArrayToFile(string fileName, byte[] value)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && System.IO.File.Exists(fileName))
                    System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);

                using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
                {
                    fs.Write(value, 0, value.Length);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string ImageToFile(string fileName, Image bmp)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return "Filename is required";
                if (System.IO.File.Exists(fileName))
                    System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
                bmp.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);
                return string.Empty;
            }
            catch (Exception ex)
            {
               throw new Exception(string.Format("Error \"{0}\" in Output.ImageToFile (fileName: {1})", ex.Message, fileName));
            }
        }

        public static byte[] FileToByteArray(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName))
                return null;

            try
            {
                using (var fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    if (fs.Length <= 0L | fs.Length > int.MaxValue)
                        return null;
                    var byteArray = new byte[((int)fs.Length)];
                    fs.Read(byteArray, 0, (int)fs.Length);
                    return byteArray;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Output.FileToByteArray (fileName: {1})", ex.Message, fileName));
            }
        }

        public static string ByteArrayToString(byte[] byteArray)
        {
            var encoding = new System.Text.ASCIIEncoding();
            return encoding.GetString(byteArray);
        }

        public static byte[] ImageToByteArray(Bitmap bmp)
        {
            if (bmp is null)
                throw new Exception("Invalid File Format");

            using (var stream = new System.IO.MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public static Bitmap Invalid()
        {
            return Resources.Invalid;
        }

        public static Bitmap NotAvailableImage()
        {
            return Resources.NotAvailableLarge;
        }

        public static byte[] StringToByteArray(string str)
        {
            try
            {
                var encoding = new System.Text.ASCIIEncoding();
                var temp = encoding.GetBytes(str);

                var byteArray = new byte[temp.Length + 1];
                temp.CopyTo(byteArray, 0);
                return byteArray;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Output.StringToByteArray (str: {1})", ex.Message, str));
            }
        }
    }
}