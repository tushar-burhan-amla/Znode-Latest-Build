

CREATE PROCEDURE [dbo].[Znode_ImportInsertUpdateUserGlobalAttributeValue]
(
    @GlobalEntityValueDetail  [GlobalEntityValueDetail] READONLY,
    @UserId            INT       ,
    @status            BIT    OUT,
    @IsNotReturnOutput BIT    = 0 )
AS
   /*
     Summary : To Insert / Update single Entity with multiple attribute values 
     Update Logic: 
*/
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
			 DECLARE @GlobalEntityId INT,
			  @MultiSelectGroupAttributeTypeName nvarchar(200)='Select'
			 ,@MediaGroupAttributeTypeName nvarchar(200)='Media'
             DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
			 DECLARE @LocaleId INT 
			 DECLARE @TBL_User TABLE (UserId [int] NULL)
			 DECLARE @TBL_DeleteUser TABLE (UserId [int] NULL,UserGlobalAttributeValueId int)
			 DECLARE @TBL_AttributeDefaultValueList TABLE 
			   (NewUserGlobalAttributeValueId int,UserGlobalAttributeValueId int,[AttributeValue] [varchar](300),[GlobalAttributeDefaultValueId] int,
			   [GlobalAttributeId] int,MediaId int,UserGlobalAttributeValueLocaleId int)

			 DECLARE @TBL_MediaValueList TABLE 
			   (NewUserGlobalAttributeValueId int,UserGlobalAttributeValueId int,GlobalAttributeId int,
			   MediaId int,MediaPath nvarchar(300),UserGlobalAttributeValueLocaleId int)
			 DECLARE @TBL_InsertGlobalEntityValue TABLE 
				([GlobalAttributeId] [int] NULL,GlobalAttributeDefaultValueId [int] NULL,UserId [int] NULL,
					UserGlobalAttributeValueId int null)
		 	 DECLARE @TBL_GlobalEntityValueDetail TABLE ([GlobalAttributeId] [int] NULL,
				[AttributeCode] [varchar](300),[GlobalAttributeDefaultValueId] [int],[GlobalAttributeValueId] [int],
				[LocaleId] [int],UserId [int],[AttributeValue] [nvarchar](max),UserGlobalAttributeValueId int,
				NewUserGlobalAttributeValueId int,GroupAttributeTypeName [varchar](300))

				SELECT TOP 1 @LocaleId = LocaleId FROM @GlobalEntityValueDetail;

				Insert into @TBL_GlobalEntityValueDetail
				([GlobalAttributeId],[AttributeCode],[GlobalAttributeDefaultValueId],
				[GlobalAttributeValueId],[LocaleId],UserId,[AttributeValue],GroupAttributeTypeName)
				Select dd.[GlobalAttributeId],dd.[AttributeCode],case when [GlobalAttributeDefaultValueId]=0 then null else 
				[GlobalAttributeDefaultValueId] end [GlobalAttributeDefaultValueId],
				case when [GlobalAttributeValueId]=0 then null else 
				[GlobalAttributeValueId] end [GlobalAttributeValueId],[LocaleId],[GlobalEntityValueId],[AttributeValue],ss.GroupAttributeType
				From @GlobalEntityValueDetail dd
				inner join [View_ZnodeGlobalAttribute] ss on ss.GlobalAttributeId=dd.GlobalAttributeId

				Update ss
				Set ss.UserGlobalAttributeValueId=dd.UserGlobalAttributeValueId
				From @TBL_GlobalEntityValueDetail ss
				inner join ZnodeUserGlobalAttributeValue dd on dd.UserId=ss.UserId
				and dd.GlobalAttributeId=ss.GlobalAttributeId
				
				insert into @TBL_User(UserId)
				Select distinct  UserId from @TBL_GlobalEntityValueDetail;

                insert into @TBL_DeleteUser
				Select p.UserId,a.UserGlobalAttributeValueId
				from ZnodeUserGlobalAttributeValue a
				inner join @TBL_User p on p.UserId=a.UserId
				Where not exists(select 1 from @TBL_GlobalEntityValueDetail dd 
				where dd.UserId=a.UserId and dd.GlobalAttributeId=a.GlobalAttributeId)
				
				               
				Delete From ZnodeUserGlobalAttributeValueLocale
				WHere exists (select 1 from @TBL_DeleteUser dd 
				Where dd.UserGlobalAttributeValueId=ZnodeUserGlobalAttributeValueLocale.UserGlobalAttributeValueId)

				Delete From ZnodeUserGlobalAttributeValue
				WHere exists (select 1 from @TBL_DeleteUser dd 
				Where dd.UserGlobalAttributeValueId=ZnodeUserGlobalAttributeValue.UserGlobalAttributeValueId)
							

				INSERT INTO [dbo].[ZnodeUserGlobalAttributeValue]
				([UserId],[GlobalAttributeId],[GlobalAttributeDefaultValueId],[CreatedBy],[CreatedDate],
				[ModifiedBy],[ModifiedDate])
				 output Inserted.GlobalAttributeId,inserted.[GlobalAttributeDefaultValueId],inserted.UserId,
				 inserted.UserGlobalAttributeValueId into @TBL_InsertGlobalEntityValue
				Select [UserId],[GlobalAttributeId],[GlobalAttributeDefaultValueId]
				,@UserId [CreatedBy],@GetDate [CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE UserGlobalAttributeValueId IS NULL				

            
				Update dd
				Set dd.NewUserGlobalAttributeValueId=ss.UserGlobalAttributeValueId
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_InsertGlobalEntityValue ss on dd.[UserId]=ss.[UserId]
				and dd.GlobalAttributeId=ss.GlobalAttributeId				

				INSERT INTO [dbo].[ZnodeUserGlobalAttributeValueLocale]
			   ([UserGlobalAttributeValueId],[LocaleId],[AttributeValue],[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select NewUserGlobalAttributeValueId,[LocaleId],[AttributeValue],@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE NewUserGlobalAttributeValueId IS not NULL
				and isnull([AttributeValue],'') <>''    
				and isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
				and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName		
				
				Update ss
				Set ss.AttributeValue=dd.AttributeValue,ss.ModifiedDate=@GetDate,ss.ModifiedBy=@UserId
				From @TBL_GlobalEntityValueDetail dd
				inner join [dbo].[ZnodeUserGlobalAttributeValueLocale] ss on ss.UserGlobalAttributeValueId =dd.UserGlobalAttributeValueId
				Where isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
				and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName	

				insert into @TBL_AttributeDefaultValueList
				(NewUserGlobalAttributeValueId,UserGlobalAttributeValueId,dd.AttributeValue,GlobalAttributeId)
				Select dd.NewUserGlobalAttributeValueId, dd.UserGlobalAttributeValueId,ss.Item,dd.GlobalAttributeId
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

				Update dd
				Set dd.GlobalAttributeDefaultValueId=ss.GlobalAttributeDefaultValueId
				from  @TBL_AttributeDefaultValueList DD
				inner join [ZnodeGlobalAttributeDefaultValue] ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and dd.AttributeValue=ss.AttributeDefaultValueCode

				Update dd
				Set dd.UserGlobalAttributeValueLocaleId=ss.UserGlobalAttributeValueLocaleId
				from  @TBL_AttributeDefaultValueList DD
				inner join [ZnodeUserGlobalAttributeValueLocale] ss on dd.UserGlobalAttributeValueId=ss.UserGlobalAttributeValueId
				and ss.GlobalAttributeDefaultValueId=dd.GlobalAttributeDefaultValueId

				delete ss
				From @TBL_GlobalEntityValueDetail dd
				inner join [ZnodeUserGlobalAttributeValueLocale] ss on dd.UserGlobalAttributeValueId=ss.UserGlobalAttributeValueId
				Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
				and not exists (Select 1 from @TBL_AttributeDefaultValueList cc 
				where cc.UserGlobalAttributeValueLocaleId=ss.UserGlobalAttributeValueLocaleId )

				INSERT INTO [dbo].[ZnodeUserGlobalAttributeValueLocale]
			   ([UserGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewUserGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewUserGlobalAttributeValueId=dd.NewUserGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

				INSERT INTO [dbo].[ZnodeUserGlobalAttributeValueLocale]
			   ([UserGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.UserGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.UserGlobalAttributeValueId=dd.UserGlobalAttributeValueId				
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
				and ss.UserGlobalAttributeValueLocaleId is null 


				insert into @TBL_MediaValueList
				(NewUserGlobalAttributeValueId,UserGlobalAttributeValueId,GlobalAttributeId,MediaId)
				Select dd.NewUserGlobalAttributeValueId, dd.UserGlobalAttributeValueId,GlobalAttributeId,ss.Item 
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

				Update dd
				Set dd.MediaPath=ss.Path
				from  @TBL_MediaValueList DD
				inner join ZnodeMedia ss on dd.MediaId=ss.MediaId

				Update dd
				Set dd.UserGlobalAttributeValueLocaleId=ss.UserGlobalAttributeValueLocaleId
				from  @TBL_MediaValueList DD
				inner join [ZnodeUserGlobalAttributeValueLocale] ss on dd.UserGlobalAttributeValueId=ss.UserGlobalAttributeValueId
				and ss.MediaId=dd.MediaId

				delete ss
				From @TBL_GlobalEntityValueDetail dd
				inner join [ZnodeUserGlobalAttributeValueLocale] ss on dd.UserGlobalAttributeValueId=ss.UserGlobalAttributeValueId
				Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
				and not exists (Select 1 from @TBL_MediaValueList cc 
				where cc.MediaId=ss.MediaId
				and cc.UserGlobalAttributeValueId=dd.UserGlobalAttributeValueId )

				INSERT INTO [dbo].[ZnodeUserGlobalAttributeValueLocale]
			   ([UserGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewUserGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewUserGlobalAttributeValueId=dd.NewUserGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

				INSERT INTO [dbo].[ZnodeUserGlobalAttributeValueLocale]
			   ([UserGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.UserGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.UserGlobalAttributeValueId=dd.UserGlobalAttributeValueId				
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
				and ss.UserGlobalAttributeValueLocaleId is null 

				Update dd 
				Set dd.MediaPath=ss.MediaPath
				from [ZnodeUserGlobalAttributeValueLocale] dd
                inner join @TBL_MediaValueList ss on 
				ss.UserGlobalAttributeValueLocaleId =dd.UserGlobalAttributeValueLocaleId										    
		
		     SELECT 0 AS ID,CAST(1 AS BIT) AS Status;    
			   
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
             SELECT ERROR_MESSAGE()
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportInsertUpdateGlobalEntity @UserId = '+CAST(@UserId AS VARCHAR(50))+',@IsNotReturnOutput='+CAST(@IsNotReturnOutput AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
			ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportInsertUpdateGlobalEntity',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;