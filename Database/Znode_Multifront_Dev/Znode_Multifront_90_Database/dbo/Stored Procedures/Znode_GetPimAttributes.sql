CREATE  PROCEDURE [dbo].[Znode_GetPimAttributes]
(   @WhereClause         VARCHAR(MAX)  = '',
    @Rows                INT           = 100,
    @PageNo              INT           = 1,
    @Order_BY            VARCHAR(1000) = '',
    @RowsCount           INT OUT,
    @LocaleId            INT           = 0,
    @PimAttributeId      VARCHAR(MAX)  = '',
    @IsReturnAllCoulumns BIT           = 0)
AS
/*
     Summary :- This Procedure is used to get the attribute details with the attribute name locale wise 
				Result is fetched order by PimAttributeId in descending order
     Unit Testing 
	 
     EXEC [Znode_GetPimAttributes] '',10,1,'',0,1,'54,53,56',1

	
*/
     BEGIN
         SET NOCOUNT ON;
		 
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetdefaultLocaleId();
            
             DECLARE @TBL_AttributeDefault TABLE
             (PimAttributeId       INT,
              ParentPimAttributeId INT,
              AttributeTypeId      INT,
              AttributeCode        VARCHAR(600),
              IsRequired           BIT,
              IsLocalizable        BIT,
              IsFilterable         BIT,
              IsSystemDefined      BIT,
              IsConfigurable       BIT,
              IsPersonalizable     BIT,
              DisplayOrder         INT,
              HelpDescription      VARCHAR(MAX),
              IsCategory           BIT,
              IsHidden             BIT,
              CreatedDate          DATETIME,
              ModifiedDate         DATETIME,
              AttributeName        NVARCHAR(MAX),
              AttributeTypeName    VARCHAR(300),
			  IsComparable BIT,
			  IsUseInSearch BIT,
			  IsFacets BIT,
			  UsedInProductsCount int,
              RowId                INT,
              CountId              INT
             );
             IF @PimAttributeId <> ''
                 BEGIN
                     SET @WhereClause = CASE  WHEN @WhereClause = '' THEN '' ELSE ' AND ' END+' EXISTS (SELECT TOP 1  1  FROM dbo.Split('''+@PimAttributeId+''','','') SP WHERE SP.Item = CTPADV.PimAttributeId )';                                            
                 END;

			--collect count of used attribute in product.
            IF OBJECT_ID('tempdb..#AttributeCount', 'U') IS NOT NULL
			Begin
				DROP TABLE tempdb..#AttributeCount 
			End
			
			CREATE TABLE tempdb..#AttributeCount ( PimAttributeId int ,UsedInProductsCount  int  )

			INSERT INTO tempdb..#AttributeCount ( PimAttributeId,UsedInProductsCount)
				SELECT AD.PimAttributeId,COUNT(PAV.PimProductId) 
				FROM ZnodePimAttribute AD Inner JOIN ZnodePimAttributeValue PAV ON AD.PimAttributeId = PAV.PimAttributeId
				WHERE AD.IsCategory =0 
				GROUP BY AD.PimAttributeId
						

             SET @SQL = '
		     ;With Cte_PimAttribute AS 
			 (
				 SELECT ZPA.PimAttributeId,ZPA.ParentPimAttributeId,ZPA.AttributeTypeId,ZPA.AttributeCode,ZPA.IsRequired,ZPA.IsLocalizable,ZPA.IsFilterable
				 ,ZPA.IsSystemDefined,ZPA.IsConfigurable,ZPA.IsPersonalizable,ZPA.DisplayOrder,ZPA.HelpDescription,ZPA.IsCategory,ZPA.IsHidden
				 ,ZPA.CreatedBy,ZPA.CreatedDate,ZPA.ModifiedBy,ZPA.ModifiedDate,ZPAL.AttributeName,ZAT.AttributeTypeName , ZPAL.LocaleId ,
				  ZPFP.IsComparable,ZPFP.IsUseInSearch,ZPFP.IsFacets,AC.UsedInProductsCount
				 FROM ZnodePimAttribute ZPA 
				 INNER JOIN ZnodePimAttributeLocale ZPAL ON (ZPAL.PimAttributeId = ZPA.PimAttributeId)
				 INNER JOIN ZnodeAttributeType ZAT ON (ZAT.AttributeTypeId = ZPA.AttributeTypeId)
				 Left Outer JOIN ZnodePimFrontendProperties ZPFP ON ZPA.PimAttributeId = ZPFP.PimAttributeId 
				 Left Outer Join tempdb..#AttributeCount AC on ZPA.PimAttributeId = AC.PimAttributeId
				 WHERE IsHidden = 0
			 )
			 , Cte_PimAttributeFirstLocale AS 
			 (
				 SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable
				 ,IsSystemDefined,IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden
				 ,AttributeName ,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId
				 ,IsComparable,IsUseInSearch,IsFacets,UsedInProductsCount
				 FROM Cte_PimAttribute CTA 
				 WHERE LocaleId = '+CAST(@localeId AS VARCHAR(20))+'
			 )
			 , Cte_PimAttributeDefaultLocale AS 
			 (
			     SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable
			     ,IsSystemDefined,IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden
			     ,AttributeName ,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId,IsComparable,IsUseInSearch,IsFacets,UsedInProductsCount
			     FROM Cte_PimAttributeFirstLocale 
			     UNION ALL 
			     SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable
				 ,IsSystemDefined,IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden
				 ,AttributeName ,AttributeTypeName,CreatedDate,ModifiedDate,LocaleId,IsComparable,IsUseInSearch,IsFacets,UsedInProductsCount
			     FROM Cte_PimAttribute CTA
			     WHERE LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(20))+'
			     AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_PimAttributeFirstLocale CTAFL WHERE CTAFL.PimAttributeId = CTA.PimAttributeId)		 
			  )
			  ,Cte_PimAttributeFilter AS 
			  (
			     SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable
				 ,IsSystemDefined,IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate
				 ,AttributeName ,AttributeTypeName,LocaleId,IsComparable,IsUseInSearch,IsFacets ,UsedInProductsCount, '+[dbo].[Fn_GetPagingRowId](@Order_BY, ' PimAttributeId DESC')+' , Count(*)Over() CountId
			     FROM Cte_PimAttributeDefaultLocale 	CTPADV 
			     WHERE 1=1 
			     '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+'
			  )
			 
			     SELECT PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable
				 ,IsSystemDefined,IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate
				 ,AttributeName ,AttributeTypeName ,ISNULL(IsComparable,0)IsComparable ,ISNULL(IsUseInSearch,0)IsUseInSearch,ISNULL(IsFacets,0) IsFacets,UsedInProductsCount,RowId ,CountId 
			     FROM Cte_PimAttributeFilter CTAF 
			     '+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows)+'
			     '+[dbo].[Fn_GetOrderByClause](@Order_BY, 'PimAttributeId DESC')+'
			     ';
           
				 INSERT INTO @TBL_AttributeDefault
				 (PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,IsConfigurable
				 ,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName,IsComparable,IsUseInSearch,IsFacets,UsedInProductsCount,RowId,CountId)
				 EXEC (@SQL);


				 IF @IsReturnAllCoulumns = 0
                 BEGIN
                     SELECT AD.PimAttributeId,AD.AttributeCode,AD.AttributeName,AD.AttributeTypeName,AD.IsRequired,AD.IsLocalizable,AD.IsSystemDefined,AD.CreatedDate,AD.IsPersonalizable,AD.IsComparable,AD.IsUseInSearch,AD.IsFacets,
					         Isnull(AD.UsedInProductsCount,0) UsedInProductsCount, ZPAGM.PimAttributeGroupId AS AttributeGroupId
                     FROM @TBL_AttributeDefault AD
					 Left Join ZnodePimAttributeGroupMapper ZPAGM ON AD.PimAttributeId = ZPAGM.PimAttributeId
		         END;
                 ELSE
                 BEGIN
                     SELECT AD.PimAttributeId,ParentPimAttributeId,AttributeTypeId,AD.AttributeCode,AD.IsRequired,AD.IsLocalizable,AD.IsFilterable,AD.IsSystemDefined
					 ,AD.IsConfigurable,AD.IsPersonalizable,AD.DisplayOrder,AD.HelpDescription,AD.IsCategory,AD.IsHidden,AD.CreatedDate,AD.ModifiedDate,AD.AttributeName,AD.AttributeTypeName,
					  AD.IsComparable,AD.IsUseInSearch,AD.IsFacets
					  , Isnull(AD.UsedInProductsCount,0) UsedInProductsCount, ZPAGM.PimAttributeGroupId AS AttributeGroupId
                     FROM @TBL_AttributeDefault AD
					 Left Join ZnodePimAttributeGroupMapper ZPAGM ON AD.PimAttributeId = ZPAGM.PimAttributeId
				 END;
				 SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_AttributeDefault), 0);
		
         END TRY
         BEGIN CATCH
		
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPimAttributes @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PimAttributeId='+@PimAttributeId+',@IsReturnAllCoulumns='+CAST(@IsReturnAllCoulumns AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPimAttributes',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
            
         END CATCH;
     END;