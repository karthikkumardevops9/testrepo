INSERT INTO [SLDestructCertItems]
	(TableId, TableName, HoldReason, RetentionHold, LegalHold, 
	 RetentionCode, SnoozeUntil, DispositionFlag, 
	 ScheduledInactivity, ScheduledDestruction, SLDestructionCertsID)
VALUES                              
	(@tableId, @tableName, @holdReason, @retentionHold, @legalHold, 
	 @retentionCode, @snoozeUntil, @dispositionFlag, 
	 @scheduledInactivity, @scheduledDestruction, @destructionCertsID)
SELECT SCOPE_IDENTITY()


        