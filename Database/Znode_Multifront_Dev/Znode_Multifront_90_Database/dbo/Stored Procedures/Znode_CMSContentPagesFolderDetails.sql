CREATE  Procedure [dbo].[Znode_CMSContentPagesFolderDetails]  
	(  
		 @WhereClause Varchar(1000)     
		,@Rows INT = 1000     
		,@PageNo INT = 0     
		,@Order_BY VARCHAR(100) =  NULL  
		,@RowsCount INT OUT
	)  
AS  
-----------------------------------------------------------------------------
--Summary: To get content page folder details 
--         Provide output for paging with dynamic where cluase                  
--		   User view : View_CMSContentPagesFolderDetails

--Unit Testing   

--Exec Znode_CMSContentPagesFolderDetails
----------------------------------------------------------------------------- 
  
BEGIN    
SET NOCOUNT ON   
BEGIN TRY   
		DECLARE @V_SQL NVARCHAR(MAX)  
		SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END   
		SET @V_SQL =	'SELECT CMSContentPagesId,PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate, ExpirationDate,IsActive,CreatedBy,
						CreatedDate,ModifiedBy,ModifiedDate,StoreName  ,CMSContentPageGroupId ,TemplateName  INTO #CMSContentPagesFolderDetails 
						FROM View_CMSContentPagesFolderDetails where  1=1 '
						+case WHEN @WhereClause IS NOT NULL and @WhereClause <> ''   THEN  ' AND '+@WhereClause ELSE '' END  
						+' SELECT  @Count=Count(1) FROM  #CMSContentPagesFolderDetails  SELECT * FROM #CMSContentPagesFolderDetails '   
						+' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,'1')+ ' OFFSET '
						+ CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '  
		
		EXEC SP_executesql @V_SQL,N'@Count INT OUT' ,@Count=@RowsCount out  
END TRY   
BEGIN CATCH   
	SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()  
END CATCH   
END