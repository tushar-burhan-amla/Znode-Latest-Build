Create PROCEDURE [dbo].[Znode_ImportInsertUpdateAccountGlobalAttributeValue]
(
    @GlobalEntityValueDetail  [GlobalEntityValueDetail] READONLY,
    @AccountId            INT       ,
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
		DECLARE @TBL_Account TABLE (AccountId [int] NULL)
		DECLARE @TBL_DeleteAccount TABLE (AccountId [int] NULL,AccountGlobalAttributeValueId int)
		DECLARE @TBL_AttributeDefaultValueList TABLE 
		(NewAccountGlobalAttributeValueId int,AccountGlobalAttributeValueId int,[AttributeValue] [varchar](300),[GlobalAttributeDefaultValueId] int,
		[GlobalAttributeId] int,MediaId int,AccountGlobalAttributeValueLocaleId int)

		DECLARE @TBL_MediaValueList TABLE 
		(NewAccountGlobalAttributeValueId int,AccountGlobalAttributeValueId int,GlobalAttributeId int,
		MediaId int,MediaPath nvarchar(300),AccountGlobalAttributeValueLocaleId int)
		DECLARE @TBL_InsertGlobalEntityValue TABLE 
		([GlobalAttributeId] [int] NULL,GlobalAttributeDefaultValueId [int] NULL,AccountId [int] NULL,
			AccountGlobalAttributeValueId int null)
		DECLARE @TBL_GlobalEntityValueDetail TABLE ([GlobalAttributeId] [int] NULL,
		[AttributeCode] [varchar](300),[GlobalAttributeDefaultValueId] [int],[GlobalAttributeValueId] [int],
		[LocaleId] [int],AccountId [int],[AttributeValue] [nvarchar](max),AccountGlobalAttributeValueId int,
		NewAccountGlobalAttributeValueId int,GroupAttributeTypeName [varchar](300))

		SELECT TOP 1 @LocaleId = LocaleId FROM @GlobalEntityValueDetail;

		Insert into @TBL_GlobalEntityValueDetail
		([GlobalAttributeId],[AttributeCode],[GlobalAttributeDefaultValueId],
		[GlobalAttributeValueId],[LocaleId],AccountId,[AttributeValue],GroupAttributeTypeName)
		Select dd.[GlobalAttributeId],dd.[AttributeCode],case when [GlobalAttributeDefaultValueId]=0 then null else 
		[GlobalAttributeDefaultValueId] end [GlobalAttributeDefaultValueId],
		case when [GlobalAttributeValueId]=0 then null else 
		[GlobalAttributeValueId] end [GlobalAttributeValueId],[LocaleId],[GlobalEntityValueId],[AttributeValue],ss.GroupAttributeType
		From @GlobalEntityValueDetail dd
		inner join [View_ZnodeGlobalAttribute] ss on ss.GlobalAttributeId=dd.GlobalAttributeId

		Update ss
		Set ss.AccountGlobalAttributeValueId=dd.AccountGlobalAttributeValueId
		From @TBL_GlobalEntityValueDetail ss
		inner join ZnodeAccountGlobalAttributeValue dd on dd.AccountId=ss.AccountId
		and dd.GlobalAttributeId=ss.GlobalAttributeId
				
		insert into @TBL_Account(AccountId)
		Select distinct  AccountId from @TBL_GlobalEntityValueDetail;

        insert into @TBL_DeleteAccount
		Select p.AccountId,a.AccountGlobalAttributeValueId
		from ZnodeAccountGlobalAttributeValue a
		inner join @TBL_Account p on p.AccountId=a.AccountId
		Where not exists(select 1 from @TBL_GlobalEntityValueDetail dd 
		where dd.AccountId=a.AccountId and dd.GlobalAttributeId=a.GlobalAttributeId)
				
				               
		Delete From ZnodeAccountGlobalAttributeValueLocale
		WHere exists (select 1 from @TBL_DeleteAccount dd 
		Where dd.AccountGlobalAttributeValueId=ZnodeAccountGlobalAttributeValueLocale.AccountGlobalAttributeValueId)

		Delete From ZnodeAccountGlobalAttributeValue
		WHere exists (select 1 from @TBL_DeleteAccount dd 
		Where dd.AccountGlobalAttributeValueId=ZnodeAccountGlobalAttributeValue.AccountGlobalAttributeValueId)
							

		INSERT INTO [dbo].[ZnodeAccountGlobalAttributeValue]
		([AccountId],[GlobalAttributeId],[GlobalAttributeDefaultValueId],[CreatedBy],[CreatedDate],
		[ModifiedBy],[ModifiedDate])
			output Inserted.GlobalAttributeId,inserted.[GlobalAttributeDefaultValueId],inserted.AccountId,
			inserted.AccountGlobalAttributeValueId into @TBL_InsertGlobalEntityValue
		Select [AccountId],[GlobalAttributeId],[GlobalAttributeDefaultValueId]
		,@AccountId [CreatedBy],@GetDate [CreatedDate],@AccountId [ModifiedBy],@GetDate [ModifiedDate]
		From @TBL_GlobalEntityValueDetail dd
		WHERE AccountGlobalAttributeValueId IS NULL				

            
		Update dd
		Set dd.NewAccountGlobalAttributeValueId=ss.AccountGlobalAttributeValueId
		From @TBL_GlobalEntityValueDetail dd
		inner join @TBL_InsertGlobalEntityValue ss on dd.[AccountId]=ss.[AccountId]
		and dd.GlobalAttributeId=ss.GlobalAttributeId				

		INSERT INTO [dbo].[ZnodeAccountGlobalAttributeValueLocale]
		([AccountGlobalAttributeValueId],[LocaleId],[AttributeValue],[CreatedBy],[CreatedDate],[ModifiedBy]
		,[ModifiedDate])
		Select NewAccountGlobalAttributeValueId,[LocaleId],[AttributeValue],@AccountId [CreatedBy],@GetDate [CreatedDate],
		@AccountId [ModifiedBy],@GetDate [ModifiedDate]
		From @TBL_GlobalEntityValueDetail dd
		WHERE NewAccountGlobalAttributeValueId IS not NULL
		and isnull([AttributeValue],'') <>''    
		and isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
		and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName		
				
		Update ss
		Set ss.AttributeValue=dd.AttributeValue,ss.ModifiedDate=@GetDate,ss.ModifiedBy=@AccountId
		From @TBL_GlobalEntityValueDetail dd
		inner join [dbo].[ZnodeAccountGlobalAttributeValueLocale] ss on ss.AccountGlobalAttributeValueId =dd.AccountGlobalAttributeValueId
		Where isnull(GroupAttributeTypeName,'') != @MultiSelectGroupAttributeTypeName
		and isnull(GroupAttributeTypeName,'') != @MediaGroupAttributeTypeName	

		insert into @TBL_AttributeDefaultValueList
		(NewAccountGlobalAttributeValueId,AccountGlobalAttributeValueId,dd.AttributeValue,GlobalAttributeId)
		Select dd.NewAccountGlobalAttributeValueId, dd.AccountGlobalAttributeValueId,ss.Item,dd.GlobalAttributeId
		From @TBL_GlobalEntityValueDetail dd
		cross apply dbo.Split(dd.AttributeValue,',') ss
		Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

		Update dd
		Set dd.GlobalAttributeDefaultValueId=ss.GlobalAttributeDefaultValueId
		from  @TBL_AttributeDefaultValueList DD
		inner join [ZnodeGlobalAttributeDefaultValue] ss on dd.GlobalAttributeId=ss.GlobalAttributeId
		and dd.AttributeValue=ss.AttributeDefaultValueCode

		Update dd
		Set dd.AccountGlobalAttributeValueLocaleId=ss.AccountGlobalAttributeValueLocaleId
		from  @TBL_AttributeDefaultValueList DD
		inner join [ZnodeAccountGlobalAttributeValueLocale] ss on dd.AccountGlobalAttributeValueId=ss.AccountGlobalAttributeValueId
		and ss.GlobalAttributeDefaultValueId=dd.GlobalAttributeDefaultValueId

		delete ss
		From @TBL_GlobalEntityValueDetail dd
		inner join [ZnodeAccountGlobalAttributeValueLocale] ss on dd.AccountGlobalAttributeValueId=ss.AccountGlobalAttributeValueId
		Where isnull(GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
		and not exists (Select 1 from @TBL_AttributeDefaultValueList cc 
		where cc.AccountGlobalAttributeValueLocaleId=ss.AccountGlobalAttributeValueLocaleId )

		INSERT INTO [dbo].[ZnodeAccountGlobalAttributeValueLocale]
		([AccountGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
		,[ModifiedDate])
		Select ss.NewAccountGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@AccountId [CreatedBy],@GetDate [CreatedDate],
		@AccountId [ModifiedBy],@GetDate [ModifiedDate]
		From @TBL_GlobalEntityValueDetail dd
		inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
		and ss.NewAccountGlobalAttributeValueId=dd.NewAccountGlobalAttributeValueId
		WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName

		INSERT INTO [dbo].[ZnodeAccountGlobalAttributeValueLocale]
		([AccountGlobalAttributeValueId],[LocaleId],GlobalAttributeDefaultValueId,[CreatedBy],[CreatedDate],[ModifiedBy]
		,[ModifiedDate])
		Select ss.AccountGlobalAttributeValueId,dd.[LocaleId],ss.GlobalAttributeDefaultValueId,@AccountId [CreatedBy],@GetDate [CreatedDate],
		@AccountId [ModifiedBy],@GetDate [ModifiedDate]
		From @TBL_GlobalEntityValueDetail dd
		inner join @TBL_AttributeDefaultValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
		and ss.AccountGlobalAttributeValueId=dd.AccountGlobalAttributeValueId				
		WHERE isnull(dd.GroupAttributeTypeName,'') = @MultiSelectGroupAttributeTypeName
		and ss.AccountGlobalAttributeValueLocaleId is null 


		insert into @TBL_MediaValueList
		(NewAccountGlobalAttributeValueId,AccountGlobalAttributeValueId,GlobalAttributeId,MediaId)
		Select dd.NewAccountGlobalAttributeValueId, dd.AccountGlobalAttributeValueId,GlobalAttributeId,ss.Item 
		From @TBL_GlobalEntityValueDetail dd
		cross apply dbo.Split(dd.AttributeValue,',') ss
		Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

		Update dd
		Set dd.MediaPath=ss.Path
		from  @TBL_MediaValueList DD
		inner join ZnodeMedia ss on dd.MediaId=ss.MediaId

		Update dd
		Set dd.AccountGlobalAttributeValueLocaleId=ss.AccountGlobalAttributeValueLocaleId
		from  @TBL_MediaValueList DD
		inner join [ZnodeAccountGlobalAttributeValueLocale] ss on dd.AccountGlobalAttributeValueId=ss.AccountGlobalAttributeValueId
		and ss.MediaId=dd.MediaId

		delete ss
		From @TBL_GlobalEntityValueDetail dd
		inner join [ZnodeAccountGlobalAttributeValueLocale] ss on dd.AccountGlobalAttributeValueId=ss.AccountGlobalAttributeValueId
		Where isnull(GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
		and not exists (Select 1 from @TBL_MediaValueList cc 
		where cc.MediaId=ss.MediaId
		and cc.AccountGlobalAttributeValueId=dd.AccountGlobalAttributeValueId )

		INSERT INTO [dbo].[ZnodeAccountGlobalAttributeValueLocale]
		([AccountGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
		,[ModifiedDate])
		Select ss.NewAccountGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@AccountId [CreatedBy],@GetDate [CreatedDate],
		@AccountId [ModifiedBy],@GetDate [ModifiedDate]
		From @TBL_GlobalEntityValueDetail dd
		inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
		and ss.NewAccountGlobalAttributeValueId=dd.NewAccountGlobalAttributeValueId
		WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName

		INSERT INTO [dbo].[ZnodeAccountGlobalAttributeValueLocale]
		([AccountGlobalAttributeValueId],[LocaleId],MediaId,MediaPath,[CreatedBy],[CreatedDate],[ModifiedBy]
		,[ModifiedDate])
		Select ss.AccountGlobalAttributeValueId,dd.[LocaleId],ss.MediaId,ss.MediaPath,@AccountId [CreatedBy],@GetDate [CreatedDate],
		@AccountId [ModifiedBy],@GetDate [ModifiedDate]
		From @TBL_GlobalEntityValueDetail dd
		inner join @TBL_MediaValueList ss on dd.GlobalAttributeId=ss.GlobalAttributeId
		and ss.AccountGlobalAttributeValueId=dd.AccountGlobalAttributeValueId				
		WHERE isnull(dd.GroupAttributeTypeName,'') = @MediaGroupAttributeTypeName
		and ss.AccountGlobalAttributeValueLocaleId is null 

		Update dd 
		Set dd.MediaPath=ss.MediaPath
		from [ZnodeAccountGlobalAttributeValueLocale] dd
        inner join @TBL_MediaValueList ss on 
		ss.AccountGlobalAttributeValueLocaleId =dd.AccountGlobalAttributeValueLocaleId										    
		
		SELECT 0 AS ID,CAST(1 AS BIT) AS Status;    
			   
        COMMIT TRAN A;
    END TRY
    BEGIN CATCH
        SELECT ERROR_MESSAGE()
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
		@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportInsertUpdateGlobalEntity @AccountId = '+CAST(@AccountId AS VARCHAR(50))+',@IsNotReturnOutput='+CAST(@IsNotReturnOutput AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
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