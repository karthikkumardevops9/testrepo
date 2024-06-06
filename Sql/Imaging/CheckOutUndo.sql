DELETE ImagePointers FROM ImagePointers p
INNER JOIN Trackables ON p.TrackablesId = Trackables.Id 
                     AND p.TrackablesRecordVersion = Trackables.RecordVersion 
INNER JOIN  Userlinks ON Trackables.Id = Userlinks.TrackablesId
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (Trackables.RecordVersion < 0)

DELETE PCFilesPointers FROM PCFilesPointers p
INNER JOIN Trackables ON p.TrackablesId = Trackables.Id 
                     AND p.TrackablesRecordVersion = Trackables.RecordVersion 
INNER JOIN  Userlinks ON Trackables.Id = Userlinks.TrackablesId
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (Trackables.RecordVersion < 0)

DELETE SLAuditUpdates FROM SLAuditUpdates
INNER JOIN Userlinks ON UserLinks.IndexTable = SLAuditUpdates.TableName AND (UserLinks.IndexTableId = SLAuditUpdates.TableId) 
INNER JOIN Trackables ON Trackables.Id = Userlinks.TrackablesId  
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (Trackables.RecordVersion < 0)
  AND (SLAuditUpdates.UpdateDateTime >= Trackables.CheckedOutDate)
  AND ((SLAuditUpdates.Action = 'Edited Annotations') OR (SLAuditUpdates.Action = 'Rotated All Annotations'))
  
DELETE Trackables FROM Trackables 
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (Trackables.RecordVersion < 0)
  
UPDATE Trackables SET CheckedOut = 0, CheckedOutDate = NULL, CheckedOutUser = NULL, CheckedOutIP = NULL, 
       CheckedOutMAC = NULL, CheckedOutUserId = NULL, CheckedOutFolder = NULL, PersistedCheckOut = 0
FROM   Trackables               
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)