INSERT INTO ScanBatches (BatchStartDateTime, PageCount, DocumentCount, AutoIndexedCount, LastScanSequence, UserName)
VALUES                  (getdate(), @pageCount, 1, 1, 1, @userName)
SELECT SCOPE_IDENTITY()