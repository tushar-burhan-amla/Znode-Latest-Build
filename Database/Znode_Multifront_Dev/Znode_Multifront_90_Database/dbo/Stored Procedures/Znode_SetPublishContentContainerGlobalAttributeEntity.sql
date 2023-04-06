CREATE PROCEDURE [dbo].[Znode_SetPublishContentContainerGlobalAttributeEntity]
(
	 @ContainerKey VARCHAR(100) = ''
	 ,@ContainerProfileVariantId INT = 0
)
AS
    
/*
exec [Znode_SetPublishContentContainerGlobalAttributeEntity] @ContainerKey = 'HomePagePromo', @ContainerProfileVariantId = 0
    Summary :	Publish Product on the basis of publish catalog
				Retrive all Product details with attributes and INSERT INTO following tables 
				1.	ZnodePublishedXml
				2.	ZnodePublishCategoryProduct
				3.	ZnodePublishProduct
				4.	ZnodePublishProductDetail

                Product details include all the type of products link, grouped, configure and bundel products (include addon) their associated products 
				collect their attributes and values into tables variables to process for publish.  
                
				Finally genrate XML for products with their attributes and inserted into ZnodePublishedXml Znode Admin process xml from sql server to mongodb
				one by one.
 */
BEGIN
	
	BEGIN TRY
		SET NOCOUNT ON;
		DECLARE @VersionId Int, @CMSContentContainerId int
		--SET @VersionId = ( SELECT TOP 1 VersionId FROM ZnodePublishContetContainerVersionEntity WHERE PublishState = @RevisionState )

		DECLARE @SetLocaleId INT, @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1;
		DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1));

		DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);
          SET @V_MediaServerThumbnailPath =
         (
             SELECT ISNULL(CASE WHEN CDNURL = '' THEN NULL ELSE CDNURL END,URL) 
             FROM ZnodeMediaConfiguration ZMC 
			 --INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
		     WHERE IsActive = 1 
         );
	
		DECLARE @TBL_GlobalAttributeGrouplist TABLE ([GlobalAttributeGroupId] INT , [AttributeGroupDisplayOrder] INT,Groups int )
		DECLARE @TBL_GlobalAttributelist      TABLE ([GlobalAttributeGroupId] INT , [GlobalAttributeId] INT,[AttributeDisplayOrder] int ,Attributes int )
		Declare	@EntityAttributeValueList as	table
		(
			IsInput bit, IsMedia BIT, AttributeValues int,WidgetGlobalAttributeValueId int,GlobalAttributeId int,AttributeValue nvarchar(max),
			GlobalAttributeValueId int,GlobalAttributeDefaultValueId int,AttributeDefaultValueCode nvarchar(300),
			AttributeDefaultValue nvarchar(300),MediaId int,MediaPath nvarchar(300) ,SwatchText nvarchar(300),DisplayOrder int,
			SingleAttributeValue nvarchar(max), CMSContainerProfileVariantId INT,LocaleId INT
		)
		Declare	@EntityAttributeSingleValueList as table(AttributeValues int,PortalGlobalAttributeValueId int,GlobalAttributeId int)

		Declare @LocaleId INT , @GetDate DATETIME =dbo.Fn_GetDate()

		IF @ContainerKey <> ''
		BEGIN
			Select @CMSContentContainerId= CMSContentContainerId 
			from ZnodeCMSContentContainer  
			WHERE ContainerKey = @ContainerKey
		END

		INSERT INTO @EntityAttributeValueList
		(
			IsInput, IsMedia,AttributeValues,WidgetGlobalAttributeValueId,GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,
			AttributeValue ,MediaId,MediaPath,SingleAttributeValue,CMSContainerProfileVariantId, LocaleId
		)
		Select case when zga.GroupAttributeType in('Input','TextArea')   then 1  else 0 end ,
				0, 1, aa.WidgetGlobalAttributeValueId,aa.GlobalAttributeId,aa.WidgetGlobalAttributeValueId,bb.GlobalAttributeDefaultValueId,

			case when zga.GroupAttributeType in('Input','TextArea')   then null  else  bb.AttributeValue  end AttributeValue,	  
			bb.MediaId,CASE WHEN bb.MediaPath IS NOT NULL THEN @V_MediaServerThumbnailPath+bb.MediaPath ELSE bb.MediaPath END AS MediaPath ,
			case when zga.GroupAttributeType in('Input','TextArea')   then bb.AttributeValue   else null end  SingleAttributeValue,
			aa.CMSContainerProfileVariantId, bb.LocaleId
		FROM ZnodeWidgetGlobalAttributeValue aa
		INNER JOIN ZnodeWidgetGlobalAttributeValueLocale bb ON bb.WidgetGlobalAttributeValueId = aa.WidgetGlobalAttributeValueId 
		INNER JOIN View_ZnodeGlobalAttribute zga on zga.[GlobalAttributeId]=aa.[GlobalAttributeId]
		WHERE (aa.CMSContainerProfileVariantId = @ContainerProfileVariantId OR @ContainerProfileVariantId = 0)
		AND (AA.CMSContentContainerId = @CMSContentContainerId OR ISNULL(@CMSContentContainerId,0) = 0 )
		AND bb.MediaId IS NULL 
				

		INSERT INTO @EntityAttributeValueList
		(IsInput,IsMedia,AttributeValues,WidgetGlobalAttributeValueId,GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,AttributeValue ,MediaId,MediaPath,SingleAttributeValue,CMSContainerProfileVariantId,LocaleId)
		Select 1,1 ,1, aa.WidgetGlobalAttributeValueId,aa.GlobalAttributeId,aa.WidgetGlobalAttributeValueId,null GlobalAttributeDefaultValueId,
			NULL AttributeValue,	  
			NULL MediaId,null MediaPath,
			NULL SingleAttributeValue, aa.CMSContainerProfileVariantId	,bb.LocaleId
		FROM dbo.ZnodeWidgetGlobalAttributeValue aa
		INNER JOIN ZnodeWidgetGlobalAttributeValueLocale bb ON bb.WidgetGlobalAttributeValueId = aa.WidgetGlobalAttributeValueId 
		INNER JOIN View_ZnodeGlobalAttribute zga on zga.[GlobalAttributeId]=aa.[GlobalAttributeId]
		WHERE (AA.CMSContentContainerId = @CMSContentContainerId OR ISNULL(@CMSContentContainerId,0) = 0 )
		AND zga.GroupAttributeType ='Media'

		UPDATE aa
		Set SingleAttributeValue= ( Select CASE WHEN bb.MediaPath IS NOT NULL THEN @V_MediaServerThumbnailPath+bb.MediaPath ELSE bb.MediaPath END AS MediaPath FROM  ZnodeWidgetGlobalAttributeValueLocale bb 
			WHERE bb.WidgetGlobalAttributeValueId = aa.WidgetGlobalAttributeValueId AND aa.LocaleId = bb.LocaleId
			FOR XML PATH ('') )				
		FROM  @EntityAttributeValueList aa
		WHERE aa.IsMedia=1

		UPDATE aa
		Set SingleAttributeValue= replace(replace(SingleAttributeValue,'</MediaPath>',','),'<MediaPath>','')
		FROM  @EntityAttributeValueList aa
		WHERE aa.IsMedia=1

		UPDATE aa
		Set SingleAttributeValue= Substring(SingleAttributeValue,1,len(SingleAttributeValue)-1)
		FROM  @EntityAttributeValueList aa
		WHERE aa.IsMedia=1 and aa.SingleAttributeValue<>''

		UPDATE aa
		SET AttributeDefaultValueCode= h.AttributeDefaultValueCode,
			SwatchText=h.SwatchText,
			AttributeValue=g.AttributeDefaultValue,
			GlobalAttributeDefaultValueId=g.GlobalAttributeDefaultValueId,
			DisplayOrder=h.DisplayOrder
		FROM  @EntityAttributeValueList aa
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeDefaultValueId = aa.GlobalAttributeDefaultValueId                                       
		INNER JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId and aa.LocaleId = g.LocaleId
          
		INSERT INTO @TBL_GlobalAttributeGrouplist
		([GlobalAttributeGroupId] , [AttributeGroupDisplayOrder],Groups )
		SELECT ss.[GlobalAttributeGroupId],ss.[AttributeGroupDisplayOrder],1
		FROM [dbo].[ZnodeGlobalGroupEntityMapper] SS
		INNER JOIN dbo.ZnodeGlobalEntity aa on ss.[GlobalEntityId]=aa.[GlobalEntityId]
		WHERE aa.EntityName='Content Containers'
		--FOR XML PATH('')

		INSERT INTO @TBL_GlobalAttributelist
		([GlobalAttributeGroupId] , [GlobalAttributeId],[AttributeDisplayOrder],Attributes )
		SELECT aa.[GlobalAttributeGroupId],aa.[GlobalAttributeId] ,aa.[AttributeDisplayOrder],1
		FROM [dbo].[ZnodeGlobalAttributeGroupMapper] aa
		INNER JOIN @TBL_GlobalAttributeGrouplist ss on ss.GlobalAttributeGroupId=aa.GlobalAttributeGroupId
			
		IF object_id('tempdb..[#GlobalAttributeGrouplist]') IS NOT NULL
			drop table tempdb..#GlobalAttributeGrouplist

		CREATE TABLE #GlobalAttributeGrouplist 
		( 
			CMSContainerProfileVariantId INT,LocaleId	int,GlobalAttributeGroupId	int	,GroupCode	varchar	(200),AttributeGroupName nvarchar(600),AttributeGroupDisplayOrder int,
			GlobalAttributeId	int	,AttributeDisplayOrder	int	,AttributeCode	nvarchar	(600),AttributeName	nvarchar	(600),IsRequired	bit	,
			AttributeTypeName	varchar	(300),AttributeTypeId	int	,SingleAttributeValue	nvarchar(max),SelectValues	nvarchar(max)
		)

		SET @LocaleId = 0 

		INSERT INTO @TBL_Locale (LocaleId) 
		SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

		SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
		WHILE @IncrementalId <= @MaxCount
		BEGIN 
			SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
			;With Cte_GetCmsContentContainerData AS 
			(
			Select distinct @SetLocaleId AS LocaleId, sv.CMSContainerProfileVariantId,
				aa.[GlobalAttributeGroupId] AS 'GlobalAttributeGroupId' ,
				zgag.GroupCode	AS 'GroupCode'		 ,  
				zgagl.AttributeGroupName  AS 'AttributeGroupName'   ,
				AttributeGroupDisplayOrder 	 , 
				ss.GlobalAttributeId	 AS 'GlobalAttributeId'	 ,
				ss.[AttributeDisplayOrder] AS 'AttributeDisplayOrder'	,
				zga.AttributeCode AS 'AttributeCode'	,			
				zgal.AttributeName AS 'AttributeName'	,
				zga.IsRequired AS 'IsRequired'	,
				zga.AttributeTypeName AS 'AttributeTypeName'      ,
				zga.AttributeTypeId AS 'AttributeTypeId' 		,
				sv.SingleAttributeValue AS 'SingleAttributeValue'	,	
	   
				( Select 
				SelectValuesEntity.DisplayOrder					as DisplayOrder,
				SelectValuesEntity.GlobalAttributeDefaultValueId as GlobalAttributeDefaultValueId,
				SelectValuesEntity.AttributeValue				as [Value]	,
				SelectValuesEntity.AttributeDefaultValueCode		as Code	,
				SelectValuesEntity.SwatchText                    as SwatchText	,
				SelectValuesEntity.MediaPath		         		as [Path]	
				FROM  @EntityAttributeValueList SelectValuesEntity 
				WHERE SelectValuesEntity.WidgetGlobalAttributeValueId = sv.WidgetGlobalAttributeValueId
				and SelectValuesEntity.LocaleId = sv.LocaleId and 
				zga.GroupAttributeType ='Select'
				FOR JSON Auto, INCLUDE_NULL_VALUES    
				)  as 'SelectValues'
			FROM 
				@TBL_GlobalAttributeGrouplist aa				
				INNER JOIN @TBL_GlobalAttributelist ss on  ss.GlobalAttributeGroupId=aa.GlobalAttributeGroupId
				INNER JOIN ZnodeGlobalAttributeGroup zgag on zgag.[GlobalAttributeGroupId]=aa.[GlobalAttributeGroupId]
				INNER JOIN ZnodeGlobalAttributeGroupLocale zgagl on zgagl.GlobalAttributeGroupId=zgag.GlobalAttributeGroupId
				INNER JOIN View_ZnodeGlobalAttribute zga       on zga.[GlobalAttributeId]=ss.[GlobalAttributeId]
				INNER JOIN ZnodeGlobalAttributeLocale zgal on zga.[GlobalAttributeId]=zgal.[GlobalAttributeId]
				INNER JOIN @EntityAttributeValueList sv   on sv.[GlobalAttributeId]=ss.[GlobalAttributeId]
				WHERE zgagl.LocaleId= zgal.LocaleId and 
				(sv.LocaleId = @SetLocaleId)  
			)
	     	, Cte_GetFirstFilterData AS
			(
				SELECT CMSContainerProfileVariantId,LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
						GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
						AttributeTypeId,SingleAttributeValue,SelectValues
				FROM Cte_GetCmsContentContainerData 
				WHERE LocaleId = @SetLocaleId
			)
			, Cte_GetDefaultFilterData AS
			(
				SELECT CMSContainerProfileVariantId,LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
						GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
						AttributeTypeId,SingleAttributeValue,SelectValues
				FROM  Cte_GetFirstFilterData 
				UNION ALL 
				SELECT CMSContainerProfileVariantId,LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
						GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
						AttributeTypeId,SingleAttributeValue,SelectValues
				FROM Cte_GetCmsContentContainerData CTEC 
				WHERE LocaleId = @DefaultLocaleId 
				AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstFilterData CTEFD WHERE CTEFD.GlobalAttributeGroupId = CTEC.GlobalAttributeGroupId )
			)
			INSERT INTO #GlobalAttributeGrouplist 
			(
				CMSContainerProfileVariantId,LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
				GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
				AttributeTypeId,SingleAttributeValue,SelectValues
			)
			SELECT DISTINCT CMSContainerProfileVariantId,@SetLocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
					GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
					AttributeTypeId,SingleAttributeValue,SelectValues
			FROM Cte_GetDefaultFilterData
						
				SET @IncrementalId = @IncrementalId +1 
		End 
		--select * into ##GlobalAttributeGrouplist from #GlobalAttributeGrouplist
	--select distinct CMSContainerProfileVariantId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,LocaleId 
	--into #GroupMaster from #GlobalAttributeGrouplist
	
	--If (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	--Begin
	--    --Data inserted into flat table ZnodePublishWidgetSliderBannerEntity (Replica of MongoDB Collection )  

	--	Delete from ZnodePublishContentContainerVariantEntity WHERE  VersionId = @VersionId
	 		 

	--	INSERT INTO ZnodePublishPortalGlobalAttributeEntity
	--	(
	--		VersionId,PublishStartTime,PortalId,PortalName,LocaleId,GlobalAttributeGroups
	--	)
	  	
			--Select Distinct PV.CMSContainerProfileVariantId,PV.LocaleId,  (Select 
			--A.GlobalAttributeGroupId AS 'GlobalAttributeGroupId',
			--A.GroupCode AS 'GroupCode',
			--A.AttributeGroupName AS 'AttributeGroupName',
			--A.AttributeGroupDisplayOrder AS 'AttributeGroupDisplayOrder',
			--A.LocaleId AS 'LocaleId', 
			--(
			--	Select DISTINCT GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName ,IsRequired ,	
			--	AttributeTypeName,AttributeTypeId,SingleAttributeValue,SelectValues 
			--	from #GlobalAttributeGrouplist B WHERE A.GlobalAttributeGroupId = B.GlobalAttributeGroupId 
			--	AND A.LocaleId = B.LocaleId AND A.CMSContainerProfileVariantId = B.CMSContainerProfileVariantId
			--	For JSON Path 
			--) AS 'GlobalAttributes'
			--from #GlobalAttributeGrouplist A WHERE A.CMSContainerProfileVariantId = PV.CMSContainerProfileVariantId AND A.LocaleId = PV.LocaleId For Json Path) GlobalAttributes
			--FROM  #GlobalAttributeGrouplist  PV 

			SELECT DISTINCT A.CMSContainerProfileVariantId,A.LocaleId,  
			REPLACE((
				SELECT DISTINCT GlobalAttributeGroupId, GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName ,IsRequired ,	
				AttributeTypeName,AttributeTypeId,SingleAttributeValue As AttributeValue,SelectValues 
				FROM #GlobalAttributeGrouplist B
				WHERE A.GlobalAttributeGroupId = B.GlobalAttributeGroupId 
				AND A.LocaleId = B.LocaleId AND A.CMSContainerProfileVariantId = B.CMSContainerProfileVariantId
				--AND GlobalAttributeId=32 and A.GlobalAttributeGroupId=23
				For JSON Path 
			),'\','') AS 'GlobalAttributes'
			FROM #GlobalAttributeGrouplist  A
	--End
	---------------------------- End Preview 
	--If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	--Begin
	--	-- Only production version id will process 
	--	Delete from ZnodePublishPortalGlobalAttributeEntity WHERE  VersionId in (Select ProductionVersionId from @Tbl_ProductionVersionId)
	--	AND PortalId = @PortalId
	 		 

	--	INSERT INTO ZnodePublishPortalGlobalAttributeEntity
	--	(
	--		VersionId,PublishStartTime,PortalId,PortalName,LocaleId,GlobalAttributeGroups
	--	)
			  	
	--		Select Distinct PV.ProductionVersionId, @GetDate,  @PortalId,@StoreName, PV.LocaleId, (Select 
	--		A.GlobalAttributeGroupId AS 'GlobalAttributeGroupId',
	--		A.GroupCode AS 'GroupCode',
	--		A.AttributeGroupName AS 'AttributeGroupName',
	--		A.AttributeGroupDisplayOrder AS 'AttributeGroupDisplayOrder',
	--		A.LocaleId AS 'LocaleId', 
	--		(
	--			Select GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName ,IsRequired ,	
	--			AttributeTypeName,AttributeTypeId,SingleAttributeValue,SelectValues 
	--			from #GlobalAttributeGrouplist B WHERE A.GlobalAttributeGroupId = B.GlobalAttributeGroupId 
	--			AND A.LocaleId = B.LocaleId
	--			For JSON Path 
	--		) AS 'GlobalAttributes'
	--		from #GroupMaster A  WHERE A.LocaleId = PV.LocaleId  For Json Path) 
	--		FROM  @Tbl_ProductionVersionId  PV 

	--End

END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE(), ERROR_PROCEDURE();

END CATCH;
END;