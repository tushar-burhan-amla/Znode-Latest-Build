CREATE PROCEDURE [dbo].[Znode_GetQuoteOrderTemplateDetail]
(   
	@WhereClause NVARCHAR(MAX),
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY    VARCHAR(100)  = '',
	@UserId		 INT,										 
	@RowsCount   INT OUT
)
AS 
/*
		Summary :- this procedure is used to find QuoteOrderTemplate details 
	    SELECT * FROM ZnodeUser  WHERE AspNeTUSerId = 'ae464cfc-95d3-40de-bf71-47993fabb41f'
		SELECT * FROM AspNetUserRoles WHERE RoleID = 'A529A670-F446-45EC-BBCB-C00D64D7C964' Userid = '50fe1032-e810-4606-b522-ebf1559e81cf'
		SELECT * FROM AspNetRoles WHERE ID = '8622E90D-7652-41E7-8563-5DED4CC671DE'

		Unit Testing 
		EXEC Znode_GetQuoteOrderTemplateDetail '',@RowsCount = 0, @Order_BY = '', @UserId = 85
*/
BEGIN
    BEGIN TRY
        SET NOCOUNT ON;
		DECLARE @SQL NVARCHAR(MAX);
		DECLARE @TBL_QuoteOrderTemplate TABLE (OmsTemplateId INT,PortalId INT,UserId INT,TemplateName NVARCHAR(1000),CreatedBy INT,CreatedDate DATETIME
		,ModifiedBy INT,ModifiedDate DATETIME,Items INT,RowId INT,CountNo INT )
		DECLARE @AccountId VARCHAR(2000) ,@UsersId VARCHAR(2000), @ProcessType  varchar(50)='Template'
		-- SELECT * FROM aspnetRoles
			
		--SET @UsersId = SUBSTRING (( SELECT ','+CAST(userId AS VARCHAr(50))  FROM Fn_GetRecurciveUserId(@UserId,@ProcessType) FOR XML PATH ('')),2,4000)
			
		SELECT distinct ZPAVL.AttributeValue as SKU, ZPADV.AttributeDefaultValueCode  
		INTO #SKU_ProductType
		FROM ZnodePimAttributeValue ZPAV
		Inner Join ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		Inner Join ZnodePimAttribute ZPA ON ZPAV.PimAttributeId = ZPA.PimAttributeId
		Inner Join ZnodePimAttributeValue ZPAV1 ON ZPAV.PimProductId = ZPAV1.PimProductId
		Inner Join ZnodePimProductAttributeDefaultValue ZPPADV ON ZPAV1.PimAttributeValueId = ZPPADV.PimAttributeValueId
		Inner Join ZnodePimAttribute ZPA1 ON ZPAV1.PimAttributeId = ZPA1.PimAttributeId
		Inner Join ZnodePimAttributeDefaultValue ZPADV ON ZPPADV.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId
		WHERE ZPA.AttributeCode = 'SKU' AND ZPA1.AttributeCode = 'ProductType'
		AND EXISTS (select * from ZnodeOmsTemplate ZOT
                        INNER JOIN ZnodeOmsTemplateLineItem ZOTL ON(ZOTL.OmsTemplateId = ZOT.OmsTemplateId)
						where ZOT.userid = @UserId AND ZPAVL.AttributeValue = ZOTL.SKU )

			SET @SQL = '
					; WITH CTE_GetOrderTemplate
						AS (
						    SELECT ZOT.OmsTemplateId,ZOT.PortalId,ZOT.UserId,ZOT.TemplateName,ZOT.CreatedBy,ZOT.CreatedDate,ZOT.ModifiedBy,ZOT.ModifiedDate,
							      SUM(case when AttributeDefaultValueCode in ( ''BundleProduct'') AND ParentOmsTemplateLineItemId IS nULL 
											then ZOTL.Quantity
											when AttributeDefaultValueCode in ( ''SimpleProduct'') AND ParentOmsTemplateLineItemId IS nULL 
											then ZOTL.Quantity
											when AttributeDefaultValueCode in ( ''SimpleProduct'',''ConfigurableProduct'',''GroupProduct'') and  ZOOLIRT.Name  in 
											(''Simple'',''Group'',''Configurable'')and   ParentOmsTemplateLineItemId IS NOT NULL 
											then ZOTL.Quantity
											else 0
									end ) Items, ZOT.TemplateType 
							FROM ZnodeOmsTemplate ZOT
                            LEFT JOIN ZnodeOmsTemplateLineItem ZOTL ON(ZOTL.OmsTemplateId = ZOT.OmsTemplateId)
							Left Join #SKU_ProductType SP ON  ZOTL.SKU = SP.SKU
							left join ZnodeOmsOrderLineItemRelationshipType ZOOLIRT ON ZOTL.OrderLineItemRelationshipTypeId = ZOOLIRT.OrderLineItemRelationshipTypeId
							WHERE ZOT.userid = '+cast(@UserId as varchar(10))+'  
							GROUP BY ZOT.OmsTemplateId,ZOT.PortalId,ZOT.UserId,ZOT.TemplateName,ZOT.CreatedBy,ZOT.CreatedDate,ZOT.ModifiedBy,ZOT.ModifiedDate,ZOT.TemplateType 						  
						  
						    )
					, CTE_GetQuoteOrderDetails AS
					(
						SELECT DISTINCT  OmsTemplateId,PortalId,UserId,TemplateName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Items
						,'+dbo.Fn_GetPagingRowId(@Order_BY,'OmsTemplateId DESC,UserId')+',Count(*)Over() CountNo
						FROM CTE_GetOrderTemplate
						WHERE 1=1 
				        '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'					  
						
					)

					SELECT OmsTemplateId,PortalId,UserId,TemplateName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Items,RowId,CountNo
					FROM CTE_GetQuoteOrderDetails
					'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

					Print @sql
					INSERT INTO @TBL_QuoteOrderTemplate (OmsTemplateId,PortalId,UserId,TemplateName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Items,RowId,CountNo)
					EXEC(@SQL)

					SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_QuoteOrderTemplate),0)
   
					SELECT OmsTemplateId,PortalId,UserId,TemplateName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Items
					FROM @TBL_QuoteOrderTemplate 
	END TRY
	BEGIN CATCH
		 
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetQuoteOrderTemplateDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@UserId= '+CAST(@UserId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
        EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetQuoteOrderTemplateDetail',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH
END