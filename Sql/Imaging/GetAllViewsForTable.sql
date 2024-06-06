SELECT [ViewName],[SQLStatement] FROM [Views] 
WHERE [Views].[TableName] = @tableName
AND [Views].[Printable] <> 1 
ORDER BY ViewOrder
