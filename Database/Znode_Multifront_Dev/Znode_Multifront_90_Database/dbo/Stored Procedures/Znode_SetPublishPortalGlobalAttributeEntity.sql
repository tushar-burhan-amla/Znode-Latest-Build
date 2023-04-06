CREATE PROCEDURE [dbo].[Znode_SetPublishPortalGlobalAttributeEntity]
     (
         @PortalId int = 0
	    ,@IsPreviewEnable int = 0 
		,@PreviewVersionId INT = 0 
		,@ProductionVersionId INT = 0 
		,@RevisionState varchar(50) = '' 
		,@UserId int = 0 
		,@Status int  Output
	  )
AS
    
/*
    Summary :	Publish Product on the basis of publish catalog
				Retrive all Product details with attributes and insert into following tables 
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
		DECLARE @Tbl_PreviewVersionId    TABLE    (PreviewVersionId int , PortalId int , LocaleId int)
		DECLARE @Tbl_ProductionVersionId TABLE    (ProductionVersionId int  , PortalId int , LocaleId int)

		If @PreviewVersionId = 0 
			Begin
   				Insert into @Tbl_PreviewVersionId 
				SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PREVIEW'
			end
		Else 
				Insert into @Tbl_PreviewVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
				where VersionId = @PreviewVersionId
		If @ProductionVersionId = 0 
   			Begin
				Insert into @Tbl_ProductionVersionId 
				SELECT distinct VersionId , PortalId , LocaleId from  ZnodePublishWebStoreEntity where (PortalId = @PortalId or @PortalId=0 ) and PublishState ='PRODUCTION'
			End 
		Else 
		
			Insert into @Tbl_ProductionVersionId SELECT distinct VersionId , PortalId, LocaleId from  ZnodePublishWebStoreEntity 
			where VersionId = @ProductionVersionId
			DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
			DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))



	           DECLARE @TBL_GlobalAttributeGrouplist TABLE ([GlobalAttributeGroupId] INT , [AttributeGroupDisplayOrder] INT,Groups int )
			   DECLARE @TBL_GlobalAttributelist      TABLE ([GlobalAttributeGroupId] INT , [GlobalAttributeId] INT,[AttributeDisplayOrder] int ,Attributes int )
			   Declare	@EntityAttributeValueList as	table
			    (IsInput bit, IsMedia BIT, AttributeValues int,PortalGlobalAttributeValueId int,GlobalAttributeId int,AttributeValue nvarchar(max),
				GlobalAttributeValueId int,GlobalAttributeDefaultValueId int,AttributeDefaultValueCode nvarchar(300),
				AttributeDefaultValue nvarchar(300),
				MediaId int,MediaPath nvarchar(300) ,SwatchText nvarchar(300),DisplayOrder int,SingleAttributeValue nvarchar(max))
			    Declare	@EntityAttributeSingleValueList as	table
			    (AttributeValues int,PortalGlobalAttributeValueId int,GlobalAttributeId int)

			   Declare @StoreName nvarchar(max),@LocaleId INT 
			   , @GetDate DATETIME =dbo.Fn_GetDate()
			  
			   --Select @LocaleId=LocaleId
			   --From ZnodePortalLocale
			   --Where PortalId=@PortalId
			   --and IsDefault=1



				Select @StoreName= StoreName 
				from ZnodePortal 
				Where PortalId =@PortalId


				insert into @EntityAttributeValueList
				(IsInput, IsMedia,AttributeValues,PortalGlobalAttributeValueId,GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,AttributeValue ,MediaId,MediaPath,SingleAttributeValue)
				Select case when zga.GroupAttributeType in('Input','TextArea')   then 1  else 0 end ,
				 0, 1, aa.PortalGlobalAttributeValueId,aa.GlobalAttributeId,aa.PortalGlobalAttributeValueId,bb.GlobalAttributeDefaultValueId,
				--case when bb.MediaPath is not null then  --@V_MediaServerThumbnailPath+
				--bb.MediaPath--+'~'+convert(nvarchar(10),bb.MediaId) 
				--else bb.AttributeValue end,	
				case when zga.GroupAttributeType in('Input','TextArea')   then null  else  bb.AttributeValue  end AttributeValue,	  
				bb.MediaId,bb.MediaPath,
				case when zga.GroupAttributeType in('Input','TextArea')   then bb.AttributeValue   else null end  SingleAttributeValue	
				from  dbo.ZnodePortalGlobalAttributeValue aa
				inner join ZnodePortalGlobalAttributeValueLocale bb ON bb.PortalGlobalAttributeValueId = aa.PortalGlobalAttributeValueId 
				inner join View_ZnodeGlobalAttribute zga       on zga.[GlobalAttributeId]=aa.[GlobalAttributeId]
				Where  PortalId=@PortalId
				and bb.MediaId is null 
				

				insert into @EntityAttributeValueList
				(IsInput,IsMedia,AttributeValues,PortalGlobalAttributeValueId,GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,AttributeValue ,MediaId,MediaPath,SingleAttributeValue)
				Select 1,1 ,1, aa.PortalGlobalAttributeValueId,aa.GlobalAttributeId,aa.PortalGlobalAttributeValueId,null GlobalAttributeDefaultValueId,
				NULL AttributeValue,	  
				NULL MediaId,null MediaPath,
				NULL SingleAttributeValue	
				from  dbo.ZnodePortalGlobalAttributeValue aa
				inner join View_ZnodeGlobalAttribute zga       on zga.[GlobalAttributeId]=aa.[GlobalAttributeId]
				Where  PortalId=@PortalId
				and zga.GroupAttributeType ='Media'


				update aa
				Set SingleAttributeValue= ( Select bb.MediaPath from  ZnodePortalGlobalAttributeValueLocale bb 
				 Where bb.PortalGlobalAttributeValueId = aa.PortalGlobalAttributeValueId
				 FOR XML PATH ('') )				
				from  @EntityAttributeValueList aa
		    	Where aa.IsMedia=1

				update aa
				Set SingleAttributeValue= replace(replace(SingleAttributeValue,'</MediaPath>',','),'<MediaPath>','')
				from  @EntityAttributeValueList aa
		    	Where aa.IsMedia=1

				update aa
				Set SingleAttributeValue= Substring(SingleAttributeValue,1,len(SingleAttributeValue)-1)
				from  @EntityAttributeValueList aa
		    	Where aa.IsMedia=1
				  


				update aa
				Set AttributeDefaultValueCode= h.AttributeDefaultValueCode,
				    SwatchText=h.SwatchText,
					AttributeValue=g.AttributeDefaultValue,
					GlobalAttributeDefaultValueId=g.GlobalAttributeDefaultValueId,
					DisplayOrder=h.DisplayOrder
				from  @EntityAttributeValueList aa
				inner JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeDefaultValueId = aa.GlobalAttributeDefaultValueId                                       
				inner JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
          
		 
			   insert into @TBL_GlobalAttributeGrouplist
			   ([GlobalAttributeGroupId] , [AttributeGroupDisplayOrder],Groups )
				SELECT ss.[GlobalAttributeGroupId],ss.[AttributeGroupDisplayOrder],1
				FROM [dbo].[ZnodeGlobalGroupEntityMapper] SS
				inner join dbo.ZnodeGlobalEntity aa on ss.[GlobalEntityId]=aa.[GlobalEntityId]
				WHere aa.EntityName='Store'
				--FOR XML PATH('')

				insert into @TBL_GlobalAttributelist
				([GlobalAttributeGroupId] , [GlobalAttributeId],[AttributeDisplayOrder],Attributes )
				SELECT aa.[GlobalAttributeGroupId],aa.[GlobalAttributeId] ,aa.[AttributeDisplayOrder],1
				FROM [dbo].[ZnodeGlobalAttributeGroupMapper] aa
				inner join @TBL_GlobalAttributeGrouplist ss on ss.GlobalAttributeGroupId=aa.GlobalAttributeGroupId
			
				IF object_id('tempdb..[#GlobalAttributeGrouplist]') IS NOT NULL
				drop table tempdb..#GlobalAttributeGrouplist

				Create Table #GlobalAttributeGrouplist ( LocaleId	int,GlobalAttributeGroupId	int	,
				GroupCode	varchar	(200),AttributeGroupName	nvarchar(600),AttributeGroupDisplayOrder	int	,
				GlobalAttributeId	int	,AttributeDisplayOrder	int	,AttributeCode	nvarchar	(600),
				AttributeName	nvarchar	(600),IsRequired	bit	,AttributeTypeName	varchar	(300),
				AttributeTypeId	int	,SingleAttributeValue	nvarchar(max),SelectValues	nvarchar(max))
				set @LocaleId = 0 
				INSERT INTO @TBL_Locale (LocaleId) SELECT LocaleId FROM ZnodeLocale WHERE IsActive =1 AND (LocaleId  = @LocaleId OR @LocaleId = 0 )
				SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
				WHILE @IncrementalId <= @MaxCount
				BEGIN 
					SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)
					;With Cte_GetCmsBlogNewsData AS 
					(
					Select distinct @SetLocaleId AS LocaleId,
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
						from  @EntityAttributeValueList SelectValuesEntity 
						Where SelectValuesEntity.PortalGlobalAttributeValueId = sv.PortalGlobalAttributeValueId
						and 
						zga.GroupAttributeType ='Select'
						FOR JSON Auto, INCLUDE_NULL_VALUES    
						)  as 'SelectValues'
					from 
						@TBL_GlobalAttributeGrouplist aa				
						inner join @TBL_GlobalAttributelist ss on  ss.GlobalAttributeGroupId=aa.GlobalAttributeGroupId
						inner join ZnodeGlobalAttributeGroup zgag on zgag.[GlobalAttributeGroupId]=aa.[GlobalAttributeGroupId]
						inner join ZnodeGlobalAttributeGroupLocale zgagl on zgagl.GlobalAttributeGroupId=zgag.GlobalAttributeGroupId
						inner join View_ZnodeGlobalAttribute zga       on zga.[GlobalAttributeId]=ss.[GlobalAttributeId]
						inner join ZnodeGlobalAttributeLocale zgal on zga.[GlobalAttributeId]=zgal.[GlobalAttributeId]
						inner join @EntityAttributeValueList sv   on sv.[GlobalAttributeId]=ss.[GlobalAttributeId]
						Where zgagl.LocaleId= zgal.LocaleId and 
						(zgagl.LocaleId = @SetLocaleId OR zgagl.LocaleId = @DefaultLocaleId)  
					)
	     	       , Cte_GetFirstFilterData AS
			       (
						SELECT LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
								GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
								AttributeTypeId,SingleAttributeValue,SelectValues
						FROM Cte_GetCmsBlogNewsData 
						WHERE LocaleId = @SetLocaleId
				   )
					, Cte_GetDefaultFilterData AS
					(
						SELECT LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
								GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
								AttributeTypeId,SingleAttributeValue,SelectValues
						FROM  Cte_GetFirstFilterData 
						UNION ALL 
						SELECT LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
								GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
								AttributeTypeId,SingleAttributeValue,SelectValues
						FROM Cte_GetCmsBlogNewsData CTEC 
						WHERE LocaleId = @DefaultLocaleId 
						AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstFilterData CTEFD WHERE CTEFD.GlobalAttributeGroupId = CTEC.GlobalAttributeGroupId )
					)
	
						insert into #GlobalAttributeGrouplist 
						(LocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
						GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
						AttributeTypeId,SingleAttributeValue,SelectValues)
						select @SetLocaleId,GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,
								GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName,IsRequired,AttributeTypeName,
								AttributeTypeId,SingleAttributeValue,SelectValues
						from Cte_GetDefaultFilterData
						
						SET @IncrementalId = @IncrementalId +1 
			   End 

			select distinct GlobalAttributeGroupId,GroupCode,AttributeGroupName,AttributeGroupDisplayOrder,LocaleId 
			into #GroupMaster from #GlobalAttributeGrouplist
	
	If @IsPreviewEnable = 1 AND (@RevisionState like '%Preview%'  OR @RevisionState like '%Production%'  ) 
	Begin
	    --Data inserted into flat table ZnodePublishWidgetSliderBannerEntity (Replica of MongoDB Collection )  

		Delete from ZnodePublishPortalGlobalAttributeEntity where  VersionId in (Select PreviewVersionId from @Tbl_PreviewVersionId )
		AND PortalId = @PortalId
	 		 

		Insert Into ZnodePublishPortalGlobalAttributeEntity
		(
			VersionId,PublishStartTime,PortalId,PortalName,LocaleId,GlobalAttributeGroups
		)
			  	
			Select Distinct PV.PreviewVersionId, @GetDate,  @PortalId,@StoreName, GM.LocaleId, (
		    Select 
			A.GlobalAttributeGroupId AS 'GlobalAttributeGroup.GlobalAttributeGroupId',
			A.GroupCode AS 'GlobalAttributeGroup.GroupCode',
			A.AttributeGroupName AS 'GlobalAttributeGroup.AttributeGroupName',
			A.AttributeGroupDisplayOrder AS 'GlobalAttributeGroup.AttributeGroupDisplayOrder',
			A.LocaleId AS 'GlobalAttributeGroup.LocaleId', 
			(
				Select GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName ,IsRequired ,	
				AttributeTypeName,AttributeTypeId,SingleAttributeValue,SelectValues 
				from #GlobalAttributeGrouplist B where A.GlobalAttributeGroupId = B.GlobalAttributeGroupId 
				AND A.LocaleId = B.LocaleId
				For JSON Path 
			) AS 'GlobalAttributeGroup.GlobalAttributes'
			from #GroupMaster A  where A.LocaleId = GM.LocaleId  For Json Path) 
			from #GroupMaster  GM Inner join @Tbl_PreviewVersionId  PV on GM.LocaleId = PV.LocaleId 
	End
	-------------------------- End Preview 
	If (@RevisionState like '%Production%' OR @RevisionState = 'None')
	Begin
		-- Only production version id will process 
		Delete from ZnodePublishPortalGlobalAttributeEntity where  VersionId in (Select ProductionVersionId from @Tbl_ProductionVersionId)
		AND PortalId = @PortalId
	 		 

		Insert Into ZnodePublishPortalGlobalAttributeEntity
		(
			VersionId,PublishStartTime,PortalId,PortalName,LocaleId,GlobalAttributeGroups
		)
			  	
			Select Distinct PV.ProductionVersionId, @GetDate,  @PortalId,@StoreName, GM.LocaleId, (
		    Select 
			A.GlobalAttributeGroupId AS 'GlobalAttributeGroup.GlobalAttributeGroupId',
			A.GroupCode AS 'GlobalAttributeGroup.GroupCode',
			A.AttributeGroupName AS 'GlobalAttributeGroup.AttributeGroupName',
			A.AttributeGroupDisplayOrder AS 'GlobalAttributeGroup.AttributeGroupDisplayOrder',
			A.LocaleId AS 'GlobalAttributeGroup.LocaleId', 
			(
				Select GlobalAttributeId,AttributeDisplayOrder,AttributeCode,AttributeName ,IsRequired ,	
				AttributeTypeName,AttributeTypeId,SingleAttributeValue,SelectValues 
				from #GlobalAttributeGrouplist B where A.GlobalAttributeGroupId = B.GlobalAttributeGroupId 
				AND A.LocaleId = B.LocaleId
				For JSON Path 
			) AS 'GlobalAttributeGroup.GlobalAttributes'
			from #GroupMaster A  where A.LocaleId = GM.LocaleId  For Json Path) 
			from #GroupMaster  GM Inner join @Tbl_ProductionVersionId  PV on GM.LocaleId = PV.LocaleId 

	End
	SET @Status =1 ;



	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE(), ERROR_PROCEDURE();
	
		SET @Status = 0;
		--DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_GetPublishProducts @PublishCatalogId = '+CAST(@PublishCatalogId AS va		rchar(max))+',@PublishCategoryId='+@PublishCategoryId+',@UserId='+CAST(@UserId AS Varchar(50))+',@NotReturnXML='+CAST(@NotReturnXML AS Varchar(50))+',@UserId = '+CAST(@UserId AS Varchar(50))+',@PimProductId='+CAST(@PimProductId AS Varchar(50))+',@VersionI		d='+CAST(@VersionId AS Varchar(50))+',@TokenId='+CAST(@TokenId AS varchar(max))+',@Status='+CAST(@Status AS varchar(10));
		--SELECT 0 AS ID, CAST(0 AS bit) AS Status;
		--ROLLBACK TRAN GetPublishProducts;
		--EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_GetPublishProducts', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;