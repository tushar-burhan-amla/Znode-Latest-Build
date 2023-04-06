CREATE PROCEDURE [dbo].[Znode_PublishCatalogEntity]
(
   @PimCatalogId  INT = 0 
  ,@RevisionState varchar(50) = '' 
  ,@UserId int = 0

)
AS
/*
	To publish all catalog product and their details
	Unit Testing : 
	Declare @Status bit 
	Exec [dbo].[ZnodePublishPortalEntity]
     @PortalId  = 1 
	,@LocaleId  = 0 
	,@RevisionState = 'PRODUCTION' 
	,@UserId = 2
	,@Status = @Status 
	--Select @Status 
*/
BEGIN
BEGIN TRY 
SET NOCOUNT ON
	Declare @Status Bit =0 
	Declare @Type varchar(50) = '',	@CMSSEOCode varchar(300);
	SET @Status = 1 
	Declare @IsPreviewEnable int
	,@PreviewVersionId INT = 0  
	,@ProductionVersionId INT = 0
	,@PublishCatalogId    INT = 0 
	,@CatalogProfileId varchar(1000)
	
	 IF OBJECT_ID('tempdb..#CatalogAttributeDetails') IS NOT NULL
		DROP TABLE #CatalogAttributeDetails
	 CREATE TABLE [dbo].[#CatalogAttributeDetails]
	 (
		[ZnodeCatalogId] [int] NULL,
		[AttributeCode] [nvarchar](300) NULL,
		[AttributeTypeName] [varchar](300) NULL,
		[IsComparable] [bit] NOT NULL,
		[IsHtmlTags] [bit] NOT NULL,
		[IsFacets] [bit] NOT NULL,
		[IsUseInSearch] [bit] NOT NULL,
		[IsPersonalizable] [bit] NOT NULL,
		[IsConfigurable] [bit] NOT NULL,
		[AttributeName] [nvarchar](300) NULL,
		[LocaleId] [int] NULL,
		[DisplayOrder] [int] NULL,
		[DefaultValueDisplayOrder] [int] NULL,
		[AttributeDefaultValue] [nvarchar](max) NULL
	 ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	 SET @PublishCatalogId = ISNULL((SELECT TOP 1 PublishCatalogId FROM ZnodePublishCatalog ZPC WHERE ZPC.PimCatalogId = @PimCatalogId), 0)
   	 If Exists (SELECT  * from ZnodePublishStateApplicationTypeMapping PSA where PSA.IsEnabled =1 and  
		Exists (select TOP 1 1  from ZnodePublishState PS where PS.PublishStateId = PSA.PublishStateId ) and ApplicationType =  'WebstorePreview')
			SET @IsPreviewEnable = 1 
		else 
			SET @IsPreviewEnable = 0 

		--Genrate preview entry 
		DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
		
		IF object_id('tempdb..[#Tbl_VersionEntity]') IS NOT NULL
			drop table tempdb..#Tbl_VersionEntity
		Create Table #Tbl_VersionEntity(PublishCatalogId int , VersionId int , LocaleId int , PublishType varchar(50) )

		IF object_id('tempdb..[#Tbl_OldVersionEntity]') IS NOT NULL
			drop table tempdb..#Tbl_OldVersionEntity
		Create Table #Tbl_OldVersionEntity(PublishCatalogId int , NewVersionId int ,OldVersionId int , LocaleId int , PublishType varchar(50) )

		if @PublishCatalogId > 0 
		  
		  UPDATE ZPC SET CatalogName = ZC.CatalogName,ExternalId = ZC.ExternalId,PimCatalogId= @PimCatalogId,CreatedBy = @UserId,
					  CreatedDate = Getdate(),ModifiedBy = @UserId,ModifiedDate = Getdate()
					  FROM ZnodePublishCatalog ZPC 
					  INNER JOIN ZnodePimCatalog ZC ON(ZC.PimCatalogId = ZPC.PimCatalogId)
					  WHERE ZPC.PimCatalogId = @PimCatalogId;
		Else
		Begin
			INSERT INTO ZnodePublishCatalog (PimCatalogId,CatalogName,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT PimCatalogId,CatalogName,ExternalId,@UserId,Getdate(),@UserId,Getdate() FROM ZnodePimCatalog AS ZPC 
			WHERE ZPC.PimCatalogId = @PimCatalogId;
                      
			SET @PublishCatalogId = SCOPE_IDENTITY();
        End

		INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 --AND (LocaleId  = @LocaleId OR @LocaleId = 0 )
		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
			if (@IsPreviewEnable = 1 ) 
			Begin
				Insert into ZnodePublishCatalogLog(PublishCatalogId,PimCatalogId,IsCatalogPublished,PublishCategoryId,
				IsCategoryPublished,PublishProductId,
				IsProductPublished,UserId,LogDateTime,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Tokem,LocaleId,PublishStateId)
				Select @PublishCatalogId,  @PimCatalogId,0,0,
				0,0,
				0,@UserId, Getdate(), @UserId, Getdate(), @UserId ,Getdate(), null ,@SetLocaleId ,DBO.Fn_GetPublishStateIdForProcessing() 

				insert into #Tbl_VersionEntity (PublishCatalogId,VersionId,LocaleId,PublishType)
				select @PublishCatalogId, @@Identity , @SetLocaleId ,'PREVIEW'
				
			End
			--Genrate production entry 
			Insert into ZnodePublishCatalogLog
			(PublishCatalogId,PimCatalogId,IsCatalogPublished,PublishCategoryId,
				IsCategoryPublished,PublishProductId,
				IsProductPublished,UserId,LogDateTime,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Tokem,LocaleId,PublishStateId)
			Select @PublishCatalogId,  @PimCatalogId,0,0,
				0,0,
				0,@UserId, Getdate(), @UserId, Getdate(), @UserId ,Getdate(), null ,@SetLocaleId ,DBO.Fn_GetPublishStateIdForProcessing() 
			
			insert into #Tbl_VersionEntity (PublishCatalogId,VersionId,LocaleId,PublishType)
			select @PublishCatalogId, @@Identity , @SetLocaleId ,'PRODUCTION'

	   	SET @IncrementalId = @IncrementalId +1 
		END 
		Declare  @VersionIdString varchar(100)= ''  
		SELECT   @VersionIdString = STUFF((SELECT ',' + cast (VersionId as varchar(50))  FROM #Tbl_VersionEntity FOR XML PATH ('')), 1, 1, '') 
	
		Truncate table ZnodePublishCatalogErrorLogEntity

		Insert into #Tbl_OldVersionEntity (PublishCatalogId,NewVersionId,OldVersionId, LocaleId, PublishType)
		Select A.PublishCatalogId , A.VersionId, B.VersionId, a.LocaleId,a.PublishType 
		from #Tbl_VersionEntity A Inner join ZnodePublishVersionEntity B on 
		A.PublishCatalogId = B.ZnodeCatalogId and A.LocaleId = B.LocaleId AND A.PublishType= B.RevisionType  
	
		Begin Transaction 
	if @Type = 'ZnodePublishCatalogEntity' OR @Type = ''
	Begin
		Insert INTO ZnodePublishCatalogEntity (VersionId,ZnodeCatalogId,CatalogName,RevisionType,LocaleId,IsAllowIndexing)
		SELECT Distinct VE.VersionId, ZPC.PublishCatalogId, PC.CatalogName,VE.PublishType, VE.LocaleId, isnull(PC.IsAllowIndexing,0)
		FROM ZnodePublishCatalog ZPC
		INNER JOIN  #Tbl_VersionEntity VE ON (VE.PublishCatalogId = ZPC.PublishCatalogId)
		Inner join ZnodePimCatalog PC on ZPC.PimCatalogId = PC.PimCatalogId
		-- Data inserted into flat table ZnodeWebStoreEntity (Replica of MongoDB Collection )  
		
		If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
		Begin
			Insert Into ZnodePublishVersionEntity (VersionId,ZnodeCatalogId,RevisionType,LocaleId,IsPublishSuccess)
			SELECT VersionId,PublishCatalogId,PublishType,LocaleId,0 from  #Tbl_VersionEntity where PublishType = 'PREVIEW'
		End
		If (@RevisionState like '%Production%' OR @RevisionState = 'None')
		Begin
			Insert Into ZnodePublishVersionEntity (VersionId,ZnodeCatalogId,RevisionType,LocaleId,IsPublishSuccess)
			SELECT VersionId,PublishCatalogId,PublishType,LocaleId,0 from  #Tbl_VersionEntity where PublishType = 'PRODUCTION'
		End

		INSERT INTO ZnodePublishCatalogErrorLogEntity
		(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
		SELECT 'ZnodePublishCatalogEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
		@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

	End
	If @Type = 'ZnodePublishProductEntity' OR @Type = ''
	Begin
		-- Process call catalog publish (include category, products with multiple types)
		Declare @PimProductId TransferId 
		EXEC [Znode_PublishLatestAssociatedProduct] @PublishCatalogId = @PublishCatalogId, @PimProductId = @PimProductId  , @UserId = @UserId , @PublishStateId =0 
		
		EXEC [dbo].[Znode_InsertPublishProductIds] @PublishCatalogId= @PublishCatalogId ,@userid =@userid ,@PimProductId = @PimProductId 
		
		EXEC Znode_GetPublishProductJson 
			 @PublishCatalogId =@PublishCatalogId 
			,@PimProductId = @PimProductId 
			,@UserId = @userid
			,@PimCatalogId = @PimCatalogId 
			,@VersionIdString = @VersionIdString
			,@Status  =@Status  Out
		
		If @Status  = 0 
			Rollback Transaction 
		
			INSERT INTO ZnodePublishCatalogErrorLogEntity(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishProductEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

		If @Status  = 0 
		Begin
			SELECT 1 AS ID,@Status AS Status;
			Return 0 
		End
	End
	If @Type = 'ZnodePublishAddOnEntity' OR @Type = ''
	Begin
		Exec [Znode_GetPublishAssociatedAddonsJson]
				@PublishCatalogId = @PublishCatalogId ,
				@PimProductId   = @PimProductId ,
				@UserId =@UserId,														 
				@VersionIdString = @VersionIdString,
				@Status	 =@Status Out

			If @Status  = 0 
				Rollback Transaction 
		
			INSERT INTO ZnodePublishCatalogErrorLogEntity(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishAddOnEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status  = 0 
			Begin
				SELECT 1 AS ID,@Status AS Status;
				Return 0 
			End
	End 
	If @Type = 'ZnodePublishCategoryEntity' OR @Type = ''
	Begin
		Exec [Znode_GetPublishCategoryJson]
				@PublishCatalogId = @PublishCatalogId ,
				@UserId =@UserId,														 
				@VersionIdString = @VersionIdString,
				@Status	 =@Status Out

			If @Status  = 0 
				Rollback Transaction 
		
			INSERT INTO ZnodePublishCatalogErrorLogEntity(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishCategoryEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status  = 0 
			Begin
				SELECT 1 AS ID,@Status AS Status;
				Return 0 
			End
	End 
	If @Type = 'AssociatedProducts' OR @Type = ''
	Begin
			Exec [Znode_GetPublishAssociatedProductsJson]
				@PublishCatalogId = @PublishCatalogId ,
				@UserId =@UserId,														 
				@VersionIdString = @VersionIdString,
				@Status	 =@Status Out

			If @Status  = 0 
				Rollback Transaction 
		
			INSERT INTO ZnodePublishCatalogErrorLogEntity(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'AssociatedProducts', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status  = 0 
			Begin
				SELECT 1 AS ID,@Status AS Status;
				Return 0 
			End
	End 
	If @Type = 'CatalogAttributeaDetail' OR @Type = ''
	Begin
		Begin Try
			Insert into [dbo].[#CatalogAttributeDetails]([ZnodeCatalogId],[AttributeCode],[AttributeTypeName],[IsComparable],
			[IsHtmlTags],[IsFacets],[IsUseInSearch],[IsPersonalizable],[IsConfigurable],[AttributeName],
			[LocaleId],[DisplayOrder],[DefaultValueDisplayOrder],[AttributeDefaultValue] )
			EXEC Znode_GetPublishProductAttribute @PublishCatalogId 
			
			insert into ZnodePublishCatalogAttributeEntity
			(VersionId,ZnodeCatalogId,AttributeCode,AttributeTypeName,IsPromoRuleCondition,IsComparable,
			IsHtmlTags,IsFacets,IsUseInSearch,IsPersonalizable,IsConfigurable,
			AttributeName,LocaleId,DisplayOrder,SelectValues)
			Select Distinct 
			B.VersionId, A.[ZnodeCatalogId],A.[AttributeCode],A.[AttributeTypeName],0 IsPromoRuleCondition,[IsComparable],
			[IsHtmlTags],[IsFacets],[IsUseInSearch],[IsPersonalizable],[IsConfigurable],
			[AttributeName],A.[LocaleId],[DisplayOrder] ,
			(Select Isnull(IA.AttributeDefaultValue,'') Value , Isnull(IA.[DefaultValueDisplayOrder],'') DisplayOrder  from [dbo].[#CatalogAttributeDetails] IA  where IA.AttributeCode = A.AttributeCode AND IA.LocaleId = A.LocaleId 
			For JSON PATH , Root('SelectValues'))
			FROM [dbo].[#CatalogAttributeDetails] A Inner join #Tbl_VersionEntity B on A.LocaleId = B.LocaleId
	
			SET @Status =1 
		
			INSERT INTO ZnodePublishCatalogErrorLogEntity(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'CatalogAttributeaDetail', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
		END TRY 
		BEGIN CATCH 
			SET @Status   = 0 
			If @Status  = 0 
				Rollback Transaction 

			INSERT INTO ZnodePublishCatalogErrorLogEntity(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'CatalogAttributeaDetail', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status  = 0 
			Begin
				SELECT 1 AS ID,@Status AS Status;
				Return 0 
			End
		End CATCH  
	End 

	If @Type = 'ZnodePublishSEOEntity' OR @Type = ''
	Begin
		   EXEC [ZnodeSetPublishSEOEntity]
			@RevisionState  = @RevisionState 
			,@CMSSEOTypeId = '1,2' 
			,@UserId  = @UserId 
			,@Status = @Status  OUTPUT 
			,@IsCatalogPublish = 1  
			,@VersionIdString = @VersionIdString 
			
			If @Status  = 0 
				Rollback Transaction 
		
			INSERT INTO ZnodePublishCatalogErrorLogEntity(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishSEOEntity', @RevisionState, Case when Isnull(@Status,0) = 0 then 'Fail' Else 'Success' end , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

			If @Status  = 0 
			Begin
				SELECT 1 AS ID,@Status AS Status;
				Return 0 
			End
	End 

	IF Exists (select TOP 1 1  from ZnodePublishCatalogErrorLogEntity where  ProcessStatus = 'Fail') 
		Begin
			Rollback transaction
			SET @Status  =0 
			SELECT 1 AS ID,@Status AS Status;
			INSERT INTO ZnodePublishCatalogErrorLogEntity
			(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
			SELECT 'ZnodePublishPortalEntity', @RevisionState , 'Fail' , Getdate(), 
			@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 
			
			Update ZnodePublishCatalogLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPublishFailed()  where PublishCatalogLogId in  (Select VersionId from #Tbl_VersionEntity Where PublishType = 'PREVIEW' )
			Update ZnodePublishCatalogLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPublishFailed()  where PublishCatalogLogId in (Select VersionId from #Tbl_VersionEntity Where PublishType = 'PRODUCTION' )
			
			Return 0 
		End

	SET @Status = 1
	Commit Transaction 
		
	SELECT @PublishCatalogId AS id,@Status AS Status;   

END TRY 
BEGIN CATCH 
	SET @Status =0  
	 SELECT 1 AS ID,@Status AS Status;   
	 Rollback transaction
	 DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
		@ErrorLine VARCHAR(100)= ERROR_LINE(),
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_PublishCatalogEntity 
		@PimCatalogId = '+CAST(@PimCatalogId  AS VARCHAR	(max))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10))
		+',@PreviewVersionId = ' + CAST(@PreviewVersionId  AS varchar(20))
		+',@ProductionVersionId = ' + CAST(@ProductionVersionId  AS varchar(20))
		+',@RevisionState = ''' + CAST(@RevisionState  AS varchar(50))
		+',@UserId = ' + CAST(@UserId AS varchar(20));	SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
	
			
	INSERT INTO ZnodePublishCatalogErrorLogEntity
	(EntityName,ErrorDescription,ProcessStatus,CreatedDate,CreatedBy,VersionId)
	SELECT 'Znode_PublishCatalogEntity', @RevisionState + isnull(@ErrorMessage,'') , 'Fail' , Getdate(), 
	@UserId , Convert( varchar(100), @PreviewVersionId) + '/' + Convert( varchar(100), @ProductionVersionId) 

	Update ZnodePublishCatalogLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPublishFailed()  where  PublishCatalogLogId in 
	(Select VersionId from #Tbl_VersionEntity Where PublishType = 'PREVIEW' )
	Update ZnodePublishCatalogLog SET PublishStateId = DBO.Fn_GetPublishStateIdForPublishFailed()  where  PublishCatalogLogId in
	(Select VersionId from #Tbl_VersionEntity Where PublishType = 'PRODUCTION' )
		                			 
	EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_PublishCatalogEntity',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
END CATCH
END