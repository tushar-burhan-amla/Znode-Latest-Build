CREATE PROCEDURE [dbo].[Znode_ImportInsertUpdatePimCategory]
(   @InsertCategory PIMCATEGORYDETAIL READONLY,
	@Status         BIT OUT,
	@UserId         INT               = 0,
	@IsImport BIT =0)
AS
   /* Summary :- This Procedure is used to get the converted category xml into table and call insert update category procedure 
     Unit Testing 
     EXEC Znode_ImportInsertUpdatePimCategory
	*/
     BEGIN
         BEGIN TRY 
             SET NOCOUNT ON;
             DECLARE @DefaultCategoryfamily INT= Dbo.fn_GetDefaultvalue('CategoryFamily');
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
			 DECLARE @TBL_PimCategoryId TABLE (PimCategoryAttributeValueId INT,PimCategoryAttributeValueLocaleId INT )
			  DECLARE @PublishStateIdForDraft INT =  [dbo].[Fn_GetPublishStateIdForDraftState]()
			  DECLARE @PublishStateIdForNotPublished INT = [dbo].[Fn_GetPublishStateIdForForNotPublishedState]() 
             DECLARE @PimCategoryAttributeValueId TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              PimAttributeId              INT,
              PimAttributeFamilyId        INT
             );
             DECLARE @PimAttributeFamilyId INT;
             DECLARE @PimCategoryId INT;
            
			 SELECT TOP 1 @PimCategoryId = PimCategoryId
             FROM @InsertCategory;
             
			 SELECT TOP 1 @PimAttributeFamilyId = PimAttributeFamilyId
             FROM @InsertCategory;
             IF NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodePimcategory AS a
                      INNER JOIN @InsertCategory AS b ON(a.PimCategoryId = b.PimCategoryId)
             )
                 BEGIN
                     INSERT INTO ZnodePimcategory
                     (PimAttributeFamilyId,
                      IsActive,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate ,
					  PublishStateid
                     )
                            SELECT PimAttributeFamilyId,
                                   1,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate ,@PublishStateIdForNotPublished
                            FROM @InsertCategory
                            GROUP BY PimCategoryId,
                                     PimAttributeFamilyId;
                     SET @PimCategoryId = SCOPE_IDENTITY();


					  INSERT INTO ZnodePimCategoryAttributeValue
             (PimCategoryId,
              PimAttributeFamilyId,
              PimAttributeId,
              PimAttributeDefaultValueId,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate
             )
             OUTPUT INSERTED.PimCategoryAttributeValueId,
                    INSERTED.PimCategoryId,
                    INSERTED.PimAttributeId,
                    INSERTED.PimAttributeFamilyId
                    INTO @PimCategoryAttributeValueId
                    SELECT @PimCategoryId,
                           PimAttributeFamilyId,
                           PimAttributeId,
                           CASE
                               WHEN PimAttributeDefaultValueId = 0
                               THEN NULL
                               ELSE PimAttributeDefaultValueId
                           END,
                           @UserId,
                           @GetDate,
                           @UserId,
                           @GetDate
                    FROM @InsertCategory AS a
                    WHERE NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePimCategoryAttributeValue AS b
                        WHERE b.PimAttributeId = a.PimAttributeId
                              AND b.PimCategoryId = a.PimCategoryId
                    );

            
             INSERT INTO ZnodePimCategoryAttributeValueLocale
             (LocaleId,
              PimCategoryAttributeValueId,
              CategoryValue,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate
             )
                    SELECT DISTINCT
                           LocaleId,
                           b.PimCategoryAttributeValueId,
                           a.AttributeValue,
                           @UserId,
                           @GetDate,
                           @UserId,
                           @GetDate
                    FROM @InsertCategory AS a
                         INNER JOIN @PimCategoryAttributeValueId AS b ON(b.PimAttributeId = a.PimAttributeId
                                                                         AND b.PimAttributeFamilyId = a.PimAttributeFamilyId)
                    WHERE NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePimCategoryAttributeValueLocale AS c
                        WHERE c.PimCategoryAttributeValueId = b.PimCategoryAttributeValueId
                              AND c.LocaleId = a.LocaleId
                    );

                 END;
             ELSE
			 BEGIN   
             UPDATE ZnodePimcategory
               SET
                   PimAttributeFamilyId = @PimAttributeFamilyId,
                   ModifiedBy = @UserId,
                   ModifiedDate = @GetDate,
				   IsCategoryPublish  = 
					-- IsCategoryPublish = 1 for status published
					-- IsCategoryPublish = 0 for status Draft
					-- IsCategoryPublish = NULL for status NOT published 
						   
					CASE 
					when IsCategoryPublish =1 then  0  -- IF status is publish then status should be draft
					when IsCategoryPublish =0 then 0   -- IF it is draft then it should be draft 
					else null END,  -- if it is not publish then it will not publish
					PublishStateid = @PublishStateIdForDraft 
             WHERE PimCategoryId = @PimCategoryId;
			 
			 UPDATE a
               SET
                   PimAttributeFamilyId = b.PimAttributeFamilyId,
                   --PimAttributeDefaultValueId = b.PimAttributeDefaultValueId,
                   ModifiedBy = @UserId,
                   ModifiedDate = @GetDate
             OUTPUT INSERTED.PimCategoryAttributeValueId,
                    INSERTED.PimCategoryId,
                    INSERTED.PimAttributeId,
                    INSERTED.PimAttributeFamilyId
                    INTO @PimCategoryAttributeValueId
             FROM ZnodePimCategoryAttributeValue a
                  INNER JOIN @InsertCategory b ON(b.PimAttributeId = a.PimAttributeId
                                                  AND b.PimCategoryId = a.PimCategoryId);

			  INSERT INTO ZnodePimCategoryAttributeValue
             (PimCategoryId,
              PimAttributeFamilyId,
              PimAttributeId,
              PimAttributeDefaultValueId,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate
             )
             OUTPUT INSERTED.PimCategoryAttributeValueId,
                    INSERTED.PimCategoryId,
                    INSERTED.PimAttributeId,
                    INSERTED.PimAttributeFamilyId
                    INTO @PimCategoryAttributeValueId
                    SELECT PimCategoryId,
                           PimAttributeFamilyId,
                           PimAttributeId,
                           CASE
                               WHEN PimAttributeDefaultValueId = 0
                               THEN NULL
                               ELSE PimAttributeDefaultValueId
                           END,
                           @UserId,
                           @GetDate,
                           @UserId,
                           @GetDate
                    FROM @InsertCategory AS a
                    WHERE NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePimCategoryAttributeValue AS b
                        WHERE b.PimAttributeId = a.PimAttributeId
                              AND b.PimCategoryId = a.PimCategoryId
                    ) and isnull(a.PimCategoryId,0) <> 0;
					 
			 UPDATE a
               SET
                   CategoryValue = c.AttributeValue,
                   ModifiedBy = @UserId,
                   ModifiedDate = @GetDate
             FROM ZnodePimCategoryAttributeValueLocale a
                  INNER JOIN @PimCategoryAttributeValueId b ON(a.PimCategoryAttributeValueId = b.PimCategoryAttributeValueId)
                  INNER JOIN @InsertCategory c ON(b.PimAttributeId = c.PimAttributeId
                                                  AND c.LocaleId = a.LocaleId);

			INSERT INTO ZnodePimCategoryAttributeValueLocale
             (LocaleId,
              PimCategoryAttributeValueId,
              CategoryValue,
              CreatedBy,
              CreatedDate,
              ModifiedBy,
              ModifiedDate
             )
                    SELECT DISTINCT
                           LocaleId,
                           b.PimCategoryAttributeValueId,
                           a.AttributeValue,
                           @UserId,
                           @GetDate,
                           @UserId,
                           @GetDate
                    FROM @InsertCategory AS a
                         INNER JOIN @PimCategoryAttributeValueId AS b ON(b.PimAttributeId = a.PimAttributeId
                                                                         AND b.PimAttributeFamilyId = a.PimAttributeFamilyId)
                    WHERE NOT EXISTS
                    (
                        SELECT TOP 1 1
                        FROM ZnodePimCategoryAttributeValueLocale AS c
                        WHERE c.PimCategoryAttributeValueId = b.PimCategoryAttributeValueId
                              AND c.LocaleId = a.LocaleId
                    );

					IF @IsImport =0
					BEGIN
					INSERT INTO @TBL_PimCategoryId(PimCategoryAttributeValueId,PimCategoryAttributeValueLocaleId)
                            SELECT ZPAV.PimCategoryAttributeValueId,ZPAVL.PimCategoryAttributeValueLocaleId
                            FROM ZnodePimCategoryAttributeValue ZPAV
                            INNER JOIN ZnodePimCategoryAttributeValueLocale ZPAVL ON (ZPAVL.PimCategoryAttributeValueId = ZPAV.PimCategoryAttributeValueId AND EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @InsertCategory TBPD
                                WHERE TBPD.LocaleId = ZPAVL.LocaleId
                                     -- AND TBPD.PimAttributeFamilyId = ZPAV.PimAttributeFamilyId
                            )) 
							WHERE EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @InsertCategory TBPD
                                WHERE TBPD.PimCategoryId = ZPAV.PimCategoryId
                                      AND TBPD.PimAttributeFamilyId = ZPAV.PimAttributeFamilyId
                            )
                                  AND NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @InsertCategory TBPDI
                                WHERE TBPDI.PimAttributeId = ZPAV.PimAttributeId
                                      AND TBPDI.PimCategoryId = ZPAV.PimCategoryId
                                      AND TBPDI.PimAttributeFamilyId = ZPAV.PimAttributeFamilyId
                            )
                                  AND EXISTS
                            (
                                SELECT TOP 1 1
                                FROM ZnodePimAttribute ZPA
                                WHERE ZPA.PimAttributeId = ZPAV.PimAttributeId
                                      AND IsLocalizable = 1
                            );

					INSERT INTO @TBL_PimCategoryId(PimCategoryAttributeValueId,PimCategoryAttributeValueLocaleId)
					SELECT ZPAV.PimCategoryAttributeValueId,PimCategoryAttributeValueLocaleId
					FROM ZnodePimCategoryAttributeValue ZPAV 
					INNER JOIN ZnodePimCategoryAttributeValueLocale ZPAVL ON (ZPAVL.PimCategoryAttributeValueId = ZPAV.PimCategoryAttributeValueId AND EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @InsertCategory TBPD
                                WHERE TBPD.LocaleId = ZPAVL.LocaleId
                                     -- AND TBPD.PimAttributeFamilyId = ZPAV.PimAttributeFamilyId
                            )) 
					INNER JOIN ZnodePimFamilyGroupMapper ZPFGMI  ON (ZPFGMI.PimAttributeId = ZPAV.PimAttributeId AND ZPFGMI.PimAttributeFamilyId = @PimAttributeFamilyId)
					WHERE EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @InsertCategory TBPD
                                WHERE TBPD.PimCategoryId = ZPAV.PimCategoryId 
                                     -- AND TBPD.PimAttributeFamilyId = ZPAV.PimAttributeFamilyId
                            )
                                  AND NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @InsertCategory TBPDI
                                WHERE TBPDI.PimAttributeId = ZPAV.PimAttributeId
                                      AND TBPDI.PimCategoryId = ZPAV.PimCategoryId
                                     -- AND TBPDI.PimAttributeFamilyId = ZPAV.PimAttributeFamilyId
                            )
					 DELETE FROM ZnodePimCategoryAttributeValueLocale
                     WHERE EXISTS
                     (
                         SELECT TOP 1 1
                         FROM @TBL_PimCategoryId TBPD
                         WHERE TBPD.PimCategoryAttributeValueId = ZnodePimCategoryAttributeValueLocale.PimCategoryAttributeValueId AND TBPD.PimCategoryAttributeValueLocaleId = ZnodePimCategoryAttributeValueLocale.PimCategoryAttributeValueLocaleId
                     );

                     DELETE FROM ZnodePimCategoryAttributeValue
                     WHERE EXISTS
                     (
                         SELECT TOP 1 1
                         FROM @TBL_PimCategoryId TBPD
                         WHERE TBPD.PimCategoryAttributeValueId = ZnodePimCategoryAttributeValue.PimCategoryAttributeValueId
                     )
					 AND  NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimCategoryAttributeValueLocale ZPCVL WHERE ZPCVL.PimCategoryAttributeValueId = ZnodePimCategoryAttributeValue.PimCategoryAttributeValueId)
                      
					 ;
					END

		 END 
          
             
            
             SELECT @PimCategoryId AS ID,
                    CAST(1 AS BIT) AS Status;
         END TRY
         BEGIN CATCH
            
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportInsertUpdatePimCategory @UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_ImportInsertUpdatePimCategory',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;