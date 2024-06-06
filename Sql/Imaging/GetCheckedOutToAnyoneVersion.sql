SELECT Tables.TableName AS IndexTable, UserLinks.IndexTableId, UserLinks.AttachmentNumber AS AttachmentNumber, 
    Trackables.Id AS TrackablesId, ABS(Trackables.RecordVersion) AS RecordVersion, Attachments.PageNumber AS PageNumber, 
    Trackables.RecordTypesId AS RecordType, Trackables.Orphan, Trackables.CheckedOut, Trackables.CheckedOutDate, 
    Trackables.CheckedOutUser, Trackables.CheckedOutUserId, Trackables.CheckedOutFolder, Trackables.CheckedOutIP, Trackables.CheckedOutMAC, 
    Trackables.OfficialRecord, Volumes.ImageTableName AS ImageTableName, [System].RenameOnScan AS RenameOnScan, 
    Attachments.ScanDirectoriesId, Attachments.[FileName], Attachments.PointerId AS PointerId, OrgFullPath, OrgFileName, 
    [System].PrintImageFooter AS PrintImageFooter, Tables.UserName AS TableUserName, Tables.OfficialRecordHandling as OfficialRecordHandling,
    SystemAddresses.PhysicalDriveLetter + CAST(Volumes.PathName AS VARCHAR(260)) + ISNULL(Directories.Path,'') + '\' + Attachments.[FileName] AS FullPath, 
    Tables.IdFieldName, Tables.DescFieldPrefixOne, Tables.DescFieldPrefixTwo, Tables.DescFieldNameOne, Tables.DescFieldNameTwo,
    Attachments.ScanBatchesId AS ScanBatchesId, Attachments.ScanSequence AS ScanSequence, ScanBatches.BatchStartDateTime AS ScanDate, 
    ScanBatches.PageCount AS ScanPageCount, ScanBatches.DocumentCount, ScanBatches.BelowDeleteSizeCount, ScanBatches.RescannedCount, 
    ScanBatches.AutoIndexedCount, ScanBatches.LastScanSequence, ScanBatches.ScanRulesIdUsed, ScanBatches.UserName AS ScanUserName
    FROM 
   (SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, ImagePointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, PageNumber, [PageCount], [FileName], ScanBatchesId, ScanSequence FROM ImagePointers 
    UNION 
    SELECT CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, OrgFileName, PCFilesPointers.Id AS PointerId, TrackablesId, TrackablesRecordVersion, ScanDirectoriesId, PageNumber, 1 AS [PageCount], [FileName], ScanBatchesId, ScanSequence FROM PCFilesPointers) 
    AS Attachments
INNER JOIN      Directories ON Attachments.ScanDirectoriesId = Directories.Id  
INNER JOIN          Volumes ON Directories.VolumesId = Volumes.Id  
INNER JOIN  SystemAddresses ON Volumes.SystemAddressesId = SystemAddresses.Id  
INNER JOIN       Trackables ON Attachments.TrackablesRecordVersion = Trackables.RecordVersion 
                           AND Attachments.TrackablesId = Trackables.Id  
INNER JOIN        Userlinks ON Trackables.Id = Userlinks.TrackablesId  
INNER JOIN           Tables ON UserLinks.IndexTable = Tables.TableName
LEFT OUTER JOIN ScanBatches ON ScanBatches.Id = Attachments.ScanBatchesId
CROSS JOIN [System]
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (Trackables.RecordVersion < 0)
ORDER BY Attachments.PageNumber