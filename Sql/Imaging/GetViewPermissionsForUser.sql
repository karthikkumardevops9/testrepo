SELECT  SecurePermission.Permission
    FROM  SecureUserGroup INNER JOIN
          SecureUser ON SecureUserGroup.UserID = SecureUser.UserID INNER JOIN
          SecureObject INNER JOIN
          SecureObjectPermission ON SecureObject.SecureObjectID = SecureObjectPermission.SecureObjectID INNER JOIN
          SecurePermission ON SecureObjectPermission.PermissionID = SecurePermission.PermissionID ON 
          SecureUserGroup.GroupID = SecureObjectPermission.GroupID
   WHERE (SecureUser.UserID = @userID) 
     AND (SecureObject.Name = @viewName) 
     AND (SecureObject.SecureObjectTypeID = 3)
ORDER BY  SecurePermission.Permission 