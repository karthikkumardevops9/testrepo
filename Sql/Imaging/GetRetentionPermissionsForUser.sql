  SELECT  SecurePermission.Permission, SecureObject.SecureObjectTypeID, SecureObject.Name
    FROM  SecureUserGroup INNER JOIN
          SecureUser ON SecureUserGroup.UserID = SecureUser.UserID INNER JOIN
          SecureObject INNER JOIN
          SecureObjectPermission ON SecureObject.SecureObjectID = SecureObjectPermission.SecureObjectID INNER JOIN
          SecurePermission ON SecureObjectPermission.PermissionID = SecurePermission.PermissionID ON 
          SecureUserGroup.GroupID = SecureObjectPermission.GroupID
   WHERE (SecureUser.UserID = @userID) 
     AND ((SecureObject.Name = 'View Inactive Records') OR (SecureObject.Name = 'View Archived Records') OR (SecureObject.Name = 'View Destroyed Records'))
     AND (SecureObject.SecureObjectTypeID = 8) AND (SecurePermission.Permission = 'Access')
ORDER BY  SecurePermission.Permission 