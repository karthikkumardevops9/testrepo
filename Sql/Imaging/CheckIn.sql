UPDATE Trackables SET CheckedOut = 0, CheckedOutDate = NULL, CheckedOutUser = NULL,
                      CheckedOutIP = NULL, CheckedOutMAC = NULL, CheckedOutUserId = NULL, PersistedCheckOut = 0
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)

UPDATE Trackables 
   SET OfficialRecord = @officialRecord, 
       OfficialRecordReconciliation = @officialRecord, 
       RecordVersion = ABS(Trackables.RecordVersion),
       CheckedOutFolder = NULL, PersistedCheckOut = 0
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (Trackables.RecordVersion < 0)
  
SELECT TOP 1 Trackables.RecordVersion FROM Trackables 
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
ORDER BY Trackables.RecordVersion DESC