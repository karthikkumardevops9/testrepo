SELECT p.Id FROM ImagePointers p
INNER JOIN  Userlinks ON p.TrackablesId = Userlinks.TrackablesId
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId) 
  AND (UserLinks.AttachmentNumber = @attachmentNumber)
  AND (p.TrackablesRecordVersion < 0)
  AND (p.PageNumber = @pageNumber)