-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Znode_InsertUpdatePromotion]
	-- Add the parameters for the stored procedure here
(
@PromotionXml         XML,
@PromotionCoupon      XML,
@ProductPromotion     XML,
@User				INT, 
@Status			    BIT = 0 OUT
)
AS
BEGIN
 BEGIN TRAN A 
	  BEGIN TRY 


	DECLARE @CouponCodeDetail VARCHAR(3000) =''
    DECLARE @PromotionId_New  INT = 0

	DECLARE @Tbl_Promotions TABLE (  PromotionId		INT, PromotionTypeId	INT, Discount			INT, OrderMinimum		INT, QuantityMinimum	INT,  DisplayOrder 		INT
	 , PromoCode			Varchar(1000), Name				NVARCHAR(1000), Description		NVARCHAR(MAX), PortalIds			VARCHAR(1000), ProfileIds			VARCHAR(1000)
	, CatalogIds			VARCHAR(1000), CategoryIds		VARCHAR(1000) , StartDate			DATETIME, EndDate			DATETIME	 , IsCouponRequired	BIT,  IsUnique Bit)


	 DECLARE @TBL_Coupns TABLE (PromotionId		INT ,PromotionCouponId INT , PromotionMessage	NVARCHAR(MAX) , Code				VARCHAR(1000) 
								, InitialQuantity	INT,AvailableQuantity	INT , IsEnableUrl		BIT, IsAllowedWithOtherCoupons BIT, IsActive BIT ) 

     DECLARE @TBL_ProductPRomotion TABLE(PromotionId		INT , ProductPromotionId INT ,PublishProductId INT ,ReferralPublishProductId  INT , PromotionProductQunatity  NUMERIC (12,6)  )


	   DECLARE @PortalId   TABLE (ID INT , PortalId INT)
	   DECLARE @ProfileId  TABLE (ID INT , ProfileId INT)
	   DECLARE @CatalogId  TABLE (ID INT , CatalogId INT)
	   DECLARE @CategoryId TABLE (ID INT , CategoryId INT)

	   


	   -- Insert Data into Temp table 
	

		   INSERT INTO @TBL_ProductPRomotion 
       
			SELECT  
						  Tbl.Col.value('PromotionId[1]','INT')		
						 ,Tbl.Col.value('ProductPromotionId[1]','INT')		
						 ,Tbl.Col.value('PublishProductId[1]','INT')	
						 ,Tbl.Col.value('ReferralPublishProductId[1]','INT')	
						 ,Tbl.Col.value('PromotionProductQunatity[1]','Numeric(12,6)')				
					
			FROM @ProductPromotion.nodes('//ProductPromotion') Tbl(Col)
			WHERE Tbl.Col.value('PublishProductId[1]','INT') IS NOT NULL 

		INSERT INTO @TBL_Coupns 

			SELECT  
						  Tbl.Col.value('PromotionId[1]','INT')		
						 ,Tbl.Col.value('PromotionCouponId[1]','INT')		
						 ,Tbl.Col.value('PromotionMessage[1]','Nvarchar(max)')	
						 ,Tbl.Col.value('Code[1]','VARCHAR(1000)')				
						 ,Tbl.Col.value('InitialQuantity[1]','INT')	
						 ,Tbl.Col.value('AvailableQuantity[1]','INT')	
						 ,Tbl.Col.value('IsEnableUrl[1]','BIT')	
						 ,Tbl.Col.value('IsAllowedWithOtherCoupons[1]','BIT')	
						 ,Tbl.Col.value('IsActive[1]','BIT')

			FROM @PromotionCoupon.nodes('//CouponModel') Tbl(Col)
			WHERE Tbl.Col.value('Code[1]','VARCHAR(1000)')	 IS NOT NULL 
			AND Tbl.Col.value('Code[1]','VARCHAR(1000)')	<>'0'

 
		  INSERT INTO @Tbl_Promotions
		
		  SELECT  Tbl.Col.value('PromotionId[1]','INT')	
				 ,Tbl.Col.value('PromotionTypeId[1]','INT')	
				 ,Tbl.Col.value('Discount[1]','NUMERIC(12,6)')		
				 ,Tbl.Col.value('OrderMinimum[1]','Numeric(12,6)')
				 ,Tbl.Col.value('QuantityMinimum[1]','Numeric(12,6)')	
				  ,Tbl.Col.value('DisplayOrder[1]','INT')		
				 ,Tbl.Col.value('PromoCode[1]','Varchar(1000)')				
				 ,Tbl.Col.value('Name[1]','NVARCHAR(1000)')						
				 ,Tbl.Col.value('Description[1]','NVARCHAR(MAX)')				
				 ,Tbl.Col.value('PortalIds[1]','Nvarchar(max)')					
				 ,Tbl.Col.value('ProfileIds[1]','Nvarchar(max)')					
				 ,Tbl.Col.value('CatalogIds[1]','Nvarchar(max)')			
				 ,Tbl.Col.value('CategoryIds[1]','Nvarchar(max)')		
				 ,Tbl.Col.value('StartDate[1]','DateTime')					
				 ,Tbl.Col.value('EndDate[1]','DateTime')					
				 ,Tbl.Col.value('IsCouponRequired[1]','BIT')			
				 ,Tbl.Col.value('IsUnique[1]','BIT')
		  FROM   @PromotionXml.nodes('//PromotionModel') Tbl(Col)
	  
	  

	  --SELECT * FROM @TBL_Coupns 
	  --SELECT * FROM @TBL_ProductPRomotion
	  --SELECT * FROM @Tbl_Promotions
	  --SELECT * FROM [ZnodePromotionCoupon] ZPC INNER JOIN @TBL_Coupns tc ON (tc.Code = zpc.Code)


	   INSERT INTO @PortalId 
	   SELECT ID ,ITEM FROM dbo.split((SELECT TOP 1 PortalIds  FROM @Tbl_Promotions)  ,',') a 

	   INSERT INTO @ProfileId 
	   SELECT ID ,ITEM FROM dbo.split( (SELECT TOP 1 ProfileIds  FROM @Tbl_Promotions),',') a 

	   INSERT INTO @CatalogId 
	   SELECT ID ,ITEM FROM dbo.split( (SELECT TOP 1 CatalogIds  FROM @Tbl_Promotions),',') a 

	   INSERT INTO @CategoryId 
	   SELECT ID ,ITEM FROM dbo.split( (SELECT TOP 1 CategoryIds  FROM @Tbl_Promotions) ,',') a 

	   IF EXISTS (SELECT TOP 1 1 FROM [ZnodePromotion] ZP  INNER JOIN @Tbl_Promotions sw  ON (zp.PromoCode = sw.PromoCode AND zp.PromotionId <> sw.PromotionId) )
	   BEGIN 
	     
		  SET @CouponCodeDetail =  (SELECT TOP 1 zp.PromoCode FROM [ZnodePromotion] ZP  INNER JOIN @Tbl_Promotions sw  ON ( zp.PromoCode = sw.PromoCode AND zp.PromotionId = sw.PromotionId) )

		  RAISERROR (15600,-1,-1, ' Is Already Exists '); 
	  
	    
	   END 
	   IF NOT EXISTS(SELECT TOP 1 1 FROM [ZnodePromotion] ZP WHERE ZP.PromotionId = (SELECT TOP 1 PromotionId  FROM @Tbl_Promotions))
		BEGIN
			INSERT INTO [dbo].[ZnodePromotion]
           ([PromoCode], [Name], [Description], [PromotionTypeId], [Discount], [StartDate], [EndDate], [OrderMinimum], [QuantityMinimum], [IsCouponRequired], [DisplayOrder],[IsUnique], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate])
			SELECT [PromoCode], [Name], [Description], [PromotionTypeId], [Discount], [StartDate], [EndDate], [OrderMinimum], [QuantityMinimum], [IsCouponRequired], [DisplayOrder],[IsUnique], @User, GETUTCDATE(), @User, GETUTCDATE()
			FROM @Tbl_Promotions

		   SET @PromotionId_New = SCOPE_IDENTITY()
		END
	   ELSE
		BEGIN
			UPDATE a
			  SET [PromoCode] = b.PromoCode
			     ,[Name] = b.Name
			     ,[Description] = b.Description
			     ,[PromotionTypeId] = b.PromotionTypeId
			     ,[Discount] = b.Discount
			     ,[StartDate] = b.StartDate
			     ,[EndDate] = b.EndDate
			     ,[OrderMinimum] = b.OrderMinimum
			     ,[QuantityMinimum] = b.QuantityMinimum
			     ,[IsCouponRequired] = b.IsCouponRequired
			     ,[DisplayOrder] = b.DisplayOrder
				 ,[IsUnique] = b.IsUnique
			     ,[ModifiedBy] = @User
			     ,[ModifiedDate] = GETUTCDATE()
			 FROM [dbo].[ZnodePromotion] a 
			 INNER JOIN @Tbl_Promotions b oN (b.PromotionId = a.PromotionId)
			--WHERE PromotionId = @PromotionId

			SET @PromotionId_New = (SELECT TOP 1 PromotionId FROM @Tbl_Promotions ) 
		END


	IF EXISTS (SELECT TOP 1 1 FROM @TBL_Coupns)
	BEGIN 
			UPDATE zpc
			    SET [PromotionMessage] = tc.PromotionMessage
			       ,[InitialQuantity] = tc.InitialQuantity
			       ,[AvailableQuantity] = tc.AvailableQuantity
			       ,[IsEnableUrl] = tc.IsEnableUrl
			       ,[IsAllowedWithOtherCoupons] = tc.IsAllowedWithOtherCoupons
			       ,[IsActive] = tc.IsActive
			       ,[ModifiedBy] = @User
			       ,[ModifiedDate] = GETUTCDATE()
			 	
			 FROM [dbo].[ZnodePromotionCoupon] zpc 
			 INNER JOIN @TBL_Coupns tc ON (tc.PromotionId = zpc.PromotionId  AND tc.Code = zpc.Code)
	
		IF EXISTS(SELECT TOP 1 1 FROM [ZnodePromotionCoupon] ZPC INNER JOIN @TBL_Coupns tc ON (tc.Code = zpc.Code AND zpc.PromotionId <> tc.PromotionId) )
		BEGIN

		  SET @CouponCodeDetail =  (SELECT TOP 1 zpc.Code FROM [dbo].[ZnodePromotionCoupon] ZPC  INNER JOIN @TBL_Coupns tc ON (tc.Code = zpc.Code) )

		  RAISERROR (15600,-1,-1, ' Is Already Exists '); 
	  
		END
		ELSE 
		BEGIN
		   
		   UPDATE aw  
		   SET PromotionId						=q.PromotionId					
				,PromotionMessage				=q.PromotionMessage
				,Code							=q.Code
				,InitialQuantity				=q.InitialQuantity
				,AvailableQuantity				=q.AvailableQuantity
				,IsEnableUrl					=q.IsEnableUrl
				,IsAllowedWithOtherCoupons		=q.IsAllowedWithOtherCoupons
				,IsActive						=q.IsActive
				,ModifiedBy						=@User
				,ModifiedDate					=GETUTCDATE()
			FROM [dbo].[ZnodePromotionCoupon]  aw 
			INNER JOIN @TBL_Coupns q ON (q.PromotionCouponId = aw.PromotionCouponId)



			INSERT INTO [dbo].[ZnodePromotionCoupon]
	       ([PromotionId], [PromotionMessage], [Code], [InitialQuantity], [AvailableQuantity], [IsEnableUrl], [IsAllowedWithOtherCoupons], [IsActive], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate])
		   SELECT @PromotionId_New, tc.PromotionMessage, tc.Code, tc.InitialQuantity, tc.AvailableQuantity, tc.IsEnableUrl, tc.IsAllowedWithOtherCoupons, tc.IsActive, @User, GETUTCDATE(), @User, GETUTCDATE()
            FROM @TBL_Coupns tc 
			WHERE NOT EXISTS (SELECT TOP 1 1 FROM [ZnodePromotionCoupon] zpc WHERE zpc.PromotionId = tc.PromotionId AND zpc.Code =  tc.Code)
            AND tc.PromotionCouponId = 0 

		END
		  
		END 
		 
	    MERGE INTO ZnodePortalPromotion TARGET 
		 USING @PortalId SOURCE 
		 ON (TARGET.PromotionId = @PromotionId_New
			AND TARGET.PortalId = SOURCE.PortalId)
		 WHEN MATCHED THEN 
		 UPDATE 
		 SET
			  TARGET.ModifiedBy						= @User
			 ,TARGET.ModifiedDate					= GETUTCDATE()
		 WHEN NOT MATCHED THEN 
		 INSERT (
				 PortalId
				,PromotionId
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate)
		 VALUES (
				 SOURCE.PortalId
				,@PromotionId_New
				,@User
				,GETUTCDATE()
				,@User
				,GETUTCDATE()
				) 
		WHEN NOT MATCHED BY SOURCE AND TARGET.PromotionId = @PromotionId_New THEN
		DELETE ;

		MERGE INTO ZnodeProfilePromotion TARGET 
		 USING @ProfileId SOURCE 
		 ON (TARGET.PromotionId = @PromotionId_New
			AND TARGET.ProfileId = SOURCE.ProfileId)
		 WHEN MATCHED THEN 
		 UPDATE 
		 SET
			  TARGET.ModifiedBy						= @User
			 ,TARGET.ModifiedDate					= GETUTCDATE()
		 WHEN NOT MATCHED THEN 
		 INSERT (
				 ProfileId
				,PromotionId
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate)
		 VALUES (
				 SOURCE.ProfileId
				,@PromotionId_New
				,@User
				,GETUTCDATE()
				,@User
				,GETUTCDATE()
				) 
		WHEN NOT MATCHED BY SOURCE AND TARGET.PromotionId = @PromotionId_New THEN
		DELETE ;

		MERGE INTO ZnodeCatalogPromotion TARGET 
		 USING @CatalogId SOURCE 
		 ON (TARGET.PromotionId = @PromotionId_New
			AND TARGET.PublishCatalalogId = SOURCE.CatalogId)
		 WHEN MATCHED THEN 
		 UPDATE 
		 SET
			  TARGET.ModifiedBy						= @User
			 ,TARGET.ModifiedDate					= GETUTCDATE()
		 WHEN NOT MATCHED THEN 
		 INSERT (
				 PublishCatalalogId
				,PromotionId
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate)
		 VALUES (
				 CASE WHEN SOURCE.CatalogId = '' THEN NULL ELSE SOURCE.CatalogId END
				,@PromotionId_New
				,@User
				,GETUTCDATE()
				,@User
				,GETUTCDATE()
				) 
		WHEN NOT MATCHED BY SOURCE AND TARGET.PromotionId = @PromotionId_New THEN
		DELETE ;

		MERGE INTO ZnodeCategoryPromotion TARGET 
		 USING @CategoryId SOURCE 
		 ON (TARGET.PromotionId = @PromotionId_New
			AND TARGET.PublishCategoryId = SOURCE.CategoryId)
		 WHEN MATCHED THEN 
		 UPDATE 
		 SET
			  TARGET.ModifiedBy						= @User
			 ,TARGET.ModifiedDate					= GETUTCDATE()
		 WHEN NOT MATCHED THEN 
		 INSERT (
				 PublishCategoryId
				,PromotionId
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate)
		 VALUES (
				 CASE WHEN SOURCE.CategoryId = '' THEN NULL ELSE SOURCE.CategoryId END
				,@PromotionId_New
				,@User
				,GETUTCDATE()
				,@User
				,GETUTCDATE()
				) 
		WHEN NOT MATCHED BY SOURCE AND TARGET.PromotionId = @PromotionId_New THEN
		DELETE ;

		MERGE INTO ZnodeProductPromotion TARGET 
		 USING (SELECT @PromotionId_New	PromotionId	, ProductPromotionId 
						 ,PublishProductId  ,ReferralPublishProductId   , PromotionProductQunatity   FROM @TBL_ProductPRomotion  where PublishProductId <> 0 ) SOURCE 
		 ON (TARGET.PromotionId = source.PromotionId
			AND TARGET.PublishProductId = SOURCE.PublishProductId
			 )
		 WHEN MATCHED THEN 
		 UPDATE 
		 SET
		       TARGET.PromotionProductQunatity = SOURCE.PromotionProductQunatity
			 ,TARGET.ReferralPublishProductId =  Source.ReferralPublishProductId 	
			 ,TARGET.ModifiedBy						= @User
			 ,TARGET.ModifiedDate					= GETUTCDATE()
		 WHEN NOT MATCHED   THEN 
		 INSERT (
				 PublishProductId
				 ,ReferralPublishProductId
				 ,PromotionProductQunatity
				,PromotionId
				,CreatedBy
				,CreatedDate
				,ModifiedBy
				,ModifiedDate)
		 VALUES (
				 CASE WHEN SOURCE.PUBLISHProductId = '' THEN NULL ELSE SOURCE.PUBLISHProductId END
				 ,CASE WHEN  Source.ReferralPublishProductId  = '' THEN NULL ELSE  Source.ReferralPublishProductId  END
				 ,SOURCE.PromotionProductQunatity
				,source.PromotionId
				,@User
				,GETUTCDATE()
				,@User
				,GETUTCDATE()
				) 
		WHEN NOT MATCHED BY SOURCE AND TARGET.PromotionId = @PromotionId_New THEN
		DELETE ;

	   SELECT @PromotionId_New ID ,'Successful' [MessageDetails] ,CAST(1 AS BIT )[Status]
	   -- output paramater 
	   SET @Status = 1

	 COMMIT TRAN A 
	  END TRY 
	  BEGIN CATCH 
	  
	   SET @Status = 0
	    SELECT @PromotionId_New ID ,@CouponCodeDetail+'  Is Already Exists' [MessageDetails], CAST(0 AS BIT )[Status]
		SELECT ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()
  ROLLBACK TRAN A    
	  END CATCH 
END