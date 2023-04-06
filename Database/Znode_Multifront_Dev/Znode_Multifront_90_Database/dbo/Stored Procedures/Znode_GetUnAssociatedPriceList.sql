CREATE PROCEDURE [dbo].[Znode_GetUnAssociatedPriceList]  
(   
 @WhereClause VARCHAR(MAX) = '',  
 @Rows        INT          = 100,  
 @PageNo      INT          = 0,  
 @Order_BY    VARCHAR(100) = NULL,  
 @RowsCount   INT OUT,  
 @Mode        VARCHAR(100) = ''
)  
AS   
/*  
    Summary: Retrieve Price list with respective to mode ( Portal / Profile / User / Account)   
    Input Parameters:  
    mode :  Portal / Profile / User / Account   
    Output : Provide paging facility with output  
    Unit Testing :   
  DECLARE @RowsCount  int   
    EXEC [Znode_GetPriceList]  
    @WhereClause= ' PriceListId=1 and PortalProfileId = 6',@Rows = 1000 ,@PageNo = 0,@Order_BY =  NULL,@RowsCount =@RowsCount  out,@Mode =  'Profile'  
    DECLARE @RowsCount  int   
    EXEC [Znode_GetPriceList]  
    @WhereClause= ' userId=7 and accountid = 1',@Rows = 1000 ,@PageNo = 0,@Order_BY =  NULL,@RowsCount =@RowsCount  out,@Mode =  'user'  
    DECLARE @RowsCount  int   
    EXEC [Znode_GetPriceList]  
    @WhereClause= ' PriceListId=9 ',@Rows = 100 ,@PageNo = 1,@Order_BY = 'PriceListId',@RowsCount =@RowsCount  out,@Mode =  'Account'  
   */  
