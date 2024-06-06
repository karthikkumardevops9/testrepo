SELECT [LowerTableName], [LowerTableFieldName], Tables.IDFieldName
FROM [Relationships] 
INNER JOIN Tables ON Tables.TableName = Relationships.LowerTableName
WHERE [Relationships].[UpperTableName] = @tableName