-- EXEC [dbo].[Znode_SplitWhereClause] 'ProductName LIKE ''%raj and suraj%''',0


CREATE PROCEDURE [dbo].[Znode_SplitWhereClause](
       @WhereClause      NVARCHAR(MAX) ,
       @IsOutPutInFilter BIT           = 0)
AS
     BEGIN
         DECLARE @OperatorTable TABLE (
                                      Name NVARCHAR(100)
                                      );
         INSERT INTO @OperatorTable
         VALUES ( ' != ') , ( ' = ') , ( ' <> ') , ( ' < ') , ( ' > ') , ( ' Like ') , ( ' In ');

         DECLARE @LogicalOperatorTable TABLE (
                                             Name NVARCHAR(100)
                                             );
         INSERT INTO @LogicalOperatorTable
         VALUES ( ' AND ') , ( ' OR ');
         BEGIN
             DECLARE @RowCount INT;
             DECLARE @SplitedString TABLE (
                                          Id              INT IDENTITY ,
                                          Name            NVARCHAR(1000) ,
                                          LogicalOperator NVARCHAR(1000)
                                          );
             DECLARE @SplitedString1 TABLE (
                                           Id              INT IDENTITY ,
                                           Name            NVARCHAR(1000) ,
                                           LogicalOperator NVARCHAR(1000)
                                           );
             WHILE ( LEN(@WhereClause) > 0
                     AND
                     ( @WhereClause LIKE '% AND %' )  AND REPLACE (REPLACE (@WhereClause,'''','~~'),'%','{') NOT LIKE '%~~{% and %{~~%' ) 
                 BEGIN
                     IF @WhereClause LIKE '% AND %' AND REPLACE (REPLACE (@WhereClause,'''','~~'),'%','{') NOT LIKE '%~~{% and %{~~%' 
                         BEGIN
                             INSERT INTO @SplitedString
                                    SELECT SUBSTRING(@WhereClause , 1
									
									 ,CASE WHEN REPLACE (REPLACE (@WhereClause,'''','~~'),'%','{') NOT LIKE '%~~{% and %{~~%' THEN 
														 CHARINDEX(' AND ' , @WhereClause)-1 ELSE CHARINDEX(' AND ' , @WhereClause,CHARINDEX(' AND ' , @WhereClause)+1)-1 END ) , ' AND ';
                             SET @WhereClause = SUBSTRING(@WhereClause ,CASE WHEN REPLACE (REPLACE (@WhereClause,'''','~~'),'%','{') NOT LIKE '%~~{% and %{~~%' THEN 
														CHARINDEX(' AND ' , @WhereClause)+4  ELSE CHARINDEX(' AND ' , @WhereClause,CHARINDEX(' AND ' , @WhereClause)+1)+4 END   , 2000);
                         END;
                 END;
             INSERT INTO @SplitedString
                    SELECT @WhereClause , 'AND';

             DECLARE @name NVARCHAR(100);
             DECLARE db_cursor CURSOR
             FOR SELECT Name
                 FROM @SplitedString;
             OPEN db_cursor;
             FETCH NEXT FROM db_cursor INTO @name;
             WHILE @@FETCH_STATUS = 0
                 BEGIN
                     SET @WhereClause = @name;
                     WHILE ( LEN(@WhereClause) > 0
                             AND
                             ( @WhereClause LIKE '% OR %' ) )
                         BEGIN
                             IF @WhereClause LIKE '% OR %'
                                 BEGIN
                                     INSERT INTO @SplitedString1
                                            SELECT SUBSTRING(@WhereClause , 1 , CHARINDEX(' OR ' , @WhereClause)-1) , 'OR';
                                     SET @WhereClause = SUBSTRING(@WhereClause , CHARINDEX(' OR ' , @WhereClause)+4 , 2000);
                                 END;
                         END;
                     INSERT INTO @SplitedString1
                            SELECT @WhereClause , 'AND';
                     FETCH NEXT FROM db_cursor INTO @name;
                 END;
             CLOSE db_cursor;
             DEALLOCATE db_cursor;
             --Select * from @SplitedString1

             DECLARE @ConvertTableData TABLE (
                                             Id                INT ,
                                             CompleteStatement NVARCHAR(2000) ,
                                             LogicalOprator    NVARCHAR(100) ,
                                             Oprator           NVARCHAR(100) ,
                                             LeftStatement     VARCHAR(2000) ,
                                             RightStatement    NVARCHAR(MAX)
                                             );
             INSERT INTO @ConvertTableData
                    SELECT * , SUBSTRING(a.Name , 1 , CHARINDEX(b.Name , a.Name)-1) AS LeftStatement , SUBSTRING(a.Name , CHARINDEX(b.Name , a.Name)+LEN(b.Name) , 2000) AS RightStatement
                    FROM @SplitedString1 AS a INNER JOIN @OperatorTable AS b ON a.Name LIKE '%'+b.Name+'%';
             IF @IsOutPutInFilter = 0
                 BEGIN
                     SELECT *
                     FROM @ConvertTableData;
                 END;
             ELSE
                 BEGIN
                     SELECT Id , ' AttributeCode ='''+RTRIM(LTRIM(LeftStatement))+''' AND AttributeValue '+Oprator+' '+RTRIM(LTRIM(RightStatement))
                     FROM @ConvertTableData;
                 END;
         END;
     END;