SELECT ISNULL((SELECT TOP 1 ABS(Trackables.RecordVersion) AS RecordVersion
FROM Trackables
INNER JOIN Userlinks ON Trackables.Id = Userlinks.TrackablesId 
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId)
  AND (UserLinks.AttachmentNumber = @attachmentNumber)   
  AND (Trackables.RecordVersion < 0)
ORDER BY UserLinks.AttachmentNumber DESC, RecordVersion DESC), 0)