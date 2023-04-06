
CREATE PROCEDURE [dbo].[Znode_EntityDetailRowCount]
( @TableName NVARCHAR(1000),
  @Id        VARCHAR(100))
AS

/*  Summary :
    Unit Testing:
		begin tran 
		EXEC Znode_EntityDetailRowCount @Id='80eded53-4136-4d50-b30a-23c829ce8841',@TableName='AspNetRoles'
		rollback tran

*/
     BEGIN
         SET NOCOUNT ON;
		 BEGIN TRY
         DECLARE @TBL_TABLE TABLE
         (COLUMNVALUE VARCHAR(100),
          DATA        NVARCHAR(100)
         );
         INSERT INTO @TBL_TABLE (COLUMNVALUE,DATA)
         EXEC (' SELECT ''CoulumnName'' columnname1,COLUMN_NAME FROM information_schema.CONSTRAINT_COLUMN_USAGE WHERE  CONSTRAINT_NAME LIKE ''%PK%'' AND Table_name = '''+@TableName+'''');
         INSERT INTO @TBL_TABLE (COLUMNVALUE,DATA)
         EXEC ('SELECT ''RowCount'' columnname ,Count(1) FROM '+@TableName);
         DECLARE @V_COLUMNNAME VARCHAR(100)=
         (
             SELECT DATA
             FROM @TBL_TABLE
             WHERE COLUMNVALUE = 'CoulumnName'
         );
         INSERT INTO @TBL_TABLE (COLUMNVALUE,DATA)
         EXEC (';with ADSDSD AS (SELECT ''IndexId'' columnname ,Row_number() Over(Order bY '+@V_COLUMNNAME+' ) RowID,'+@V_COLUMNNAME+'  FROM '+@TableName+' ) SELECT columnname ,RowID FROM ADSDSD WHERE '+@V_COLUMNNAME+'= '''+@Id+'''  ');
         
		 INSERT INTO @TBL_TABLE (COLUMNVALUE,DATA)
         EXEC (';with ADSDSD AS (SELECT ''Leadvalue'' columnname ,LEAD('+@V_COLUMNNAME+') Over(Order bY '+@V_COLUMNNAME+' ) RowID,'+@V_COLUMNNAME+'  FROM '+@TableName+' ) SELECT columnname ,RowID FROM ADSDSD WHERE '+@V_COLUMNNAME+'= '''+@Id+'''  ');
         
		 INSERT INTO @TBL_TABLE (COLUMNVALUE,DATA)
         EXEC (';with ADSDSD AS (SELECT ''Lagvalue'' columnname ,LaG('+@V_COLUMNNAME+') Over(Order bY '+@V_COLUMNNAME+' ) RowID,'+@V_COLUMNNAME+'  FROM '+@TableName+' ) SELECT columnname ,RowID FROM ADSDSD WHERE '+@V_COLUMNNAME+'= '''+@Id+'''  ');
         SELECT *,ISNULL(ROW_NUMBER() OVER(ORDER BY [RowCount]), 0) AS RowID
         FROM @TBL_TABLE PIVOT(MAX(DATA) FOR ColumnValue IN([RowCount],[IndexId],[Leadvalue],[Lagvalue])) AS piv;
		
		 END TRY
		 BEGIN CATCH
		     DECLARE @Status BIT ;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_EntityDetailRowCount @TableName = '+@TableName+',@Id='+@Id+',@Status='+CAST(@Status AS VARCHAR(200));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
			 ROLLBACK TRAN DeleteAccount;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_EntityDetailRowCount',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
            
		 END CATCH
     END;