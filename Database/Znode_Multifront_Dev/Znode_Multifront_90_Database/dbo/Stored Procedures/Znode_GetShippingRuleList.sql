CREATE PROCEDURE [dbo].[Znode_GetShippingRuleList](
	  @PortalId int= 0
	  ,@ProfileId int= 0
	  ,@UserId int= 0
	  ,@ShippingId int= 0
	  ,@CountyCode nvarchar(20)= '')
AS

/*
     Summary:- This procedure is used to get the  shipping rule detials for checkout pages 
	 Unit Testing 
	 exec sp_executesql N'Znode_GetShippingRuleList @PortalId, 0,@UserId, @ShippingId,@CountyCode',N'@PortalId int,@ProfileId int,@ShippingId int,@CountyCode nvarchar(2),@UserId int',@PortalId=1,@ProfileId=0,@ShippingId=2,@CountyCode=N'US',@UserId=-1
 */

BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @TBL_ShippingInfo TABLE
		( 
		ShippingId int
		);
		IF ISNULL(@UserId, 0) <> 0 OR 
		   ISNULL(@PortalId, 0) > 0 AND 
		   ISNULL(@ProfileId, 0) > 0
		BEGIN
			DECLARE @PortalIds varchar(2000)= '', @ProfileIds varchar(2000)= '', @ShippingIds varchar(2000);
			IF ISNULL(@UserId, 0)<> 0
			BEGIN
			   SET @PortalIds = @PortalId
			   EXEC Znode_GetUserPortalAndProfile @UserId, @PortalIds OUT, @ProfileIds OUT;
			END;
			ELSE
			BEGIN
				SET @PortalIds = @PortalId;
				SET @ProfileIds = @ProfileId;
			END;
			EXEC Znode_GetCommonShipping 1, @ProfileIds, @ShippingIds OUT;
	
			INSERT INTO @TBL_ShippingInfo( ShippingId )
				   SELECT Item
				   FROM dbo.Split( @ShippingIds, ',' ) AS SP;
		END;

		SELECT ShippingRuleId,ShippingRuleId,ShippingId,ShippingRuleTypeCode,ClassName,LowerLimit,UpperLimit,BaseCost,PerItemCost,PerItemCost,Custom1
        Custom2,Custom3,ExternalId,CreatedBy,CreatedBy,CreatedDate,ModifiedBy, ModifiedDate FROM ZnodeShippingRule AS ZSR
		
		WHERE EXISTS
		(
			SELECT TOP 1 1
			FROM @TBL_ShippingInfo AS TBSI
			WHERE TBSI.ShippingId = ZSR.ShippingId
		) AND  ZSR.ShippingId = @ShippingId;
		
	END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetShippingRuleList @PortalId = '+cast (@PortalId AS VARCHAR(50))+',@ProfileId='+CAST(@ProfileId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@CountyCode='+@CountyCode+',@ShippingId='+CAST(@ShippingId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetShippingRuleList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END;