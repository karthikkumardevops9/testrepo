SELECT DISTINCT SecurePermission.Permission
      FROM SecureObjectPermission 
INNER JOIN SecureObject ON SecureObjectPermission.SecureObjectID = SecureObject.SecureObjectID 
INNER JOIN SecurePermission ON SecureObjectPermission.PermissionID = SecurePermission.PermissionID 
LEFT OUTER JOIN SecureUserGroup ON SecureObjectPermission.GroupID = SecureUserGroup.GroupID
     WHERE ((SecureObjectPermission.PermissionID = 1 OR SecureObjectPermission.PermissionID = 3 OR SecureObjectPermission.PermissionID = 4 
        OR   SecureObjectPermission.PermissionID = 5 OR SecureObjectPermission.PermissionID = 6) 
       AND   SecureObject.Name = @secureObjectName AND SecureObject.SecureObjectTypeID = 11 AND SecureObject.BaseID > 0 
       AND  (SecureUserGroup.UserID = @userId OR SecureObjectPermission.GroupID = -1))