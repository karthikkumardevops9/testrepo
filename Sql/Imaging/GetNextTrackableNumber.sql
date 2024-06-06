BEGIN TRANSACTION
SELECT TOP 1 NextTrackablesId FROM SysNextTrackable WITH (TABLOCKX)
UPDATE SysNextTrackable SET NextTrackablesId = NextTrackablesId + 1 
COMMIT TRANSACTION