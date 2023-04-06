CREATE  PROCEDURE [dbo].[Znode_GetPromotionalCoupon](
       @WhereClause VARCHAR(1000) ,
       @Rows        INT           = 1000 ,
       @PageNo      INT           = 1 ,
       @Order_BY    VARCHAR(100)  = NULL ,
       @RowsCount   INT OUT)
AS
    ------------------------------------------------------------------------------
    --Summary : Get all Promotion with their coupon code with paging 
    -- Unit Testing 
    --DECLARE @RowsCount INT;
    --EXEC Znode_GetPromotionalCoupon
    --@WhereClause = '',
    --@Rows = 1000,
    --@PageNo = 0,
    --@Order_BY = NULL,
    --@RowsCount = @RowsCount OUT;
    --SELECT @RowsCount;
    ------------------------------------------------------------------------------
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @V_SQL NVARCHAR(MAX);
             SET @PageNo = CASE
                               WHEN @PageNo = 0
                               THEN @PageNo
                               ELSE ( @PageNo - 1 ) * @Rows
                           END;
             SET @V_SQL = '
					   With Promotion AS (
					   SELECT ZP.PromotionId, ZP.PromoCode , ZP.Name , Zpt.Name PromotionTypeName , Zp.Discount , Zp.DisplayOrder , ZP.StartDate , ZP.EndDate , ZPC.Code
					   
					   FROM ZnodePromotion AS ZP INNER JOIN ZnodePromotionCoupon AS ZPC ON ZP.PromotionId = ZPC.PromotionId
										    INNER JOIN ZnodePromotionType AS ZPT ON ZP.PromotionTypeId = ZPt.PromotionTypeId )
										    Select * INTO #PromotionalCoupon from Promotion
					   WHERE 1 = 1 
				    '+ CASE
                              WHEN @WhereClause IS NOT NULL
                                   AND
                                   @WhereClause <> ''
                              THEN ' AND '+@WhereClause
                              ELSE ''
                          END+' SELECT  @Count=Count(1) FROM  #PromotionalCoupon 
					  SELECT * FROM #PromotionalCoupon '+' Order BY '+ ISNULL(CASE
                                                                                      WHEN @Order_BY = ''
                                                                                      THEN NULL
                                                                                      ELSE @Order_BY
                                                                                  END , '1')+' OFFSET '+CAST(@PageNo AS VARCHAR(100))+' ROWS FETCH NEXT '+CAST(@Rows AS VARCHAR(100))+' ROWS ONLY  ';
             EXEC SP_executesql @V_SQL , N'@Count INT OUT' , @Count = @RowsCount OUT;
         END TRY
         BEGIN CATCH
             SELECT ERROR_LINE() , ERROR_MESSAGE() , ERROR_NUMBER();
         END CATCH;
     END;