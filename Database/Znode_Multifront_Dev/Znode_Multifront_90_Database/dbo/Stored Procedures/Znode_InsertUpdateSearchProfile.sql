CREATE PROCEDURE [dbo].[Znode_InsertUpdateSearchProfile]
(   
	@SearchProfileId       int,
    @ProfileName           nvarchar(200),
	@SearchQueryTypeId     int,
	@SearchSubQueryTypeId  int,
    @SearchProfileFeatureList SearchProfileFeatureList readonly ,
    @SearchProfileAttributeList SearchProfileAttributeList readonly ,
    @UserId                INT,
	@PublishCatalogId      int,
	@Operator			   nvarchar(20),	
	@IsDefault             bit=0,
	@SearchProfileFieldValue  SearchProfileFieldValueFactor  readonly
)
AS 
   /* 
    Summary: This Procedure is used to save and edit the quote line item      
    Unit Testing   
    Exec Znode_InsertUpdateQuoteLineItem 
	Unit Testing

	 GO 
	declare @p7 dbo.SearchProfileFetureList
	insert into @p7 values(N'0',1)

	declare @p8 dbo.SearchProfileAttributeList
	insert into @p8 values(N'ProductSpecification',0,0,1)
	insert into @p8 values(N'ShortDescription',0,0,3)
	insert into @p8 values(N'ProductName',0,0,2)
	insert into @p8 values(N'FeatureDescription',0,0,4)
	insert into @p8 values(N'SKU',0,0,5)

	exec sp_executesql N'Znode_InsertUpdateSearchProfile  @SearchProfileId,@ProfileName,@SearchQueryTypeId,@SearchSubQueryTypeId,@SearchProfileFeatureList,@SearchProfileAttributeList,@UserId,@PublishCatalogId,@Operator,@IsDefault',N'@SearchProfileId int,
		@ProfileName nvarchar(17),@SearchQueryTypeId int,@SearchSubQueryTypeId int,@SearchProfileFeatureList [dbo].[SearchProfileFetureList] READONLY,@SearchProfileAttributeList [dbo].[SearchProfileAttributeList] READONLY,@UserId int,@PublishCatalogId int,
		@Operator nvarchar(2),@IsDefault bit',@SearchProfileId=0,@ProfileName=N'FineFoodsProfile1',@SearchQueryTypeId=2,@SearchSubQueryTypeId=0,@SearchProfileFeatureList=@p7,@SearchProfileAttributeList=@p8,@UserId=0,@PublishCatalogId=3,@Operator=N'OR',@IsDefault=0
	*/
