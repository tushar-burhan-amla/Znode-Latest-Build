CREATE PROCEDURE [dbo].[Znode_GetGlobalAttributes]  
(   @WhereClause         VARCHAR(MAX)  = '',  
    @Rows                INT           = 100,  
    @PageNo              INT           = 1,  
    @Order_BY            VARCHAR(1000) = '',  
    @RowsCount           INT OUT,  
    @LocaleId            INT           = 0,  
    @GlobalAttributeId      VARCHAR(MAX)  = '',  
    @IsReturnAllCoulumns BIT           = 0)  
AS  
/*  
     Summary :- This Procedure is used to get the attribute details with the attribute name locale wise   
    Result is fetched order by GlobalAttributeId in descending order  
     Unit Testing   
    
     EXEC [Znode_GetGlobalAttributes] '',10,1,'',0,1,'54,53,56',1  
   
*/  
     BEGIN  
         SET NOCOUNT ON;  
     
         BEGIN TRY  
             DECLARE @SQL NVARCHAR(MAX);  
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetdefaultLocaleId();  
              
             DECLARE @TBL_AttributeDefault TABLE  
             (GlobalAttributeId       INT,  
              AttributeTypeId      INT,  
              AttributeCode        VARCHAR(600),  
              IsRequired           BIT,  
              IsLocalizable        BIT,  
              DisplayOrder         INT,  
              HelpDescription      VARCHAR(MAX),    
              CreatedDate          DATETIME,  
              ModifiedDate         DATETIME,  
              AttributeName        NVARCHAR(MAX),  
              AttributeTypeName    VARCHAR(300),  
			  GlobalEntityId	   INT,
			  EntityName		   NVARCHAR(300), 
              RowId                INT,  
              CountId              INT  
             );  
             IF @GlobalAttributeId <> ''  
                 BEGIN  
                     SET @WhereClause = CASE  WHEN @WhereClause = '' THEN '' ELSE ' AND ' END+' EXISTS (SELECT TOP 1  1  FROM dbo.Split('''+@GlobalAttributeId+''','','') SP WHERE SP.Item = CTPADV.GlobalAttributeId )';                                      
                                                                                                                                      
                 END;  
             SET @SQL = '  
       ;With Cte_GlobalAttribute AS   
    (  
     SELECT ZPA.GlobalAttributeId,ZPA.AttributeTypeId,ZPA.AttributeCode,ZPA.IsRequired,ZPA.IsLocalizable,  
     ZPA.DisplayOrder,ZPA.HelpDescription  
     ,ZPA.CreatedBy,ZPA.CreatedDate,ZPA.ModifiedBy,ZPA.ModifiedDate,ZPAL.AttributeName,ZAT.AttributeTypeName , ZPAL.LocaleId,ZPA.GlobalEntityId,ZGE.EntityName   
     FROM ZnodeGlobalAttribute ZPA   
     INNER JOIN ZnodeGlobalAttributeLocale ZPAL ON (ZPAL.GlobalAttributeId = ZPA.GlobalAttributeId)  
     INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)  
	 INNER JOIN ZnodeGlobalEntity ZGE ON (ZPA.GlobalEntityId = ZGE.GlobalEntityId)
       
    )  
    , Cte_GlobalAttributeFirstLocale AS   
    (  
     SELECT GlobalAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable  
     ,DisplayOrder,HelpDescription  
     ,AttributeName ,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId,GlobalEntityId,EntityName
     FROM Cte_GlobalAttribute CTA   
     WHERE LocaleId = '+CAST(@localeId AS VARCHAR(20))+'  
    )  
    , Cte_GlobalAttributeDefaultLocale AS   
    (  
        SELECT GlobalAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,DisplayOrder,HelpDescription  
        ,AttributeName ,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId,GlobalEntityId,EntityName  
        FROM Cte_GlobalAttributeFirstLocale   
        UNION ALL   
        SELECT GlobalAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,DisplayOrder,HelpDescription  
     ,AttributeName ,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId,GlobalEntityId,EntityName  
        FROM Cte_GlobalAttribute CTA  
        WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(20))+'  
        AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GlobalAttributeFirstLocale CTAFL WHERE CTAFL.GlobalAttributeId = CTA.GlobalAttributeId)     
     )  
     ,Cte_GlobalAttributeFilter AS   
     (  
        SELECT GlobalAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,DisplayOrder,HelpDescription,CreatedDate,ModifiedDate  
     ,AttributeName ,AttributeTypeName,LocaleId,GlobalEntityId,EntityName, '+[dbo].[Fn_GetPagingRowId](@Order_BY, ' GlobalAttributeId DESC')+' , Count(*)Over() CountId  
        FROM Cte_GlobalAttributeDefaultLocale  CTPADV   
        WHERE 1=1   
        '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'  
     )  
      
        SELECT GlobalAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,DisplayOrder,HelpDescription,CreatedDate,ModifiedDate  
     ,AttributeName ,AttributeTypeName,GlobalEntityId,EntityName ,RowId ,CountId   
        FROM Cte_GlobalAttributeFilter CTAF   
        '+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows)+'  
        '+[dbo].[Fn_GetOrderByClause](@Order_BY, 'GlobalAttributeId DESC')+'  
        ';  
              INSERT INTO @TBL_AttributeDefault  
     (GlobalAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,DisplayOrder,HelpDescription,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName,GlobalEntityId,EntityName,RowId,CountId)  
     EXEC (@SQL);  
     IF @IsReturnAllCoulumns = 0  
                 BEGIN  
                     SELECT GlobalAttributeId,AttributeCode,AttributeName,AttributeTypeName,IsRequired,IsLocalizable,GlobalEntityId,EntityName,CreatedDate  
                     FROM @TBL_AttributeDefault;  
                 END;  
                 ELSE  
                 BEGIN  
                     SELECT GlobalAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable  
      ,DisplayOrder,HelpDescription,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName,GlobalEntityId,EntityName  
                     FROM @TBL_AttributeDefault;  
                 END;  
                 SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_AttributeDefault), 0);  
    
         END TRY  
         BEGIN CATCH  
  select  error_message()  
  DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetGlobalAttributes @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@GlobalAttributeId='+@GlobalAttributeId+',@IsReturnAllCoulumns='+CAST(@IsReturnAllCoulumns AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'Znode_GetGlobalAttributes',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
              
         END CATCH;  
     END;  