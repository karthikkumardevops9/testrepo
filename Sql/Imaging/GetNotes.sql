SELECT a.Id, a.[Table], a.TableId, a.NoteDateTime, a.Annotation, s.UserId AS UserId, 
	CASE WHEN s.DisplayName IS NOT NULL AND s.DisplayName > '' THEN s.DisplayName 
         WHEN a.DeskOf IS NOT NULL AND a.DeskOf > '' THEN a.DeskOf 
		 ELSE a.UserName
	END AS UserName
FROM Annotations a LEFT JOIN SecureUser s ON s.UserName = a.UserName
 WHERE a.TableId = @pointerId
   AND a.[Table] = @noteTable