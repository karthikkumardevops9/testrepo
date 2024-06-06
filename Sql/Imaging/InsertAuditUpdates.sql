INSERT INTO [SLAuditUpdates] 
      ([TableName], [TableId], [OperatorsId], [NetworkLoginName], [Domain], [ComputerName], 
       [MacAddress], [IP], [Module], [Action], [DataBefore], [DataAfter], [ActionType], [UpdateDateTime]) 
SELECT TableName, @tableId, @userName, @networkUserName, @domain, @computerName, 
       @macAddress, @ipAddress, @module, @action, @dataBefore, @dataAfter, @actionType, getdate() 
FROM   Tables
WHERE  TableName = @tableName   
  AND (AuditAttachments <> 0) AND (AuditAttachments IS NOT NULL)
SELECT SCOPE_IDENTITY() 