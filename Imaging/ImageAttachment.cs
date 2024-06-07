using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Leadtools;
using Leadtools.Annotations;
using Leadtools.Codecs;
using Leadtools.Controls;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
using MSRecordsEngine.Properties;

namespace MSRecordsEngine.Imaging
{
    [Serializable()]
    public partial class ImageAttachment : Attachment
    {

        private ImageAttachment() : base()
        {
            PointerTableName = "ImagePointers";
        }

        public ImageAttachment(int userId, string tableName, string paddedTableId, string databaseName, Size thumbSize, DataRow row, SqlConnection conn) : base(userId, tableName, paddedTableId, databaseName, thumbSize, row, conn)
        {
            PointerTableName = "ImagePointers";
        }

        internal override Attachments.AttachmentTypes AttachmentType
        {
            get
            {
                return Attachments.AttachmentTypes.tkImage;
            }
        }

        internal override Page AddNewPage(int index)
        {
            try
            {
                if (index < 0)
                    index = 0;
                PagesList.Add(new ImagePage(this, index));
                return PagesList[index];
            }
            catch (Exception ex)
            {
                if (PagesList.Count > 0)
                {
                    return PagesList[PagesList.Count - 1];
                }
                else
                {
                    
                    throw;
                }
            }
        }

        internal override Thumb Thumbs(int index)
        {
            try
            {
                if (index >= ThumbsList.Count)
                    index = ThumbsList.Count - 1;
                if (index < 0)
                    index = 0;

                return ThumbsList[index];
            }
            catch (Exception ex)
            {
                if (ThumbsList.Count > 0)
                {
                    return ThumbsList[ThumbsList.Count - 1];
                }
                else
                {
                    throw;
                }
            }
        }

        internal override void BurstMultiPage(DataTable dt, int newVersionNumber, SqlConnection conn, int totalPages)
        {
            int newPageNumber = 1;

            foreach (DataRow row in dt.Rows)
            {
                if (totalPages == 1)
                {
                    CopyPointers(row, newPageNumber, newVersionNumber, false, conn);
                    newPageNumber += 1;
                }
                else
                {
                    for (int pageNumber = 0, loopTo = totalPages - 1; pageNumber <= loopTo; pageNumber++)
                    {
                        int newId = BurstPage(row, pageNumber + 1, newPageNumber, newVersionNumber, conn, totalPages);
                        if (newId != 0)
                            CopyAnnotations(row["PointerId"].ToString(), newId, pageNumber + 1, conn);
                        newPageNumber += 1;
                    }
                }
            }
        }

        internal override void BurstMultiPage(DataTable dt, int newVersionNumber, SqlConnection conn)
        {
            int newPageNumber = 1;

            foreach (DataRow row in dt.Rows)
            {
                var part = new AttachmentPart(this, AttachmentNumber, true, row, false, conn);

                if (part.PageCount == 1)
                {
                    CopyPointers(row, newPageNumber, newVersionNumber, false, conn);
                    newPageNumber += 1;
                }
                else
                {
                    for (int pageNumber = 0, loopTo = part.PageCount - 1; pageNumber <= loopTo; pageNumber++)
                    {
                        int newId = BurstPage(row, pageNumber + 1, newPageNumber, newVersionNumber, conn);
                        if (newId != 0)
                            CopyAnnotations(row["PointerId"].ToString(), newId, pageNumber + 1, conn);
                        newPageNumber += 1;
                    }
                }
            }
        }

