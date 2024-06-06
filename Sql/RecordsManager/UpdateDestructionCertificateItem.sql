UPDATE [SLDestructCertItems]
	SET HoldReason = @holdReason, RetentionHold = @retentionHold, LegalHold = @legalHold,
        RetentionCode = @retentionCode, SnoozeUntil = @snoozeUntil, DispositionFlag = @dispositionFlag,
		ScheduledInactivity = @scheduledInactivity, ScheduledDestruction = @scheduledDestruction,
		SLDestructionCertsID = @destructionCertsID 
	WHERE [ID] = @destructionCertItemID                       
