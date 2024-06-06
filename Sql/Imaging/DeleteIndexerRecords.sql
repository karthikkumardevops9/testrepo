DELETE FROM [SLIndexer]
WHERE (IndexType <> 8) AND (IndexTableName = @tableName) AND (IndexTableID = @tableId) AND (AttachmentNumber = @attachmentNumber) {0} 

DELETE FROM [SLIndexerCache]
WHERE (IndexType <> 8) AND (IndexTableName = @tableName) AND (IndexTableID = @tableId) AND (AttachmentNumber = @attachmentNumber) {0} 