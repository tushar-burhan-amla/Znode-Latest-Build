
CREATE PROC [dbo].[Znode_GetObjectColumnList]
( @Type        VARCHAR      = NULL,
  @Object_Name VARCHAR(200) = NULL)
AS
/*
 Summary: This Procedure is used to Get ColumnList of an Object
 Unit Testing:
 EXEC Znode_GetObjectColumnList 

*/
     BEGIN
         SET NOCOUNT ON;
         IF @Type IS NULL
             BEGIN
                 SELECT SO.name AS ObjectName,column_id AS columnId,SCO.name AS ColumnName,STY.name AS DataType
                 FROM sys.objects AS SO
                      INNER JOIN sys.columns AS SCO ON(SO.object_id = SCO.object_id)
                      INNER JOIN sys.types AS STY ON SCO.user_type_id = STY.user_type_id
                 WHERE type IN('U', 'V')
                      AND (SO.name = @Object_Name
                           OR @Object_Name IS NULL);
             END;
        ELSE
             BEGIN
                 IF @Type = 'P'
                    AND @Object_Name IS NOT NULL
                     BEGIN
                         DECLARE @Sp_query NVARCHAR(MAX);
                         SELECT @Sp_query = SpCalling
                         FROM ZNodeApplicationSpList
                         WHERE spname = @Object_Name;
                         EXEC sp_executesql
                              @Sp_query;
                     END;
                 ELSE
                     BEGIN
                         IF @Type = 'P'
                             BEGIN
                                 SELECT SpName AS ObjectName,1 AS columnId,'' AS ColumnName,'' AS DataType,SpCalling
                                 FROM ZNodeApplicationSpList;
                             END;
                         ELSE
                             BEGIN
                                 IF @Type = 'U'
                                    AND @Object_Name IS NOT NULL
                                     BEGIN
                                         SELECT SO.name AS ObjectName,column_id AS columnId,SCO.name AS ColumnName,STY.name AS DataType
                                         FROM sys.objects AS SO
                                              INNER JOIN sys.columns AS SCO ON(SO.object_id = SCO.object_id)
                                              INNER JOIN sys.types AS STY ON SCO.user_type_id = STY.user_type_id
                                         WHERE SO.type = 'U'
                                               AND (type = @Type
                                                    OR @Type IS NULL)
                                               AND (SO.name = @Object_Name
                                                    OR @Object_Name IS NULL);
                                     END;
                                 ELSE
                                     BEGIN
                                         SELECT SO.name AS ObjectName,column_id AS columnId,SCO.name AS ColumnName,STY.name AS DataType
                                         FROM sys.objects AS SO
                                              INNER JOIN sys.columns AS SCO ON(SO.object_id = SCO.object_id)
                                              INNER JOIN sys.types AS STY ON SCO.user_type_id = STY.user_type_id
                                         WHERE(type = @Type
                                               OR @Type IS NULL)
                                              AND (SO.name = @Object_Name
                                                   OR @Object_Name IS NULL);
                                     END;
                             END;
                     END;
             END; 
     END;
