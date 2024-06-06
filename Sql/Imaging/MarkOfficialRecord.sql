UPDATE Trackables 
SET OfficialRecord = 0,
    OfficialRecordReconciliation = 0
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)

UPDATE Trackables 
SET OfficialRecord = 1,
    OfficialRecordReconciliation = 1
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (ABS(Trackables.RecordVersion) = @versionNumber)
