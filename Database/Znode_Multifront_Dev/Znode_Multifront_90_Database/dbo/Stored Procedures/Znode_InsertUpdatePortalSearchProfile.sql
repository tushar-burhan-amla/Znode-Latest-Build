CREATE PROCEDURE [dbo].[Znode_InsertUpdatePortalSearchProfile]
(   @SearchProfileId       int,
    @UserPortalList TransferId readonly ,
    @UserId                INT    )
AS 
   /* 
   SUMMARY : Stored Procedure to insertupdate searchProfileid based on Profileid and portalid list
   Unit Testing:

   -- EXEC Znode_InsertUpdatePortalSearchProfile @SearchProfileId = 2,@UserId = 2,@UserPortalList = 1,2
   
   
   	*/
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate()
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
		
			DECLARE @TBL_PortalList TABLE(PortalSearchProfileId INT)

			INSERT INTO ZnodePortalSearchProfile (PortalId,PublishCatalogId,SearchProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT DISTINCT UPL.Id, ZSP.PublishCatalogId,ZSP.SearchProfileId,@UserId [CreatedBy],@GetDate[CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
			FROM ZnodePortalCatalog ZPA 
			INNER JOIN ZnodePublishCatalogSearchProfile ZSP ON (ZSP.PublishCatalogId = ZPA .PublishCatalogId)
			INNER JOIN @UserPortalList UPL  ON (ZPA.PortalId = UPL.Id)
			WHERE NOT EXISTS (SELECT 1 FROM ZnodePortalSearchProfile ZPSP
							WHERE ZPSP.SearchProfileId = ZSP.SearchProfileId
								AND ZPSP.PortalId = UPL.Id 
								AND ZSP.PublishCatalogId = ZPSP.PublishCatalogId)		 
			AND ZSP.SearchProfileId = @SearchProfileId
			
			INSERT INTO ZnodePortalSearchProfile (PortalId,PublishCatalogId,SearchProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT DISTINCT UPL.Id, ZSP.PublishCatalogId,ZSP.SearchProfileId,@UserId [CreatedBy],@GetDate[CreatedDate],@UserId [ModifiedBy],@GetDate [ModifiedDate]
			FROM ZnodePortalAccount ZPA 
			INNER JOIN ZnodeAccount ZA ON (ZPA.AccountId = ZA.AccountId )
			INNER JOIN ZnodePublishCatalogSearchProfile ZSP ON (ZSP.PublishCatalogId = ZA.PublishCatalogId)
			INNER JOIN @UserPortalList UPL  ON (ZPA.PortalId = UPL.Id)
			WHERE NOT EXISTS (SELECT 1 FROM ZnodePortalSearchProfile ZPSP
							WHERE ZPSP.SearchProfileId = ZSP.SearchProfileId 
							AND ZPSP.PortalId = UPL.Id AND ZSP.PublishCatalogId = ZPSP.PublishCatalogId)		 
			AND ZSP.SearchProfileId = @SearchProfileId

			-- Fetch ids which is not inserted
			IF EXISTS ( Select Id PortalId from @UserPortalList
			Where Id not in (
			SELECT ZPS.PortalId FROM ZnodePortalSearchProfile ZPS
			where  ZPS.SearchProfileId = @SearchProfileId )
			)

			BEGIN
					
			SELECT @SearchProfileId AS ID,CAST(0 AS BIT) AS Status;   
			
			Select Id PortalId from @UserPortalList
			Where Id not in (
			SELECT ZPS.PortalId FROM ZnodePortalSearchProfile ZPS
			where ZPS.SearchProfileId = @SearchProfileId )

			END
		 
			ELSE
			BEGIN
			SELECT @SearchProfileId AS ID,CAST(1 AS BIT) AS Status;   
			END
		 			
			 COMMIT TRAN A;
         END TRY
         BEGIN CATCH
            select ERROR_MESSAGE()
		
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
			 ROLLBACK TRAN A;
    
         END CATCH;
     END;