BEGIN  
BEGIN TRY  
    SET NOCOUNT ON;  
    DECLARE @SQL NVARCHAR(MAX);  
    DECLARE @TBL_AccountList TABLE (AccountId INT,AccountName NVARCHAR(400),ListCode VARCHAR(200),ListName VARCHAR(600),PriceListId INT,  
    CultureName VARCHAR(200),ActivationDate DATETIME,ExpirationDate DATETIME,Precedence INT, IsParentAccount INT,
	PriceListAccountId INT,RowId INT, CountNo INT,CurrencyName VARCHAR(200)  )  
  
    DECLARE @TBL_UserList TABLE (PriceListUserId INT, UserId INT,UserName VARCHAR(200),ListCode VARCHAR(200),ListName VARCHAR(600),PriceListId INT,  
    CultureName VARCHAR(200),ActivationDate DATETIME,ExpirationDate DATETIME,Precedence INT, RowId INT, CountNo INT , CurrencyName VARCHAR(200)  
    )  
  
 
        IF @Mode = ''  
        BEGIN  
			  SET @SQL = ' With PriceList AS   
			   (
				  SELECT zpl.ListCode,zpl.ListName,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName 
				  ,zpl.PriceListId ,zpl.ActivationDate,zpl.ExpirationDate ,zpl.ModifiedDate, ZCC.CurrencyName  
				  FROM ZnodePriceList AS zpl 
				  INNER JOIN dbo.ZnodeCulture zc ON zpl.CultureId = zc.CultureId 
				  INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId)
				)
				,CTE_PriceList AS  
				(  
				  SELECT DISTINCT ListCode,ListName,CultureName,PriceListId,ActivationDate,ExpirationDate,ModifiedDate, CurrencyName, 
				 '+dbo.Fn_GetPagingRowId(@Order_BY,'PriceListId DESC')+',Count(*)Over() CountNo  
				 FROM PriceList  
				 WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'  
				)  
				SELECT ListCode,ListName,CultureName,PriceListId,ActivationDate,ExpirationDate,ModifiedDate,CurrencyName,CountNo  
				INTO #TM_PriceList   
				FROM CTE_PriceList  
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
  
		END;    
        IF @Mode = 'Profile'  
        BEGIN  
			  SET @SQL = ' With ProfileList as 
			  (
					SELECT ZPL.ListCode,ZPL.ListName,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName,   
					ZPL.PriceListId , ZPL.ActivationDate ,ZPL.ExpirationDate , ZPL.ModifiedDate,ZCC.CurrencyName  , ZCC.CurrencyId, zc.CultureId 
					FROM ZnodePriceList AS ZPL 
					INNER JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId 
					INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId) 
			  )	,		  
			  CTE_ProfileList AS    
			   (
					SELECT DISTINCT ZPP.PortalProfileId, ZP.ProfileId, ZP.ProfileName,PL.ListCode,PL.ListName,PL.CultureName,   
					PL.PriceListId , PL.ActivationDate ,PL.ExpirationDate , PL.ModifiedDate,PL.CurrencyName   
					,'+dbo.Fn_GetPagingRowId(@Order_BY,'ListName ASC')+',Count(*)Over() CountNo  
					FROM ProfileList PL 
					CROSS APPLY ZnodeProfile ZP
					INNER JOIN ZnodePortalProfile AS ZPP ON zp.ProfileId = ZPP.ProfileId 
					WHERE not exists(select * from ZnodePriceListProfile ZPLP where ZPLP.PortalProfileId = ZPP.PortalProfileId AND PL.PriceListId = ZPLP.PriceListId)
					AND 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'        
			   )  
				SELECT DISTINCT PortalProfileId,ProfileName,ListCode,ListName, CultureName, PriceListId ,ActivationDate ,ExpirationDate , ModifiedDate,CurrencyName,RowId,CountNo  
				INTO #TM_PriceList   
				FROM CTE_ProfileList  
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
  
    END;  
    ELSE  
    BEGIN  
		IF @Mode = 'Portal'  
		BEGIN  
			  SET @SQL = ' With PortalList as
			  (
				  SELECT ZPL.ListCode,ZPL.ListName, ZPL.PriceListId ,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName ,  
				  ZPL.ActivationDate ,ZPL.ExpirationDate , ZPL.ModifiedDate,zc.CultureId  ,ZCC.CurrencyName  ,ZCC.CurrencyId
				  FROM ZnodePriceList AS ZPL 
				  INNER JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId  
				  INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId)
			  )			  
			  ,CTE_PortalList AS  
			  (
				  SELECT DISTINCT zp.PortalId,  zp.StoreName, PL.ListCode,PL.ListName, PL.PriceListId ,PL.CultureName ,  PL.CurrencyId,
				  PL.ActivationDate ,PL.ExpirationDate , PL.ModifiedDate,PL.CurrencyName  ,'+dbo.Fn_GetPagingRowId(@Order_BY,'listname ASC')+',Count(*)Over() CountNo 
				  FROM PortalList PL
				  CROSS APPLY dbo.ZnodePortal zp 
				  WHERE not exists(select * from ZnodePriceListPortal ZPLU where zp.PortalId = ZPLU.PortalId AND PL.PriceListId = ZPLU.PriceListId)
				  AND 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
			)
			SELECT PortalId,StoreName,ListCode,ListName,PriceListId,CultureName,ActivationDate,ExpirationDate,  
			ModifiedDate,CurrencyId ,CurrencyName,CountNo  
			INTO #TM_PriceList   
			FROM CTE_PortalList  
			'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  

	   END;  
	   ELSE  
	   BEGIN  
			  IF @Mode = 'User'  
			  BEGIN  
				  SET @SQL = ' With UserList as
				  (
					  SELECT ZPL.ListCode,ZPL.ListName, ZPL.PriceListId , substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName ,  
					  ActivationDate ,ExpirationDate ,ZCC.CurrencyName ,ZCC.CurrencyId,zc.CultureId
					  FROM ZnodePriceList AS ZPL 
					  LEFT JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId
					  INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId) 
				  )				  
				  ,CTE_UserList AS  
				  (
					  SELECT DISTINCT Zu.Userid, Isnull(zu.FirstName,'''') + '' '' +Isnull( zu.MiddleName,'''') + '' ''+ Isnull(zu.LastName,'''') UserName,  
					  PL.ListCode,PL.ListName, PL.PriceListId , PL.CultureName ,  
					  ActivationDate ,ExpirationDate ,zu.AccountId,PL.CurrencyName ,'+dbo.Fn_GetPagingRowId(@Order_BY,'PL.ListName ASC')+',Count(*)Over() CountNo
					  FROM UserList PL 
					  cross apply dbo.ZnodeUser zu 
					  WHERE not exists(select * from ZnodePriceListUser ZPLU where zu.UserId = ZPLU.UserId AND PL.PriceListId = ZPLU.PriceListId)
					  AND 1=1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
				)            
				SELECT Userid, UserName,ListCode,ListName, PriceListId , CultureName, ActivationDate ,ExpirationDate , CurrencyName,RowId,CountNo  
				INTO #TM_PriceList   
				FROM CTE_UserList  
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
			 END;  
			 ELSE  
			 BEGIN  
				IF @Mode = 'Account'  
				BEGIN  
					SET @SQL = ' With AccountList AS   
					(
						SELECT DISTINCT ZPL.ListCode,ZPL.ListName,ZPL.PriceListId,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName,  
						ActivationDate ,ExpirationDate,ZCC.CurrencyName, zc.CultureId, ZCC.CurrencyId
						FROM ZnodePriceList AS ZPL 
						INNER JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId  
						INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId)
					)
         
					,CTE_GetAccountList as  
					(  
						SELECT  DISTINCT za.Accountid,za.Name AccountName,ListCode,ListName,PriceListId,CultureName,ActivationDate,ExpirationDate,
						CASE WHEN za.parentAccountId IS NULL OR parentAccountId = 0  THEN CAST(1 AS BIT ) ELSE CAST(0 AS BIT ) END IsParentAccount   
						,CurrencyName,'+dbo.Fn_GetPagingRowId(@Order_BY,'Accountid DESC')+',Count(*)Over() CountNo   
						FROM AccountList AL
						CROSS APPLY dbo.ZnodeAccount ZA
						WHERE not exists(select * from ZnodePriceListAccount ZPLA  where ZA.Accountid = ZPLA.Accountid and ZPLA.PriceListId = AL.PriceListId )
						AND 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'    
					)  
					SELECT DISTINCT Accountid,AccountName,ListCode,ListName,PriceListId,CultureName,ActivationDate,ExpirationDate,IsParentAccount,  
					CurrencyName,RowId,CountNo  
					INTO #TM_PriceList   
					FROM CTE_GetAccountList  
					'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
				END;  
			 END;  
		END;  
    END;  
  
     SET @SQL = @SQL+ ' SET @Count = ISNULL((SELECT TOP 1 CountNo FROM #TM_PriceList ),0)  
                          SELECT * FROM #TM_PriceList  '  
		Print @sql
          EXEC SP_executesql  
                  @SQL,  
                  N'@Count INT OUT',  
                  @Count = @RowsCount OUT;  
        
     END TRY  
     BEGIN CATCH  
		  DECLARE @Status BIT ;  
		  SET @Status = 0;  
		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetUnAssociatedPriceList @WhereClause = '+cast (@WhereClause AS VARCHAR(50))
		  +',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Mode='+@Mode+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
		 SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
		EXEC Znode_InsertProcedureErrorLog  
		@ProcedureName = 'Znode_GetUnAssociatedPriceList',  
		@ErrorInProcedure = @Error_procedure,  
		@ErrorMessage = @ErrorMessage,  
		@ErrorLine = @ErrorLine,  
		@ErrorCall = @ErrorCall;  
    END CATCH;  
END;