INSERT INTO Trackables (Id, RecordVersion, RecordTypesId, [PageCount], Orphan, 
					    Verified, OfficialRecord, OfficialRecordReconciliation)
SELECT t.Id, ((SELECT TOP 1 Trackables.RecordVersion FROM Trackables 
			  INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
			  WHERE (UserLinks.IndexTable = @tableName) 
                AND (UserLinks.IndexTableId = @tableId) 
                AND (UserLinks.AttachmentNumber = @attachmentNumber)
              ORDER BY Trackables.RecordVersion DESC) + 1) * -1 AS RecordVersion, 
       RecordTypesId, [PageCount], Orphan, Verified, OfficialRecord, OfficialRecordReconciliation
FROM Trackables AS t
INNER JOIN Userlinks ON t.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (t.RecordVersion = @versionNumber)

UPDATE Trackables SET CheckedOut = 0, PersistedCheckOut = 0, CheckedOutDate = getdate(), CheckedOutUser = @userName,
                      CheckedOutIP = @IPAddress, CheckedOutMAC = @MACAddress, CheckedOutUserId = @userId
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)

UPDATE Trackables SET CheckedOut = 1, CheckedOutFolder = @checkedOutFolder, PersistedCheckOut = {0}
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND ((Trackables.RecordVersion = @versionNumber)
   OR (Trackables.RecordVersion < 0))

SELECT Trackables.RecordVersion 
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (Trackables.RecordVersion < 0)