        private int BurstPage(DataRow row, int pageNumber, int newPageNumber, int newVersionNumber, SqlConnection conn, int totalPages)
        {
            int newId;

            string fileName = row["FileName"].ToString();
            fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + newPageNumber.ToString() + Path.GetExtension(fileName);

            try
            {
                using (var cmd = new SqlCommand(Resources.BurstImagePointers, conn))
                {
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    cmd.Parameters.AddWithValue("@versionNumber", newVersionNumber);
                    cmd.Parameters.AddWithValue("@pageNumber", newPageNumber);
                    cmd.Parameters.AddWithValue("@Id", row["PointerId"]);
                    newId = Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                newId = 0;
            }

            if (newId != 0)
            {
                string imageTableName = row["ImageTableName"].ToString();
                string fullPath = row["FullPath"].ToString();
                string imageFileName = row["FileName"].ToString();

                if (!string.IsNullOrEmpty(row["OrgFullPath"].ToString()))
                {
                    if (!string.IsNullOrEmpty(imageFileName) && Information.IsNumeric(imageFileName))
                    {
                        imageTableName = row["OrgFullPath"].ToString();
                        fullPath = row["FileName"].ToString();
                    }
                }

                // Dim newFileName As String = BurstImage(fullPath, imageTableName, imageFileName, pageNumber, fileName, conn)
                UpdatePointerFileName(newId, fileName, imageTableName, (int)row["ScanDirectoriesId"], 0, true, conn, totalPages);
            }

            return newId;
        }

        private int BurstPage(DataRow row, int pageNumber, int newPageNumber, int newVersionNumber, SqlConnection conn)
        {
            int newId;

            string fileName = row["FileName"].ToString();
            fileName = Path.GetFileNameWithoutExtension(fileName) + "_" + newPageNumber.ToString() + Path.GetExtension(fileName);

            try
            {
                using (var cmd = new SqlCommand(Resources.BurstImagePointers, conn))
                {
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    cmd.Parameters.AddWithValue("@versionNumber", newVersionNumber);
                    cmd.Parameters.AddWithValue("@pageNumber", newPageNumber);
                    cmd.Parameters.AddWithValue("@Id", row["PointerId"]);
                    newId = Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                newId = 0;
            }

            if (newId != 0)
            {
                string imageTableName = row["ImageTableName"].ToString();
                string fullPath = row["FullPath"].ToString();
                string imageFileName = row["FileName"].ToString();

                if (!string.IsNullOrEmpty(row["OrgFullPath"].ToString()))
                {
                    if (!string.IsNullOrEmpty(imageFileName) && Information.IsNumeric(imageFileName))
                    {
                        imageTableName = row["OrgFullPath"].ToString();
                        fullPath = row["FileName"].ToString();
                    }
                }

                string newFileName = BurstImage(fullPath, imageTableName, imageFileName, pageNumber, fileName, conn);
                if (string.Compare(fileName, newFileName, true) != 0)
                    UpdatePointerFileName(newId, newFileName, (int)row["ScanDirectoriesId"], 0, true, conn);
            }

            return newId;
        }

        private string BurstImage(string fullPath, string imageTableName, string imageFileName, int pageNumber, string fileName, SqlConnection conn)
        {
            if (!string.IsNullOrEmpty(imageTableName))
                return BurstBlobImage(imageTableName, imageFileName, pageNumber, conn);
            var codec = new RasterCodecs();

            using (RasterImage img = codec.Load(fullPath, 0, CodecsLoadByteOrder.RgbOrGray, pageNumber, pageNumber))
            {
                codec.Save(img, Path.GetDirectoryName(fullPath) + Path.DirectorySeparatorChar + fileName, img.OriginalFormat, img.BitsPerPixel);
            }

            return fileName;
        }

        private string BurstBlobImage(string imageTableName, string recordId, int newPageNumber, SqlConnection conn)
        {
            string newRecordId = string.Empty;
            string fileExtension = string.Empty;
            int trackableType = 0;
            var newImageStream = new MemoryStream();

            try
            {
                DataTable dt = Attachments.FillDataTable(string.Format(Resources.GetDatabaseImage, imageTableName), conn, "@recordId", recordId);
                if (dt.Rows.Count == 0)
                    return string.Empty;

                fileExtension = dt.Rows[0]["FileExtension"].ToString();
                trackableType = (int)dt.Rows[0]["TrackableType"];
                var codec = new RasterCodecs();
                RasterImage img = codec.Load(new MemoryStream((byte[])dt.Rows[0]["ImageField"]), 0, CodecsLoadByteOrder.RgbOrGray, newPageNumber, newPageNumber);
                codec.Save(img, newImageStream, img.OriginalFormat, img.BitsPerPixel);
                img.Dispose();

                using (var cmd = new SqlCommand(string.Format(Resources.InsertImageTableRecords, imageTableName), conn))
                {
                    cmd.Parameters.AddWithValue("@fileExtension", fileExtension);
                    cmd.Parameters.AddWithValue("@trackableType", trackableType);
                    newRecordId = cmd.ExecuteScalar().ToString();
                }

                using (var cmd = new SqlCommand(string.Format(Resources.GetDatabaseImage, imageTableName), conn))
                {
                    cmd.Parameters.AddWithValue("@recordId", newRecordId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.UpdateCommand = new SqlCommand(string.Format(Resources.UpdateImageTableRecords, imageTableName), conn);
                        da.UpdateCommand.Parameters.Add("@imageField", SqlDbType.Image, newImageStream.Capacity, "ImageField");
                        da.UpdateCommand.Parameters.AddWithValue("@recordId", newRecordId);

                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                            return string.Empty;

                        dt.Rows[0].BeginEdit();
                        dt.Rows[0]["ImageField"] = newImageStream.ToArray();
                        dt.Rows[0].EndEdit();
                        da.Update(dt);
                        return newRecordId;
                    }
                }
            }
            catch (Exception ex)
            {
                
                Debug.Print(ex.Message);
                return string.Empty;
            }
            finally
            {
                newImageStream.Dispose();
            }
        }

        internal override string CopyFileWithDotPrefix(string imageTableName, string fullPath, SqlConnection conn)
        {
            string newPath = string.Empty;
            if (string.IsNullOrEmpty(imageTableName))
                newPath = Path.Combine(Path.GetDirectoryName(fullPath), "." + Path.GetFileName(fullPath));
            return CopyFile(imageTableName, fullPath, newPath, conn);
        }

        internal override int CopyRecord(string currentId, int newPageNumber, int newVersionNumber, string newImageTableId, SqlConnection conn)
        {
            string sql = Resources.CopyImagePointers;
            if (!string.IsNullOrEmpty(newImageTableId))
                sql = Resources.CopyDatabaseImagePointers;

            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", currentId);
                    cmd.Parameters.AddWithValue("@versionNumber", newVersionNumber);
                    cmd.Parameters.AddWithValue("@pageNumber", newPageNumber);
                    if (!string.IsNullOrEmpty(newImageTableId))
                        cmd.Parameters.AddWithValue("@fileName", newImageTableId);
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
               
                return 0;
            }
        }

        internal string RotatePages(string IpAddress, int pageNumber, int angle, SqlConnection conn)
        {
            if (AttachmentParts.Count == 0)
            {
                if (pageNumber == 0)
                    return "The was a problem locating the attachment. It may have been deleted by another user. Please refresh and try again.";
                return "The was a problem locating the page. It may have been deleted by another user. Please refresh and try again.";
            }

            int pageInFile = -1;
            int partNumber = -1;
            int runningPageCount = 0;

            if (pageNumber == 0)
                return RotatePages(IpAddress, partNumber, pageInFile, angle, conn);

            foreach (AttachmentPart part in AttachmentParts)
            {
                partNumber += 1;

                if (runningPageCount + part.PageCount >= pageNumber)
                {
                    pageInFile = pageNumber - runningPageCount;
                    break;
                }

                runningPageCount += part.PageCount;
            }

            if (pageInFile == 0 || partNumber < 0)
                return "There was a problem locating the page. It may have been deleted by another user. Please refresh and try again.";
            return RotatePages(IpAddress, partNumber, pageInFile, angle, conn);
        }

        private string RotatePages(string IpAddress, int partNumber, int pageInFile, int angle, SqlConnection conn)
        {
            string rtn = string.Empty;
            string workingFile = Path.GetTempFileName();
            Exporter.DeleteFile(workingFile);
            string tempFile = Path.GetTempFileName();
            Exporter.DeleteFile(tempFile);

            int pointerId = 0;
            int partCount = -1;
            int runningPageCount = 0;
            RasterImage image = default;
            string imageTableId = string.Empty;
            string imageTableName = string.Empty;
            var fileList = new List<string>();

            var codec = new RasterCodecs();
            CodecsImageInfo info = default;
            var imageViewer = new ImageViewer();
            imageViewer.AutoDisposeImages = true;

            try
            {
                foreach (AttachmentPart part in AttachmentParts)
                {
                    partCount += 1;

                    if (pageInFile == -1 || pageInFile != -1 && partCount == partNumber)
                    {
                        fileList.Add(part.FullPath);
                        imageTableName = part.ImageTableName;
                        imageTableId = part.FileName;

                        if (!string.IsNullOrEmpty(imageTableName))
                        {
                            byte[] byteArray = BlobImageToStream(imageTableName, imageTableId, conn);

                            if (byteArray is not null)
                            {
                                image = Page.ByteArrayToRasterImage(byteArray);
                                info = codec.GetInformation(new MemoryStream(byteArray), false);
                            }
                        }
                        else if (!string.IsNullOrEmpty(fileList[fileList.Count - 1]))
                        {
                            image = codec.Load(fileList[fileList.Count - 1], 0, CodecsLoadByteOrder.RgbOrGray, 1, -1);
                            info = codec.GetInformation(fileList[fileList.Count - 1], false);
                        }

                        if (image is not null && info is not null)
                        {
                            for (int pageNumber = 1, loopTo = image.PageCount; pageNumber <= loopTo; pageNumber++)
                            {
                                image.Page = pageNumber;

                                if (pageInFile == -1 || pageNumber == pageInFile)
                                {
                                    imageViewer.Image = image;
                                    pointerId = part.GetPointerId(VersionInfo, runningPageCount + pageNumber, part.FileName, conn);
                                    RotatePageWithAnnotations(IpAddress, imageViewer, part, pointerId, pageNumber, angle, conn);
                                }

                                codec.Save(image, workingFile, info.Format, info.BitsPerPixel, pageNumber, pageNumber, pageNumber, CodecsSavePageMode.Append);
                            }

                            if (!string.IsNullOrEmpty(imageTableName))
                            {
                                if (IsATiffFormat(info.Format, image.PageCount))
                                {
                                    codec.CompactFile(workingFile, tempFile, 0);
                                    UpdateAttachment.UpdateImageTableRecord(imageTableName, imageTableId, Output.FileToByteArray(tempFile), conn);
                                }
                                else
                                {
                                    UpdateAttachment.UpdateImageTableRecord(imageTableName, imageTableId, Output.FileToByteArray(workingFile), conn);
                                }
                            }
                            else if (IsATiffFormat(info.Format, image.PageCount))
                            {
                                codec.CompactFile(workingFile, fileList[fileList.Count - 1], 0);
                            }
                            else
                            {
                                File.Copy(workingFile, fileList[fileList.Count - 1], true);
                            }

                            Exporter.DeleteFile(workingFile);
                            Exporter.DeleteFile(tempFile);
                        }
                        else
                        {
                            rtn = "There was a problem getting page information. It may have been deleted by another user. Please refresh and try again.";
                            if (pageInFile == -1)
                                rtn = "There was a problem getting some page information. All or part of the attachment may have been deleted by another user. Please refresh and try again.";
                            //Logs.Loginformation(rtn);
                        }

                        if (pageInFile != -1 || !string.IsNullOrEmpty(rtn))
                            break;
                    }

                    runningPageCount += part.PageCount;
                }
            }
            catch (Exception ex)
            {
                rtn = ex.Message;
            }
            finally
            {
                Exporter.DeleteFile(workingFile);
                Exporter.DeleteFile(tempFile);
            }

            if (imageViewer.Image is not null)
                imageViewer.Image.Dispose();
            imageViewer = default;

            if (string.IsNullOrEmpty(rtn))
            {
                if (pageInFile == -1)
                {
                    string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, AttachmentNumber, Math.Abs(VersionInfo.Version));
                    Attachments.AuditUpdateByTableId(UserID, TableName, TableId, string.Empty, string.Empty, IpAddress, string.Empty, "Rotated All Annotations", data, data, AuditType.AttachmentViewerActionType.RotateAnnotations, conn);
                }

                foreach (string fileName in fileList)
                    DeleteCache(fileName, false);
            }

            return rtn;
        }

