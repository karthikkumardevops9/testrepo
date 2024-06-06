ISNULL((SELECT CAST(ISNULL(Locations.RequestFieldNameToken,0) AS int) + CAST(ISNULL(TrackingStatus.EmployeesId, 0) AS BIT) AS Requestable
FROM         Locations RIGHT OUTER JOIN
                      TrackingStatus ON Locations.Id = CAST(TrackingStatus.LocationsId AS INT) 
WHERE     (TrackingStatus.TrackedTableId = [@ViewName].[@pKey]) AND (TrackingStatus.TrackedTable = '@TableName')),0) as Requestable