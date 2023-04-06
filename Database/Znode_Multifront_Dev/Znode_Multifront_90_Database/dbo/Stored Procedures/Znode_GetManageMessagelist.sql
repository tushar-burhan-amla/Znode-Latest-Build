
Create procedure [dbo].[Znode_GetManageMessagelist]  
(    
  @WhereClause NVARCHAR(Max)       
 ,@Rows INT = 100       
 ,@PageNo INT = 1       
 ,@Order_BY VARCHAR(1000) = ''    
 ,@RowsCount INT OUT    
 ,@LocaleId INT =1    
)    
AS    
/*  
 Summary: Get Managed Message Details list for a PortalId  
 Unit Testing:  
 declare @p7 int  
 set @p7=NULL  
 exec sp_executesql N'Znode_GetManageMessagelist @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId',N'@WhereClause nvarchar(57),@Rows int,@PageNo int,@Order_By nvarchar(17),@RowCount int output,@LocaleId int',@WhereClause=N'localeid = 1 and (Po
rtalId in (''1'',''4'',''5'',''6'',''32'',''33''))',@Rows=50,@PageNo=1,@Order_By=N'PublishStatus asc',@RowCount=@p7 output,@LocaleId=1  
 select @p7   
  
 */  
BEGIN      
  BEGIN TRY     
    SET NOCOUNT ON        
    DECLARE @SQL NVARCHAR(MAX)    
        
 DECLARE @DefaultLocaleId VARCHAR(100)= dbo.Fn_GetDefaultLocaleId();  
    DECLARE @TBL_ManageMessage TABLE (CMSPortalMessageId INT,CMSMessageId INT,[Message] NVARCHAR(max),Location NVARCHAR(100),StoreName NVARCHAR(max)  
        ,LocaleId INT,PortalId INT,CMSMessageKeyId INT,MessageTag NVARCHAR(max),RowId INT,CountNo INT,PublishStatus NVARCHAR(max),IsGlobalContentBlock NVARCHAR(5))  
                
    SET @SQL = '   
     ;With Cte_ManageMessage AS   
     (  
     SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag,  
     StateName as PublishStatus,IsGlobalContentBlock 
     FROM View_Getmanagemessagelist  
     WHERE  LocaleId IN ('+CAST(@LocaleId AS VARCHAR(50))+' , '+CAST(@DefaultLocaleId AS VARCHAR(50))+')   
     )  
  
     ,CTE_ManageMessageLocale As  
     (  
      SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId  
      ,MessageTag,PublishStatus,IsGlobalContentBlock  
      FROM Cte_ManageMessage  
      WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'    
     )  
     ,CTE_ManageMessageBothLocale AS  
     (  
      SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag,PublishStatus,IsGlobalContentBlock  
      FROM CTE_ManageMessageLocale  
      UNION ALL  
      SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag,PublishStatus,IsGlobalContentBlock  
      FROM Cte_ManageMessage cmm  
      WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(50))+'   
      AND NOT EXISTS (SELECT TOP 1 1 FROM CTE_ManageMessageLocale CMML WHERE  CMML.CMSMessageKeyId = cmm.CMSMessageKeyId)  
     )  
  
     ,Cte_ManageMessageFilter AS   
     (  
      SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId  
      ,MessageTag,PublishStatus,IsGlobalContentBlock ,'+dbo.Fn_GetPagingRowId(@Order_BY,'CMSPortalMessageId DESC')+',Count(*)Over() CountNo   
      FROM CTE_ManageMessageBothLocale   
      WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'  
     )  
     
     SELECT CMSPortalMessageId, CMSMessageId, [Message], Location,StoreName,LocaleId,PortalId,CMSMessageKeyId, MessageTag,PublishStatus,IsGlobalContentBlock,RowId,CountNo   
     FROM Cte_ManageMessageFilter  
     '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
  
     print @sql  
  
  
     INSERT INTO @TBL_ManageMessage (CMSPortalMessageId,CMSMessageId,[Message],Location,StoreName,LocaleId,PortalId,CMSMessageKeyId,MessageTag,PublishStatus,IsGlobalContentBlock,RowId,CountNo)  
     EXEC (@SQL)  
     SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ManageMessage ),0)  
	
     SELECT CMSPortalMessageId,CMSMessageId,[Message],Location,LocaleId,CASE WHEN PortalID IS NULL THEN 'All Store' ELSE StoreName END AS StoreName,
	 PortalId,CMSMessageKeyId,MessageTag,PublishStatus,IsGlobalContentBlock  
     FROM @TBL_ManageMessage  
      
  
  
   END TRY       
   BEGIN CATCH          
          DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),   
    @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetManageMessagelist @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS  
   VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''');  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'Znode_GetManageMessagelist',  
    @ErrorInProcedure = 'Znode_GetManageMessagelist',  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;    
   END CATCH       
END
