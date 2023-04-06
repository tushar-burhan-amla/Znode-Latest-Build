CREATE PROCEDURE [dbo].[Znode_GetPublishCatalog]
(   @PimCatalogId INT,
    @UserId       INT,
    @Status       BIT = 0 OUT,
	@PublishTimeoutSeconds INT = 0,
	@LocaleId  TransferId Readonly,
	@IsCategoryPublishInProcess BIT = 0,
	@isDebug INT = 0
	 )
AS
/*
     Summary:- This Procedure is used to get data of catalog for publish 
     Unit Testing
	 begin tran
	 DECLARE @RT transferId 
	 INSERT INTO @RT 
	 VALUES (1),()
	  DECLARE @rerer INT =0 
     EXEC Znode_GetPublishCatalog 18 ,2,@rerer OUT SELECT @rerer
	 UPDATE ZnodePublishCatalogLog SET IsCatalogPublished = 0 WHERE IsCatalogPublished IS NULL 
	 SELECT * FROM ZnodePublishcatalogLog 
	 -- SELECT CASE WHEN DATEDIFF(s, LogDateTIme ,@GetDate) > 1500 THEN 1 ELSE 0 END, DATEDIFF(s, LogDateTIme,GETUTCDATE() ) ,*  FROM ZnodePublishCatalogLog  WHERE publishCatalogId = 5  AND isCatalogPublished IS NULL 
	 rollback tran
	*/
