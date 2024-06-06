INSERT INTO [SLCollectionItems] ([CollectionId], [Ticket], [Table], [TableId], [Index], [AttachmentType], [DisplayText], [Extension], [UseOverlay], [TableUserName]) 
SELECT @collectionId, @ticket, TableName, @tableId, @attachmentNumber, @versionNumber, @displayText, @extension, @useOverlay, UserName FROM Tables WHERE TableName = @tableName