        private void RotatePageWithAnnotations(string IpAddress, ImageViewer imageViewer, AttachmentPart part, int pointerId, int pageNumber, int angle, SqlConnection conn)
        {
            // Try
            // Dim annotations As String = Attachments.GetAnnotation(Attachments.AttachmentTypes.tkImage, pageNumber, pointerId, conn)
            // Dim rotate As New ImageProcessing.RotateCommand(angle * 100, ImageProcessing.RotateCommandFlags.Resize, New RasterColor(Drawing.Color.White))

            // If String.IsNullOrEmpty(annotations) Then
            // rotate.Run(imageViewer.Image)
            // Return
            // End If

            // Dim origin As New PointF(CSng(imageViewer.Image.ImageWidth / 2), CSng(imageViewer.Image.ImageHeight / 2))
            // Dim automationManager As New AnnAutomationManager
            // automationManager.RasterCodecs = New Codecs.RasterCodecs
            // Dim annAuto As New AnnAutomation(automationManager, imageViewer)
            // Dim annotationCodec As New AnnCodecs

            // annAuto.Container.Transform = imageViewer.Transform
            // annotationCodec.Load(New System.IO.MemoryStream(Export.Output.StringToByteArray(annotations)), annAuto.Container, 1)

            // Dim pts As PointF() = {
            // New PointF(0, 0),
            // New PointF(imageViewer.Image.ImageWidth, 0),
            // New PointF(imageViewer.Image.ImageWidth, imageViewer.Image.ImageHeight),
            // New PointF(0, imageViewer.Image.ImageHeight)
            // }

            // Dim m As New Drawing2D.Matrix

            // With m
            // .RotateAt(angle, origin)
            // .TransformPoints(pts)
            // End With

            // Dim xMin As Single = pts(0).X
            // Dim yMin As Single = pts(0).Y

            // For i As Integer = 1 To pts.Length - 1
            // If pts(i).X < xMin Then xMin = pts(i).X
            // If pts(i).Y < yMin Then yMin = pts(i).Y
            // Next

            // Dim xTranslate As Single = -xMin
            // Dim yTranslate As Single = -yMin
            // Dim annotationsArray As Byte()

            // rotate.Run(imageViewer.Image)
            // Dim offset As Integer = 0
            // For Each ann As AnnObject In annAuto.Container.Objects
            // ann.Rotate(angle, New AnnPoint(origin))
            // If xTranslate <> 0 OrElse yTranslate <> 0 Then ann.Translate(xTranslate, yTranslate)
            // Next

            // Using mem As New System.IO.MemoryStream
            // 'annotationCodec.Save(mem, annAuto.Container, AnnCodecsFormat.Native, 1, AnnCodecsSavePageMode.Overwrite)
            // annotationCodec.Save(mem, annAuto.Container, AnnCodecsFormat.Xml, 1, AnnCodecsSavePageMode.Overwrite)
            // mem.Flush()
            // annotationsArray = mem.ToArray
            // End Using

            // Dim data As String = String.Format("Attachment: {1}{0}Version: {2}{0}Page: {3}", vbCrLf, AttachmentNumber, Math.Abs(VersionInfo.Version), pageNumber)
            // Dim paddedTableId As String = Attachments.PadTableId(TableName, TableId, conn)
            // Attachments.SaveAnnotation(IpAddress, UserID, TableName, paddedTableId, Attachments.AttachmentTypes.tkImage, pageNumber, pointerId, data, annotationsArray, False, "Rotated Page", AuditType.AttachmentViewerActionType.RotateAnnotations, conn)
            // Catch ex As Exception
            // slimShared.logWarning(String.Format("Error ""{0}"" in ImageAttachment.RotatePageWithAnnotations (TableName: {1}, TableId: {2}, AttachmentNumber: {3}, PageNumber: {4})", ex.Message, TableName, TableId, AttachmentNumber, pageNumber))
            // Throw
            // End Try
        }

