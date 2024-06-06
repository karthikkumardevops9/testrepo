INSERT INTO [SLAuditUpdChildren] 
      ([SLAuditUpdatesId], [TableName], [TableId]) 
SELECT @auditUpdateId, TableName, @tableId  
FROM   Tables
WHERE  TableName = @tableName   
