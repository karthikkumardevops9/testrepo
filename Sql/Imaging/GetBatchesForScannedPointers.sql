SELECT s.Id, MAX(s.BatchStartDateTime) AS BatchDate, MAX(s.DocumentCount) AS DocumentCount, MAX(UserName) AS UserName, 
    MAX(ScanRulesIdUsed) AS ScanRule, MAX(CASE WHEN s.RecordType = 5 THEN 'PDF Scans' ELSE 'Image Scans' END) AS RecordType, 
    (SELECT COUNT(*) FROM ImagePointers p
     INNER JOIN Trackables t ON p.TrackablesRecordVersion = t.RecordVersion 
                           AND p.TrackablesId = t.Id 
                           AND (t.Orphan <> 0 AND t.Orphan IS NOT NULL)
     WHERE p.ScanBatchesId = s.Id) AS OrphanCount 
FROM ScanBatches s 
INNER JOIN ImagePointers p ON p.ScanBatchesId = s.Id
INNER JOIN    Trackables t ON p.TrackablesRecordVersion = t.RecordVersion 
                          AND p.TrackablesId = t.Id 
WHERE (t.Orphan <> 0 AND t.Orphan IS NOT NULL) 
GROUP BY s.Id
UNION
SELECT s.Id, MAX(s.BatchStartDateTime) AS BatchDate, MAX(s.DocumentCount) AS DocumentCount, MAX(UserName) AS UserName, 
    MAX(ScanRulesIdUsed) AS ScanRule, MAX(CASE WHEN s.RecordType = 5 THEN 'PDF Scans' ELSE 'Image Scans' END) AS RecordType, 
    (SELECT COUNT(*) FROM PCFilesPointers p
     INNER JOIN Trackables t ON p.TrackablesRecordVersion = t.RecordVersion 
                           AND p.TrackablesId = t.Id 
                           AND (t.Orphan <> 0 AND t.Orphan IS NOT NULL)
     WHERE p.ScanBatchesId = s.Id) AS OrphanCount 
FROM ScanBatches s 
INNER JOIN PCFilesPointers p ON p.ScanBatchesId = s.Id
INNER JOIN      Trackables t ON p.TrackablesRecordVersion = t.RecordVersion 
                            AND p.TrackablesId = t.Id 
WHERE (t.Orphan <> 0 AND t.Orphan IS NOT NULL) 
GROUP BY s.Id
ORDER BY s.Id DESC