CREATE PROCEDURE [dbo].[Znode_AdminUsers]
(	@RoleName		VARCHAR(200),
    @UserName		VARCHAR(200),
    @WhereClause	XML,
    @Rows			INT           = 100,
    @PageNo			INT           = 1,
    @Order_By		VARCHAR(1000) = '',
    @RowCount		INT        = 0 OUT,
	@IsCallOnSite   BIT = 0 ,
	@PortalId		VARCHAR(1000) = 0,
	@IsGuestUser    BIT = 0,
	@ColumnName     dbo.SelectColumnList ReadOnly,
	@SalesRepUserId int = 0
)
AS
/* 
    Summary: List of users with details and shows link with ASPNet tables 
    This procedure is used for finding both users and admin users 
    here use three view "View_RoleUsers" for check  @UserName is present or not 
    "View_AdminUserDetail"  this view use for admin users 
    "View_CustomerUserDetail" Use for customer users 
    Unit Testing   
	SELECT * FROM ZnodeUser 
    DECLARE @EDE INT=0  EXEC Znode_AdminUsers '','admin@znode.com',@WhereClause='',@Order_By='',@PageNo= 1 ,@Rows= 214,@IsCallOnSite='false',@PortalId=0,@RowCount=@EDE OUT  SELECT @EDE
*/
BEGIN
    BEGIN TRY
    SET NOCOUNT ON;
			
    DECLARE @SQL NVARCHAR(MAX)= '', @PaginationWhereClause VARCHAR(300)= dbo.Fn_GetRowsForPagination(@PageNo, @Rows, ' WHERE RowId');
           
	----Verifying that the @SalesRepUserId is having 'Sales Rep' role
	IF NOT EXISTS
	(
		SELECT * FROM ZnodeUser ZU
		INNER JOIN AspNetZnodeUser ANZU ON ZU.UserName = ANZU.UserName
		INNER JOIN AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
		INNER JOIN AspNetUserRoles ANUR ON ANU.Id = ANUR.UserId
		Where Exists(select * from AspNetRoles ANR Where Name = 'Sales Rep' AND ANUR.RoleId = ANR.Id) 
		AND ZU.UserId = @SalesRepUserId
	)   
	Begin
		SET @SalesRepUserId = 0
	End

	if OBJECT_ID('tempdb..##CustomerUserAddDetail') is not null
		drop table ##CustomerUserAddDetail

	if OBJECT_ID('tempdb..##View_CustomerUserAddDetail') is not null
		drop table ##View_CustomerUserAddDetail

	IF OBJECT_ID('tempdb..#TBL_RowCount') is not null
		DROP TABLE #TBL_RowCount
	Create table #TBL_RowCount(RowsCount int )
	-----Split where clause XMl 
	CREATE TABLE #WhereColumnList(RowId Int identity, filterName varchar(max), WhereCondition varchar(max))
	insert into #WhereColumnList(filterName,WhereCondition)
	SELECT 
			Tbl.Col.value('key[1]', 'varchar(max)') as filterName,
			Tbl.Col.value('condition[1]', 'varchar(max)') WhereCondition
	FROM   @WhereClause.nodes('//filter') Tbl(Col) 
	----Address column in global search
	declare @AddressGlobalSearch varchar(1000) =''
	declare @GlobalSearch varchar(100) = ''
			
	select @GlobalSearch = substring(WhereCondition,charindex(' like ',WhereCondition), charindex(' OR ',WhereCondition)-charindex(' like ',WhereCondition)) 
	from #WhereColumnList
	where filtername like '%|%'
	and filtername <> ''
	and filterName in ('CityName','CountryName','PostalCode','StateName','CompanyName') 

			

	if isnull(@GlobalSearch,'') <> ''
	begin
		select @AddressGlobalSearch = '('+'CityName '+ @GlobalSearch+' OR '+'CountryName '+ @GlobalSearch+' OR '+'PostalCode '+ @GlobalSearch+' OR '+'StateName '+ @GlobalSearch+' OR '+'CompanyName '+ @GlobalSearch+')'
	end
	else
	begin
		SET @AddressGlobalSearch = ''
	end
	----Global search where clause
	declare @WhereClauseGlobal varchar(1000)=''
	select @WhereClauseGlobal = ISNULL(WhereCondition,'')
	from #WhereColumnList
	where filtername like '%|%'
	and filtername <> ''
			
	----Where clause columns except Address columns
	declare @WhereClause1 varchar(max) = ''
	select @WhereClause1 = COALESCE(@WhereClause1 + '', '') + WhereCondition+' And '
	--case when @WhereClause1 <> ''  then ' And ' else '' end
	from #WhereColumnList a
	where filterName not like '%|%' and
	filterName not in ('CountryName','CityName','StateName','PostalCode','CompanyName')
	and filtername <> ''

	if @WhereClause1 <> ''
	begin
		set @WhereClause1=isnull(substring(@WhereClause1,1,len(@WhereClause1)-3),'')
	end
	else
	begin
		set @WhereClause1 = ''
	end

	----Where clause columns
	declare @AddressColumnWhereClause varchar(max) 
	select @AddressColumnWhereClause = COALESCE(@AddressColumnWhereClause + '', '') + WhereCondition+' And '
	from #WhereColumnList a
	where filterName not like '%|%' and
	filterName in ('CountryName','CityName','StateName','PostalCode','CompanyName')
	and filtername <> ''
			
	if isnull(@AddressColumnWhereClause,'') <> ''
	begin
		set @AddressColumnWhereClause=isnull(substring(@AddressColumnWhereClause,1,len(@AddressColumnWhereClause)-3),'')
    end
	else
	begin
		set @AddressColumnWhereClause = ''
	end

	declare @WhereClauseAll varchar(max) = ''
	select @WhereClauseAll = COALESCE(@WhereClauseAll + '', '') + WhereCondition+' And '
	from #WhereColumnList a
			

	set @WhereClauseAll=isnull(CASE WHEN @WhereClauseAll = '' THEN '' ELSE substring(@WhereClauseAll,1,len(@WhereClauseAll)-3) END ,'')
	-------------- 
			
	IF @PortalId  <> '0' 
	BEGIN 
		SET @WhereClauseAll = CASE WHEN  @WhereClauseAll = '' THEN ' (PortalId IN ('+@PortalId+') OR PortalId IS NULL) ' ELSE @WhereClauseAll+' AND (PortalId IN ('+@PortalId+') OR PortalId IS NULL) ' END 

		SET @WhereClause1 = CASE WHEN  @WhereClause1 = '' THEN ' (isnull(PortalId,0) IN ('+@PortalId+') OR PortalId IS NULL) ' ELSE @WhereClause1+' AND (isnull(PortalId,0) IN ('+@PortalId+') OR PortalId IS NULL) ' END 
			
	END 

	IF EXISTS ( SELECT TOP 1 1 FROM View_RoleUsers  WHERE Username = @UserName   )  AND @RoleName <> ''  
	-- this check for admin user
    BEGIN
		SET @SQL = ' SELECT  A.UserId,AspNetUserId,UserName,FirstName,MiddleName,LastName,Email,EmailOptIn,BudgetAmount,A.CreatedBy,A.CreatedDate,A.ModifiedBy,A.ModifiedDate
		,RoleId,RoleName,IsActive,IsLock,FullName,AccountName,PermissionsName,PermissionCode,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId,PhoneNumber
		,ExternalId,ApprovalName,ApprovalUserId,AccountUserOrderApprovalId ,CustomerPaymentGUID
		INTO #Cte_AdminUserDetail
		FROM View_AdminUserDetail A
		'+CASE WHEN @PortalId  <> '0' THEN ' INNER JOIN ZnodeUserPortal ZUP ON (ZUP.UserId = A.UserId) 'ELSE '' END  +'	 
		'+dbo.Fn_GetWhereClause(@WhereClauseAll, ' WHERE ')+'
				
		;with Cte_AdminUserDetailRowId AS 
		(
		SELECT UserId,AspNetUserId,UserName,FirstName,MiddleName,LastName,Email,EmailOptIn,BudgetAmount,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		,RoleId,RoleName,IsActive,IsLock,FullName,AccountName,PermissionsName,PermissionCode,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId,PhoneNumber
		,ExternalId,ApprovalName,ApprovalUserId,AccountUserOrderApprovalId,CustomerPaymentGUID ,RANK()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC')+',UserId DESC) RowId
		FROM  #Cte_AdminUserDetail a
		where (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and a.UserId = SalRep.CustomerUserid) or '+cast(@SalesRepUserId as varchar(10))+' = 0)
		)
					 
		SELECT UserId,AspNetUserId,UserName,FirstName,MiddleName,LastName,Email,EmailOptIn,BudgetAmount,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		,RoleId,RoleName,IsActive,IsLock,FullName,AccountName,PermissionsName,PermissionCode,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId,PhoneNumber
		,ExternalId,ApprovalName,ApprovalUserId,AccountUserOrderApprovalId,CustomerPaymentGUID ,RowId 
		INTO #AccountDetails
		FROM Cte_AdminUserDetailRowId 
					 
		SET @Count= ISNULL((SELECT  Count(1) FROM #AccountDetails ),0)
					 
		SELECT DISTINCT UserId,AspNetUserId,UserName,FirstName,MiddleName,LastName,Email,EmailOptIn,BudgetAmount,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		,RoleId,RoleName,IsActive,IsLock,FullName,AccountName,PermissionsName,PermissionCode,DepartmentName,DepartmentId,AccountId,AccountPermissionAccessId,PhoneNumber
		,ExternalId,ApprovalName,ApprovalUserId,AccountUserOrderApprovalId ,CustomerPaymentGUID
		FROM #AccountDetails '+@PaginationWhereClause+' '+dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC' );
		EXEC SP_executesql
		@SQL,
		N'@Count INT OUT',
		@Count = @RowCount OUT;

				
	END;
	-- For Customer user
    ELSE   
	BEGIN
		IF @roleName = ''
		BEGIN
			if OBJECT_ID('tempdb..##CustomerUserAddDetail') is not null
			drop table ##CustomerUserAddDetail

			if OBJECT_ID('tempdb..##View_CustomerUserAddDetail') is not null
			drop table ##View_CustomerUserAddDetail
				
			if OBJECT_ID('tempdb..##UserList') is not null
			drop table ##UserList

			CREATE TABLE ##UserList(UserId int,AddressID int)

			declare @UserList varchar(1000)=''

			------To get the list of user having adress column in global search
			if (@AddressGlobalSearch <> '')
			begin
				
			set @UserList = 'select a.UserId, b.AddressID	from ZnodeUserAddress a	inner join ZnodeAddress b on a.AddressId = b.AddressId	where '+@AddressGlobalSearch
			--print @UserList
			insert into ##UserList(UserId, b.AddressID)
			exec (@UserList)
			
			end
			----To get the list of user having adress column in where clause 
			if (@AddressColumnWhereClause <> '')
			begin
					
			set @UserList = 'select a.UserId, b.AddressID	from ZnodeUserAddress a	inner join ZnodeAddress b on a.AddressId = b.AddressId	where '+@AddressColumnWhereClause
			--print @UserList
			insert into ##UserList(UserId,AddressID)
			exec (@UserList)
					
			end

			If @IsGuestUser= 0 
			AND
			NOT Exists (Select filterName from #WhereColumnList where filterName in ('accountid','isaccountcustomer','UserId') and filtername <> '')
			-- Customer List with GuestUsers
			Begin
				SET @SQL = 
					'SELECT a.userId,a.AspNetuserId,a.UserName,a.FirstName,a.MiddleName,a.LastName
					,a.PhoneNumber,
						a.Email,a.EmailOptIn,a.CreatedBy,CONVERT( DATETIME, a.CreatedDate) CreatedDate,A.ModifiedBy,
						CONVERT( DATETIME, a.ModifiedDate) ModifiedDate, 0 RoleId,''''  RoleName,
						(ISNULL(RTRIM(LTRIM(a.FirstName)), '''')+'' ''+ISNULL(RTRIM(LTRIM(a.MiddleName)), '''')+CASE
						WHEN ISNULL(RTRIM(LTRIM(a.MiddleName)), '''') = ''''	THEN ''''
						ELSE '' ''	END+ISNULL(RTRIM(LTRIM(a.LastName)), ''''))  FullName
						,a.AccountId, '''' TypeOfRole,CASE WHEN a.AspNetuserId IS NULL THEN 1 ELSE 0 END IsGuestUser
						into ##View_CustomerUserAddDetail
						FROM ZnodeUser a
						Where a.AspNetUserId IS NOT NULL
						AND (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and a.UserId = SalRep.CustomerUserid ) or '+cast(@SalesRepUserId as varchar(10))+' = 0)
					' 
				EXEC (@SQL)
			End	
			Else If @IsGuestUser= 1 
			Begin
					SET @SQL='SELECT a.userId,a.AspNetuserId,a.UserName,a.FirstName,a.MiddleName,a.LastName
					,a.PhoneNumber,
					a.Email,a.EmailOptIn,a.CreatedBy,CONVERT( DATETIME, a.CreatedDate) CreatedDate,A.ModifiedBy,
					CONVERT( DATETIME, a.ModifiedDate) ModifiedDate,ur.RoleId,r.Name RoleName,
					(ISNULL(RTRIM(LTRIM(a.FirstName)), '''')+'' ''+ISNULL(RTRIM(LTRIM(a.MiddleName)), '''')+CASE
					WHEN ISNULL(RTRIM(LTRIM(a.MiddleName)), '''') = '''' THEN '''' ELSE '' '' END+ISNULL(RTRIM(LTRIM(a.LastName)), '''')) FullName,
					a.AccountId,r.TypeOfRole,CASE WHEN a.AspNetuserId IS NULL THEN 1 ELSE 0 END IsGuestUser
					into ##View_CustomerUserAddDetail
					FROM ZnodeUser a
					LEFT JOIN AspNetUserRoles ur ON(ur.UserId = a.AspNetUserId)
					LEFT JOIN AspNetRoles r ON(r.Id = ur.RoleId)                       
					LEFT JOIN ZnodePortalAccount ZPA ON(ZPA.AccountId = a.AccountId) 
					WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeUSer ZUQ WHERE ZUQ.UserId = a.UserId AND ZUQ.EmailOptIn = 1 AND ZUQ.AspNetUserId IS NULL )
					AND a.AspNetuserId is null
					AND (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and a.UserId = SalRep.CustomerUserid ) or '+cast(@SalesRepUserId as varchar(10))+' = 0)'
				EXEC (@SQL)
			End
			Else IF Exists (Select filterName from #WhereColumnList where filterName in ('UserId') and filtername <> '')
			and  @IsGuestUser= 0   
			-- Customer List for user edit single user 
			Begin
			SET @SQL='SELECT a.userId,a.AspNetuserId,a.UserName,a.FirstName,a.MiddleName,a.LastName
			,a.PhoneNumber, a.Email,a.EmailOptIn,a.CreatedBy,CONVERT( DATETIME, a.CreatedDate) CreatedDate,A.ModifiedBy,
					CONVERT( DATETIME, a.ModifiedDate) ModifiedDate,ur.RoleId,r.Name RoleName,
					(ISNULL(RTRIM(LTRIM(a.FirstName)), '''')+'' ''+ISNULL(RTRIM(LTRIM(a.MiddleName)), '''')+CASE
					WHEN ISNULL(RTRIM(LTRIM(a.MiddleName)), '''') = '''' THEN '''' ELSE '' '' END+ISNULL(RTRIM(LTRIM(a.LastName)), '''')) FullName,
					a.AccountId, r.TypeOfRole,CASE WHEN a.AspNetuserId IS NULL THEN 1 ELSE 0 END IsGuestUser
					into ##View_CustomerUserAddDetail
					FROM ZnodeUser a
					LEFT JOIN ZnodeAccount e ON(e.AccountId = a.AccountId)
					LEFT JOIN AspNetUserRoles ur ON(ur.UserId = a.AspNetUserId)
					LEFT JOIN AspNetRoles r ON(r.Id = ur.RoleId)                       
					LEFT JOIN ZnodePortalAccount ZPA ON(ZPA.AccountId = a.AccountId) 
					WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeUSer ZUQ WHERE ZUQ.UserId = a.UserId AND ZUQ.EmailOptIn = 1 AND ZUQ.AspNetUserId IS NULL )
					AND a.AspNetuserId is not null
					and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = '+cast(@SalesRepUserId as varchar(10))+' and a.UserId = SalRep.CustomerUserid ) or '+cast(@SalesRepUserId as varchar(10))+' = 0)'
				print @SQL
						
				EXEC (@SQL)
			End	
			Else -- Account user List 
			Begin
					SELECT a.userId,a.AspNetuserId,a.UserName,a.PhoneNumber,a.FirstName,a.MiddleName,a.LastName
					,a.Email,a.EmailOptIn,a.CreatedBy,CONVERT( DATETIME, a.CreatedDate) CreatedDate,A.ModifiedBy,
					CONVERT( DATETIME, a.ModifiedDate) ModifiedDate,ur.RoleId,r.Name RoleName,
					(ISNULL(RTRIM(LTRIM(a.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(a.MiddleName)), '')+CASE
					WHEN ISNULL(RTRIM(LTRIM(a.MiddleName)), '') = '' THEN '' ELSE ' ' END+ISNULL(RTRIM(LTRIM(a.LastName)), '')) FullName,
					a.AccountId, r.TypeOfRole,CASE WHEN a.AspNetuserId IS NULL THEN 1 ELSE 0 END IsGuestUser
					into ##View_CustomerUserAddDetail
					FROM ZnodeUser a
					LEFT JOIN ZnodeAccount e ON(e.AccountId = a.AccountId)
					LEFT JOIN AspNetUserRoles ur ON(ur.UserId = a.AspNetUserId)
					LEFT JOIN AspNetRoles r ON(r.Id = ur.RoleId)                       
					LEFT JOIN ZnodePortalAccount ZPA ON(ZPA.AccountId = a.AccountId) 
					WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeUSer ZUQ WHERE ZUQ.UserId = a.UserId AND ZUQ.EmailOptIn = 1 AND ZUQ.AspNetUserId IS NULL )
					and (exists(select * from ZnodeSalesRepCustomerUserPortal SalRep where SalRep.SalesRepUserId = @SalesRepUserId and a.UserId = SalRep.CustomerUserid ) or @SalesRepUserId = 0)
			End
					
			alter table ##View_CustomerUserAddDetail 
			add StoreName varchar(1000), CountryName varchar(1000),CityName varchar(1000),StateName varchar(1000),
			PostalCode varchar(1000), CompanyName varchar(1000), SalesRepUserName varchar(600),SalesRepFullName varchar(1000), PortalId INT 

			IF @PortalId <> '' 
			BEGIN 
				UPDATE a SET a.PortalId = b.PortalId
				FROM ##View_CustomerUserAddDetail a 
				INNER JOIN ZnodeUserPortal b ON (b.Userid = a.Userid )
			END 

			IF (@WhereClauseAll like '%StoreName%' or @Order_By like '%StoreName%' )
			BEGIN
				CREATE NONCLUSTERED INDEX ##View_CustomerUserAddDetail_UserId
				ON [dbo].[##View_CustomerUserAddDetail] ([userId])

				update  a set StoreName = CASE WHEN zp.StoreName IS NULL THEN 'ALL' ELSE zp.StoreName END 
						        -- , PortalId = up.PortalId
				from ##View_CustomerUserAddDetail a
				Left join  ZnodeUserPortal up ON(up.UserId = a.UserId)  
				Left JOIN ZnodePortal zp ON (up.PortalId = zp.PortalId)
							
			END
					
			IF (@WhereClauseAll like '%CountryName%' OR @WhereClauseAll like '%CityName%' OR @WhereClauseAll like '%StateName%' OR @WhereClauseAll like '%PostalCode%' OR @WhereClauseAll like '%CompanyName%')
			BEGIN
			 
				update  a set CountryName = ZA.CountryName, CityName = za.CityName, StateName = ZA.StateName, 
				PostalCode = ZA.PostalCode, CompanyName = ZA.CompanyName
				from ##View_CustomerUserAddDetail a
				inner join ZnodeAccountAddress ZAA on a.AccountId = ZAA.AccountId
				inner  JOIN ZnodeAddress ZA on ZA.AddressId = ZAA.AddressId
				where isnull(a.AccountId,0)<> 0-- is not null
	 
				update  a set CountryName = ZA.CountryName, CityName = za.CityName, StateName = ZA.StateName, 
				PostalCode = ZA.PostalCode, CompanyName = ZA.CompanyName
				from ##View_CustomerUserAddDetail a
				inner join ZnodeUserAddress ZUA on a.UserId = ZUA.UserId
				inner  JOIN ZnodeAddress ZA on ZA.AddressId = zua.AddressId
			END

			set @Rows = @PageNo * @Rows

			SET @SQL = '			
						
				create table #AccountDetail
				(
					UserId int,AspNetuserId nvarchar(200),UserName nvarchar(200),FirstName varchar(200),MiddleName varchar(200),LastName varchar(200),
					PhoneNumber nvarchar(100),Email nvarchar(100),EmailOptIn bit,CreatedBy int,CreatedDate datetime,ModifiedBy int,ModifiedDate datetime,
					RoleId varchar(200),RoleName varchar(200), FullName  varchar(1000),
					StoreName varchar(200),CountryName varchar(200), CityName varchar(200), StateName varchar(200), PostalCode varchar(200), CompanyName varchar(200)
					,AccountId int,SalesRepUserName varchar(200),SalesRepFullName varchar(200) ,RowId int identity 
				) 
				'+
				+' insert into #AccountDetail(UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,
				EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,FullName,
				StoreName, CountryName, CityName, StateName, PostalCode, CompanyName,AccountId)
				SELECT top '+cast(@Rows as varchar(10))+'UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,
				EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,FullName,
				StoreName, CountryName, CityName, StateName, PostalCode, CompanyName,AccountId
				FROM ##View_CustomerUserAddDetail where 1=1'+
				dbo.Fn_GetWhereClause(case when @WhereClauseGlobal<>'' and @WhereClause1 <> '' then @WhereClauseGlobal+' And '+@WhereClause1 else @WhereClauseAll end, ' AND ')+
				dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC') + '
			
				Insert Into #TBL_RowCount 
				SELECT count(*)
				FROM ##View_CustomerUserAddDetail where 1=1'+
				dbo.Fn_GetWhereClause(case when @WhereClauseGlobal<>'' and @WhereClause1 <> '' then @WhereClauseGlobal+' And '+@WhereClause1 else @WhereClauseAll end, ' AND ')
				+'
						
				SELECT  UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,
				EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,FullName,
				StoreName,AccountId,
				CountryName, CityName, StateName, PostalCode, CompanyName, SalesRepUserName, SalesRepFullName
				,Row_Number()Over('+dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC')+')  RowNumber
				into ##CustomerUserAddDetail
				FROM #AccountDetail '+@PaginationWhereClause  +' '+ dbo.Fn_GetOrderByClause(@Order_By, 'UserId DESC');

				print @SQL
			EXEC (@SQL)

			Select @RowCount= isnull(RowsCount,0) from #TBL_RowCount

			ALTER TABLE ##CustomerUserAddDetail ADD AddressId Int

			------To get data for StoreName
			update  a 
			set StoreName = CASE WHEN zp.StoreName IS NULL THEN 'ALL' ELSE zp.StoreName END 
			from ##CustomerUserAddDetail a
			Left join  ZnodeUserPortal up ON(up.UserId = a.UserId)  
			Left JOIN ZnodePortal zp ON (up.PortalId = zp.PortalId)
	
			----To get data for CountryName, CityName, StateName, PostalCode, CompanyName
			IF (EXISTS(SELECT * FROM @ColumnName where ([StringColumn] LIKE '%CountryName%' OR [StringColumn] LIKE '%CityName%' OR [StringColumn] LIKE '%StateName%' OR [StringColumn] LIKE '%PostalCode%' OR [StringColumn] LIKE '%CompanyName%'))
			OR (@WhereClauseAll like '%CountryName%' OR @WhereClauseAll like '%CityName%' OR @WhereClauseAll like '%StateName%' OR @WhereClauseAll like '%PostalCode%' OR @WhereClauseAll like '%CompanyName%'))
			BEGIN
				update  a set CountryName = ZA.CountryName, CityName = za.CityName, StateName = ZA.StateName, 
				PostalCode = ZA.PostalCode, CompanyName = ZA.CompanyName, a.AddressId = ZA.AddressId
				from ##CustomerUserAddDetail a
				inner join ZnodeAccountAddress ZAA on a.AccountId = ZAA.AccountId
				inner  JOIN ZnodeAddress ZA on ZA.AddressId = ZAA.AddressId
				where isnull(a.AccountId,0)<> 0-- is not null
	 
				update  a set CountryName = ZA.CountryName, CityName = za.CityName, StateName = ZA.StateName, 
				PostalCode = ZA.PostalCode, CompanyName = ZA.CompanyName, a.AddressId = ZA.AddressId
				from ##CustomerUserAddDetail a
				inner join ZnodeUserAddress ZUA on a.UserId = ZUA.UserId
				inner  JOIN ZnodeAddress ZA on ZA.AddressId = zua.AddressId
			END
	
			----Updating SalesRep for user if any 
			update CUAD
			set CUAD.SalesRepUserName = ZU.UserName, 
			CUAD.SalesRepFullName = (ISNULL(RTRIM(LTRIM(ZU.FirstName)), '')+' '+ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '')
			+CASE
			WHEN ISNULL(RTRIM(LTRIM(ZU.MiddleName)), '') = ''
			THEN ''
			ELSE ' '
			END+ISNULL(RTRIM(LTRIM(ZU.LastName)), '')) 
			from ##CustomerUserAddDetail CUAD
			inner join ZnodeSalesRepCustomerUserPortal SRCUP ON CUAD.UserId = SRCUP.CustomerUserid 
			inner join ZnodeUser ZU ON SRCUP.SalesRepUserId = ZU.UserId

			if ( exists(select * from ##UserList) OR @AddressColumnWhereClause <> '')
			begin
				SELECT UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,
				EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,
				FullName,
				StoreName,
				CountryName, CityName, StateName, PostalCode, CompanyName, SalesRepUserName, SalesRepFullName
				from ##CustomerUserAddDetail CUAD
				where exists(select * from ##UserList UL where CUAD.UserId = UL.UserId and CUAD.AddressId = UL.AddressID )
				Order by RowNumber
			end
			else
			begin
				SELECT UserId,AspNetuserId,UserName,FirstName,MiddleName,LastName,PhoneNumber,Email,
				EmailOptIn,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,RoleId,RoleName,
				FullName,
				StoreName,
				CountryName, CityName, StateName, PostalCode, CompanyName, SalesRepUserName, SalesRepFullName
				from ##CustomerUserAddDetail
				Order by RowNumber
			end
	
			if OBJECT_ID('tempdb..##CustomerUserAddDetail') is not null
			drop table ##CustomerUserAddDetail

			if OBJECT_ID('tempdb..##View_CustomerUserAddDetail') is not null
			drop table ##View_CustomerUserAddDetail
				
		END;
		ELSE
		BEGIN
			SELECT * FROM View_CustomerUserDetail AS VICUD WHERE 1 = 0;
			SET @RowCount = 0;
		END;
    END;			
    END TRY
    BEGIN CATCH
		--SELECT ERROR_MESSAGE()
		DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_AdminUsers @RoleName = '+@RoleName+' ,@UserName='+@UserName+',@WhereClause='+cast(@WhereClause as varchar(max))+' ,@Rows= '+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_By='+@Order_By+',@RowCount='+CAST(@RowCount AS VARCHAR(50));
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName    = 'Znode_AdminUsers',
		@ErrorInProcedure = @ERROR_PROCEDURE,
		@ErrorMessage     = @ErrorMessage,
		@ErrorLine        = @ErrorLine,
		@ErrorCall        = @ErrorCall;
	END CATCH;
END;