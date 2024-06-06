SELECT     dbo.SLRequestor.TableName, dbo.SLRequestor.TableId, dbo.SLRequestor.Status, dbo.TrackingStatus.TrackedTable, --TRACK_CONTAINER_ID, 
                      TRACK_CONTAINER_DESCRIPTION1 as DescFieldName1, TRACK_CONTAINER_DESCRIPTION2 as DescFieldName2, dbo.SLRequestor.DateRequested, dbo.Employees.Name,
                      dbo.SLRequestor.EmployeeID, dbo.SLRequestor.Priority, dbo.SLRequestor.DateNeeded, dbo.SLRequestor.FileRoomOrder, dbo.SLRequestor.Instructions, 
                      dbo.SLRequestor.ID
FROM         dbo.SLRequestor INNER JOIN
                      TRACKABLE_TABLE_NAME ON dbo.SLRequestor.TableId = TRACKABLE_TABLE_ID INNER JOIN
                      dbo.TrackingStatus ON TRACKABLE_TABLE_ID = dbo.TrackingStatus.TrackedTableId INNER JOIN
                      CONTAINER_TABLE_NAME ON TRACK_CONTAINER_ID = CONTAINER_ID INNER JOIN
                      dbo.Employees ON dbo.SLRequestor.EmployeeId = dbo.Employees.Id
WHERE     (dbo.SLRequestor.TableName = 'TRACKABLE_TABLE_NAME') AND (dbo.SLRequestor.Status = 'TRACKABLE_STATUS_TYPE') 
AND (dbo.TrackingStatus.TrackedTable = 'TRACKABLE_TABLE_NAME') AND (dbo.SLRequestor.TableID = 'TRACKABLE_TABLE_VALUEOFID')