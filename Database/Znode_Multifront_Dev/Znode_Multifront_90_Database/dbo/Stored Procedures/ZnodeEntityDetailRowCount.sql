-- EXEC ZnodeEntityDetailRowCount @Id='80eded53-4136-4d50-b30a-23c829ce8841',@TableName='AspNetRoles'
-- SELECT * FROM AspNetRoles


CREATE PROCEDURE [dbo].[ZnodeEntityDetailRowCount](
       @TableName NVARCHAR(1000) ,
       @Id        VARCHAR(100))
AS
     BEGIN
         SET NOCOUNT ON;
         DECLARE @TABLE TABLE (
                              COLUMNVALUE VARCHAR(100) ,
                              DATA        NVARCHAR(100)
                              );
         INSERT INTO @TABLE ( COLUMNVALUE , DATA
                            )
         EXEC (' SELECT ''CoulumnName'' columnname1,COLUMN_NAME FROM information_schema.CONSTRAINT_COLUMN_USAGE WHERE  CONSTRAINT_NAME LIKE ''%PK%'' AND Table_name = '''+@TableName+'''');
         INSERT INTO @TABLE ( COLUMNVALUE , DATA
                            )
         EXEC ('SELECT ''RowCount'' columnname ,Count(1) FROM '+@TableName);
         DECLARE @V_COLUMNNAME VARCHAR(100)= ( SELECT DATA
                                               FROM @TABLE
                                               WHERE COLUMNVALUE = 'CoulumnName'
                                             );
         INSERT INTO @TABLE ( COLUMNVALUE , DATA
                            )
         EXEC (';with ADSDSD AS (SELECT ''IndexId'' columnname ,Row_number() Over(Order bY '+@V_COLUMNNAME+' ) RowID,'+@V_COLUMNNAME+'  FROM '+@TableName+' ) SELECT columnname ,RowID FROM ADSDSD WHERE '+@V_COLUMNNAME+'= '''+@Id+'''  ');
         INSERT INTO @TABLE ( COLUMNVALUE , DATA
                            )
         EXEC (';with ADSDSD AS (SELECT ''Leadvalue'' columnname ,LEAD('+@V_COLUMNNAME+') Over(Order bY '+@V_COLUMNNAME+' ) RowID,'+@V_COLUMNNAME+'  FROM '+@TableName+' ) SELECT columnname ,RowID FROM ADSDSD WHERE '+@V_COLUMNNAME+'= '''+@Id+'''  ');
         INSERT INTO @TABLE ( COLUMNVALUE , DATA
                            )
         EXEC (';with ADSDSD AS (SELECT ''Lagvalue'' columnname ,LaG('+@V_COLUMNNAME+') Over(Order bY '+@V_COLUMNNAME+' ) RowID,'+@V_COLUMNNAME+'  FROM '+@TableName+' ) SELECT columnname ,RowID FROM ADSDSD WHERE '+@V_COLUMNNAME+'= '''+@Id+'''  ');
         SELECT * , ISNULL(ROW_NUMBER() OVER(ORDER BY [RowCount]) , 0) AS RowID
         FROM @TABLE PIVOT(MAX(DATA) FOR ColumnValue IN([RowCount] , [IndexId] , [Leadvalue] , [Lagvalue])) AS piv;
     END;  

