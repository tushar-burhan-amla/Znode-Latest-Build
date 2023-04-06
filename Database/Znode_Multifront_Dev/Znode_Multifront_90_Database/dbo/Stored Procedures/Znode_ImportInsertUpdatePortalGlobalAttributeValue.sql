

CREATE PROCEDURE [dbo].[Znode_ImportInsertUpdatePortalGlobalAttributeValue]
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
			 DECLARE @TBL_Portal TABLE (PortalId [int] NULL)
			 DECLARE @TBL_DeletePortal TABLE (PortalId [int] NULL,PortalGlobalAttributeValueId int)
			 DECLARE @TBL_AttributeDefaultValueList TABLE 
			   (NewPortalGlobalAttributeValueId int,PortalGlobalAttributeValueId int,[AttributeValue] [varchar](300),[GlobalAttributeDefaultValueId] int,
			   [GlobalAttributeId] int,MediaId int,PortalGlobalAttributeValueLocaleId int)

			 DECLARE @TBL_MediaValueList TABLE 
			   (NewPortalGlobalAttributeValueId int,PortalGlobalAttributeValueId int,GlobalAttributeId int,
			   MediaId int,MediaPath nvarchar(300),PortalGlobalAttributeValueLocaleId int)
			 DECLARE @TBL_InsertGlobalEntityValue TABLE 
				([GlobalAttributeId] [int] NULL,GlobalAttributeDefaultValueId [int] NULL,PortalId [int] NULL,
					PortalGlobalAttributeValueId int null)
		 	 DECLARE @TBL_GlobalEntityValueDetail TABLE ([GlobalAttributeId] [int] NULL,
				[AttributeCode] [varchar](300),[GlobalAttributeDefaultValueId] [int],[GlobalAttributeValueId] [int],
				[LocaleId] [int],PortalId [int],[AttributeValue] [nvarchar](max),PortalGlobalAttributeValueId int,
				NewPortalGlobalAttributeValueId int,GroupAttributeTypeName [varchar](300))

				SELECT TOP 1 @LocaleId = LocaleId FROM @GlobalEntityValueDetail;

				Insert into @TBL_GlobalEntityValueDetail
				([GlobalAttributeId],[AttributeCode],[GlobalAttributeDefaultValueId],
				[GlobalAttributeValueId],[LocaleId],PortalId,[AttributeValue],GroupAttributeTypeName)
				Select dd.[GlobalAttributeId],dd.[AttributeCode],case when [GlobalAttributeDefaultValueId]=0 then null else 
				[GlobalAttributeDefaultValueId] end [GlobalAttributeDefaultValueId],
				case when [GlobalAttributeValueId]=0 then null else 
				[GlobalAttributeValueId] end [GlobalAttributeValueId],[LocaleId],[GlobalEntityValueId],[AttributeValue],ss.GroupAttributeType
				From @GlobalEntityValueDetail dd
				inner join [View_ZnodeGlobalAttribute] ss on ss.GlobalAttributeId=dd.GlobalAttributeId

				Update ss
				Set ss.PortalGlobalAttributeValueId=dd.PortalGlobalAttributeValueId
				From @TBL_GlobalEntityValueDetail ss
				inner join ZnodePortalGlobalAttributeValue dd on dd.PortalId=ss.PortalId
				and dd.GlobalAttributeId=ss.GlobalAttributeId
				
				insert into @TBL_Portal(PortalId)
				Select distinct  PortalId from @TBL_GlobalEntityValueDetail;

                insert into @TBL_DeletePortal
				Select p.PortalId,a.PortalGlobalAttributeValueId
				from ZnodePortalGlobalAttributeValue a
				inner join @TBL_Portal p on p.PortalId=a.PortalId
				Where not exists(select 1 from @TBL_GlobalEntityValueDetail dd 
				where dd.PortalId=a.PortalId and dd.GlobalAttributeId=a.GlobalAttributeId)
				
				               
				Delete From ZnodePortalGlobalAttributeValueLocale
				WHere exists (select 1 from @TBL_DeletePortal dd 
				Where dd.PortalGlobalAttributeValueId=ZnodePortalGlobalAttributeValueLocale.PortalGlobalAttributeValueId)

				Delete From ZnodePortalGlobalAttributeValue
				WHere exists (select 1 from @TBL_DeletePortal dd 
				Where dd.PortalGlobalAttributeValueId=ZnodePortalGlobalAttributeValue.PortalGlobalAttributeValueId)
							

				INSERT INTO [dbo].[ZnodePortalGlobalAttributeValue]
				([PortalId],[GlobalAttributeId],[GlobalAttributeDefaultValueId],[CreatedBy],[CreatedDate],
				[ModifiedBy],[ModifiedDate])
				 output Inserted.GlobalAttributeId,inserted.[GlobalAttributeDefaultValueId],inserted.PortalId,
				 inserted.PortalGlobalAttributeValueId into @TBL_InsertGlobalEntityValue
				Select [PortalId],[GlobalAttributeId],[GlobalAttributeDefaultValueId]
				,@UserId [CreatedBy],@GetDate [CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE PortalGlobalAttributeValueId IS NULL				

            
				Update dd
				Set dd.NewPortalGlobalAttributeValueId=ss.PortalGlobalAttributeValueId
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_InsertGlobalEntityValue ss on dd.[PortalId]=ss.[PortalId]
				and dd.GlobalAttributeId=ss.GlobalAttributeId				

				INSERT INTO [dbo].[ZnodePortalGlobalAttributeValueLocale]
			   ([PortalGlobalAttributeValueId],[LocaleId],[AttributeValue],[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select NewPortalGlobalAttributeValueId,[LocaleId],[AttributeValue],@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE NewPortalGlobalAttributeValueId IS not NULL
				and isnull([AttributeValue],'') <>''    
				and isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
				and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName		
				
				Update ss
				Set ss.AttributeValue=dd.AttributeValue,ss.ModifiedDate=@GetDate,ss.ModifiedBy=@UserId
				From @TBL_GlobalEntityValueDetail dd
				inner join [dbo].[ZnodePortalGlobalAttributeValueLocale] ss on ss.PortalGlobalAttributeValueId =dd.PortalGlobalAttributeValueId
				Where isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
				and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName	

				insert into @TBL_AttributeDefaultValueList
				(NewPortalGlobalAttributeValueId,PortalGlobalAttributeValueId,dd.AttributeValue,GlobalAttributeId)
				Select dd.NewPortalGlobalAttributeValueId, dd.PortalGlobalAttributeValueId,ss.Item,dd.GlobalAttributeId
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

				Update dd
				Set dd.GlobalAttributeDefaultValueId=ss.GlobalAttributeDefaultValueId
				from  @TBL_AttributeDefaultValueList DD
				inner join [ZnodeGlobalAttributeDefaultValue] ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and dd.AttributeValue=ss.AttributeDefaultValueCode

				Update dd
				Set dd.PortalGlobalAttributeValueLocaleId=ss.PortalGlobalAttributeValueLocaleId
				from  @TBL_AttributeDefaultValueList DD
				inner join [ZnodePortalGlobalAttributeValueLocale] ss on dd.PortalGlobalAttributeValueId=ss.PortalGlobalAttributeValueId
				and ss.GlobalAttributeDefaultValueId=dd.GlobalAttributeDefaultValueId

				delete ss
				From @TBL_GlobalEntityValueDetail dd
				inner join [ZnodePortalGlobalAttributeValueLocale] ss on dd.PortalGlobalAttributeValueId=ss.PortalGlobalAttributeValueId
				Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
				and not exists (Select 1 from @TBL_AttributeDefaultValueList cc 
				where cc.PortalGlobalAttributeValueLocaleId=ss.PortalGlobalAttributeValueLocaleId )

				INSERT INTO [dbo].[ZnodePortalGlobalAttributeValueLocale]
			   ([PortalGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewPortalGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewPortalGlobalAttributeValueId=dd.NewPortalGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

				INSERT INTO [dbo].[ZnodePortalGlobalAttributeValueLocale]
			   ([PortalGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.PortalGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.PortalGlobalAttributeValueId=dd.PortalGlobalAttributeValueId				
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
				and ss.PortalGlobalAttributeValueLocaleId is null 


				insert into @TBL_MediaValueList
				(NewPortalGlobalAttributeValueId,PortalGlobalAttributeValueId,GlobalAttributeId,MediaId)
				Select dd.NewPortalGlobalAttributeValueId, dd.PortalGlobalAttributeValueId,GlobalAttributeId,ss.Item 
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

				Update dd
				Set dd.MediaPath=ss.Path
				from  @TBL_MediaValueList DD
				inner join ZnodeMedia ss on dd.MediaId=ss.MediaId

				Update dd
				Set dd.PortalGlobalAttributeValueLocaleId=ss.PortalGlobalAttributeValueLocaleId
				from  @TBL_MediaValueList DD
				inner join [ZnodePortalGlobalAttributeValueLocale] ss on dd.PortalGlobalAttributeValueId=ss.PortalGlobalAttributeValueId
				and ss.MediaId=dd.MediaId

				delete ss
				From @TBL_GlobalEntityValueDetail dd
				inner join [ZnodePortalGlobalAttributeValueLocale] ss on dd.PortalGlobalAttributeValueId=ss.PortalGlobalAttributeValueId
				Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
				and not exists (Select 1 from @TBL_MediaValueList cc 
				where cc.MediaId=ss.MediaId 
				and cc.PortalGlobalAttributeValueId=dd.PortalGlobalAttributeValueId )

				INSERT INTO [dbo].[ZnodePortalGlobalAttributeValueLocale]
			   ([PortalGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewPortalGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewPortalGlobalAttributeValueId=dd.NewPortalGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

				INSERT INTO [dbo].[ZnodePortalGlobalAttributeValueLocale]
			   ([PortalGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.PortalGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.PortalGlobalAttributeValueId=dd.PortalGlobalAttributeValueId				
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
				and ss.PortalGlobalAttributeValueLocaleId is null 

				Update dd 
				Set dd.MediaPath=ss.MediaPath
				from [ZnodePortalGlobalAttributeValueLocale] dd
                inner join @TBL_MediaValueList ss on 
				ss.PortalGlobalAttributeValueLocaleId =dd.PortalGlobalAttributeValueLocaleId										    
		
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