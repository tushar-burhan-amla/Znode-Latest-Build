-- SELECT * FROM View_GetCMSCustomerReviewInformation
-- exec Znode_GetCMSWidgetTitleConfiguration @WhereClause='',@RowsCount=null,@Rows = 10,@PageNo=1,@Order_BY = Null


CREATE PROCEDURE [dbo].[Znode_GetCMSWidgetTitleConfiguration](
       @WhereClause VARCHAR(1000) ,
       @Rows        INT           = 1000 ,
       @PageNo      INT           = 0 ,
       @Order_BY    VARCHAR(100)  = NULL ,
       @RowsCount   INT OUT ,
       @LocaleId    INT           = 0)
AS
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @V_SQL NVARCHAR(MAX);
             IF @LocaleId = 0
                 BEGIN
                     SELECT @LocaleId = a.FeatureValues
                     FROM ZnodeGlobalSetting AS a
                     WHERE a.FeatureName = 'Locale';
                 END;
             SET @PageNo = CASE
                               WHEN @PageNo = 0
                               THEN @PageNo
                               ELSE ( @PageNo - 1 ) * @Rows
                           END;
             SET @V_SQL = '  SELECT CMSWidgetTitleConfigurationId ,CMSWidgetsId,MediaPath,Title,Url,PortalId,DisplayName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate 
    INTO #CMSWidgetTitleConfiguration FROM '+' View_GetCMSWidgetTitleConfiguration '+' WHERE 1=1  '+CASE
                                                                                                        WHEN @WhereClause = ''
                                                                                                        THEN ''
                                                                                                        ELSE ' AND '+@WhereClause
                                                                                                    END+' SELECT  @Count=Count(1) FROM  #CMSWidgetTitleConfiguration  SELECT * FROM #CMSWidgetTitleConfiguration '+' Order BY '+ISNULL(CASE
                                                                                                                                                                                                                                           WHEN @Order_BY = ''
                                                                                                                                                                                                                                           THEN NULL
                                                                                                                                                                                                                                           ELSE @Order_BY
                                                                                                                                                                                                                                       END , '1')+' OFFSET '+CAST(@PageNo AS VARCHAR(100))+' ROWS FETCH NEXT '+CAST(@Rows AS VARCHAR(100))+' ROWS ONLY  ';
             PRINT @V_SQL;
             EXEC SP_executesql @V_SQL , N'@Count INT OUT' , @Count = @RowsCount OUT;
         END TRY
         BEGIN CATCH
             SELECT ERROR_LINE() , ERROR_MESSAGE() , ERROR_NUMBER();
         END CATCH;
     END;