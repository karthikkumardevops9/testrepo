UPDATE Userlinks SET AttachmentNumber = 1
WHERE (UserLinks.IndexTable = @tableName) 
  AND (UserLinks.IndexTableId = @tableId)
  AND ((AttachmentNumber = 0) OR (AttachmentNumber IS NULL))

