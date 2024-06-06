SELECT DISTINCT SecurePermission.Permission, SecureObject.SecureObjectTypeID
    FROM SecureUserGroup 
INNER JOIN SecureUser ON SecureUserGroup.UserID = SecureUser.UserID 
INNER JOIN SecureObject 
    INNER JOIN SecureObjectPermission ON SecureObject.SecureObjectID = SecureObjectPermission.SecureObjectID 
    INNER JOIN SecurePermission ON SecureObjectPermission.PermissionID = SecurePermission.PermissionID 
ON SecureUserGroup.GroupID = SecureObjectPermission.GroupID
WHERE  SecureObject.Name = @tableName AND SecureObject.BaseID > 0 
  AND (SecureUserGroup.UserID = @userId OR SecureObjectPermission.GroupID = -1) 
  AND (SecureObject.SecureObjectTypeID = 4 OR SecureObject.SecureObjectTypeID = 6 OR SecureObject.SecureObjectTypeID = 13
       OR (SecureObject.SecureObjectTypeID = 2 AND SecurePermission.PermissionID = 1)) 
ORDER BY SecurePermission.Permission 