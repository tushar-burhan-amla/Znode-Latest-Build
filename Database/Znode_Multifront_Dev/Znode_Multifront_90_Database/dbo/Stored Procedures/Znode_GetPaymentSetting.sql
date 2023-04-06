CREATE PROCEDURE [dbo].[Znode_GetPaymentSetting]
(
  @WhereClause  NVARCHAR(Max)  = '',
  @Rows         INT            = 100,
  @PageNo       INT            = 1,
  @Order_BY     VARCHAR(1000)  = ' DisplayOrder ASC',
  @RowsCount    INT OUT            ,
  @PortalId     INT            = 0 ,
  @ProfileId    INT            = 0 ,
  @UserId       INT			   = 0 ,
  @IsAssociated INT            = 0
)
AS
 /*
   Summary :- This procedure is used to get the associated and Unassociated list of paymentsetting for portal and profile

   Unit Testing

   DECLARE @profilei int = 0
   EXEC Znode_GetPaymentSetting @WhereClause = '', @RowsCount =  @profilei OUT ,@PortalId = 0 ,@ProfileId= 0 ,@IsAssociated = 0 ,@UserId= 0  SELECT  @profilei

 */
 BEGIN
  BEGIN TRY
   SET NOCOUNT ON
     DECLARE @SQL NVARCHAR(MAX)= '',@FilterWhereClause VARCHAR(2000) = '' ,@InternalOrderBy VARCHAR(2000)= ''

	 DECLARE @TBL_PaymentSetting TABLE (PaymentSettingId INT, PaymentApplicationSettingId INT,PaymentTypeId INT,PaymentGatewayId INT
										,PaymentName VARCHAR(600),IsActive BIT,DisplayOrder INT,IsTestMode BIT,IsPoDocUploadEnable BIT
										,IsPoDocRequire BIT,CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME
										,PortalId INT,StoreName NVARCHAR(max),IsAssociated BIT,ProfileId int,ProfileName NVARCHAR(200),
										PaymentTypeName VARCHAr(500),GatewayName VARCHAR(300),RowId INT,CountNo INT,PaymentDisplayName nvarchar(1200),
										PaymentExternalId VARCHAR(100),IsApprovalRequired BIT,PaymentCode VARCHAR(200),GatewayCode VARCHAR(200),
										IsCallToPaymentAPI BIT,IsBillingAddressOptional Bit,IsOABRequired BIT,PortalPaymentGroupId INT,PublishState VARCHAR(50),IsUsedForWebStorePayment BIT )


     IF ISNULL(@UserId,0) <> 0  OR (ISNULL(@PortalId,0) > 0 AND ISNULL(@ProfileId,0) > 0)
	 BEGIN
	  DECLARE  @PortalIds VARCHAR(2000) = '' ,@ProfileIds VARCHAR(2000) = '' ,@PaymentSettingIds VARCHAR(2000)
	  IF ISNULL(@UserId,0) <> 0
	  BEGIN
	  
	  SET @PortalIds = @PortalId
	  EXEC Znode_GetUserPortalAndProfile @UserId ,@PortalIds OUT,@ProfileIds OUT
	  
	  END
	  ELSE
	  BEGIN
	 
	   SET @PortalIds = @PortalId
	   SET @ProfileIds = @ProfileId

	  END
	  SET @ProfileIds = CASE WHEN CAST(@ProfileId AS VARCHAr(200))  <= '0' THEN @ProfileIds ELSE CAST(@ProfileId AS VARCHAr(200)) END 
	  EXEC Znode_GetCommonPaymentSetting @PortalIds,@ProfileIds,@PaymentSettingIds OUT

	  SET @FilterWhereClause = ' AND  PaymentSettingId IN ('+ISNULL(@PaymentSettingIds,'0')+') '

	 END

	 SELECT PaymentSettingId ,ZPPG.PortalPaymentGroupId,ZPA.PortalId 
	 INTO #ZnodePortalPaymentApprovers
	 FROM ZnodePortalPaymentApprovers ZPPA 
	 INNER JOIN [ZnodePortalPaymentGroup] ZPPG ON( ZPPA.PortalPaymentGroupId = ZPPG.PortalPaymentGroupId)  
     INNER JOIN [ZnodePortalApproval] ZPA ON (ZPA.PortalApprovalId = ZPPG.PortalApprovalId) 
	 WHERE  ZPA.EnableApprovalManagement =1 AND ZPA.PortalId = @PortalId AND ZPPG.isActive = 1 
	 



	IF ISNULL(@PortalId,0) > 0 AND (   ISNULL(@ProfileId,0) = 0)
	BEGIN
	
	 SET @SQL = '
	             ;With Cte_PaymentSetting AS
				 (
				 SELECT  ZPS.PaymentSettingId,ZPS.PaymentApplicationSettingId,ZPS.PaymentTypeId,ZPS.PaymentGatewayId,ZPS.PaymentName,ZPS.IsActive
						,ZPS.DisplayOrder,ZPS.IsTestMode,ZPS.IsPoDocUploadEnable,ZPS.IsPoDocRequire,ZPS.CreatedBy,ZPS.CreatedDate,ZPS.ModifiedBy,ZPS.ModifiedDate
						,ZP.PortalId,ZP.StoreName, 0 as IsAssociated, NULL ProfileId, NULL ProfileName,ZPT.BehaviorType PaymentTypeName,ZPG.GatewayName
						, CASE WHEN ZPPS.PaymentDisplayName IS NULL OR ZPPS.PaymentDisplayName = ''''  THEN ZPS.PaymentDisplayName ELSE ZPPS.PaymentDisplayName END   PaymentDisplayName,  ZPPS.PaymentExternalId ,
						 CASE WHEN YU.PaymentSettingId IS NOT NULL  THEN 1 ELSE 0 END AS IsApprovalRequired , ZPS.PaymentCode, ZPG.GatewayCode,ZPT.IsCallToPaymentAPI, ZPS.IsBillingAddressOptional,ISNULL(ZPS.IsOABRequired,0) IsOABRequired
						 ,YU.PortalPaymentGroupId, ZPPOS.DisplayName PublishState , ZPPS.IsUsedForWebStorePayment
				 FROM ZnodePaymentSetting ZPS
				 INNER JOIN ZnodePaymentType  ZPT ON (ZPT.PaymentTypeId = ZPS.PaymentTypeId)
				 LEFT JOIN ZnodePaymentGateway ZPG ON (ZPG.PaymentGatewayId= ZPS.PaymentGatewayId)
				 LEFT JOIN #ZnodePortalPaymentApprovers YU ON (YU.PaymentSettingId = ZPS.PaymentSettingId)
				 CROSS APPLY ZnodePortal ZP
				 LEFT JOIN ZnodePortalPaymentSetting ZPPS on ( ZPPS.PortalId = ZP.PortalId AND ZPPS.PaymentSettingId = ZPS.PaymentSettingId)
				 LEFT JOIN ZnodePublishState ZPPOS ON (ZPPOS.PublishStateId = ZPPS.PublishStateId )
				 WHERE  ISNULL(ZPPS.IsUsedForWebStorePayment,0) =  CASE WHEN '+CAST(@IsAssociated AS VARCHAR(10)) +' = 1 THEN  1  ELSE 0 END
				 )

				 '
	 --IF @userId <> 0 
	 --BEGIN 
		--SET @IsAssociated = 1  	
	 --END 
	 
	 SET @FilterWhereClause = ' WHERE PortalId = '+CAST(@PortalId AS VARCHAR(50))
	 SET @InternalOrderBy = ' PaymentSettingId,PortalId '
    END
	ELSE IF ISNULL(@ProfileId,0) > 0 AND ( ISNULL(@UserId,0) = 0 AND  ISNULL(@PortalId,0) = 0)
	BEGIN
	  SET @SQL = '
	            ;With Cte_PaymenTSetting AS
				 (
				 SELECT  ZPS.PaymentSettingId,ZPS.PaymentApplicationSettingId,ZPS.PaymentTypeId,ZPS.PaymentGatewayId,ZPS.PaymentName,ZPS.IsActive
						,CASE WHEN '+CAST(@ProfileId AS VARCHAR(200))+' >= 0 AND '+CAST(@IsAssociated AS VARCHAR(200))+' = 1 THEN ISNULL(ZPPS.DisplayOrder,ZPS.DisplayOrder) ELSE ZPS.DisplayOrder END AS DisplayOrder,ZPS.IsTestMode,ZPS.IsPoDocUploadEnable,ZPS.IsPoDocRequire,ZPS.CreatedBy,ZPS.CreatedDate,ZPS.ModifiedBy,ZPS.ModifiedDate
						,NULL PortalId,NULL StoreName, CASE WHEN ZPPS.ProfilePaymentSettingId IS NULL THEN 0 ELSE 1 END IsAssociated ,ZP.ProfileId,ZP.ProfileName,ZPT.BehaviorType PaymentTypeName,ZPG.GatewayName
						,ZPS.PaymentDisplayName	, NULL PaymentExternalId,
						 CASE WHEN YU.PaymentSettingId IS NOT NULL  THEN 1 ELSE 0 END AS IsApprovalRequired  , ZPS.PaymentCode, ZPG.GatewayCode,ZPT.IsCallToPaymentAPI, ZPS.IsBillingAddressOptional,0 IsOABRequired,YU.PortalPaymentGroupId
						 , ISNULL(ZPPOS.DisplayName,''Production'' )  PublishState , 0 AS IsUsedForWebStorePayment
				 FROM ZnodePaymentSetting ZPS
				 INNER JOIN ZnodePaymentType  ZPT ON (ZPT.PaymentTypeId = ZPS.PaymentTypeId)
				 LEFT JOIN ZnodePaymentGateway ZPG ON (ZPG.PaymentGatewayId= ZPS.PaymentGatewayId)
				 LEFT JOIN #ZnodePortalPaymentApprovers YU ON (YU.PaymentSettingId = ZPS.PaymentSettingId)
				 CROSS APPLY ZnodeProfile ZP
				 LEFT JOIN ZnodeProfilePaymentSetting ZPPS on ( ZPPS.ProfileId = ZP.ProfileId AND ZPPS.PaymentSettingId = ZPS.PaymentSettingId)
				 LEFT JOIN ZnodePublishState ZPPOS ON (ZPPOS.PublishStateId = ZPPS.PublishStateId)
				 )
               '
	   SET @FilterWhereClause = ' WHERE ProfileId = '+CAST(@ProfileId AS VARCHAR(50))+'
									AND  IsAssociated = '+CAST(@IsAssociated AS VARCHAR(50))+CASE WHEN @FilterWhereClause = '' THEN ' ' ELSE @FilterWhereClause END 
	  SET @InternalOrderBy = ' PaymentSettingId,ProfileId '
	END
	ELSE
	BEGIN
	  SET @SQL = '
	            ;With Cte_PaymenTSetting AS
				 (
				 SELECT   ZPS.PaymentSettingId,ZPS.PaymentApplicationSettingId,ZPS.PaymentTypeId,ZPS.PaymentGatewayId,ZPS.PaymentName,ZPS.IsActive
						,ISNULL(YOPU.DisplayOrder, ZPS.DisplayOrder) DisplayOrder,ZPS.IsTestMode,ZPS.IsPoDocUploadEnable,ZPS.IsPoDocRequire,ZPS.CreatedBy,ZPS.CreatedDate,ZPS.ModifiedBy,ZPS.ModifiedDate
						,NULL PortalId,NULL StoreName, NULL IsAssociated ,NULL ProfileId,NULL ProfileName,ZPT.BehaviorType PaymentTypeName,ZPG.GatewayName	
						,  CASE WHEN '+CAST(@PortalId AS VARCHAR(100))+' > 0  AND ZPPS.PaymentDisplayName  IS NOT NULL  THEN ZPPS.PaymentDisplayName  ELSE  ZPS.PaymentDisplayName   END PaymentDisplayName   , NULL PaymentExternalId, CASE WHEN YU.PaymentSettingId IS NOT NULL  THEN 1 ELSE 0 END AS IsApprovalRequired 
						 , ZPS.PaymentCode, ZPG.GatewayCode,ZPT.IsCallToPaymentAPI ,ZPS.IsBillingAddressOptional,ZPS.IsOABRequired,YU.PortalPaymentGroupId
						 , ISNULL(ZPPOS.DisplayName,''Production'' )  PublishState , 0 AS IsUsedForWebStorePayment
				 FROM ZnodePaymentSetting ZPS
				 INNER JOIN ZnodePaymentType  ZPT ON (ZPT.PaymentTypeId = ZPS.PaymentTypeId)
				 LEFT JOIN ZnodePaymentGateway ZPG ON (ZPG.PaymentGatewayId= ZPS.PaymentGatewayId)
				 LEFT JOIN ZnodeProfilePaymentSetting YOPU ON (YOPU.PaymentSettingId = ZPS.PaymentSettingId AND YOPU.ProfileId = '+CAST(@ProfileId AS NVARCHAr(200))+')
				 LEFT JOIN #ZnodePortalPaymentApprovers YU ON (YU.PaymentSettingId = ZPS.PaymentSettingId) 
				 LEFT JOIN ZnodePortalPaymentSetting ZPPS ON (ZPS.PaymentSettingId  = ZPPS.PaymentSettingId AND ZPPS.PortalId = CASE WHEN '+CAST(@PortalId AS VARCHAR(100))+' > 0  THEN '+CAST(@PortalId AS VARCHAR(100))+' ELSE  YU.PortalId END    )
				 LEFT JOIN ZnodePublishState ZPPOS ON (ZPPOS.PublishStateId = '+CASE WHEN  CAST(@ProfileId AS VARCHAr(200))  > '0' AND EXISTS (SELECT TOP 1 1 FROM ZnodeProfilePaymentSetting NT WHERE NT.ProfileId  = @ProfileId ) 
				 THEN  ' YOPU.PublishStateId '  ELSE ' ZPPS.PublishStateId ' END +')
				  ) '
	 SET @FilterWhereClause = CASE WHEN @FilterWhereClause ='' THEN ' WHERE 1=1 ' ELSE ' WHERE 1=1 '+@FilterWhereClause END
	 SET @InternalOrderBy = ' PaymentSettingId '
	END

	SET @SQL = @SQL+ ', Cte_PaymentSettingFilter AS
				 (

					SELECT PaymentSettingId,PaymentApplicationSettingId,PaymentTypeId,PaymentGatewayId,PaymentName,IsActive
						,DisplayOrder,IsTestMode,IsPoDocUploadEnable,IsPoDocRequire,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
						,PortalId,StoreName, ProfileId, ProfileName , PaymentTypeName,GatewayName,PaymentDisplayName,PaymentExternalId,IsApprovalRequired, PaymentCode, GatewayCode,IsCallToPaymentAPI, IsBillingAddressOptional,IsOABRequired,PortalPaymentGroupId,PublishState,IsUsedForWebStorePayment
						,'+dbo.Fn_GetPagingRowId(@Order_BY,@InternalOrderBy)+',Count(*)Over() CountNo

					FROM Cte_PaymenTSetting
				    '+@FilterWhereClause+'
					'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				 )

				 SELECT PaymentSettingId,PaymentApplicationSettingId,PaymentTypeId,PaymentGatewayId,PaymentName,IsActive
						,DisplayOrder,IsTestMode,IsPoDocUploadEnable,IsPoDocRequire,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
						,PortalId,StoreName, ProfileId, ProfileName,PaymentTypeName,GatewayName,PaymentDisplayName,PaymentExternalId,IsApprovalRequired, PaymentCode, GatewayCode,IsCallToPaymentAPI,IsBillingAddressOptional,IsOABRequired,PortalPaymentGroupId,PublishState,IsUsedForWebStorePayment,RowId ,CountNo
				 FROM Cte_PaymentSettingFilter '
				 +[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)

     PRINT  @SQL 
	
	 INSERT INTO @TBL_PaymentSetting (PaymentSettingId,PaymentApplicationSettingId,PaymentTypeId,PaymentGatewayId,PaymentName,IsActive
						,DisplayOrder,IsTestMode,IsPoDocUploadEnable,IsPoDocRequire,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
						,PortalId,StoreName, ProfileId, ProfileName,PaymentTypeName,GatewayName,PaymentDisplayName,PaymentExternalId,
						IsApprovalRequired, PaymentCode, GatewayCode,IsCallToPaymentAPI,IsBillingAddressOptional,IsOABRequired,
						PortalPaymentGroupId,PublishState,IsUsedForWebStorePayment,RowID,CountNo)
	 EXEC (@SQL)

	IF @IsAssociated = 1 AND @ProfileId = 0
	BEGIN
		 SELECT PaymentSettingId,PaymentApplicationSettingId,PaymentTypeId,PaymentGatewayId,PaymentName,IsActive
							,DisplayOrder,IsTestMode,IsPoDocUploadEnable,IsPoDocRequire,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
							,PortalId,StoreName, ProfileId, ProfileName,PaymentTypeName,GatewayName,PaymentDisplayName,PaymentExternalId,IsApprovalRequired, PaymentCode, GatewayCode,IsCallToPaymentAPI, IsBillingAddressOptional,IsOABRequired,PortalPaymentGroupId,PublishState
		 FROM @TBL_PaymentSetting
		 WHERE ISNULL(IsUsedForWebStorePayment,0) = 1
		 ORDER BY RowID,DisplayOrder
		 SET @RowsCount = ISNULL((SELECT top 1 CountNo FROM @TBL_PaymentSetting),0)
	END
	ELSE
	BEGIN
		SELECT PaymentSettingId,PaymentApplicationSettingId,PaymentTypeId,PaymentGatewayId,PaymentName,IsActive
							,DisplayOrder,IsTestMode,IsPoDocUploadEnable,IsPoDocRequire,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
							,PortalId,StoreName, ProfileId, ProfileName,PaymentTypeName,GatewayName,PaymentDisplayName,PaymentExternalId,IsApprovalRequired, PaymentCode, GatewayCode,IsCallToPaymentAPI, IsBillingAddressOptional,IsOABRequired,PortalPaymentGroupId,PublishState
		 FROM @TBL_PaymentSetting
		 WHERE ISNULL(IsUsedForWebStorePayment,0) = 0
		 ORDER BY RowID,DisplayOrder
		 
		 SET @RowsCount = ISNULL((SELECT top 1 CountNo FROM @TBL_PaymentSetting),0)
		 
	END
	

  END TRY
  BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPaymentSetting @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@ProfileId='+CAST(@ProfileId AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));

             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPaymentSetting',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
  END CATCH
 END