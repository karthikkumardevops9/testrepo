DELETE i FROM [SLCollectionItems] i
INNER JOIN SLCollections c ON c.Id = i.CollectionId
WHERE c.[Name] = @userId AND i.[Table]= @tableName AND i.[TableId] = @tableId {0}

DELETE c FROM SLCollections c 
LEFT OUTER JOIN SLCollectionItems i on i.CollectionId = c.Id
WHERE c.[Name] = @userId AND i.[Table] IS NULL

