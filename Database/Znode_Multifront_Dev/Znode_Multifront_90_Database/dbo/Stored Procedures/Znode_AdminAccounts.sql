----  SELECT * FROM AspNetUserRoles b  INNER JOIN
--                         AspNetRoles c ON (b.RoleId = c.Id) INNER JOIN
--                         AspNetUsers d ON (b.userId = d .id)   WHERE d.Username ='admin12345' AND c.Name = 'Admin'
-- DECLARE @EDE INT  EXEC Znode_AdminAccounts 'ADMIN','admin12345',@WhereClause='',@Order_By='',@RowCount=@EDE OUT  SELECT @EDE
CREATE  Procedure [dbo].[Znode_AdminAccounts]
(
@RoleName  Varchar(200)
,@UserName  Varchar (200)
,@WhereClause VARCHAR(1000) = nULL 
,@Rows      int = 100
,@PageNo    int = 0
,@Order_By  Varchar(1000) = NULL 
,@RowCount  BIGInt = 10 OUT 
)
AS 
BEGIN 
 BEGIN TRY 
  SET NOCOUNT ON 

    DECLARE @V_SQL  NVARCHAR(1000)

	IF EXISTS ( SELECT TOP 1  1 FROM AspNetUserRoles b  INNER JOIN
                         AspNetRoles c ON (b.RoleId = c.Id) INNER JOIN
                         AspNetUsers d ON (b.userId = d .id)   WHERE d.Username =@UserName AND c.Name = 'Admin' )    AND  @ROleName <> '' 

    BEGIN 
	
	 SELECT * 
	 INTO #TEMP_Account
	 FROM View_AccountRoles
	
	--SET @RowsCount = (SELECT Count(1) FROM #TEMP_Account)
	SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END 
	
	SET @V_SQL = ' SELECT * INTO #TEMP_Account12 FROM #TEMP_Account WHERE 1=1 ' +case WHEN @WhereClause IS NOT NULL AND @WhereClause <> '' THEN  ' AND '+@WhereClause 
	                                                                              ELSE '' END
								+' SELECT  @tempo=Count(1) FROM #TEMP_Account12  SELECT * FROM #TEMP_Account12 ' 
								+' Order BY '+CASE WHEN @Order_BY = '' OR @Order_BY IS NULL THEN '1' ELSE @Order_BY END + ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '
	
	PRINT @V_SQL
	EXEC SP_executesql @V_SQL,N'@tempo INT OUT' ,@tempo=@RowCount out
	DROP TABLE #TEMP_Account

	END 
	
	ELSE IF EXISTS ( SELECT TOP 1  1 FROM AspNetUserRoles b  INNER JOIN
                         AspNetRoles c ON (b.RoleId = c.Id) INNER JOIN
                         AspNetUsers d ON (b.userId = d .id)   WHERE d.UserName =@UserName  )   AND  @ROleName <> '' 

    BEGIN 
	 ---PRINT '11'
	 SELECT * 
	 INTO #TEMP_Account1
	 FROM View_AccountRoles
	 WHERE RoleName = 'Customer'
	 
	
	SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END 
	
     SET @V_SQL = ' SELECT * INTO #TEMP_Account11 FROM #TEMP_Account1 WHERE 1=1 ' +case WHEN @WhereClause IS NOT NULL AND @WhereClause <> '' THEN  ' AND '+@WhereClause ELSE '' END
								+' SELECT  @tempo=Count(1) FROM #TEMP_Account11  SELECT * FROM #TEMP_Account11 ' 
								+' Order BY '+CASE WHEN @Order_BY = '' OR @Order_BY IS NULL THEN '1' ELSE @Order_BY END + ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '
	
	PRINT @V_SQL
	EXEC SP_executesql @V_SQL,N'@tempo INT OUT' ,@tempo=@RowCount out
	
	DROP TABLE #TEMP_Account1
	
	END 

	ELSE IF  @roleName = ''  

    BEGIN 
	 ---PRINT '11'
	 SELECT * 
	 INTO #TEMP_Account11
	 FROM View_CustomerAccountDetails a 
	 WHERE NOT EXISTS (SELECT  TOP 1 1 FROM AspNetUserRoles b  WHERE a.Userid = b.userid  )
	 
	
	SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END 
	
     SET @V_SQL = ' SELECT * INTO #TEMP_Account111 FROM #TEMP_Account11 WHERE 1=1 ' +case WHEN @WhereClause IS NOT NULL AND @WhereClause <> '' THEN  ' AND '+@WhereClause ELSE '' END
								+' SELECT  @tempo=Count(1) FROM #TEMP_Account111   SELECT * FROM #TEMP_Account111 ' 
								+' Order BY '+CASE WHEN @Order_BY = '' OR @Order_BY IS NULL THEN '1' ELSE @Order_BY END + ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '
	
	PRINT @V_SQL
	EXEC SP_executesql @V_SQL,N'@tempo INT OUT' ,@tempo=@RowCount out
	
	DROP TABLE #TEMP_Account1
	
	END 
    ELSE 
	begin 
    SELECT * 
	--- INTO #TEMP_Account11
	 FROM View_CustomerAccountDetails a WHERE 1=0
	 set @RowCount = 0 
	 end 
 END TRY 
 BEGIN CATCH 
  SELECT ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()
 END CATCH 
END