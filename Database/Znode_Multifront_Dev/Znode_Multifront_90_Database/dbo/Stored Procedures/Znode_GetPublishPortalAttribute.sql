CREATE  PROCEDURE [dbo].[Znode_GetPublishPortalAttribute](
        @PortalId varchar(2000)= ''
	  , @VersionId int= 0
	  , @UserId int
	  , @NotReturnXML int= NULL	  
	  , @IsDebug bit= 0
	  , @TokenId nvarchar(max)= ''
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
			  
			   Select @LocaleId=LocaleId
			   From ZnodePortalLocale
			   Where PortalId=@PortalId
			   and IsDefault=1


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

			

		DELETE FROM ZnodePublishedPortalXml 
		WHERE PublishPortalLogId = @versionId   AND LocaleId = @LocaleId 


		INSERT INTO [dbo].[ZnodePublishedPortalXml]
        ([PublishedXML],PortalId,[PublishPortalLogId],[LocaleId],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
		select (	Select 			
				case 
		 when GROUPING_ID(ss.[GlobalAttributeId]) = 0 then 5 
		 when GROUPING_ID(ss.Attributes) = 0 then 4 
         when GROUPING_ID(AA.[GlobalAttributeGroupId]) = 0 then 3 
		 when GROUPING_ID(aa.Groups) = 0 then 2 
         else 1 
         end as tag,
       case
	     when GROUPING_ID(ss.[GlobalAttributeId]) = 0 then 4 
		 when GROUPING_ID(ss.Attributes) = 0 then 3 
         when GROUPING_ID(aa.[GlobalAttributeGroupId]) = 0 then 2 
		 when GROUPING_ID(aa.Groups) = 0 then 1 
         else null
       end as parent,
       null								as 'PortalGlobalAttributeEntity!1',
	   @PortalId						as 'PortalGlobalAttributeEntity!1!PortalId!element',
	   @StoreName						as 'PortalGlobalAttributeEntity!1!PortalName!element',
	   null								as 'GlobalAttributeGroups!2',
       aa.[GlobalAttributeGroupId]		as 'GlobalAttributeGroupEntity!3!GlobalAttributeGroupId!element', 
	   zgag.GroupCode					as 'GlobalAttributeGroupEntity!3!GroupCode!element',
	   zgagl.AttributeGroupName         as 'GlobalAttributeGroupEntity!3!GroupName!element',
       [AttributeGroupDisplayOrder]		as 'GlobalAttributeGroupEntity!3!AttributeGroupDisplayOrder!element', 
	   null								as 'GlobalAttributes!4',
       ss.[GlobalAttributeId]			as 'GlobalAttributeEntity!5!GlobalAttributeId!element', 
       ss.[AttributeDisplayOrder]		as 'GlobalAttributeEntity!5!DisplayOrder!element',
	   zga.AttributeCode				as 'GlobalAttributeEntity!5!AttributeCode!element',
	   zgal.AttributeName				as 'GlobalAttributeEntity!5!AttributeName!element',
	   zga.IsRequired					as 'GlobalAttributeEntity!5!IsRequired!element',
	   zga.AttributeTypeName            as 'GlobalAttributeEntity!5!AttributeTypeName!element',
	   zga.AttributeTypeId				as 'GlobalAttributeEntity!5!AttributeTypeId!element',
	   sv.SingleAttributeValue			as 'GlobalAttributeEntity!5!AttributeValues!element',
	   ( Select 
	    SelectValuesEntity.DisplayOrder					as DisplayOrder,
	   SelectValuesEntity.GlobalAttributeDefaultValueId as GlobalAttributeDefaultValueId,
		 SelectValuesEntity.AttributeValue				as [Value]	,
	   SelectValuesEntity.AttributeDefaultValueCode		as Code	,
	   SelectValuesEntity.SwatchText                    as SwatchText	,
	   SelectValuesEntity.MediaPath		         		as [Path]	
	    from  @EntityAttributeValueList SelectValuesEntity 
		Where SelectValuesEntity.PortalGlobalAttributeValueId = sv.PortalGlobalAttributeValueId
		  and zga.GroupAttributeType ='Select'
		  FOR XML PATH('SelectValuesEntity'), type
		  ) as 'GlobalAttributeEntity!5!SelectValues!element'
		from @TBL_GlobalAttributeGrouplist aa				
		inner join @TBL_GlobalAttributelist ss on  ss.GlobalAttributeGroupId=aa.GlobalAttributeGroupId
		inner join ZnodeGlobalAttributeGroup zgag on zgag.[GlobalAttributeGroupId]=aa.[GlobalAttributeGroupId]
		inner join ZnodeGlobalAttributeGroupLocale zgagl on zgagl.GlobalAttributeGroupId=zgag.GlobalAttributeGroupId
		inner join View_ZnodeGlobalAttribute zga       on zga.[GlobalAttributeId]=ss.[GlobalAttributeId]
		inner join ZnodeGlobalAttributeLocale zgal on zga.[GlobalAttributeId]=zgal.[GlobalAttributeId]
		inner join @EntityAttributeValueList sv   on sv.[GlobalAttributeId]=ss.[GlobalAttributeId]
		Where zgagl.LocaleId=@LocaleId
		   and zgal.LocaleId=@LocaleId
		group by grouping sets ((),  (aa.Groups),
		(aa.Groups,aa.[GlobalAttributeGroupId] , [AttributeGroupDisplayOrder],zgag.GroupCode, zgag.DisplayOrder,zgagl.AttributeGroupName),
		(aa.Groups,aa.[GlobalAttributeGroupId],ss.Attributes),
		(aa.Groups,aa.[GlobalAttributeGroupId] ,ss.Attributes, ss.[GlobalAttributeId], ss.[AttributeDisplayOrder]	,
		zga.AttributeTypeId,zga.AttributeCode,zgal.AttributeName,zga.IsRequired,zga.IsLocalizable,zga.IsActive,zga.DisplayOrder,zga.AttributeTypeName,
		sv.SingleAttributeValue,zga.GroupAttributeType,sv.PortalGlobalAttributeValueId)
		)
		order by aa.Groups,aa.[GlobalAttributeGroupId]  ,ss.Attributes,ss.[GlobalAttributeId],sv.PortalGlobalAttributeValueId  
	    for xml explicit, type  )as PortalXml,@PortalId,@versionId,@LocaleId,@UserId,@GetDate,@UserId,@GetDate

		Select  [PublishedXML] PortalXMl 
		FROM ZnodePublishedPortalXml 
		WHERE PublishPortalLogId = @versionId   AND LocaleId = @LocaleId 
				
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE(), ERROR_PROCEDURE();
		DECLARE @Status bit;
		SET @Status = 0;
		--DECLARE @Error_procedure varchar(1000)= ERROR_PROCEDURE(), @ErrorMessage nvarchar(max)= ERROR_MESSAGE(), @ErrorLine varchar(100)= ERROR_LINE(), @ErrorCall nvarchar(max)= 'EXEC Znode_GetPublishProducts @PublishCatalogId = '+CAST(@PublishCatalogId AS varchar(max))+',@PublishCategoryId='+@PublishCategoryId+',@UserId='+CAST(@UserId AS Varchar(50))+',@NotReturnXML='+CAST(@NotReturnXML AS Varchar(50))+',@UserId = '+CAST(@UserId AS Varchar(50))+',@PimProductId='+CAST(@PimProductId AS Varchar(50))+',@VersionId='+CAST(@VersionId AS Varchar(50))+',@TokenId='+CAST(@TokenId AS varchar(max))+',@Status='+CAST(@Status AS varchar(10));
		--SELECT 0 AS ID, CAST(0 AS bit) AS Status;
		--ROLLBACK TRAN GetPublishProducts;
		--EXEC Znode_InsertProcedureErrorLog @ProcedureName = 'Znode_GetPublishProducts', @ErrorInProcedure = @Error_procedure, @ErrorMessage = @ErrorMessage, @ErrorLine = @ErrorLine, @ErrorCall = @ErrorCall;
	END CATCH;
END;