BEGIN
    BEGIN TRAN GetPublishCatalog;
    BEGIN TRY

	DECLARE @LocaleIds TABLE (LocaleId INT )
	DECLARE @LocaleIDsin TransferId 
	DECLARE @PublishStateIdForProcessing  INT = [dbo].[Fn_GetPublishStateIdForProcessing]()
	,@PublishStateIdForPublishFailed INT =  [dbo].[Fn_GetPublishStateIdForPublishFailed]()
	INSERT INTO  @LocaleIDsin 
	SELECT * FROM @LocaleID
	DELETE FROM  @LocaleIDsin WHERE id = 0 


	DECLARE @StartPublishProcess BIT = 0 
	DECLARE @GetDate  DATETIME = dbo.Fn_GetDate();
	DECLARE @PublishCatalogId INT= ISNULL((SELECT TOP 1 PublishCatalogId FROM ZnodePublishCatalog ZPC WHERE ZPC.PimCatalogId = @PimCatalogId), 0), @PublishCataloglogId INT= 0;
	DECLARE @TBL_DeletePublishCataLogLogId TABLE (PublishCatalogLogId INT )
	DECLARE @MaxCatalogLog INT = ( SELECT max(PublishcataloglogId) FROM ZnodePublishCataloglog  WHERE PimCatalogId = @PimCatalogId)
	  
	INSERT INTO @LocaleIds 
	SELECT id 
	FROM @LocaleIDsin RT 
	UNION ALL 
	SELECT LocaleId 
	FROM ZnodeLocale 
	WHERE IsActive = 1
	AND NOT EXISTS (SELECT TOP 1 1 FROM @LocaleIDsin ) 

	INSERT INTO   @TBL_DeletePublishCataLogLogId
	SELECT PublishCatalogLogId FROM ZnodePublishCatalogLog ZPCC WHERE ZPCC.PublishCatalogLogId < (@MaxCatalogLog - 2 )
	AND ZPCC.PimCatalogId = @PimCatalogId

    DECLARE @CatalogProfileId VARCHAR(MAX)= '';
                                                   
    IF EXISTS (SELECT TOP 1 1 FROM ZnodePublishcatalogLog  WHERE  IsCatalogPublished  IS NULL OR  PublishStateId = @PublishStateIdForProcessing   )
		BEGIN
		
				SET @StartPublishProcess =1
				SET @Status = 0;
				
		END 
	ELSE 
		BEGIN
			    
		SET @StartPublishProcess = 0
		SET @Status = 1;
		END 	

		IF @isDebug =1 
		BEGIN 
		SELECT @StartPublishProcess
		END 

		IF (@PublishCatalogId <> 0 AND @StartPublishProcess = 0 )
                BEGIN
			
					UPDATE ZnodePublishCatalogLog SET  IsCatalogPublished = 0 ,PublishStateId = @PublishStateIdForPublishFailed 
					WHERE PublishCatalogId = @PublishCatalogId 
					AND  PublishStateId = @PublishStateIdForProcessing

					UPDATE ZPC SET CatalogName = ZC.CatalogName,ExternalId = ZC.ExternalId,PimCatalogId= @PimCatalogId,CreatedBy = @UserId,
					CreatedDate = @GetDate,ModifiedBy = @UserId,ModifiedDate = @GetDate 
					FROM ZnodePublishCatalog ZPC 
					INNER JOIN ZnodePimCatalog ZC ON(ZC.PimCatalogId = ZPC.PimCatalogId)
					WHERE ZPC.PimCatalogId = @PimCatalogId;
              
			  		INSERT INTO ZnodePublishCatalogLog (PublishCatalogId , IsProductPublished,IsCategoryPublished,IsCatalogPublished,pimCatalogId,UserId,LogDateTime,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,LocaleId,PublishStateId )
					SELECT @PublishCatalogId,NULL,CASE WHEN @IsCategoryPublishInProcess = 1 THEN 0 ELSE NULL END,NULL,@PimCatalogId,@UserId,@GetDate,@UserId,@GetDate,@UserId,@GetDate,LocaleId,@PublishStateIdForProcessing
					FROM @LocaleIds a
					WHERE @PublishCataloglogId = 0 

					SET @PublishCataloglogId = SCOPE_IDENTITY();
				END
            ELSE IF @StartPublishProcess = 0 
                BEGIN
				     
					INSERT INTO ZnodePublishCatalog (PimCatalogId,CatalogName,ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
                    SELECT PimCatalogId,CatalogName,ExternalId,@UserId,@GetDate,@UserId,@GetDate 
					FROM ZnodePimCatalog AS ZPC 
					WHERE ZPC.PimCatalogId = @PimCatalogId;
                      
					SET @PublishCatalogId = SCOPE_IDENTITY();
                     
					INSERT INTO ZnodePublishCatalogLog (PublishCatalogId,IsProductPublished,IsCategoryPublished,IsCatalogPublished,pimCatalogId,UserId,LogDateTime,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,LocaleId,PublishStateId )
					SELECT @PublishCatalogId,NULL,CASE WHEN @IsCategoryPublishInProcess = 1 THEN 0 ELSE NULL END,NULL,@PimCatalogId,@UserId,@GetDate,@UserId,@GetDate,@UserId,@GetDate,LocaleId,@PublishStateIdForProcessing
					FROM @LocaleIds
					WHERE @PublishCataloglogId = 0 

                    SET @PublishCataloglogId = SCOPE_IDENTITY();
                END;

				-- here find the profile attached to the catalog 
        SET @CatalogProfileId = SUBSTRING((SELECT ','+CAST(ProfileId AS VARCHAR(20)) FROM ZnodeProfile ZPC WHERE PimCatalogId = @PimCatalogId FOR XML PATH('')), 2, 4000);              

	      
	SELECT ZPC.PublishCatalogId ZnodeCatalogId,CatalogName,@CatalogProfileId TempProfileIds,Max(PublishCatalogLogId) VersionId  ,yu.localeid LocaleId,PublishStateId
	FROM ZnodePublishCatalog ZPC
	INNER JOIN  ZnodePublishCatalogLog YU ON (YU.PublishCatalogId = ZPC.PublishCatalogId)
	WHERE ZPC.PublishCatalogId = @PublishCatalogId
	AND (YU.IsCatalogPublished IS NULL OR  YU.PublishStateId= @PublishStateIdForProcessing)
	AND  @StartPublishProcess = 0 
	GROUP BY 	ZPC.PublishCatalogId,CatalogName,yu.localeid,YU.IsCatalogPublished,PublishStateId ;

	COMMIT TRAN GetPublishCatalog;
		
    END TRY
    BEGIN CATCH
            DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishCatalog @PimCatalogId = '+CAST(@PimCatalogId AS VARCHAR(50))+',@UserId ='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));
            SET @Status = 0;
            ROLLBACK TRAN GetPublishCatalog;
            EXEC Znode_InsertProcedureErrorLog
                @ProcedureName = 'Znode_GetPublishCatalog',
                @ErrorInProcedure = @Error_procedure,
                @ErrorMessage = @ErrorMessage,
                @ErrorLine = @ErrorLine,
                @ErrorCall = @ErrorCall;
    END CATCH;
END;