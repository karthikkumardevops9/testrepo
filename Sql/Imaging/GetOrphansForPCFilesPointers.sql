SELECT Trackables.Id AS TrackablesId, Trackables.RecordVersion, PageNumber, Pages AS PageCount, 
    Trackables.RecordTypesId AS RecordType, Trackables.Orphan, Trackables.CheckedOut, Trackables.CheckedOutDate, 
    Trackables.CheckedOutUser, Trackables.CheckedOutUserId, Trackables.CheckedOutFolder, Trackables.CheckedOutIP, Trackables.CheckedOutMAC, 
    Trackables.OfficialRecord, Volumes.ImageTableName AS ImageTableName, PCFilesPointers.ScanDirectoriesId, PCFilesPointers.Id AS PointerId, 
    [System].PrintImageFooter AS PrintImageFooter, 'Orphans' AS TableUserName, 0 as OfficialRecordHandling, [System].RenameOnScan AS RenameOnScan, 
    SystemAddresses.PhysicalDriveLetter + CAST(Volumes.PathName AS VARCHAR(260)) + ISNULL(Directories.Path,'') + '\' + PCFilesPointers.[FileName] AS FullPath, 
    'Orphans' AS IndexTable, '' AS IndexTableId, 1 AS AttachmentNumber, CAST(OrgFullPath AS VARCHAR(260)) AS OrgFullPath, [FileName], OrgFileName  
    FROM PCFilesPointers
INNER JOIN     Directories ON PCFilesPointers.ScanDirectoriesId = Directories.Id 
INNER JOIN         Volumes ON Directories.VolumesId = Volumes.Id 
INNER JOIN SystemAddresses ON Volumes.SystemAddressesId = SystemAddresses.Id 
INNER JOIN      Trackables ON PCFilesPointers.TrackablesRecordVersion = Trackables.RecordVersion 
                          AND PCFilesPointers.TrackablesId = Trackables.Id 
CROSS JOIN [System]
WHERE (Trackables.Orphan <> 0 AND Trackables.Orphan IS NOT NULL) 
  AND (PCFilesPointers.ScanBatchesId = 0 OR PCFilesPointers.ScanBatchesId IS NULL)
ORDER BY Trackables.Id DESC, Trackables.RecordVersion DESC