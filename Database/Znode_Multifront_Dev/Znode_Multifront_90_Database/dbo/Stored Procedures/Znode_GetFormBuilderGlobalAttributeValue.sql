CREATE   PROCEDURE [dbo].[Znode_GetFormBuilderGlobalAttributeValue]
(
    @FormBuilderId  int=null,
    @UserId			int= null,
	@PortalId		int = null,
	@FormBuilderSubmitId int=null,
    @LocaleId       INT = 0
)
AS
/*
	 Summary :- This procedure is used to get the Attribute and EntityValue attribute value as per filter pass
	 Unit Testing
	 BEGIN TRAN
	 EXEC [Znode_GetFormBuilderGlobalAttributeValue_back] 1
	 ROLLBACK TRAN

*/
     BEGIN
 BEGIN TRY
 declare @EntityValue nvarchar(200),@FormCode nvarchar(200),@DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId()

 If isnull(@LocaleId,0) =0
 Begin
   Set @LocaleId =@DefaultLocaleId
 End

 If isnull( @FormBuilderSubmitId,0) >0
 Select @FormBuilderId=FormBuilderId
 From ZnodeFormBuilderSubmit
 Where FormBuilderSubmitId=@FormBuilderSubmitId

  DECLARE @V_MediaServerThumbnailPath VARCHAR(4000);
          SET @V_MediaServerThumbnailPath =
         (
             SELECT ISNULL(CASE WHEN ZMC.CDNURL = '' THEN NULL ELSE ZMC.CDNURL END,ZMC.URL)+ZMSM.ThumbnailFolderName+'/'  
             FROM ZnodeMediaConfiguration ZMC
			 INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
		     WHERE IsActive = 1
         );