        private void PrintMatrix(string value, System.Drawing.Drawing2D.Matrix mat)
        {
            int index = 0;
            Debug.Print("----{0}----", value);

            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    Debug.Print("{0}, {1}", mat.Elements[index], Interaction.IIf(i == 2, 1, 0));
                    index += 1;
                }
            }

            Debug.Print("----{0}----", value);
        }

        private bool IsATiffFormat(RasterImageFormat format, int pageCount)
        {
            if (pageCount <= 1)
                return false;

            switch (format)
            {
                case var @case when @case == RasterImageFormat.Ccitt:
                case var case1 when case1 == RasterImageFormat.CcittGroup31Dim:
                case var case2 when case2 == RasterImageFormat.CcittGroup32Dim:
                case var case3 when case3 == RasterImageFormat.CcittGroup4:
                case var case4 when case4 == RasterImageFormat.Tif:
                case var case5 when case5 == RasterImageFormat.TifAbc:
                case var case6 when case6 == RasterImageFormat.TifAbic:
                case var case7 when case7 == RasterImageFormat.Ccitt:
                case var case8 when case8 == RasterImageFormat.CcittGroup31Dim:
                case var case9 when case9 == RasterImageFormat.CcittGroup32Dim:
                case var case10 when case10 == RasterImageFormat.CcittGroup4:
                case var case11 when case11 == RasterImageFormat.TifCmp:
                case var case12 when case12 == RasterImageFormat.TifCmw:
                case var case13 when case13 == RasterImageFormat.TifCmyk:
                case var case14 when case14 == RasterImageFormat.TifCustom:
                case var case15 when case15 == RasterImageFormat.TifDxf:
                case var case16 when case16 == RasterImageFormat.TifJ2k:
                case var case17 when case17 == RasterImageFormat.TifJbig:
                case var case18 when case18 == RasterImageFormat.TifJbig2:
                case var case19 when case19 == RasterImageFormat.TifJpeg411:
                case var case20 when case20 == RasterImageFormat.TifJpeg422:
                case var case21 when case21 == RasterImageFormat.TifLead1Bit:
                case var case22 when case22 == RasterImageFormat.TifLeadMrc:
                case var case23 when case23 == RasterImageFormat.TifLzw:
                case var case24 when case24 == RasterImageFormat.TifLzwCmyk:
                case var case25 when case25 == RasterImageFormat.TifLzwYcc:
                case var case26 when case26 == RasterImageFormat.TifPackBits:
                case var case27 when case27 == RasterImageFormat.TifPackBitsCmyk:
                case var case28 when case28 == RasterImageFormat.TifPackbitsYcc:
                case var case29 when case29 == RasterImageFormat.TifUnknown:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
    }
}
