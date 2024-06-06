SELECT Relationships.*, [lower].IdFieldName FROM Relationships 
INNER JOIN Tables [upper] ON Relationships.UpperTableName = [upper].TableName
INNER JOIN Tables [lower] ON Relationships.LowerTableName = [lower].TableName
WHERE LowerTableName = @tableName
  AND ([upper].AuditUpdate <> 0      AND [upper].AuditUpdate IS NOT NULL) 
  AND ([lower].AuditAttachments <> 0 AND [lower].AuditAttachments IS NOT NULL) 