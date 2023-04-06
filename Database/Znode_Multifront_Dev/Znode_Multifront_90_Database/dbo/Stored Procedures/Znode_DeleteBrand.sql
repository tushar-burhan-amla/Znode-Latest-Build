

CREATE  PROCEDURE [dbo].[Znode_DeleteBrand]
( @BrandId VARCHAR(2000) = '',
  @Status  BIT OUT,
  @BrandIds TransferId READONLY	,
  @IsDebug bit = 0 )
AS
 /*   
     Summary: Delete brand detail  
			  This procdure will delete brands if is associated with product or not 
			  here is no any check for association as per requirement 
			  Delete table sequence 
			  1.ZnodeBrandDetails
			  SELECT * FROM ZnodeBRandDetails
     Unit Testing  
			  begin tran 
			  EXEC Znode_DeleteBrand 17,1
			  rollback tran
   */
     BEGIN
         BEGIN TRAN DeleteBrand;
         BEGIN TRY
             SET NOCOUNT ON;
			 -- table use to hold the brand id 
             DECLARE @TBL_DeletdBrandId TABLE(BrandId INT, BrandCode NVARCHAR(1200)); 
             DECLARE @TBL_CMSSEODetailId TABLE(CMSSEODetailId INT);
             DECLARE @TBL_DeletedBrands TABLE(BrandId INT);
			 DECLARE @FinalCount INT = 0  

             INSERT INTO @TBL_DeletdBrandId
					SELECT a.BrandId , a.BrandCode
					FROM ZnodeBrandDetails a
					INNER JOIN dbo.split(@BrandId, ',') AS SP ON (a.BrandId = SP.Item)
					WHERE  @BrandId <> ''


			 INSERT INTO @TBL_DeletdBrandId (BrandId)
			   SELECT Id 
			   FROM @BrandIds


             INSERT INTO @TBL_CMSSEODetailId(CMSSEODetailId)
                    SELECT CMSSEODetailId
                    FROM ZnodeCMSSEODetail ZCSD
                         INNER JOIN ZnodeCMSSEOType ZST ON(ZCSD.CMSSEOTypeId = ZST.CMSSEOTypeId)
                    WHERE ZST.Name = 'Brand'
                          AND EXISTS
                    (
                        SELECT TOP 1 1
                        FROM @TBL_DeletdBrandId TBDB
                        WHERE TBDB.BrandCode = ZCSD.SEOCode
                    );

			Delete from ZnodePromotionBrand where  EXISTS
                    (
                        SELECT TOP 1 1
                        FROM @TBL_DeletdBrandId TBDB
                        WHERE TBDB.BrandId = ZnodePromotionBrand.BrandId
                    );
			 Delete from ZnodeCMSWidgetBrand  where  EXISTS
                    (
                        SELECT TOP 1 1
                        FROM @TBL_DeletdBrandId TBDB
                        WHERE TBDB.BrandId = ZnodeCMSWidgetBrand.BrandId
                    );
             DELETE FROM ZnodeCMSSEODetailLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_CMSSEODetailId TBCSO
                 WHERE TBCSO.CMSSEODetailId = ZnodeCMSSEODetailLocale.CMSSEODetailId
             );
             DELETE FROM ZnodeCMSSEODetail
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_CMSSEODetailId TBCSO
                 WHERE TBCSO.CMSSEODetailId = ZnodeCMSSEODetail.CMSSEODetailId
             );

			  DELETE FROM ZnodeBrandPortal
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletdBrandId AS TBDA
                 WHERE TBDA.BrandId = ZnodeBrandPortal.BrandId
             );

			 DELETE FROM ZnodePortalBrand
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletdBrandId AS TBDA
                 WHERE TBDA.BrandId = ZnodePortalBrand.BrandId
             );

			  DELETE FROM ZnodeBrandProduct
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletdBrandId AS TBDA
                 WHERE TBDA.BrandId = ZnodeBrandProduct.BrandId
             );


             DELETE FROM ZnodeBrandDetailLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletdBrandId AS TBDA
                 WHERE TBDA.BrandId = ZnodeBrandDetailLocale.BrandId
             );
             DELETE FROM ZnodeBrandDetails
             OUTPUT DELETED.BrandId
                    INTO @TBL_DeletedBrands
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletdBrandId AS TBDA
                 WHERE TBDA.BrandId = ZnodeBrandDetails.BrandId
             );


			 SET @FinalCount = (	SELECT COUNT(1)
							FROM dbo.split( @BrandId, ',' ) AS a   WHERE @BrandId <> '' 
							)
			SET @FinalCount = CASE WHEN @FinalCount = 0 THEN  (	SELECT COUNT(1)
							FROM @BrandIds AS a
							)	ELSE @FinalCount END


             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeletedBrands
             ) = @FinalCount
              -- check statement if count is equal the data set return true aother wise false 
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteBrand;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteBrand @BrandId = '+@BrandId+',@Status='+CAST(@Status AS VARCHAR(50));
             SELECT 0 AS ID,
        CAST(0 AS BIT) AS Status,ERROR_MESSAGE();
             SET @Status = 0;
             ROLLBACK TRAN DeleteBrand;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteBrand',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;