Declare	@AttributeList as	table (GlobalAttributeGroupId int,GlobalAttributeId int,AttributeGroupDisplayOrder int
,AttributeDisplayOrder int)

 Select @EntityValue=FormCode,@FormCode=FormCode
 from ZnodeFormBuilder
 Where FormBuilderId=@FormBuilderId

            Declare	@EntityAttributeList as	table  (GlobalEntityId int,EntityName nvarchar(300),EntityValue nvarchar(max),
			GlobalAttributeGroupId int,GlobalAttributeId int,AttributeTypeId int,AttributeTypeName nvarchar(300),
			 AttributeCode nvarchar(300) ,IsRequired bit,IsLocalizable bit,AttributeName  nvarchar(300) , HelpDescription nvarchar(max)
			,AttributeGroupDisplayOrder int,AttributeDisplayOrder int)

			Declare @EntityAttributeValidationList  as	table
			( GlobalAttributeId int, ControlName nvarchar(300), ValidationName nvarchar(300),SubValidationName nvarchar(300),
			 RegExp nvarchar(300), ValidationValue nvarchar(300),IsRegExp Bit)

			Declare	@EntityAttributeValueList as	table  (GlobalAttributeId int,AttributeValue nvarchar(max),
			GlobalAttributeValueId int,GlobalAttributeDefaultValueId int,AttributeDefaultValueCode nvarchar(300),
			AttributeDefaultValue nvarchar(300),
			MediaId int,MediaPath nvarchar(300) )



			Declare	@EntityAttributeDefaultValueList as	table  (GlobalAttributeDefaultValueId int,GlobalAttributeId int,
			AttributeDefaultValueCode nvarchar(300),AttributeDefaultValue nvarchar(300),RowId int,IsEditable bit,DisplayOrder int )

			insert into @AttributeList
			 Select qq.GlobalAttributeGroupId,isnull(dd.GlobalAttributeId ,qq.GlobalAttributeId) ,qq.DisplayOrder,dd.AttributeDisplayOrder
			   from dbo.ZnodeFormBuilderAttributeMapper  qq
			   left join ZnodeGlobalAttributeGroupMapper dd on dd.GlobalAttributeGroupId=qq.GlobalAttributeGroupId
			   Where qq.FormBuilderId=@FormBuilderId

	insert into @EntityAttributeList
		(	GlobalEntityId ,EntityName ,EntityValue ,
		GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
		AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription ,AttributeGroupDisplayOrder,AttributeDisplayOrder )
		SELECT @FormBuilderId GlobalEntityId,'FormBuilder' EntityName,@EntityValue EntityValue,qq.GlobalAttributeGroupId,
		c.GlobalAttributeId,c.AttributeTypeId,q.AttributeTypeName,c.AttributeCode,c.IsRequired,
		c.IsLocalizable,null AttributeName,c.HelpDescription,qq.AttributeGroupDisplayOrder,qq.AttributeDisplayOrder
     FROM @AttributeList AS qq
          INNER JOIN dbo.ZnodeGlobalAttribute AS c ON qq.GlobalAttributeId = c.GlobalAttributeId
          INNER JOIN dbo.ZnodeAttributeType AS q ON c.AttributeTypeId = q.AttributeTypeId

		  update c
		  Set AttributeName=f.AttributeName
		  from  @EntityAttributeList c
		  INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
		   where  f.LocaleId=@LocaleId

		 if  @LocaleId !=@DefaultLocaleId
		 Begin
				update c
				Set AttributeName=f.AttributeName
				from  @EntityAttributeList c
				INNER JOIN dbo.ZnodeGlobalAttributeLocale AS f ON c.GlobalAttributeId = f.GlobalAttributeId
				Where c.AttributeName is null
				and f.LocaleId=@DefaultLocaleId
		 End


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
		  Select GlobalAttributeId,aa.FormBuilderGlobalAttributeValueId,bb.GlobalAttributeDefaultValueId,
		  case when bb.MediaPath is not null then  bb.MediaPath  else bb.AttributeValue end,
		  bb.MediaId,bb.MediaPath
		  from  dbo.ZnodeFormBuilderSubmit ss
		  inner join dbo.ZnodeFormBuilderGlobalAttributeValue aa on ss.FormBuilderSubmitId =aa.FormBuilderSubmitId
		  inner join ZnodeFormBuilderGlobalAttributeValueLocale bb ON bb.FormBuilderGlobalAttributeValueId = aa.FormBuilderGlobalAttributeValueId
		  Where  ss.FormBuilderId=@FormBuilderId
		  and ss.FormBuilderSubmitId=@FormBuilderSubmitId

		  update aa
		  Set AttributeDefaultValueCode= h.AttributeDefaultValueCode,
         	  GlobalAttributeDefaultValueId=h.GlobalAttributeDefaultValueId,
			  AttributeValue=case when aa.AttributeValue is  null then h.AttributeDefaultValueCode else aa.AttributeValue end
		  from  @EntityAttributeValueList aa
		  inner JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeDefaultValueId = aa.GlobalAttributeDefaultValueId

		  update h
		  Set AttributeDefaultValue=g.AttributeDefaultValue
		  from  @EntityAttributeValueList h
		  inner JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
          where  g.LocaleId=@LocaleId

		 if  @LocaleId !=@DefaultLocaleId
		 Begin
				update h
				Set AttributeDefaultValue=g.AttributeDefaultValue
				from  @EntityAttributeValueList h
				inner JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
                Where h.AttributeDefaultValue is null
				and g.LocaleId=@DefaultLocaleId
		 End


		  insert into @EntityAttributeDefaultValueList
		  (GlobalAttributeDefaultValueId,GlobalAttributeId,AttributeDefaultValueCode,
			AttributeDefaultValue ,RowId ,IsEditable ,DisplayOrder )
		  Select  h.GlobalAttributeDefaultValueId, aa.GlobalAttributeId,h.AttributeDefaultValueCode,null AttributeDefaultValue,0,ISNULL(h.IsEditable, 1),
		  h.DisplayOrder
		  from  @EntityAttributeList aa
		  inner JOIN dbo.ZnodeGlobalAttributeDefaultValue h ON h.GlobalAttributeId = aa.GlobalAttributeId

		  update h
		  Set h.AttributeDefaultValue=g.AttributeDefaultValue
          from @EntityAttributeDefaultValueList h
		  inner JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
		  Where g.LocaleId=@LocaleId

		  if  @LocaleId !=@DefaultLocaleId
		 Begin
				  update h
				  Set h.AttributeDefaultValue=g.AttributeDefaultValue
				  from @EntityAttributeDefaultValueList h
				  inner JOIN dbo.ZnodeGlobalAttributeDefaultValueLocale g ON h.GlobalAttributeDefaultValueId = g.GlobalAttributeDefaultValueId
				  Where g.LocaleId=@DefaultLocaleId
				  and  h.AttributeDefaultValue is null
		 End



		  if not exists (Select 1 from @EntityAttributeList )
			Begin
			insert into @EntityAttributeList
			(	GlobalEntityId ,EntityName ,EntityValue ,
			GlobalAttributeGroupId ,GlobalAttributeId ,AttributeTypeId ,AttributeTypeName ,
			AttributeCode  ,IsRequired ,IsLocalizable ,AttributeName,HelpDescription  )
			SELECT 0 GlobalEntityId,'FormBuilder' EntityName,@EntityValue EntityValue,0 GlobalAttributeGroupId,
			0 GlobalAttributeId,0 AttributeTypeId,''AttributeTypeName,''AttributeCode,0 IsRequired,
			0 IsLocalizable,'' AttributeName,'' HelpDescription
			End



			SELECT  GlobalEntityId,EntityName,EntityValue,GlobalAttributeGroupId,
			AA.GlobalAttributeId,AttributeTypeId,AttributeTypeName,AttributeCode,IsRequired,
			IsLocalizable,AttributeName,ControlName,ValidationName,SubValidationName,RegExp,
			ValidationValue,cast(isnull(IsRegExp,0) as bit)  IsRegExp,
			HelpDescription,AttributeValue,GlobalAttributeValueId,bb.GlobalAttributeDefaultValueId,
			aab.AttributeDefaultValueCode,
			aab.AttributeDefaultValue,isnull(aab.RowId,0)   RowId,cast(isnull(aab.IsEditable,0) as bit)   IsEditable
			,bb.MediaId--,aa.AttributeGroupDisplayOrder,aa.AttributeDisplayOrder
			fROM @EntityAttributeList AA
			left join @EntityAttributeDefaultValueList aab on aab.GlobalAttributeId=AA.GlobalAttributeId
			left join @EntityAttributeValidationList vl on vl.GlobalAttributeId=aa.GlobalAttributeId
			LEFT JOIN @EntityAttributeValueList BB ON BB.GlobalAttributeId=AA.GlobalAttributeId
		    and ( (aab.GlobalAttributeDefaultValueId=bb.GlobalAttributeDefaultValueId	)
			or  ( bb.MediaId is not null and isnull(vl.ValidationName,'')='IsAllowMultiUpload'  and bb.GlobalAttributeDefaultValueId is null )
			or  ( bb.MediaId is  null and  bb.GlobalAttributeDefaultValueId is null ))
			order by GlobalEntityId,AttributeGroupDisplayOrder,GlobalAttributeGroupId,aa.AttributeDisplayOrder, GlobalAttributeId,aab.DisplayOrder,aab.GlobalAttributeDefaultValueId


			--Select CMSWidgetsId,WidgetsKey,CMSMappingId,TypeOFMapping,FormBuilderId,
			--		FormTitle,ButtonText,IsTextMessage,TextMessage,RedirectURL
			--from ZnodeCMSFormWidgetConfiguration
			--where FormBuilderId=@FormBuilderId

			SELECT 0 AS ID,CAST(1 AS BIT) AS Status;
		  END TRY
         BEGIN CATCH
		 SELECT ERROR_MESSAGE()
             DECLARE @Status BIT ;
		  SET @Status = 0;
		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
		   @ErrorLine VARCHAR(100)= ERROR_LINE(),
		    @ErrorCall NVARCHAR(MAX)= null
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

          EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetGlobalEntityValueAttributeValues',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
         END CATCH;
     END;