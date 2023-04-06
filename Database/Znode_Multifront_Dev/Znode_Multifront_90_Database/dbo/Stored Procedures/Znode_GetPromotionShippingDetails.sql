CREATE PROCEDURE [dbo].[Znode_GetPromotionShippingDetails]
(   @WhereClause NVARCHAR(max),
    @Rows				INT            = 100,
    @PageNo				INT            = 1,
    @Order_BY			VARCHAR(1000)  = '',
    @RowsCount			INT  out,
	@PortalId			INT,
	@IsAssociated		BIT           = 0,
    @PromotionId		INT			  = 0 
)		
AS 
/*
    Summary: This procedure is used to find the ShippingDetails of user for portal 
	Unit Testing: 
	declare @aa int
	EXEC Znode_GetPromotionShippingDetails @WhereClause='Userid = 5 '
	 ,@PortalId ='5',  @RowsCount= 0

     EXEC Znode_GetPromotionShippingDetails @WhereClause='' ,@PortalId ='1',  @RowsCount= 0
*/

     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX)
			 DECLARE @TBL_ShippingDetailsList TABLE  (PortalId int, ShippingId int,ShippingName varchar(200),ShippingCode nvarchar(max), HandlingCharge numeric(28,6),HandlingChargeBasedOn varchar(50),  RowId INT, CountNo INT  )

			IF @PortalId = 0
			BEGIN
			SET @SQL ='
						;WITH CTE_GetShippingDetails AS
						(
						select DISTINCT
							--zps.PortalId,
							zs.ShippingId,zs.ShippingName, zs.ShippingCode,zs.HandlingCharge,zs.HandlingChargeBasedOn
						from ZnodeShipping zs
						inner join ZnodePortalShipping zps on(zs.ShippingId =zps.ShippingId  ) ';
			
			END
			ELSE
			 SET @SQL ='
						;WITH CTE_GetShippingDetails AS
						(
						select DISTINCT
							--zps.PortalId,
							zs.ShippingId,zs.ShippingName, zs.ShippingCode,zs.HandlingCharge,zs.HandlingChargeBasedOn
						from ZnodeShipping zs
						inner join ZnodePortalShipping zps on(zs.ShippingId =zps.ShippingId and zps.PortalId = '+cast(@PortalId as varchar(200))+' )';



			 If @PromotionId > 0 and @IsAssociated = 1
			 begin
				SET @SQL =@SQL +' inner join ZnodePromotionShipping zpsh on(zs.ShippingId = zpsh.ShippingId and zpsh.PromotionId = '+cast(@PromotionId as varchar(200))+' )';
			 End 
			 SET @SQL =@SQL +')
						, CTE_GetShippingDetailsList AS
						(
						SELECT DISTINCT --PortalId,
						ShippingId,ShippingName,ShippingCode,HandlingCharge,HandlingChargeBasedOn,
						'+dbo.Fn_GetPagingRowId(@Order_BY,'ShippingId ASC')+',Count(*)Over() CountNo 
						FROM CTE_GetShippingDetails
						WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'					
						)

						SELECT DISTINCT --PortalId,
						ShippingId,ShippingName,ShippingCode,HandlingCharge,HandlingChargeBasedOn,RowId,CountNo
						FROM CTE_GetShippingDetailsList
						'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			

			print @sql
			INSERT INTO @TBL_ShippingDetailsList (--PortalId,
						ShippingId,ShippingName,ShippingCode,HandlingCharge,HandlingChargeBasedOn,RowId,CountNo)
			EXEC(@SQL)
			SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_ShippingDetailsList ),0)
			
			SELECT --PortalId,
			ShippingId,ShippingName,ShippingCode,HandlingCharge,HandlingChargeBasedOn , @PromotionId as PromotionId 
			FROM @TBL_ShippingDetailsList

	     END TRY
		 BEGIN CATCH
			 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPromotionShippingDetails @WhereClause = '+CAST(@WhereClause AS VARCHAR(MAX))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@PortalId='+cast(@PortalId as varchar(200))+'@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPromotionShippingDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH
     END