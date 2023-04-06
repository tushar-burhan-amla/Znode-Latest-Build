  -- SELECT * FROM View_ManageProductList
 
CREATE    Procedure [dbo].[Znode_CMSWidgetsConfigurationList]  
  
(  
	 @WhereClause Varchar(1000)     
	,@Rows INT = 1000     
	,@PageNo INT = 0     
	,@Order_BY VARCHAR(100) =  NULL  
	,@RowsCount INT OUT
)  
AS  
-----------------------------------------------------------------------------
--Summary: To get list of CMS widgets configuration 
--		   Use tablevarible @CMSWidgetsConfigurationList to sort data 	                  
--		   User view : View_CMSWidgetsConfigurationList
--
--Unit Testing   

 -- exec Znode_CMSWidgetsConfigurationList @WhereClause='',@RowsCount=0,@Rows = 100,@PageNo=0,@Order_BY = Null
----------------------------------------------------------------------------- 
  

BEGIN    
SET NOCOUNT ON
BEGIN TRY   
   DECLARE  @V_SQL NVARCHAR(MAX)
   SET @V_SQL = 'DECLARE @CMSWidgetsConfigurationList  TABLE 
								(
									CMSWidgetTitleConfigurationId	int
									,CMSMappingId					int
									,cmsWidgetsId					int
									,WidgetsKey						nvarchar (256)
									,TypeOFMapping					nvarchar (100)
									,Title							nvarchar (600)
									,Url							nvarchar(Max)
									,MediaId						INT 
									,Image							varchar (300)
								)
    INSERT INTO @CMSWidgetsConfigurationList 
    SELECT CMSWidgetTitleConfigurationId, CMSMappingId, cmsWidgetsId, WidgetsKey , TypeOFMapping ,Title,Url,MediaId,
	[dbo].[FN_GetMediaThumbnailMediaPath]( [Image]) FROM View_CMSWidgetsConfigurationList 
    SELECT @Count = COUNT (1) FROM @CMSWidgetsConfigurationList

    SELECT * FROM @CMSWidgetsConfigurationList WHERE '+ CASE WHEN @WhereClause = '' THEN '1=1' ELSE @WhereClause END 
    +' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,'1')
   
   EXEC SP_executesql @V_SQL,N'@Count INT OUT' ,@Count=@RowsCount out  
END TRY   
BEGIN CATCH   
	SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()  
END CATCH   
END