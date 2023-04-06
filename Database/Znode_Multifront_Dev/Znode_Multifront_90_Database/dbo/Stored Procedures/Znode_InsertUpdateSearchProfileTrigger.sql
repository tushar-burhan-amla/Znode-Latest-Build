
CREATE  PROCEDURE [dbo].[Znode_InsertUpdateSearchProfileTrigger]
(   @SearchProfileId       int,
    @KeywordList SelectColumnList readonly ,
    @ProfileList TransferId readonly ,
    @UserId                INT  ,
	@IsConfirmation  bit=0)
AS 
   /* 	*/
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate(),@PublishCatalogSearchProfileId int ,@Status bit =0
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY

		 DECLARE @TBL_FilteredTrigger TABLE (Keyword nvarchar(2000),ProfileId INT)

		-- fetch catalogid of SearchProfileId passed as parameter
		SET @PublishCatalogSearchProfileId = 
		(SELECT PublishCatalogId FROM  ZnodePublishCatalogSearchProfile a
		WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeSearchProfile b where  a.SearchProfileId = b.SearchProfileId and  a.SearchProfileId = @SearchProfileId ) )


		-- fetch trigger 
		INSERT INTO @TBL_FilteredTrigger(Keyword,ProfileId)
		SELECT Keyword,ProfileId FROM ZnodeSearchProfileTrigger c
		WHERE EXISTS (select TOP 1 1 FROM ZnodeSearchProfile a WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogSearchProfile b where a.SearchProfileId = b.SearchProfileId and b.PublishCatalogId = @PublishCatalogSearchProfileId) AND a.SearchProfileId = c.SearchProfileId)


		If EXISTS (Select 1 FROM @KeywordList ) AND  EXISTS (SELECT 1 FROM @ProfileList )  
		 BEGIN
		 
			IF EXISTS (SELECT 1 FROM @KeywordList KL   CROSS JOIN @ProfileList UPL
			INNER JOIN  @TBL_FilteredTrigger ss on KL.[StringColumn]=ss.Keyword  and UPL.Id=ss.ProfileId )
			AND @IsConfirmation=0
				BEGIN
					SET @Status=0
				END
			ELSE 
				BEGIN
					SET @Status=1
				END  
		    IF @Status=1
				BEGIN
					INSERT INTO [dbo].ZnodeSearchProfileTrigger([SearchProfileId],Keyword,ProfileId,[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
					Select @SearchProfileId,KL.[StringColumn] ,UPL.Id,@UserId [CreatedBy],@GetDate[CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
					FROM @KeywordList KL   
					CROSS JOIN @ProfileList UPL
					WHERE NOT EXISTS(SELECT 1 FROM ZnodeSearchProfileTrigger ss WHERE KL.[StringColumn]=ss.Keyword  and UPL.Id=ss.ProfileId and ss.SearchProfileId = @SearchProfileId)
					AND @IsConfirmation=0


					UPDATE ss
					Set SS.SearchProfileId=@SearchProfileId,ss.ModifiedBy=@UserId,ss.ModifiedDate=@GetDate
					FROM  @KeywordList KL   
					CROSS JOIN @ProfileList UPL 
					INNER JOIN ZnodeSearchProfileTrigger ss on  KL.[StringColumn]=ss.Keyword and UPL.Id=ss.ProfileId 
					WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeSearchProfile a WHERE EXISTS 
																				(SELECT TOP 1 1 FROM ZnodePublishCatalogSearchProfile b WHERE a.SearchProfileId = b.SearchProfileId AND b.PublishCatalogId = @PublishCatalogSearchProfileId) AND a.SearchProfileId = ss.SearchProfileId)


				ENd
		 End
		 ELSE If EXISTS (Select 1 FROM @KeywordList ) AND  NOT EXISTS (Select 1 FROM @ProfileList )  
			BEGIN
				IF EXISTS (Select 1 FROM @KeywordList KL   INNER JOIN  @TBL_FilteredTrigger ss on KL.[StringColumn]=ss.Keyword  AND ss.ProfileId IS NULL)
				AND @IsConfirmation=0
					BEGIN
						SET @Status=0
					END
				ELSE 
					BEGIN
						SET @Status=1
					END 
				IF @Status=1
					Begin
						INSERT INTO [dbo].ZnodeSearchProfileTrigger
						([SearchProfileId],Keyword,[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
						Select @SearchProfileId,KL.[StringColumn] ,@UserId [CreatedBy],@GetDate[CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
						FROM @KeywordList KL  
						WHERE  NOT EXISTS(SELECT 1 FROM ZnodeSearchProfileTrigger ss Where KL.[StringColumn]=ss.Keyword AND ss.ProfileId IS NULL AND ss.SearchProfileId = @SearchProfileId )
						AND @IsConfirmation=0

						UPDATE ss
						Set SS.SearchProfileId=@SearchProfileId,ss.ModifiedBy=@UserId,ss.ModifiedDate=@GetDate
						FROM  @KeywordList KL 
						INNER JOIN ZnodeSearchProfileTrigger ss on  KL.[StringColumn]=ss.Keyword and ss.ProfileId IS NULL 
						WHERE EXISTS (select TOP 1 1 FROM ZnodeSearchProfile a WHERE EXISTS 
																			(SELECT TOP 1 1 FROM ZnodePublishCatalogSearchProfile b where a.SearchProfileId = b.SearchProfileId and b.PublishCatalogId = @PublishCatalogSearchProfileId) AND a.SearchProfileId = ss.SearchProfileId)

					END
			 END
		 ELSE IF NOT EXISTS (SELECT 1 FROM @KeywordList ) AND   EXISTS (SELECT 1 FROM @ProfileList )  
			Begin
		
				IF EXISTS (SELECT 1 FROM @ProfileList UPL INNER JOIN  @TBL_FilteredTrigger ss on ss.Keyword IS NULL AND UPL.Id=ss.ProfileId )
				AND @IsConfirmation=0
					BEGIN
							SET @Status=0
					END
				ELSE 
					BEGIN
							SET @Status=1
					END 

				IF @Status=1
					Begin
						INSERT INTO [dbo].ZnodeSearchProfileTrigger([SearchProfileId],ProfileId,[CreatedBy],[CreatedDate],[ModifiedBy],[ModifiedDate])
						Select @SearchProfileId,UPL.Id,@UserId [CreatedBy],@GetDate[CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
						FROM  @ProfileList UPL
						WHERE  NOT EXISTS(SELECT 1 FROM ZnodeSearchProfileTrigger ss WHERE  ss.Keyword IS NULL AND UPL.Id=ss.ProfileId and ss.SearchProfileId = @SearchProfileId)
						and @IsConfirmation=0

				
						Update ss
						Set SS.SearchProfileId=@SearchProfileId,ss.ModifiedBy=@UserId,ss.ModifiedDate=@GetDate
						FROM  @ProfileList UPL
						INNER JOIN ZnodeSearchProfileTrigger ss on ss.Keyword IS NULL AND UPL.Id=ss.ProfileId
						WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeSearchProfile a WHERE EXISTS 
																					(SELECT TOP 1 1 FROM ZnodePublishCatalogSearchProfile b where a.SearchProfileId = b.SearchProfileId and b.PublishCatalogId = @PublishCatalogSearchProfileId) AND a.SearchProfileId = ss.SearchProfileId) 

					ENd


			End

		  SELECT @SearchProfileId AS ID,CAST(@Status AS BIT) AS Status;   			
		  COMMIT TRAN A;
         END TRY
         BEGIN CATCH
        
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateSearchProfileTrigger @UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
			 ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertUpdateSearchProfileTrigger',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;