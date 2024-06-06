 SELECT COUNT(*) FROM SecureObjectPermission 
LEFT OUTER JOIN SecureUserGroup ON SecureObjectPermission.GroupID = SecureUserGroup.GroupID 
  WHERE SecureObjectPermission.SecureObjectID = 1 AND SecureObjectPermission.PermissionID = 1 
    AND (SecureUserGroup.UserID = @userId OR SecureObjectPermission.GroupID = -1)
