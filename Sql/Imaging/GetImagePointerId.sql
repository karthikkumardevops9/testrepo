SELECT Id FROM ImagePointers 
    WHERE TrackablesId = @trackablesId 
    AND ABS(TrackablesRecordVersion) = @recordVersion 
    AND PageNumber = @pageNumber
    AND [FileName] = @fileName