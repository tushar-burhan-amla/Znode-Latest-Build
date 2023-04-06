


CREATE PROCEDURE [dbo].[Znode_InsertUpdateFormBuilderGlobalAttributeValue]
(   @GlobalEntityValueXml NVARCHAR(max),
	@FormBuilderId     int,
	@LocaleId          int,
	@PortalId          int=0,
    @UserId            INT  )
AS
   /*
     Summary : To Insert / Update single Entity with multiple attribute values 
     Update Logic: 
*/
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
		     Declare  @status            BIT,  @GlobalEntityValueId int,@NewUserId int,
			 @FormBuilderSubmitId int , @GlobalEntityValueDetail  [GlobalEntityValueDetail]

			 If isnull(@UserId,0)=0
			 Begin
			 Set @UserId=-1
			 End
			 Else
			 Set @NewUserId=@UserId


			 
 Select @GlobalEntityValueId=FormBuilderId 
 from ZnodeFormBuilder
 Where FormBuilderId=@FormBuilderId


			 DECLARE @ConvertedXML XML = REPLACE(REPLACE(REPLACE(@GlobalEntityValueXml,' & ', '&amp;'),'"', '&quot;'),'''', '&apos;')
             
             INSERT INTO @GlobalEntityValueDetail
			 (GlobalAttributeId,GlobalAttributeValueId,GlobalAttributeDefaultValueId,AttributeCode,AttributeValue
			 ,LocaleId,GlobalEntityValueId)
			SELECT Tbl.Col.value('GlobalAttributeId[1]', 'int') AS GlobalAttributeId,
			Tbl.Col.value('GlobalAttributeValueId[1]', 'int') AS GlobalAttributeValueId,
			Tbl.Col.value('GlobalAttributeDefaultValueId[1]', 'int') AS GlobalAttributeDefaultValueId,
			Tbl.Col.value('AttributeCode[1]', 'NVARCHAR(300)') AS AttributeCode,
			Tbl.Col.value('AttributeValue[1]', 'NVARCHAR(300)') AS AttributeValue,
			Tbl.Col.value('LocaleId[1]', 'INT') AS LocaleId,
			@GlobalEntityValueId AS GlobalEntityValueId
			FROM @ConvertedXML.nodes('//ArrayOfFormSubmitAttributeModel/FormSubmitAttributeModel') AS Tbl(Col);

			 DECLARE @GlobalEntityId INT,
			  @MultiSelectGroupAttributeTypeName nvarchar(200)='Select'
			 ,@MediaGroupAttributeTypeName nvarchar(200)='Media'
             DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
			 DECLARE @TBL_FormBuilder TABLE (FormBuilderId [int] NULL)
			 DECLARE @TBL_DeleteFormBuilder TABLE (FormBuilderId [int] NULL,FormBuilderGlobalAttributeValueId int)
			 DECLARE @TBL_AttributeDefaultValueList TABLE 
			   (NewFormBuilderGlobalAttributeValueId int,FormBuilderGlobalAttributeValueId int,[AttributeValue] [varchar](300),[GlobalAttributeDefaultValueId] int,
			   [GlobalAttributeId] int,MediaId int,FormBuilderGlobalAttributeValueLocaleId int)

			 DECLARE @TBL_MediaValueList TABLE 
			   (NewFormBuilderGlobalAttributeValueId int,FormBuilderGlobalAttributeValueId int,GlobalAttributeId int,
			   MediaId int,MediaPath nvarchar(300),FormBuilderGlobalAttributeValueLocaleId int)
			 DECLARE @TBL_InsertGlobalEntityValue TABLE 
				([GlobalAttributeId] [int] NULL,GlobalAttributeDefaultValueId [int] NULL,FormBuilderId [int] NULL,
					FormBuilderGlobalAttributeValueId int null)
		 	 DECLARE @TBL_GlobalEntityValueDetail TABLE ([GlobalAttributeId] [int] NULL,
				[AttributeCode] [varchar](300),[GlobalAttributeDefaultValueId] [int],[GlobalAttributeValueId] [int],
				[LocaleId] [int],FormBuilderId [int],[AttributeValue] [nvarchar](max),FormBuilderGlobalAttributeValueId int,
				NewFormBuilderGlobalAttributeValueId int,GroupAttributeTypeName [varchar](300))

				--SELECT TOP 1 @LocaleId = LocaleId
				-- FROM @GlobalEntityValueDetail;

				Insert into @TBL_GlobalEntityValueDetail
				([GlobalAttributeId],[AttributeCode],[GlobalAttributeDefaultValueId],
				[GlobalAttributeValueId],[LocaleId],FormBuilderId,[AttributeValue],GroupAttributeTypeName)
				Select dd.[GlobalAttributeId],dd.[AttributeCode],case when [GlobalAttributeDefaultValueId]=0 then null else 
				[GlobalAttributeDefaultValueId] end [GlobalAttributeDefaultValueId],
				case when [GlobalAttributeValueId]=0 then null else 
				[GlobalAttributeValueId] end [GlobalAttributeValueId],[LocaleId],[GlobalEntityValueId],[AttributeValue],ss.GroupAttributeType
				From @GlobalEntityValueDetail dd
				inner join [View_ZnodeGlobalAttribute] ss on ss.GlobalAttributeId=dd.GlobalAttributeId

				
				insert into @TBL_FormBuilder(FormBuilderId)
				Select distinct  FormBuilderId from @TBL_GlobalEntityValueDetail;

               INSERT INTO [dbo].[ZnodeFormBuilderSubmit]
               ([FormBuilderId],LocaleId,[PortalId],[UserId],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
			   Select @GlobalEntityValueId,@LocaleId,@PortalId,@NewUserId,@UserId [CreatedBy],@GetDate [CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]

			   Set @FormBuilderSubmitId =SCOPE_IDENTITY()

				INSERT INTO [dbo].[ZnodeFormBuilderGlobalAttributeValue]
				(FormBuilderSubmitId,[GlobalAttributeId],[GlobalAttributeDefaultValueId],[CreatedBy],[CreatedDate],
				[ModifiedBy],[ModifiedDate])
				 output Inserted.GlobalAttributeId,inserted.[GlobalAttributeDefaultValueId],@GlobalEntityValueId,
				 inserted.FormBuilderGlobalAttributeValueId into @TBL_InsertGlobalEntityValue
				Select @FormBuilderSubmitId,[GlobalAttributeId],[GlobalAttributeDefaultValueId]
				,@UserId [CreatedBy],@GetDate [CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE FormBuilderGlobalAttributeValueId IS NULL				

            
				Update dd
				Set dd.NewFormBuilderGlobalAttributeValueId=ss.FormBuilderGlobalAttributeValueId
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_InsertGlobalEntityValue ss on dd.[FormBuilderId]=ss.[FormBuilderId]
				and dd.GlobalAttributeId=ss.GlobalAttributeId	
				

				INSERT INTO [dbo].[ZnodeFormBuilderGlobalAttributeValueLocale]
			   ([FormBuilderGlobalAttributeValueId],[LocaleId],[AttributeValue],[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select NewFormBuilderGlobalAttributeValueId,[LocaleId],[AttributeValue],@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE NewFormBuilderGlobalAttributeValueId IS not NULL
				and isnull([AttributeValue],'') <>''    
				and isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
				and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName		
				
				

				insert into @TBL_AttributeDefaultValueList
				(NewFormBuilderGlobalAttributeValueId,FormBuilderGlobalAttributeValueId,dd.AttributeValue,GlobalAttributeId)
				Select dd.NewFormBuilderGlobalAttributeValueId, dd.FormBuilderGlobalAttributeValueId,ss.Item,dd.GlobalAttributeId
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

				Update dd
				Set dd.GlobalAttributeDefaultValueId=ss.GlobalAttributeDefaultValueId
				from  @TBL_AttributeDefaultValueList DD
				inner join [ZnodeGlobalAttributeDefaultValue] ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and dd.AttributeValue=ss.AttributeDefaultValueCode

				
				

				INSERT INTO [dbo].[ZnodeFormBuilderGlobalAttributeValueLocale]
			   ([FormBuilderGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewFormBuilderGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewFormBuilderGlobalAttributeValueId=dd.NewFormBuilderGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

				


				insert into @TBL_MediaValueList
				(NewFormBuilderGlobalAttributeValueId,FormBuilderGlobalAttributeValueId,GlobalAttributeId,MediaPath)
				Select dd.NewFormBuilderGlobalAttributeValueId, dd.FormBuilderGlobalAttributeValueId,GlobalAttributeId,ss.Item 
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

			
				

				INSERT INTO [dbo].[ZnodeFormBuilderGlobalAttributeValueLocale]
			   ([FormBuilderGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewFormBuilderGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewFormBuilderGlobalAttributeValueId=dd.NewFormBuilderGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

													    
		
		     SELECT 0 AS ID,CAST(1 AS BIT) AS Status;    
			   
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             --SELECT ERROR_MESSAGE()
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportInsertUpdateGlobalEntity @UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT @FormBuilderSubmitId AS ID,CAST(0 AS BIT) AS Status;                    
			ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportInsertUpdateGlobalEntity',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;