BEGIN
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate(),@PublishCatalogSearchProfileId int ,@ReturnMessage  NVARCHAR(max)= ''
  
         BEGIN TRAN A;
         BEGIN TRY
		  IF ISNULL(@SEARCHPROFILEID,0)=0  
		  BEGIN
			IF EXISTS (SELECT TOP 1 1 from ZnodeSearchProfile ZSP WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogSearchProfile PCP WHERE PCP.PublishCatalogId =@PublishCatalogId AND PCP.SearchProfileId =ZSP.SearchProfileId   ) AND ProfileName = @ProfileName  )
			BEGIN 
				SELECT @SearchProfileId AS ID,'ProfileName Already Exists' As MessageDetails,CAST(0 AS BIT) AS Status;   	  
				Return ; 
			END 
			INSERT INTO [dbo].[ZnodeSearchProfile] ([ProfileName],[SearchQueryTypeId],[SearchSubQueryTypeId],[Operator],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate],[PublishStateId])
			Select @ProfileName,@SearchQueryTypeId,@SearchSubQueryTypeId,@Operator,@UserId,@GetDate,@UserId,@GetDate,
				(SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName='Not Published')
			Set @SearchProfileId=SCOPE_IDENTITY()
		  End
		  else 
		  Begin
			IF EXISTS (SELECT TOP 1 1  FROM [ZnodeSearchProfile] ZSP WHERE [ProfileName] = @ProfileName AND 
			EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogSearchProfile PCP WHERE PCP.PublishCatalogId =@PublishCatalogId AND PCP.SearchProfileId =ZSP.SearchProfileId   ) AND 
			 ZSP.SearchProfileId <>  Isnull(@SearchProfileId,0)  )
			BEGIN 
				SELECT @SearchProfileId AS ID,'ProfileName Already Exists' As MessageDetails,CAST(0 AS BIT) AS Status;   	  
				Return ; 
			END 

		    Update a
			Set a.[ProfileName]=@ProfileName,
			a.SearchQueryTypeId=@SearchQueryTypeId,
			a.SearchSubQueryTypeId=@SearchSubQueryTypeId,
			a.Operator = @Operator,
			a.PublishStateId=(SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName='Draft'),
			a.ModifiedBy=@UserId,
			a.ModifiedDate=@GetDate
			from [dbo].[ZnodeSearchProfile] a
			Where SearchProfileId=@SearchProfileId
		    
		  End 

		  delete f
		  from ZnodeSearchProfileAttributeMapping f
		  Where SearchProfileId=@SearchProfileId
		  and not exists(Select 1 from @SearchProfileAttributeList d
		  where f.AttributeCode=d.AttributeCode )
		  AND f.IsFacets = 0

		  INSERT INTO [dbo].ZnodeSearchProfileAttributeMapping([SearchProfileId],[AttributeCode],[IsFacets],[IsUseInSearch],[BoostValue],[IsNgramEnabled],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
		  Select @SearchProfileId,[AttributeCode],[IsFacets],[IsUseInSearch],[BoostValue],[IsNgramEnabled],@UserId [CreatedBy],@GetDate[CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
		   from @SearchProfileAttributeList d
		  where  not exists(Select 1 from ZnodeSearchProfileAttributeMapping f
		  where f.AttributeCode=d.AttributeCode 
		  and f.SearchProfileId=@SearchProfileId)

		  Update f
		  Set f.[IsFacets]=(CASE WHEN d.[IsFacets]=0 THEN f.[IsFacets] ELSE d.[IsFacets] END),f.[IsUseInSearch]=d.[IsUseInSearch],
		  f.[BoostValue]=d.[BoostValue],
		  f.[IsNgramEnabled] = d.[IsNgramEnabled]
		  From [dbo].ZnodeSearchProfileAttributeMapping f
		  inner join @SearchProfileAttributeList d
		  on f.AttributeCode=d.AttributeCode
		   and f.SearchProfileId=@SearchProfileId
		   --WHERE f.IsUseInSearch = 1

		    UPDATE f
			SET f.[IsUseInSearch]=0
			FROM [dbo].ZnodeSearchProfileAttributeMapping f
			LEFT JOIN @SearchProfileAttributeList d ON f.AttributeCode=d.AttributeCode AND f.SearchProfileId=@SearchProfileId
			WHERE f.SearchProfileId=@SearchProfileId AND d.[IsUseInSearch] IS NULL

		   delete f
		  from [ZnodeSearchProfileFeatureMapping] f
		  Where SearchProfileId=@SearchProfileId
		  and not exists(Select 1 from @SearchProfileFeatureList d
		  where f.SearchFeatureId=d.SearchProfileFeatureId )

		  INSERT INTO [dbo].[ZnodeSearchProfileFeatureMapping]([SearchProfileId],SearchFeatureId,[SearchFeatureValue],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
		  Select @SearchProfileId,d.SearchProfileFeatureId,[SearchFeatureValue]
		  ,@UserId [CreatedBy],@GetDate[CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
		   from @SearchProfileFeatureList d
		  where  not exists(Select 1 from [ZnodeSearchProfileFeatureMapping] f
		  where f.SearchFeatureId=d.SearchProfileFeatureId 
		  and f.SearchProfileId=@SearchProfileId)

		  Update f
		  Set f.[SearchFeatureValue]=d.SearchFeatureValue,
		  f.ModifiedBy=@UserId,
		  f.ModifiedDate=@GetDate
		  From [dbo].[ZnodeSearchProfileFeatureMapping] f
		  inner join @SearchProfileFeatureList d
		  on f.SearchFeatureId=d.SearchProfileFeatureId  and f.SearchProfileId=@SearchProfileId
          
		   Select @PublishCatalogSearchProfileId=PublishCatalogSearchProfileId
		   From  [dbo].[ZnodePublishCatalogSearchProfile]
		   Where [PublishCatalogId]=@PublishCatalogId and  SearchProfileId = @SearchProfileId 


		   If @IsDefault=1
		      update [ZnodePublishCatalogSearchProfile]
			  set [IsDefault]=@IsDefault,
			  ModifiedBy=@UserId,
		      ModifiedDate=@GetDate
			  From  [ZnodePublishCatalogSearchProfile]
			  Where PublishCatalogSearchProfileId!=@PublishCatalogSearchProfileId
			  and [PublishCatalogId]=@PublishCatalogId

		   If Isnull(@PublishCatalogSearchProfileId,0)>0
		   Begin
		      update [ZnodePublishCatalogSearchProfile]
			  set [IsDefault]=@IsDefault,
			  ModifiedBy=@UserId,
		      ModifiedDate=@GetDate
			  From  [ZnodePublishCatalogSearchProfile]
			  Where PublishCatalogSearchProfileId=@PublishCatalogSearchProfileId
		   End
		   Else
		   INSERT INTO [dbo].[ZnodePublishCatalogSearchProfile] ([PublishCatalogId],[SearchProfileId],[IsDefault],[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
		   Select @PublishCatalogId,@SearchProfileId,@IsDefault,@UserId,@GetDate,@UserId,@GetDate

		   DELETE VF
		   FROM ZnodeSearchProfileFieldValueFactor VF
		   WHERE VF.SearchProfileId = @SearchProfileId
		   AND NOT EXISTS (SELECT 1 FROM @SearchProfileFieldValue SFP WHERE SFP.FieldName = VF.FieldName)


		   INSERT INTO ZnodeSearchProfileFieldValueFactor (SearchProfileId,FieldName,FieldValueFactor,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		   SELECT @SearchProfileId,SPF.FieldName,SPF.FieldValueFactor,@UserId,@GetDate,@UserId,@GetDate
		   FROM @SearchProfileFieldValue SPF
		   WHERE NOT EXISTS (SELECT 1 FROM ZnodeSearchProfileFieldValueFactor pf WHERE pf.FieldName = SPF.FieldName AND pf.SearchProfileId = @SearchProfileId)

		   UPDATE VF
		   SET FieldValueFactor = SPF.FieldValueFactor
		   FROM ZnodeSearchProfileFieldValueFactor VF
		   INNER JOIN @SearchProfileFieldValue SPF ON (SPF.FieldName = VF.FieldName) AND VF.SearchProfileId = @SearchProfileId


		  SELECT @SearchProfileId AS ID,'Successfull' MessageDetails,CAST(1 AS BIT) AS Status;   			
			 COMMIT TRAN A;
         END TRY
         BEGIN CATCH
        
		    -- SET @Status = 0;
		  ----   DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 ----@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateQuoteLineItem @CartLineItemXML = '+CAST(@CartLineItemXML AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,'UnSuccessfull' MessageDetails,CAST(0 AS BIT) AS Status;                    
			 ROLLBACK TRAN A;
    --         EXEC Znode_InsertProcedureErrorLog
				--@ProcedureName = 'Znode_InsertUpdateQuoteLineItem',
				--@ErrorInProcedure = @Error_procedure,
				--@ErrorMessage = @ErrorMessage,
				--@ErrorLine = @ErrorLine,
				--@ErrorCall = @ErrorCall;
         END CATCH;
END;