CREATE PROCEDURE [dbo].[Znode_GetZnodeShippingDetails]
(
     @ZipCode VARCHAR(20)
	,@ProfileId int = 0
	,@PortalId  INT = 0
	,@UserId    INT = 0 
	,@ShippingTypeName VARCHAR(200) = ''  
	,@CountryCode NVARCHAR(300) =''
	,@StateCode  Nvarchar(max) = ''
)
AS 
/*

    Summary: Retrive List of shipping 
    Input Parameters:ZipCode
    Unit Testing   
    -- select * from znodecity where statecode = 'sy'
	Exec Znode_GetZnodeShippingDetails  @ZipCode = '440015'	
	Exec Znode_GetZnodeShippingDetails '53202', 3
	Exec Znode_GetZnodeShippingDetails '53202', NULL
	Exec Znode_GetZnodeShippingDetails '', 1,1
	--select * from ZnodeShipping
	exec Znode_GetZnodeShippingDetails '71342', 0,1,-1,'', 'US'
*/
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @CountryCodeInter VARCHAR(200) 
		DECLARE @Tlb_ShippingDetails TABLE
		( 
			[ShippingId] [int] NOT NULL, [ShippingTypeId] [int] NOT NULL, [ShippingCode] [nvarchar](max) NOT NULL, 
			[HandlingCharge] [numeric](12, 6) NOT NULL, [HandlingChargeBasedOn] [varchar](50) NULL, [DestinationCountryCode] [nvarchar](10) NULL, 
			[StateCode] [nvarchar](20) NULL, [CountyFIPS] [nvarchar](50) NULL, [Description] [nvarchar](max) NOT NULL, [IsActive] [bit] NOT NULL, 
			[DisplayOrder] [int] NOT NULL, [ZipCode] [nvarchar](50) NULL,
			[ShippingTypeName] [nvarchar](50) NULL,
			[ClassName] [nvarchar](100) NULL
		);
		
		DECLARE @Tlb_ShippingDetailsResult TABLE
		( 
			[ShippingId] [int] NOT NULL, 
			[ShippingTypeId] [int] NOT NULL,  
			[ShippingCode] [nvarchar](max) NOT NULL,
		    [HandlingCharge] [numeric](12, 6) NOT NULL, 
			[HandlingChargeBasedOn] [varchar](50) NULL, 
			[DestinationCountryCode] [nvarchar](10) NULL, 
			[StateCode] [nvarchar](20) NULL, 
			[CountyFIPS] [nvarchar](50) NULL, 
			[Description] [nvarchar](max) NOT NULL, 
			[IsActive] [bit] NOT NULL, 
			[DisplayOrder] [int] NOT NULL, 
			[ZipCode] [nvarchar](50) NULL,
			[ShippingTypeName] [nvarchar](50) NULL,
			[ClassName] [nvarchar](100) NULL
		);

		DECLARE @ShippingIds VARCHAR(2000) = '' ,@ProfileIds varchar(2000)= ''

		IF ISNULL(@UserId, 0) <> 0 OR 
		   (ISNULL(@PortalId, 0) > 0 AND 
		   ISNULL(@ProfileId, 0) > 0)
		BEGIN
			DECLARE @PortalIds varchar(2000)= '';
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

			EXEC Znode_GetCommonShipping @PortalIds, @ProfileIds, @ShippingIds OUT;
		
			
		END;
		
		DECLARE @ZipCodeLength int, @Attempt int, @Criteria nvarchar(100);
		SET @ZipCodeLength = LEN(@ZipCode);
		SET @Attempt = 1;
		
		
		--SELECT @ShippingIds
		SET @CountryCodeInter =  ISNULL((SELECT TOP 1 CountryCode FROM ZnodeCity WHERE ZIP = @ZipCode ),  @CountryCode)


	--	SELECT @CountryCodeInter

		INSERT INTO @Tlb_ShippingDetails( ShippingId, 
		ShippingTypeId, 
		ShippingCode,
		 HandlingCharge, 
		 HandlingChargeBasedOn, 
		 DestinationCountryCode, 
		 StateCode, 
		 CountyFIPS, 
		 Description,
		  IsActive, 
		  DisplayOrder, 
		  ZipCode,
		 ShippingTypeName,
		 ClassName
		   )
		SELECT ZS.ShippingId, 
			   ZS.ShippingTypeId,  
			   ShippingCode, 
			   HandlingCharge, 
			   HandlingChargeBasedOn, 
			   DestinationCountryCode, 
			   StateCode, 
			   CountyFIPS, 
			   ZS.Description, 
			   ZS.IsActive, 
			   DisplayOrder, 
			   ZipCode,
			   ZST.Name as ShippingTypeName,
			   ZST.ClassName
		FROM ZnodeShipping  ZS 
		LEFT JOIN ZnodeShippingTypes ZST ON (ZS.ShippingTypeId = ZST.ShippingTypeId)
			   WHERE EXISTS (SELECT TOP 1 1 FROM dbo.Split(@ShippingIds,',') SP WHERE ZS.ShippingId = SP.Item) 
			   AND ZST.Name like '%'+ @ShippingTypeName +'%'
	
		INSERT INTO @Tlb_ShippingDetailsResult
		SELECT ShippingId, ShippingTypeId,  ShippingCode, HandlingCharge, HandlingChargeBasedOn, DestinationCountryCode, StateCode, CountyFIPS, Description, IsActive, DisplayOrder, ZipCode, ShippingTypeName,ClassName
		FROM  @Tlb_ShippingDetails  ZS 
		CROSS APPLY dbo.split(ZipCode   ,',') SP
		WHERE     @ZipCode LIKE REPLACE(dbo.FN_TRIM(sp.Item),'*','%')

	--	SELECT @CountryCodeInter

        Select [ShippingId] , 
				[ShippingTypeId] ,  
				[ShippingCode] , 
				[HandlingCharge],
				[HandlingChargeBasedOn] , 
				[DestinationCountryCode] , 
				[StateCode] , 
				[CountyFIPS] , 
				[Description] ,
				[IsActive] ,
				[DisplayOrder] , 
				[ZipCode]  ,
				[ShippingTypeName],
				ClassName
		from @Tlb_ShippingDetailsResult 

		UNION    
		
		Select ZS.[ShippingId] , ZS.[ShippingTypeId] , [ShippingCode] , [HandlingCharge],[HandlingChargeBasedOn] , [DestinationCountryCode] , [StateCode] , [CountyFIPS] , 
			   ZS.[Description] , ZS.[IsActive] , [DisplayOrder] , [ZipCode]   , ZST.[Name] as [ShippingTypeName],ClassName
		from ZnodeShipping ZS 
		LEFT JOIN ZnodeShippingTypes ZST ON (ZS.ShippingTypeId = ZST.ShippingTypeId)
	   -- INNER JOIN ZnodeProfileShipping ZPS ON(ZS.ShippingId = ZPS.ShippingId)
		where ( [DestinationCountryCode] is null  OR [DestinationCountryCode]  = @CountryCodeInter ) 
		and  ZS.ShippingId not in (Select ShippingId from @Tlb_ShippingDetailsResult)
		--and  (ZS.StateCode = @StateCode  OR @StateCode = '')
		and   EXISTS (SELECT TOP 1 1 FROM dbo.Split(@ShippingIds,',') SP WHERE ZS.ShippingId = SP.Item)
		and ZST.Name like '%'+ @ShippingTypeName +'%'

			END TRY
	BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetZnodeShippingDetails @ZipCode = '+@ZipCode+',@ProfileId='+CAST(@ProfileId AS VARCHAR(50))+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetZnodeShippingDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH;
END;