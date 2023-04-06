CREATE PROCEDURE [dbo].[Znode_GetPortalCountry]
(   @WhereClause  VARCHAR(MAX),
    @Rows         INT          = 100,
    @PageNo       INT          = 1,
    @Order_BY     VARCHAR(100) = '',
    @RowsCount    INT OUT,
    @IsAssociated INT          = 0 )
AS
   /*
    Summary : Get all country, display their IsActive flag 1 when it will mapped with any portal
    		  IsActive flag 0 when it is not associated with any portal 
    Unit Testing 
       DECLARE @RowsCount INT
       EXEC Znode_GetPortalCountry @WhereClause = 'PortalId = 2 and countryname like ''united%'' ',
       @Rows = 1000,
       @PageNo = 1,
       @Order_BY = DisplayOrder,
       @IsAssociated =0 ,
       @RowsCount = @RowsCount OUT;
      
    */
     BEGIN
         SET NOCOUNT ON;
		 
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_CountryToPortal TABLE
			 (CountryId			INT,
			  CountryCode		VARCHAR(100),
			  CountryName		VARCHAR(200) ,
			  IsActive			BIT,
			  IsDefault			BIT,
			  PortalCountryId	INT,
			  PortalId			INT,
			  DisplayOrder		INT ,
			  RowId				INT,
			  CountNo			INT )

             SET @SQL = '
						;with Cte_portalcountry AS 
						(SELECT ZC.CountryId,ZC.CountryCode,ZC.CountryName, '+CASE WHEN @IsAssociated = 0 THEN 'ZC.IsActive ' ELSE 'ZC.IsActive' END+' IsActive,'+CASE
                         WHEN @IsAssociated = 0 THEN ' ZC.IsDefault ' ELSE 'ZPC.IsDefault' END+' IsDefault ,ISNULL(ZPC.PortalCountryId,0) PortalCountryId,Zp.PortalId ,CASE WHEN ZPC.PortalCountryId IS NULL THEN 0 ELSE 1 END IsAssociated ,ZC.DisplayOrder 
                         FROM ZnodeCountry ZC CROSS APPLY znodeportal zp LEFT OUTER JOIN ZnodePortalCountry ZPC ON ZPC.CountryCode= ZC.CountryCode  AND ZPC.PortalID = zp.PortalId  where  ZC.IsActive=1  ) 
					  
						,Cte_filterPortalCountry AS 
						(
						SELECT CountryId,CountryCode,CountryName,IsActive,IsDefault,PortalCountryId,PortalId,DisplayOrder 
						,'+dbo.Fn_GetPagingRowId(@Order_By,'DisplayOrder,CountryId')+',Count(*)Over() CountNo 
						FROM Cte_portalcountry   
						WHERE IsAssociated = '+CAST(@IsAssociated AS VARCHAR(10))+'
						'+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
						)

						SELECT CountryId,CountryCode,CountryName,IsActive,IsDefault,PortalCountryId,PortalId,DisplayOrder ,RowId,CountNo 
						FROM Cte_filterPortalCountry
						'+dbo.Fn_GetPaginationWhereClause(@PageNo,@rows)

			 INSERT INTO @TBL_CountryToPortal (CountryId,CountryCode,CountryName,IsActive,IsDefault,PortalCountryId,PortalId,DisplayOrder ,RowId,CountNo )
			 EXEC(@SQL)

			 SET @RowsCount =ISNULL((SELECT TOP 1 CountNo  FROM @TBL_CountryToPortal),0)
			 SELECT CountryId,CountryCode,CountryName,IsActive,IsDefault,PortalCountryId,PortalId,DisplayOrder 
			 FROM @TBL_CountryToPortal
			 	 
         END TRY
         BEGIN CATCH
          DECLARE @Status BIT ;
		  SET @Status = 0;
		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPortalCountry @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@IsAssociated='+CAST(@IsAssociated AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
          SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  

          EXEC Znode_InsertProcedureErrorLog
            @ProcedureName = 'Znode_GetPortalCountry',
            @ErrorInProcedure = @Error_procedure,
            @ErrorMessage = @ErrorMessage,
            @ErrorLine = @ErrorLine,
            @ErrorCall = @ErrorCall;
         END CATCH;
     END;