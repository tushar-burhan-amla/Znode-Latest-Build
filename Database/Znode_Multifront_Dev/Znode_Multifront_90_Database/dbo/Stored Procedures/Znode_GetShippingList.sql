CREATE PROCEDURE [dbo].[Znode_GetShippingList]
(
	   @WhereClause nvarchar(max)= ''
	   , @Rows int= 100
	   , @PageNo int= 1
	   , @Order_BY varchar(1000)= ' DisplayOrder ASC'
	   , @RowsCount int OUT
	   , @ProfileId int= 0
	   , @PortalId int= 0
	   , @UserId int= 0
	   , @IsAssociated int= 0
)
AS
/*
  Summary: This Procedure is used to get shipping details According to associated profile.

  Unit Testing:

   DECLARE @profileid int = 0
	EXEC [Znode_GetShippingList]  @WhereClause ='' ,  @ProfileId = 0,@PortalId=1   ,@IsAssociated=0, @RowsCount = @profileid OUT ,@UserId = 4   SELECT @profileid

*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @SQL nvarchar(max), @FilterWhereClause varchar(2000)= '', @InternalOrderBy varchar(2000)= '';
		SET @Order_By = CASE WHEN @Order_By = '' THEN ' DisplayOrder ASC' ELSE @Order_By END 
		DECLARE @TBL_ProfileShipping TABLE
		(
			ProfileId int, ProfileName nvarchar(200), PortalId int, StoreName nvarchar(200), ShippingId int, ShippingTypeId int
			, ShippingCode nvarchar(max), HandlingCharge numeric(28,6), HandlingChargeBasedOn varchar(50), DestinationCountryCode nvarchar(50)
			, StateCode nvarchar(40), CountyFIPS nvarchar(100), Description nvarchar(max), IsActive bit, DisplayOrder int, ZipCode nvarchar(max)
			, CreatedDate datetime, ModifiedDate datetime, ShippingTypeName nvarchar(max), PortalShippingId int, ProfileShippingId int
			, ShippingName varchar(200), ClassName varchar(100), DeliveryTimeframe varchar(MAX), RowId int, CountNo int
			, PublishState varchar(600)
		);
		IF ISNULL(@UserId, 0) <> 0 OR
		   (ISNULL(@PortalId, 0) > 0 AND
		   ISNULL(@ProfileId, 0) > 0)
		BEGIN
			DECLARE @PortalIds varchar(2000)= '', @ProfileIds varchar(2000)= '', @ShippingIds varchar(2000)= '';
			IF ISNULL(@UserId, 0) <> 0
			BEGIN
			    SET @PortalIds = @PortalId
				EXEC Znode_GetUserPortalAndProfile @UserId, @PortalIds OUT, @ProfileIds OUT;
			END;
			ELSE
			BEGIN
				SET @PortalIds = @PortalId;
				SET @ProfileIds = @ProfileId;
			END;

			SET @ProfileIds = CASE WHEN CAST(@ProfileId AS VARCHAR(200))  <= '0' AND NOT EXISTS (SELECT TOP 1 1  FROM ZnodeProfileShipping WHERE ProfileId = @ProfileId) THEN @ProfileIds ELSE CAST(@ProfileId AS VARCHAR(200)) END 
			EXEC Znode_GetCommonShipping @PortalIds, @ProfileIds, @ShippingIds OUT;

			SET @FilterWhereClause = '  WHERE ShippingId IN ( '+ISNULL(@ShippingIds,'0') +' ) ';
		END;
		IF ISNULL(@ProfileId, 0) > 0 AND
		   ISNULL(@UserId, 0) = 0 AND
		   ISNULL(@PortalId, 0) = 0
		BEGIN
			SET @SQL = '
			   ;WITH CTE_GetAssociatedShipping as
			  (
				SELECT  ZP.ProfileId, ZP.ProfileName,NULL PortalId, NULL StoreName ,ZS.ShippingId,ZS.ShippingTypeId,ZS.ShippingCode,ZS.HandlingCharge,ZS.HandlingChargeBasedOn
			    ,ZS.DestinationCountryCode,ZS.StateCode,ZS.CountyFIPS,ZS.Description,ZS.IsActive,CASE WHEN '+CAST(@ProfileId AS VARCHAR(200))+' >= 0 AND '+CAST(@IsAssociated AS VARCHAR(200))+' = 1 THEN ISNULL(ZPS.DisplayOrder,ZS.DisplayOrder) ELSE ZS.DisplayOrder END DisplayOrder,ZS.ZipCode,ZS.CreatedDate,ZS.ModifiedDate
				,CASE WHEN ZPS.ShippingId IS NULL THEN 0 ELSE 1 END IsAssociated,ZST.Name ShippingTypeName , ZPS.ProfileShippingId 
				, NULL PortalShippingId,ZS.ShippingName,ZST.ClassName,ZS.DeliveryTimeframe,CASE WHEN '+CAST(@ProfileId AS VARCHAR(200))+' >= 0 AND '+CAST(@IsAssociated AS VARCHAR(200))+' = 1 THEN ISNULL(ZPPOS.DisplayName,''Production'' )     ELSE ''Production'' END  PublishState

				FROM ZnodeShipping ZS
				INNER JOIN ZnodeShippingTypes ZST ON (ZST.ShippingTypeId = ZS.ShippingTypeId)
				CROSS APPLY ZnodeProfile ZP
				LEFT JOIN ZnodeProfileShipping ZPS ON(ZP.ProfileId = ZPS.ProfileId AND ZS.ShippingId = ZPS.ShippingId)
				LEFT JOIN ZnodePublishState ZPPOS ON (ZPPOS.PublishStateId = ZPS.PublishstateId )
			  )';
			SET @FilterWhereClause = 'WHERE ProfileId = '+CAST(@ProfileId AS varchar(50))+' AND IsAssociated = '+CAST(@IsAssociated AS varchar(50));
			SET @InternalOrderBy = ' ProfileId,ShippingId ';
		END;
		ELSE
		BEGIN
			IF ISNULL(@PortalId, 0) > 0 AND
			   ISNULL(@UserId, 0) = 0 AND
			   ISNULL(@ProfileId, 0) = 0
			BEGIN
				SET @SQL = '
			   ;WITH CTE_GetAssociatedShipping as
			  (
				SELECT NULL ProfileId,NULL ProfileName ,ZPP.PortalId, ZPP.StoreName,ZS.ShippingId,ZS.ShippingTypeId,ZS.ShippingCode,ZS.HandlingCharge,ZS.HandlingChargeBasedOn
			    ,ZS.DestinationCountryCode,ZS.StateCode,ZS.CountyFIPS,ZS.Description,ZS.IsActive,ZS.DisplayOrder,ZS.ZipCode,ZS.CreatedDate,ZS.ModifiedDate
				,CASE WHEN ZPS.PortalShippingId IS NULL THEN 0 ELSE 1 END IsAssociated,ZST.Name ShippingTypeName ,ZPS.PortalShippingId, NULL ProfileShippingId,ZS.ShippingName,ZST.ClassName,ZS.DeliveryTimeframe
				,ISNULL(ZPSS.DisplayName,''Production'' ) PublishState
				FROM ZnodeShipping ZS
				INNER JOIN ZnodeShippingTypes ZST ON (ZST.ShippingTypeId = ZS.ShippingTypeId)
				CROSS APPLY ZnodePortal ZPP
				LEFT JOIN ZnodePortalShipping ZPS ON(ZPP.PortalId = ZPS.PortalId AND ZS.ShippingId = ZPS.ShippingId)
				LEFT JOIN ZnodePublishState ZPSS ON (ZPS.PublishStateId =  ZPSS.PublishStateId )
			  )';
				SET @FilterWhereClause = 'WHERE PortalId = '+CAST(@PortalId AS varchar(50))+' AND IsAssociated = '+CAST(@IsAssociated AS varchar(50));
				SET @InternalOrderBy = ' PortalId ,ShippingId ';
			END;
			ELSE
			BEGIN			
				SET @SQL = '
			   ;With  CTE_GetAssociatedShipping as
			   (
			   SELECT NULL ProfileId, NULL ProfileName,NULL PortalId, NULL StoreName,ZS.ShippingId,ZS.ShippingTypeId,ZS.ShippingCode,ZS.HandlingCharge,ZS.HandlingChargeBasedOn
			   ,ZS.DestinationCountryCode,ZS.StateCode,ZS.CountyFIPS,ZS.Description,ZS.IsActive,ISNULL(ZPP.DisplayOrder, ZS.DisplayOrder  ) DisplayOrder,ZS.ZipCode,ZS.CreatedDate,ZS.ModifiedDate
			   , NULL IsAssociated ,ZST.Name ShippingTypeName , NULL PortalShippingId, NULL ProfileShippingId,ZS.ShippingName,ZST.ClassName,ZS.DeliveryTimeframe
			   ,ISNULL(ZPSS.DisplayName,''Production'' ) PublishState
			   FROM ZnodeShipping ZS
			   INNER JOIN ZnodeShippingTypes ZST ON (ZST.ShippingTypeId = ZS.ShippingTypeId)
			   LEFT JOIN ZnodeProfileShipping ZPP ON (ZPP.ShippingId = ZS.shippingId AND ZPP.Profileid = '+CAST(@ProfileId AS NVARCHAR(200))+')
			   LEFT JOIN ZnodePortalShipping ZPS ON( '+CAST(@PortalId AS VARCHAR(200))+'= ZPS.PortalId AND ZS.ShippingId = ZPS.ShippingId)
			   LEFT JOIN ZnodePublishState ZPSS ON (CASE WHEN '+CAST(@ProfileId AS VARCHAR(200))+' >= 0 AND EXISTS (SELECT TOP 1 1  FROM ZnodeProfileShipping TUI  WHERE TUI.Profileid = '+CAST(@ProfileId AS NVARCHAR(200))+' )  THEN ZPP.PublishSTateID ELSE ZPS.PublishSTateID  END =  ZPSS.PublishStateId ) 
			     )
			   ';
				SET @FilterWhereClause = CASE
										 WHEN ISNULL(@FilterWhereClause,'') = '' THEN ' WHERE 1=1 '
										 ELSE @FilterWhereClause
										 END;
				SET @InternalOrderBy = ' ShippingId ';
				
			END;
		END;

		--SELECT @InternalOrderBy
		SET @SQL = @SQL+'
	           , CTE_GetShipping AS
			 (
				SELECT	ProfileId,ProfileName,PortalId,StoreName,ShippingId,ShippingTypeId,ShippingCode,HandlingCharge,HandlingChargeBasedOn
			   ,DestinationCountryCode,StateCode,CountyFIPS,Description,IsActive,DisplayOrder,ZipCode,CreatedDate,ModifiedDate,ShippingTypeName , PortalShippingId, ProfileShippingId,ShippingName,ClassName,DeliveryTimeframe,PublishState
					,'+dbo.Fn_GetPagingRowId( @Order_BY, @InternalOrderBy )+',Count(*)Over() CountNo
				FROM CTE_GetAssociatedShipping
				'+ISNULL(@FilterWhereClause,'')+'
				'+dbo.Fn_GetFilterWhereClause( @WhereClause )+'
			 )

	      SELECT ProfileId,ProfileName,PortalId,StoreName,ShippingId,ShippingTypeId,ShippingCode,HandlingCharge,HandlingChargeBasedOn
					 ,DestinationCountryCode,StateCode,CountyFIPS,Description,IsActive,DisplayOrder,ZipCode,CreatedDate,ModifiedDate
					 ,ShippingTypeName,PortalShippingId,ProfileShippingId,ShippingName,ClassName,DeliveryTimeframe,PublishState,RowId,CountNo
		  FROM CTE_GetShipping
		  '+dbo.Fn_GetPaginationWhereClause( @PageNo, @Rows );

		PRINT @SQL 
		INSERT INTO @TBL_ProfileShipping( ProfileId, ProfileName, PortalId, StoreName, ShippingId, ShippingTypeId, ShippingCode, HandlingCharge, HandlingChargeBasedOn, DestinationCountryCode, StateCode, CountyFIPS, Description, IsActive, DisplayOrder, ZipCode, CreatedDate, ModifiedDate, ShippingTypeName, PortalShippingId, ProfileShippingId, ShippingName, ClassName, DeliveryTimeframe, PublishState,RowId, CountNo )
		EXEC (@SQL);

		SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_ProfileShipping), 0);
		SELECT ShippingId,ProfileId, ProfileName, PortalId, StoreName,  ShippingTypeId, ShippingCode, HandlingCharge, HandlingChargeBasedOn, DestinationCountryCode, StateCode, CountyFIPS, Description, IsActive, DisplayOrder, ZipCode, CreatedDate, ModifiedDate, ShippingTypeName, PortalShippingId, ProfileShippingId, ShippingName, ClassName, DeliveryTimeframe,PublishState
		FROM @TBL_ProfileShipping;

	END TRY
	BEGIN CATCH
		    DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetShippingList @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@ProfileId='+CAST(@ProfileId AS VARCHAR(50))+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));

            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetShippingList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END;