CREATE PROCEDURE [dbo].[Znode_ImportInsertUpdateWidgetGlobalAttributeValue]
(
    @GlobalEntityValueDetail  [GlobalEntityValueDetail] READONLY,
    @UserId            INT       ,
    @status            BIT    OUT,
    @IsNotReturnOutput BIT    = 0 
)
AS
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
			 DECLARE @GlobalEntityId INT,
			  @MultiSelectGroupAttributeTypeName nvarchar(200)='Select'
			 ,@MediaGroupAttributeTypeName nvarchar(200)='Media'
             DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
			 declare  @CMSContentWidgetId int
			 declare  @CMSWidgetProfileVariantId int
			 DECLARE @TBL_Widget TABLE (CMSWidgetProfileVariantId [int] NULL)
			 DECLARE @TBL_DeleteUser TABLE (CMSWidgetProfileVariantId [int] NULL,WidgetGlobalAttributeValueId int, LocaleId int)
			 DECLARE @TBL_AttributeDefaultValueList TABLE 
			   (NewWidgetGlobalAttributeValueId int,WidgetGlobalAttributeValueId int,[AttributeValue] [varchar](300),[GlobalAttributeDefaultValueId] int,
			   [GlobalAttributeId] int,MediaId int,WidgetGlobalAttributeValueLocaleId int,LocaleId int)


			 DECLARE @TBL_MediaValueList TABLE 
			   (NewWidgetGlobalAttributeValueId int,WidgetGlobalAttributeValueId int,GlobalAttributeId int,
			   MediaId int,MediaPath nvarchar(300),WidgetGlobalAttributeValueLocaleId int,LocaleId int)


		 	 DECLARE @TBL_GlobalEntityValueDetail TABLE ([GlobalAttributeId] [int] NULL,
				[AttributeCode] [varchar](300),[GlobalAttributeDefaultValueId] [int],[GlobalAttributeValueId] [int],
				[LocaleId] [int],CMSWidgetProfileVariantId [int], [AttributeValue] [nvarchar](max),WidgetGlobalAttributeValueId int,
				NewWidgetGlobalAttributeValueId int,GroupAttributeTypeName [varchar](300))
				
				SELECT TOP 1 @CMSWidgetProfileVariantId = GlobalEntityValueId FROM @GlobalEntityValueDetail;

				 Select @CMSContentWidgetId = CCW.CMSContentContainerId from ZnodeCMSContentContainer CCW
				 inner join ZnodeCMSContainerProfileVariant CWPV  on CCW.CMSContentContainerId = CWPV.CMSContentContainerId
				 Where CWPV.CMSContainerProfileVariantId = @CMSWidgetProfileVariantId

				Insert into @TBL_GlobalEntityValueDetail
				([GlobalAttributeId],[AttributeCode],[GlobalAttributeDefaultValueId],
				[GlobalAttributeValueId],[LocaleId],CMSWidgetProfileVariantId,[AttributeValue],GroupAttributeTypeName)
				Select dd.[GlobalAttributeId],dd.[AttributeCode],case when [GlobalAttributeDefaultValueId]=0 then null else 
				[GlobalAttributeDefaultValueId] end [GlobalAttributeDefaultValueId],
				case when [GlobalAttributeValueId]=0 then null else 
				[GlobalAttributeValueId] end [GlobalAttributeValueId],[LocaleId],[GlobalEntityValueId],[AttributeValue],ss.GroupAttributeType
				From @GlobalEntityValueDetail dd
				inner join [View_ZnodeGlobalAttribute] ss on ss.GlobalAttributeId=dd.GlobalAttributeId

				Update ss
				Set ss.WidgetGlobalAttributeValueId=dd.WidgetGlobalAttributeValueId
				From @TBL_GlobalEntityValueDetail ss
				inner join ZnodeWidgetGlobalAttributeValue dd on dd.CMSContainerProfileVariantId=ss.CMSWidgetProfileVariantId
				and dd.GlobalAttributeId=ss.GlobalAttributeId
				
				insert into @TBL_Widget(CMSWidgetProfileVariantId)
				Select distinct  CMSWidgetProfileVariantId from @TBL_GlobalEntityValueDetail;

                insert into @TBL_DeleteUser(CMSWidgetProfileVariantId ,WidgetGlobalAttributeValueId , LocaleId)
				Select p.CMSWidgetProfileVariantId,a.WidgetGlobalAttributeValueId, b.LocaleId
				from ZnodeWidgetGlobalAttributeValue a
				INNER JOIN ZnodeWidgetGlobalAttributeValueLocale B ON A.WidgetGlobalAttributeValueId = B.WidgetGlobalAttributeValueId
				inner join @TBL_Widget p on p.CMSWidgetProfileVariantId=a.CMSContainerProfileVariantId
				Where not exists(select 1 from @TBL_GlobalEntityValueDetail dd 
				where dd.CMSWidgetProfileVariantId=a.CMSContainerProfileVariantId and dd.GlobalAttributeId=a.GlobalAttributeId
				)
				             
				Delete From ZnodeWidgetGlobalAttributeValueLocale
				WHere exists (select 1 from @TBL_DeleteUser dd 
					Where dd.WidgetGlobalAttributeValueId=ZnodeWidgetGlobalAttributeValueLocale.WidgetGlobalAttributeValueId
					and ZnodeWidgetGlobalAttributeValueLocale.LocaleId = dd.LocaleId)
				and exists (select 1 from @TBL_GlobalEntityValueDetail dd 
					Where ZnodeWidgetGlobalAttributeValueLocale.LocaleId = dd.LocaleId)

				Delete From ZnodeWidgetGlobalAttributeValue
				WHere exists (select 1 from @TBL_DeleteUser dd 
				Where dd.WidgetGlobalAttributeValueId=ZnodeWidgetGlobalAttributeValue.WidgetGlobalAttributeValueId)
				and not exists(select * from ZnodeWidgetGlobalAttributeValueLocale where ZnodeWidgetGlobalAttributeValue.WidgetGlobalAttributeValueId = ZnodeWidgetGlobalAttributeValueLocale.WidgetGlobalAttributeValueId)
							

				INSERT INTO [dbo].[ZnodeWidgetGlobalAttributeValue]
				([CMSContentContainerId],[CMSContainerProfileVariantId],[GlobalAttributeId],[GlobalAttributeDefaultValueId],[CreatedBy],[CreatedDate],
				[ModifiedBy],[ModifiedDate])
				Select @CMSContentWidgetId,[CMSWidgetProfileVariantId],[GlobalAttributeId],[GlobalAttributeDefaultValueId]
				,@UserId [CreatedBy],@GetDate [CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE WidgetGlobalAttributeValueId IS NULL		
				AND NOT EXISTS(SELECT * FROM ZnodeWidgetGlobalAttributeValue WGAV WHERE WGAV.CMSContentContainerId = @CMSContentWidgetId 
					AND WGAV.CMSContainerProfileVariantId = dd.CMSWidgetProfileVariantId AND WGAV.GlobalAttributeId = dd.GlobalAttributeId )

				Update dd
				Set dd.NewWidgetGlobalAttributeValueId=WGAV.WidgetGlobalAttributeValueId
				From @TBL_GlobalEntityValueDetail dd
				inner join [ZnodeWidgetGlobalAttributeValue] WGAV on WGAV.CMSContentContainerId = @CMSContentWidgetId 
					AND WGAV.CMSContainerProfileVariantId = dd.CMSWidgetProfileVariantId AND WGAV.GlobalAttributeId = dd.GlobalAttributeId

				INSERT INTO [dbo].[ZnodeWidgetGlobalAttributeValueLocale]
			   ([WidgetGlobalAttributeValueId],[LocaleId],[AttributeValue],[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select NewWidgetGlobalAttributeValueId,[LocaleId],[AttributeValue],@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				WHERE NewWidgetGlobalAttributeValueId IS not NULL
				and isnull([AttributeValue],'') <>''    
				and isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
				and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName	
				AND NOT EXISTS(SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WGAV WHERE WGAV.[WidgetGlobalAttributeValueId] = DD.NewWidgetGlobalAttributeValueId
					AND WGAV.[LocaleId] = DD.[LocaleId])
				
				Update ss
				Set ss.AttributeValue=dd.AttributeValue,ss.ModifiedDate=@GetDate,ss.ModifiedBy=@UserId
				From @TBL_GlobalEntityValueDetail dd
				inner join [dbo].[ZnodeWidgetGlobalAttributeValueLocale] ss on ss.WidgetGlobalAttributeValueId =dd.WidgetGlobalAttributeValueId AND SS.[LocaleId] = DD.[LocaleId]
				Where isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
				and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName	

				insert into @TBL_AttributeDefaultValueList
				(NewWidgetGlobalAttributeValueId,WidgetGlobalAttributeValueId,dd.AttributeValue,GlobalAttributeId,LocaleId)
				Select dd.NewWidgetGlobalAttributeValueId, dd.WidgetGlobalAttributeValueId,ss.Item,dd.GlobalAttributeId,dd.LocaleId
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

				Update dd
				Set dd.GlobalAttributeDefaultValueId=ss.GlobalAttributeDefaultValueId
				from  @TBL_AttributeDefaultValueList DD
				inner join [ZnodeGlobalAttributeDefaultValue] ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and dd.AttributeValue=ss.AttributeDefaultValueCode

				Update dd
				Set dd.WidgetGlobalAttributeValueLocaleId=ss.WidgetGlobalAttributeValueLocaleId
				from  @TBL_AttributeDefaultValueList DD
				inner join [ZnodeWidgetGlobalAttributeValueLocale] ss on dd.WidgetGlobalAttributeValueId=ss.WidgetGlobalAttributeValueId
				and ss.GlobalAttributeDefaultValueId=dd.GlobalAttributeDefaultValueId and dd.LocaleId = ss.LocaleId

				delete ss
				From @TBL_GlobalEntityValueDetail dd
				inner join [ZnodeWidgetGlobalAttributeValueLocale] ss on dd.WidgetGlobalAttributeValueId=ss.WidgetGlobalAttributeValueId
				Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName and dd.LocaleId = ss.LocaleId
				and not exists (Select 1 from @TBL_AttributeDefaultValueList cc 
				where cc.WidgetGlobalAttributeValueLocaleId=ss.WidgetGlobalAttributeValueLocaleId )
				and exists(select * from @TBL_GlobalEntityValueDetail dd1 where dd1.LocaleId = ss.LocaleId)

				INSERT INTO [dbo].[ZnodeWidgetGlobalAttributeValueLocale]
			   ([WidgetGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewWidgetGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewWidgetGlobalAttributeValueId=dd.NewWidgetGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
				AND NOT EXISTS(SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WGAV WHERE WGAV.[WidgetGlobalAttributeValueId] = ss.NewWidgetGlobalAttributeValueId
					AND WGAV.[LocaleId] = DD.[LocaleId])

				INSERT INTO [dbo].[ZnodeWidgetGlobalAttributeValueLocale]
			   ([WidgetGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.WidgetGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.WidgetGlobalAttributeValueId=dd.WidgetGlobalAttributeValueId				
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
				and ss.WidgetGlobalAttributeValueLocaleId is null 
				AND NOT EXISTS(SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WGAV WHERE WGAV.[WidgetGlobalAttributeValueId] = ss.WidgetGlobalAttributeValueId
					AND WGAV.[LocaleId] = DD.[LocaleId])


				insert into @TBL_MediaValueList
				(NewWidgetGlobalAttributeValueId,WidgetGlobalAttributeValueId,GlobalAttributeId,MediaId,LocaleId)
				Select dd.NewWidgetGlobalAttributeValueId, dd.WidgetGlobalAttributeValueId,GlobalAttributeId,Case when ss.Item = 0 then null else ss.Item end,dd.LocaleId
				From @TBL_GlobalEntityValueDetail dd
				cross apply dbo.Split(dd.AttributeValue,',') ss
				Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

				Update dd
				Set dd.MediaPath=ss.Path
				from  @TBL_MediaValueList DD
				inner join ZnodeMedia ss on dd.MediaId=ss.MediaId

				Update dd
				Set dd.WidgetGlobalAttributeValueLocaleId=ss.WidgetGlobalAttributeValueLocaleId
				from  @TBL_MediaValueList DD
				inner join [ZnodeWidgetGlobalAttributeValueLocale] ss on dd.WidgetGlobalAttributeValueId=ss.WidgetGlobalAttributeValueId
				and ss.MediaId=dd.MediaId and dd.LocaleId = ss.LocaleId

				delete ss
				From @TBL_GlobalEntityValueDetail dd
				inner join [ZnodeWidgetGlobalAttributeValueLocale] ss on dd.WidgetGlobalAttributeValueId=ss.WidgetGlobalAttributeValueId and ss.GlobalAttributeDefaultValueId = dd.GlobalAttributeDefaultValueId
				Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
				and not exists (Select 1 from @TBL_MediaValueList cc 
					where cc.MediaId=ss.MediaId and dd.LocaleId = cc.LocaleId
					and cc.WidgetGlobalAttributeValueId=dd.WidgetGlobalAttributeValueId )
				and exists(select * from @TBL_GlobalEntityValueDetail dd1 where dd1.LocaleId = ss.LocaleId)

				INSERT INTO [dbo].[ZnodeWidgetGlobalAttributeValueLocale]
			   ([WidgetGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.NewWidgetGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.NewWidgetGlobalAttributeValueId=dd.NewWidgetGlobalAttributeValueId
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
				AND NOT EXISTS(SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WGAV WHERE WGAV.[WidgetGlobalAttributeValueId] = ss.NewWidgetGlobalAttributeValueId
					AND WGAV.[LocaleId] = DD.[LocaleId])

				INSERT INTO [dbo].[ZnodeWidgetGlobalAttributeValueLocale]
			   ([WidgetGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
			   ,[ModifiedDate])
				Select ss.WidgetGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@UserId [CreatedBy],@GetDate [CreatedDate],
				@UserId [ModifiedBy],@GetDate [ModifiedDate]
				From @TBL_GlobalEntityValueDetail dd
				inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
				and ss.WidgetGlobalAttributeValueId=dd.WidgetGlobalAttributeValueId				
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
				and ss.WidgetGlobalAttributeValueLocaleId is null 
				AND NOT EXISTS(SELECT * FROM ZnodeWidgetGlobalAttributeValueLocale WGAV WHERE WGAV.[WidgetGlobalAttributeValueId] = ss.WidgetGlobalAttributeValueId
					AND WGAV.[LocaleId] = DD.[LocaleId])

				--Update dd 
				--Set dd.MediaPath=ss.MediaPath
				--from [ZnodeWidgetGlobalAttributeValueLocale] dd
    --            inner join @TBL_MediaValueList ss on 
				--ss.WidgetGlobalAttributeValueLocaleId =dd.WidgetGlobalAttributeValueLocaleId
				
				Update wgavl 
				Set wgavl.MediaPath=ss.MediaPath, wgavl.MediaId = ss.MediaId
				from [ZnodeWidgetGlobalAttributeValueLocale] wgavl
				inner join @TBL_GlobalEntityValueDetail dd on wgavl.WidgetGlobalAttributeValueId =dd.WidgetGlobalAttributeValueId AND wgavl.[LocaleId] = DD.[LocaleId]	 	 
                inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId and ss.WidgetGlobalAttributeValueId=dd.WidgetGlobalAttributeValueId AND ss.[LocaleId] = DD.[LocaleId]						 
				WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
		
		     SELECT 1 AS ID,CAST(1 AS BIT) AS Status;    
			   
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
				@ProcedureName = 'Znode_ImportInsertUpdateWidgetGlobalAttributeValue',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;