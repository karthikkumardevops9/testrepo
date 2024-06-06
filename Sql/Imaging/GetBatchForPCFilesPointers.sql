SELECT -1 AS Id, NULL AS BatchDate, NULL AS DocumentCount, 'PC Files' AS UserName, NULL AS ScanRule, 'PC Files' AS RecordType, COUNT(*) AS OrphanCount 
FROM PCFilesPointers p
LEFT OUTER JOIN ScanBatches s ON s.Id = p.ScanBatchesId
INNER JOIN Trackables t ON p.TrackablesRecordVersion = t.RecordVersion 
                       AND p.TrackablesId = t.Id 
                       AND (t.Orphan <> 0 AND t.Orphan IS NOT NULL) 
WHERE s.Id IS NULL
