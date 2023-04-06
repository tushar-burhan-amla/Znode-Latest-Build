CREATE PROCEDURE [dbo].[Znode_GetPriceList]  
(   @WhereClause VARCHAR(MAX) = '',  
 @Rows        INT          = 100,  
 @PageNo      INT          = 0,  
 @Order_BY    VARCHAR(100) = NULL,  
 @RowsCount   INT OUT,  
 @Mode        VARCHAR(100) = '')  
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
			  (SELECT zpl.ListCode,zpl.ListName,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName 
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
			  SET @SQL = ' With ProfileList AS    
			   (SELECT ZPP.PortalProfileId , zp.ProfileName,ZPL.ListCode,ZPL.ListName,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName,   
			  ZPL.PriceListId , ZPL.ActivationDate ,ZPL.ExpirationDate , ZPLP.Precedence , ZPL.ModifiedDate,ZPP.PortalId,ZPLP.PriceListProfileId,ZCC.CurrencyName   
			  FROM ZnodePriceList AS ZPL INNER JOIN ZnodePriceListProfile AS ZPLP ON ZPL.PriceListId = ZPLP.PriceListId  
			  INNER JOIN ZnodePortalProfile AS ZPP ON ZPLP.PortalProfileId = ZPP.PortalProfileId  
			  INNER JOIN dbo.ZnodeProfile zp ON ZPP.ProfileId  = zp.ProfileId  
			  INNER JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId 
			  INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId)) 
  
			  ,CTE_ProfileList AS  
			  (  
				SELECT DISTINCT PortalProfileId , ProfileName,ListCode,ListName, CultureName, PriceListId ,ActivationDate ,ExpirationDate ,  
				Precedence,PriceListProfileId , ModifiedDate,CurrencyName,'+dbo.Fn_GetPagingRowId(@Order_BY,'PortalProfileId DESC')+',Count(*)Over() CountNo  
				FROM ProfileList  
				WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'        
			  )  
				SELECT PortalProfileId , ProfileName,ListCode,ListName, CultureName, PriceListId ,ActivationDate ,ExpirationDate ,  
				Precedence,PriceListProfileId , ModifiedDate,CurrencyName,RowId,CountNo  
				INTO #TM_PriceList   
				FROM CTE_ProfileList  
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
  
    END;  
    ELSE  
    BEGIN  
		IF @Mode = 'Portal'  
		BEGIN  
			  SET @SQL = ' With PortalList AS  
			  (SELECT ZPLP.PriceListPortalId, zp.PortalId,  zp.StoreName, ZPL.ListCode,ZPL.ListName, ZPL.PriceListId ,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName ,  
			  ZPL.ActivationDate ,ZPL.ExpirationDate , ZPLP.Precedence ,ZPL.ModifiedDate,zc.CurrencyId  ,ZCC.CurrencyName   
			  FROM ZnodePriceList AS ZPL INNER JOIN ZnodePriceListPortal AS ZPLP ON ZPL.PriceListId = ZPLP.PriceListId  
			  INNER JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId  
			  INNER JOIN dbo.ZnodePortal zp ON ZPLP.PortalId = zp.PortalId 
			  INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId) ) 
          
				,CTE_PortalList AS  
				(  
				  SELECT DISTINCT PriceListPortalId,PortalId,StoreName,ListCode,ListName,PriceListId,CultureName,ActivationDate,ExpirationDate,  
			   Precedence,ModifiedDate,CurrencyId,CurrencyName,'+dbo.Fn_GetPagingRowId(@Order_BY,'PriceListPortalId DESC')+',Count(*)Over() CountNo  
			   FROM PortalList  
			   WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'    
				)  
				SELECT PriceListPortalId,PortalId,StoreName,ListCode,ListName,PriceListId,CultureName,ActivationDate,ExpirationDate,  
			   Precedence,ModifiedDate,CurrencyId ,CurrencyName,CountNo  
			   INTO #TM_PriceList   
			   FROM CTE_PortalList  
			   '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  

	   END;  
	   ELSE  
	   BEGIN  
			  IF @Mode = 'User'  
			  BEGIN  
				  SET @SQL = ' With UserList AS  
				  (SELECT ZPLU.PriceListUserId, Zu.Userid, Isnull(zu.FirstName,'''') + '' '' +Isnull( zu.MiddleName,'''') + '' ''+ Isnull(zu.LastName,'''') UserName,  
				  ZPL.ListCode,ZPL.ListName, ZPL.PriceListId , substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName ,  
				  ActivationDate ,ExpirationDate , ZPLU.Precedence,zu.AccountId,ZCC.CurrencyName  FROM  dbo.ZnodeUser zu    
				  INNER JOIN ZnodePriceListUser AS ZPLU ON ZPLU.UserId= zu.UserId   
				  INNER JOIN  ZnodePriceList AS ZPL ON (ZPL.PriceListId = ZPLU.PriceListId )  
				  LEFT JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId
				  INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId)  )   
         
				   ,CTE_UserList AS  
				   (  
					 SELECT DISTINCT PriceListUserId, Userid, UserName,ListCode,ListName, PriceListId , CultureName, ActivationDate ,ExpirationDate ,  
					 Precedence,CurrencyName,'+dbo.Fn_GetPagingRowId(@Order_BY,'PriceListUserId DESC')+',Count(*)Over() CountNo  
					 FROM  UserList WHERE 1=1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'       
				   )  
  
				   SELECT PriceListUserId, Userid, UserName,ListCode,ListName, PriceListId , CultureName, ActivationDate ,ExpirationDate ,Precedence,
				   CurrencyName,RowId,CountNo  
				   INTO #TM_PriceList   
				   FROM CTE_UserList  
				   '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
			 END;  
			 ELSE  
			 BEGIN  
				IF @Mode = 'Account'  
				BEGIN  
					SET @SQL = ' With AccountList AS   
					(SELECT za.Accountid,za.Name AccountName,ZPL.ListCode,ZPL.ListName,ZPL.PriceListId,substring(zc.CultureName,1,charindex(''('',zc.CultureName)-1) as CultureName,  
					ActivationDate ,ExpirationDate,ZPLA.Precedence,CASE WHEN za.parentAccountId IS NULL OR parentAccountId = 0  THEN CAST(1 AS BIT ) ELSE CAST(0 AS BIT ) END IsParentAccount ,PriceListAccountId  
					,ZCC.CurrencyName
					FROM ZnodePriceList AS ZPL INNER JOIN ZnodePriceListAccount AS ZPLA ON ZPL.PriceListId = ZPLA.PriceListId  
					INNER JOIN dbo.ZnodeCulture zc ON ZPL.CultureId = zc.CultureId  
					INNER JOIN dbo.ZnodeAccount za ON za.AccountId = ZPLA.AccountId
					INNER JOIN  ZnodeCurrency ZCC ON (ZCC.CurrencyId = zc.currencyId)   )
         
					,CTE_GetAccountList as  
					(  
					SELECT  DISTINCT Accountid,AccountName,ListCode,ListName,PriceListId,CultureName,ActivationDate,ExpirationDate,Precedence,IsParentAccount  
					,PriceListAccountId,CurrencyName,'+dbo.Fn_GetPagingRowId(@Order_BY,'Accountid DESC')+',Count(*)Over() CountNo   
					FROM AccountList WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'    
					)  
					SELECT Accountid,AccountName,ListCode,ListName,PriceListId,CultureName,ActivationDate,ExpirationDate,Precedence,IsParentAccount,  
					PriceListAccountId,CurrencyName,RowId,CountNo  
					INTO #TM_PriceList   
					FROM CTE_GetAccountList  
					'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)  
				END;  
			 END;  
		END;  
    END;  
  
     SET @SQL = @SQL+ ' SET @Count = ISNULL((SELECT TOP 1 CountNo FROM #TM_PriceList ),0)  
                          SELECT * FROM #TM_PriceList  '  
		--Print @sql
          EXEC SP_executesql  
                  @SQL,  
                  N'@Count INT OUT',  
                  @Count = @RowsCount OUT;  
        
     END TRY  
     BEGIN CATCH  
		  DECLARE @Status BIT ;  
		  SET @Status = 0;  
		  DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPriceList @WhereClause = '+cast (@WhereClause AS VARCHAR(50))
		  +',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Mode='+@Mode+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
		 SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
		EXEC Znode_InsertProcedureErrorLog  
		@ProcedureName = 'Znode_GetPriceList',  
		@ErrorInProcedure = @Error_procedure,  
		@ErrorMessage = @ErrorMessage,  
		@ErrorLine = @ErrorLine,  
		@ErrorCall = @ErrorCall;  
    END CATCH;  
END;  
  
  