CREATE PROCEDURE [dbo].[Znode_GetWidgetGlobalAttributeValue]
(
    @EntityName       nvarchar(200) = 0,
    @GlobalEntityValueId   INT = 0,
	@LocaleCode       VARCHAR(100) = '',
    @GroupCode  nvarchar(200) = null,
    @SelectedValue bit = 0
)
AS
 BEGIN
 BEGIN TRY
 declare @EntityValue nvarchar(200), @LocaleId int, @WidgetId int, @DefaultLocaleId INT

 

  DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);
          SET @V_MediaServerThumbnailPath =
         (
             SELECT ISNULL(CASE WHEN CDNURL = '' THEN NULL ELSE CDNURL END,URL) 
             FROM ZnodeMediaConfiguration ZMC 
			 --INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
		     WHERE IsActive = 1 
         );

 Select @EntityValue= Name,@WidgetId = CCW.CMSContentContainerId from ZnodeCMSContentContainer CCW
 inner join ZnodeCMSContainerProfileVariant CWPV  on CCW.CMSContentContainerId = CWPV.CMSContentContainerId
 Where CWPV.CMSContainerProfileVariantId = @GlobalEntityValueId

 	DECLARE @GlobalFamilyId int
	SET  @GlobalFamilyId = (select top 1 FM.GlobalAttributeFamilyId from ZnodeGlobalEntity GE inner join  ZnodeGlobalEntityFamilyMapper FM
	on GE.GlobalEntityId = FM.GlobalEntityId
	where GE.EntityName =  @EntityName and  (FM.GlobalEntityValueId = @GlobalEntityValueId ))
  

            Declare	@EntityAttributeList as	table  (GlobalEntityId int,EntityName nvarchar(300),EntityValue nvarchar(max),
			GlobalAttributeGroupId int,GlobalAttributeId int,AttributeTypeId int,AttributeTypeName nvarchar(300),
			 AttributeCode nvarchar(300) ,IsRequired bit,IsLocalizable bit,AttributeName  nvarchar(300) , HelpDescription nvarchar(max),DisplayOrder int
			) 
			 
			Declare @EntityAttributeValidationList  as	table  
			( GlobalAttributeId int, ControlName nvarchar(300), ValidationName nvarchar(300),SubValidationName nvarchar(300),
			 RegExp nvarchar(300), ValidationValue nvarchar(300),IsRegExp Bit)

			Declare	@EntityAttributeValueList as	table  (GlobalAttributeId int,AttributeValue nvarchar(max),
			GlobalAttributeValueId int,GlobalAttributeDefaultValueId int,AttributeDefaultValueCode nvarchar(300),
			AttributeDefaultValue nvarchar(300),
			MediaId int,MediaPath nvarchar(300),IsEditable bit,DisplayOrder int )



			Declare	@EntityAttributeDefaultValueList as	table  (GlobalAttributeDefaultValueId int,GlobalAttributeId int,
			AttributeDefaultValueCode nvarchar(300),AttributeDefaultValue nvarchar(300),RowId int,IsEditable bit,DisplayOrder int )

			set @LocaleId = (select top 1 LocaleId from ZnodeLocale where Code = @LocaleCode)
			set @DefaultLocaleId = (select top 1 LocaleId from ZnodeLocale WHERE IsDefault = 1)

			if isnull(@LocaleId,0)=0
				set @LocaleId = (select top 1 LocaleId from ZnodeLocale WHERE IsDefault = 1)

            IF ISnull(@GroupCode, '') = ''
            Begin
			
				insert into @EntityAttributeList
					(	GlobalEntityId ,EntityName ,EntityValue ,
					GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
					AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription,DisplayOrder  ) 
				SELECT qq.GlobalEntityId,qq.EntityName,@EntityValue EntityValue,ww.GlobalAttributeGroupId,
					c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
					c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
				 FROM dbo.ZnodeGlobalEntity AS qq
					  INNER JOIN dbo.ZnodeGlobalAttributeFamily AS w ON qq.GlobalEntityId = w.GlobalEntityId
					  INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = w.GlobalAttributeFamilyId
					  INNER JOIN dbo.ZnodeGlobalAttributeGroupMapper AS ww ON ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId
					  INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
					  INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
					  INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
					  Where qq.EntityName=@EntityName AND ( f.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
					  and w.GlobalAttributeFamilyId = @GlobalFamilyId

				insert into @EntityAttributeList
				SELECT qq.GlobalEntityId,qq.EntityName,@EntityValue EntityValue,ww.GlobalAttributeGroupId,
					c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
					c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
				 FROM dbo.ZnodeGlobalEntity AS qq
					  INNER JOIN dbo.ZnodeGlobalAttributeFamily AS w ON qq.GlobalEntityId = w.GlobalEntityId
					  INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = w.GlobalAttributeFamilyId
					  INNER JOIN dbo.ZnodeGlobalAttributeGroupMapper AS ww ON ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId
					  INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
					  INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
					  INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
					  Where qq.EntityName=@EntityName AND ( f.LocaleId = isnull(@DefaultLocaleId, 0 ) or isnull(@DefaultLocaleId,0) = 0 )
					  and w.GlobalAttributeFamilyId = @GlobalFamilyId
					AND NOT EXISTS(SELECT * FROM @EntityAttributeList Z
						WHERE qq.GlobalEntityId = Z.GlobalEntityId AND qq.EntityName = Z.EntityName AND c.GlobalAttributeId = Z.GlobalAttributeId)

				IF NOT EXISTS(SELECT * FROM @EntityAttributeList)
				BEGIN
					insert into @EntityAttributeList
					(	GlobalEntityId ,EntityName ,EntityValue ,
					GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
					AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription,DisplayOrder  ) 
					SELECT qq.GlobalEntityId,qq.EntityName,@EntityValue EntityValue,ww.GlobalAttributeGroupId,
						c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
						c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
					 FROM dbo.ZnodeGlobalEntity AS qq
					  INNER JOIN dbo.ZnodeGlobalAttributeFamily AS w ON qq.GlobalEntityId = w.GlobalEntityId
					  INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = w.GlobalAttributeFamilyId
					  INNER JOIN dbo.ZnodeGlobalAttributeGroupMapper AS ww ON ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId
					  INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
					  INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
					  INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
					  Where qq.EntityName=@EntityName ---AND ( f.LocaleId = isnull(@DefaultLocaleId, 0 ) or isnull(@DefaultLocaleId,0) = 0 )
					  --and w.GlobalAttributeFamilyId = @GlobalFamilyId
				END
			
			
			END
			Else

               Begin
                       insert into @EntityAttributeList
                               ( GlobalEntityId ,EntityName ,EntityValue ,
                               GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
                               AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription,DisplayOrder  )
                               SELECT qq.GlobalEntityId,qq.EntityName,@EntityValue EntityValue,ww.GlobalAttributeGroupId,
                               c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
                               c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
                        FROM dbo.ZnodeGlobalEntity AS qq
						INNER JOIN dbo.ZnodeGlobalAttributeFamily AS w ON qq.GlobalEntityId = w.GlobalEntityId
					  INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = w.GlobalAttributeFamilyId
					  INNER JOIN dbo.ZnodeGlobalAttributeGroupMapper AS ww ON ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId
					  INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
					  INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
					  INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
					  Where qq.EntityName=@EntityName AND ( f.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
					  and w.GlobalAttributeFamilyId = @GlobalFamilyId	
                                 AND exists( select 1 from ZnodeGlobalAttributeGroup g where ww.GlobalAttributeGroupId = g.GlobalAttributeGroupId and g.GroupCode = @GroupCode )	
        
					IF NOT EXISTS(SELECT * FROM @EntityAttributeList)
					BEGIN
						insert into @EntityAttributeList
                               ( GlobalEntityId ,EntityName ,EntityValue ,
                               GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
                               AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription,DisplayOrder  )
                               SELECT qq.GlobalEntityId,qq.EntityName,@EntityValue EntityValue,ww.GlobalAttributeGroupId,
                               c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
                               c.IsLocalizable,f.AttributeName,c.HelpDescription,c.DisplayOrder
						FROM dbo.ZnodeGlobalEntity AS qq
						INNER JOIN dbo.ZnodeGlobalAttributeFamily AS w ON qq.GlobalEntityId = w.GlobalEntityId
						  INNER JOIN dbo.ZnodeGlobalFamilyGroupMapper AS FGM ON FGM.GlobalAttributeFamilyId = w.GlobalAttributeFamilyId
						  INNER JOIN dbo.ZnodeGlobalAttributeGroupMapper AS ww ON ww.GlobalAttributeGroupId = FGM.GlobalAttributeGroupId
						  INNER JOIN dbo.ZnodeGlobalAttribute AS c ON ww.GlobalAttributeId = c.GlobalAttributeId
						  INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId
						  INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
						  Where qq.EntityName=@EntityName --AND ( f.LocaleId = isnull(@DefaultLocaleId, 0 ) or isnull(@DefaultLocaleId,0) = 0 )
						  --and w.GlobalAttributeFamilyId = @GlobalFamilyId	
                          AND exists( select 1 from ZnodeGlobalAttributeGroup g where ww.GlobalAttributeGroupId = g.GlobalAttributeGroupId and g.GroupCode = @GroupCode )	
        
					END
			   
			   
			   END


		  INSERT INTO @EntityAttributeValidationList
		  (GlobalAttributeId,ControlName , ValidationName ,SubValidationName ,
		RegExp, ValidationValue,IsRegExp)

		

		 Select aa.GlobalAttributeId,i.ControlName,i.Name AS ValidationName,j.ValidationName AS SubValidationName,
		j.RegExp,k.Name AS ValidationValue,CAST(CASE WHEN j.RegExp IS NULL THEN 0 ELSE 1 END AS BIT) AS IsRegExp
		
		fROM @EntityAttributeList aa
		  inner  JOIN dbo.ZnodeGlobalAttributeValidation AS k ON k.GlobalAttributeId = aa.GlobalAttributeId
          inner  JOIN dbo.ZnodeAttributeInputValidation AS i ON k.InputValidationId = i.InputValidationId
          LEFT  JOIN dbo.ZnodeAttributeInputValidationRule AS j ON k.InputValidationRuleId = j.InputValidationRuleId

		  insert into @EntityAttributeValueList
		  (GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,AttributeValue ,MediaId,MediaPath)
		  Select DISTINCT GlobalAttributeId,aa.WidgetGlobalAttributeValueId,bb.GlobalAttributeDefaultValueId,
		  case when bb.MediaPath is not null then  @V_MediaServerThumbnailPath+bb.MediaPath--+'~'+convert(nvarchar(10),bb.MediaId) 
		  else bb.AttributeValue end,		  
		  bb.MediaId,bb.MediaPath
		  from  dbo.ZnodeWidgetGlobalAttributeValue aa
		   inner join ZnodeWidgetGlobalAttributeValueLocale bb ON bb.WidgetGlobalAttributeValueId = aa.WidgetGlobalAttributeValueId 
		  Where  aa.CMSContainerProfileVariantId=@GlobalEntityValueId and aa.CMSContentContainerId = @WidgetId		
		  AND ( bb.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
	

		  update aa
		  Set AttributeDefaultValueCode= h.AttributeDefaultValueCode,
              AttributeDefaultValue=g.AttributeDefaultValue,
			  GlobalAttributeDefaultValueId=g.GlobalAttributeDefaultValueId,
			  AttributeValue=case when aa.AttributeValue is  null then h.AttributeDefaultValueCode else aa.AttributeValue end,
			  IsEditable = ISNULL(h.IsEditable, 1),DisplayOrder = h.DisplayOrder
		  from  @EntityAttributeValueList aa
		  inner JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeDefaultValueId = aa.GlobalAttributeDefaultValueId                                       
          inner JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
		  WHERE ( G.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
          
		  insert into @EntityAttributeDefaultValueList
		  (GlobalAttributeDefaultValueId,GlobalAttributeId,AttributeDefaultValueCode,
			AttributeDefaultValue ,RowId ,IsEditable ,DisplayOrder )
		  Select  h.GlobalAttributeDefaultValueId, aa.GlobalAttributeId,h.AttributeDefaultValueCode,g.AttributeDefaultValue,0,ISNULL(h.IsEditable, 1),
		  h.DisplayOrder
		  from  @EntityAttributeList aa
		  inner JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeId = aa.GlobalAttributeId
          inner JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
		  WHERE ( G.LocaleId = isnull(@LocaleId, 0 ) or isnull(@LocaleId,0) = 0 )
		 
			if not exists (Select 1 from @EntityAttributeList )
			Begin
			insert into @EntityAttributeList
			(	GlobalEntityId ,EntityName ,EntityValue ,
			GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
			AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription  ) 
			SELECT qq.GlobalEntityId,qq.EntityName,@EntityValue EntityValue,0 GlobalAttributeGroupId,
			0 GlobalAttributeId,0 AttributeTypeId,''AttributeTypeName,''AttributeCode,0 IsRequired,
			0 IsLocalizable,'' AttributeName,'' HelpDescription
			FROM dbo.ZnodeGlobalEntity AS qq
			 Where qq.EntityName=@EntityName 
			End
				

			SELECT GlobalEntityId,EntityName,EntityValue,GlobalAttributeGroupId,
			AA.GlobalAttributeId,AttributeTypeId,AttributeTypeName,AttributeCode,IsRequired,
			IsLocalizable,AttributeName,ControlName,ValidationName,SubValidationName,RegExp,
			ValidationValue,cast(isnull(IsRegExp,0) as bit)  IsRegExp,
			HelpDescription,AttributeValue,GlobalAttributeValueId,bb.GlobalAttributeDefaultValueId,
			aab.AttributeDefaultValueCode,
			aab.AttributeDefaultValue,isnull(aab.RowId,0)   RowId,cast(isnull(aab.IsEditable,0) as bit)   IsEditable
			,bb.MediaId,AA.DisplayOrder
			fROM @EntityAttributeList AA				
			left join @EntityAttributeDefaultValueList aab on aab.GlobalAttributeId=AA.GlobalAttributeId	
			left join @EntityAttributeValidationList vl on vl.GlobalAttributeId=aa.GlobalAttributeId			
			LEFT JOIN @EntityAttributeValueList BB ON BB.GlobalAttributeId=AA.GlobalAttributeId		 
		    and ( (ISNULL(aab.GlobalAttributeDefaultValueId,0)=ISNULL(bb.GlobalAttributeDefaultValueId,0))
			or  ( bb.MediaId is not null and isnull(vl.ValidationName,'')='IsAllowMultiUpload'  and bb.GlobalAttributeDefaultValueId is null )
			or  ( bb.MediaId is  null and  bb.GlobalAttributeDefaultValueId is null ))
			order by AA.DisplayOrder, aab.DisplayOrder

			SELECT 1 AS ID,CAST(1 AS BIT) AS Status;       
		  END TRY
         BEGIN CATCH
		 SELECT ERROR_MESSAGE()
             DECLARE @Status BIT ;
		  SET @Status = 0;

		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
		   @ErrorLine VARCHAR(100)= ERROR_LINE(),
		   @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetWidgetGlobalAttributeValue 
		   @EntityName = '+@EntityName+',@GlobalEntityValueId='+CAST(@GlobalEntityValueId AS VARCHAR(10))+',@LocaleCode='+@LocaleCode+',
		   @GroupCode = '+@GroupCode+',@SelectedValue = '+CAST(@SelectedValue AS VARCHAR(10));   
		 
          EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetWidgetGlobalAttributeValue',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
         END CATCH;
     END;