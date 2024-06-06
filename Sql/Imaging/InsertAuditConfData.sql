INSERT INTO [SLAuditConfData] ([TableName], [TableId], [OperatorsId], [NetworkLoginName], [Domain], [ComputerName], [MacAddress], [IP], [AccessDateTime], [ModuleName]) 
SELECT TableName, @tableId, @userName, @networkUserName, @domain, @computerName, @macAddress, @ipAddress, getdate(), @action
FROM   Tables
WHERE  TableName = @tableName   
  AND (AuditAttachments <> 0) AND (AuditAttachments IS NOT NULL) 