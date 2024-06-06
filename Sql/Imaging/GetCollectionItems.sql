SELECT [Ticket], [Table], [TableId], [Index] AS AttachmentNumber, [AttachmentType] AS VersionNumber, [DisplayText], [TableUserName], [Extension], [UseOverlay] FROM [SLCollectionItems] 
INNER JOIN SLCollections ON SLCollections.Id = SLCollectionItems.CollectionId
WHERE SLCollections.Operator = @userId