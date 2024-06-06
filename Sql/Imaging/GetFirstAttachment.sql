SELECT Tables.TableName AS IndexTable, UserLinks.IndexTableId, ISNULL(UserLinks.AttachmentNumber, 1) AS AttachmentNumber, 
    Trackables.Id AS TrackablesId, Trackables.RecordVersion, Attachments.PageNumber AS PageNumber, 
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
  AND (Trackables.RecordVersion = (SELECT TOP 1 Trackables.RecordVersion FROM Trackables WHERE (Attachments.TrackablesId = Trackables.Id) ORDER BY Trackables.RecordVersion DESC))
  AND (ISNULL(UserLinks.AttachmentNumber, 1) = (SELECT TOP 1 ISNULL(UserLinks.AttachmentNumber, 1) FROM UserLinks WHERE (UserLinks.IndexTable = @tableName) AND (UserLinks.IndexTableId = @tableId) ORDER BY UserLinks.AttachmentNumber))
ORDER BY ISNULL(UserLinks.AttachmentNumber, 1) DESC, Attachments.TrackablesId DESC, Attachments.TrackablesRecordVersion DESC, Attachments.PageNumber ASC