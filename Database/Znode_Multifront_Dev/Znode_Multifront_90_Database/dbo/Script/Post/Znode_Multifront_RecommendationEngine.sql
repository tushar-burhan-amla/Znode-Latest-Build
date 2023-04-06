
if not exists(select * from sys.databases where name = 'Znode_Multifront_RecommendationEngine')
begin
CREATE DATABASE [Znode_Multifront_RecommendationEngine]
 
ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET ANSI_NULL_DEFAULT ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET ANSI_NULLS ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET ANSI_PADDING ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET ANSI_WARNINGS ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET ARITHABORT ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET AUTO_CLOSE OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET AUTO_SHRINK OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET AUTO_UPDATE_STATISTICS ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET CURSOR_CLOSE_ON_COMMIT OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET CURSOR_DEFAULT  LOCAL 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET CONCAT_NULL_YIELDS_NULL ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET NUMERIC_ROUNDABORT OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET QUOTED_IDENTIFIER ON 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET RECURSIVE_TRIGGERS OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET  DISABLE_BROKER 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET DATE_CORRELATION_OPTIMIZATION OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET TRUSTWORTHY OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET ALLOW_SNAPSHOT_ISOLATION OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET PARAMETERIZATION SIMPLE 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET READ_COMMITTED_SNAPSHOT OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET HONOR_BROKER_PRIORITY OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET RECOVERY FULL 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET  MULTI_USER 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET PAGE_VERIFY NONE  

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET DB_CHAINING OFF 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET TARGET_RECOVERY_TIME = 0 SECONDS 

ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET DELAYED_DURABILITY = DISABLED 

end
go
USE [Znode_Multifront_RecommendationEngine]
GO


if not exists(select * from sys.tables where name = 'ZnodeRecommendationProcessingLogs')
begin 
	CREATE TABLE ZnodeRecommendationProcessingLogs
	(
	RecommendationProcessingLogsId  Int Primary Key Identity(1,1),
	PortalId  int,
	Status nvarchar(600) not null,
	LastProcessedOrderId  int not null,
	LastProcessedOrderDate  DateTime not null,
	CreatedBy int not null,
	CreatedDate datetime not null,
	ModifiedBy int not null,
	ModifiedDate datetime not null
	)
end

go
if not exists(select * from sys.tables where name = 'ZnodeRecommendationBaseProducts')
begin 
	CREATE TABLE ZnodeRecommendationBaseProducts  
	(
	RecommendationBaseProductsId  BigInt Primary Key Identity(1,1),
	SKU  nvarchar(600) not null,
	PortalId  int,
	RecommendationProcessingLogsId  int not null,
	CreatedBy int not null,
	CreatedDate datetime not null,
	ModifiedBy int not null,
	ModifiedDate datetime not null
	)

	alter table ZnodeRecommendationBaseProducts add constraint  FK_ZnodeRecommendationBaseProducts_RecommendationProcessingLogsId foreign key (RecommendationProcessingLogsId)
	references ZnodeRecommendationProcessingLogs(RecommendationProcessingLogsId)
end
 go
 if not exists(select * from sys.tables where name = 'ZnodeRecommendedProducts')
begin 
	CREATE TABLE ZnodeRecommendedProducts  
	(
	RecommendedProductsId  BigInt Primary Key Identity,
	RecommendationBaseProductsId BigInt not null,
	SKU  nvarchar(600) not null,
	Quantity  decimal(28,6),
	Occurrence  int not null,
	CreatedBy int not null,
	CreatedDate datetime not null,
	ModifiedBy int not null,
	ModifiedDate datetime not null
	)
	alter table ZnodeRecommendedProducts add constraint  FK_ZnodeRecommendedProducts_RecommendationBaseProductsId 
	foreign key (RecommendationBaseProductsId)
	references ZnodeRecommendationBaseProducts(RecommendationBaseProductsId)
end

go
if not exists(SELECT * FROM SYS.TABLE_TYPES WHERE IS_USER_DEFINED = 1 and name = 'RecommendationProcessedData')
begin 
	CREATE TYPE [dbo].[RecommendationProcessedData] AS TABLE(
		[RecommendationBaseProductsId] [bigint] NULL,
		[BaseSKU] [nvarchar](600) NULL,
		[PortalID] [int] NULL,
		[RecommendationProcessingLogsId] [int] NULL,
		[RecommendedProductsId] [bigint] NULL,
		[RecommendedSKU] [nvarchar](600) NULL,
		[Quantity] [numeric](28, 6) NULL,
		[Occurrence] [int] NULL
	)
end
go
 if exists(select * from sys.procedures where name = 'Znode_RecommendationProcessedData')
	drop proc Znode_RecommendationProcessedData
 go
CREATE PROCEDURE [dbo].[Znode_RecommendationProcessedData] 
(
	@UserID int = 2,
	@TableName varchar(500),
	@ProcessingTimeLimit int,
	@Status bit out
)
--Exec [Znode_RecommendationProcessedData] @TableName = '[##RecommendationData_1810e12a-cbed-4ceb-903c-0640f15c1e78]'
--,@ProcessingTimeLimit = 1000,@Status=0
as
begin

	begin tran
	set nocount on;
	declare @getdate datetime= getdate()
	
	if OBJECT_ID ('tempdb..#RecommendationProcessedData') is not null
		drop table #RecommendationProcessedData

	CREATE TABLE #RecommendationProcessedData(
	    ID int Primary Key identity,
		[RecommendationBaseProductsId] [bigint] NULL,
		[BaseSKU] [nvarchar](600) NULL,
		[PortalID] [int] NULL,
		[RecommendationProcessingLogsId] [int] NULL,
		[RecommendedProductsId] [bigint] NULL,
		[RecommendedSKU] [nvarchar](600) NULL,
		[Quantity] [numeric](28, 6) NULL,
		[Occurrence] [int] NULL
	)

	DECLARE @SQL VARCHAR(MAX)

	SET @SQL = '
	SELECT [RecommendationBaseProductsId],[BaseSKU],[PortalID],[RecommendationProcessingLogsId],
	       [RecommendedProductsId],[RecommendedSKU],[Quantity],[Occurrence]   FROM '+@TableName
	
	INSERT INTO #RecommendationProcessedData
	EXEC (@SQL)

	DECLARE @MaxCount INT, @MinRow INT, @MaxRow INT, @Rows numeric(10,2),@RowId int = 1, @ProcessTimeInLoop int = 0;
	SELECT @MaxCount = COUNT(*) FROM #RecommendationProcessedData 

	SELECT @Rows = 20000
        
	SELECT @MaxCount = CEILING(@MaxCount / @Rows);

	---- To get the min and max rows for import in loop
	;WITH cte AS 
	(
		SELECT RowId = 1, 
				MinRow = 1, 
                MaxRow = cast(@Rows as int)
        UNION ALL
        SELECT RowId + 1, 
                MinRow + cast(@Rows as int), 
                MaxRow + cast(@Rows as int)
        FROM cte
        WHERE RowId + 1 <= @MaxCount
	)
    SELECT RowId, MinRow, MaxRow
    INTO #Temp_ImportLoop
    FROM cte
	option (maxrecursion 0);
	
	while ( @ProcessTimeInLoop <= @ProcessingTimeLimit and @RowId <= @MaxCount )
	begin
		select @MinRow = MinRow, @MaxRow = MaxRow from #Temp_ImportLoop where RowId = @RowId
		
		----updating RecommendationBaseProducts data
		update ZRBP set ModifiedBy = @UserID, ModifiedDate = @getdate
		from #RecommendationProcessedData RPD
		inner join ZnodeRecommendationBaseProducts ZRBP ON RPD.BaseSKU = ZRBP.SKU 
						 and ISNULL(RPD.PortalId,0) = isnull(ZRBP.PortalId,0) and RPD.RecommendationProcessingLogsId = ZRBP.RecommendationProcessingLogsId
        where RPD.Id BETWEEN @MinRow AND @MaxRow

		----Inserting RecommendationBaseProducts data against portal
		insert into ZnodeRecommendationBaseProducts (SKU,PortalId,RecommendationProcessingLogsId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		select distinct BaseSKU, PortalID, RecommendationProcessingLogsId, @UserID, @getdate, @UserID, @getdate 
		from #RecommendationProcessedData RPD
		where not exists(select * from ZnodeRecommendationBaseProducts ZRBP where RPD.BaseSKU = ZRBP.SKU 
						 and ISNULL(RPD.PortalId,0) = isnull(ZRBP.PortalId,0) and RPD.RecommendationProcessingLogsId = ZRBP.RecommendationProcessingLogsId)
		and isnull(RPD.RecommendationBaseProductsId,0) = 0
		and RPD.Id BETWEEN @MinRow AND @MaxRow

		----updating new inserted base SKU RecommendationBaseProductsId in temp table
		update RPD set RecommendationBaseProductsId = ZRBP.RecommendationBaseProductsId
		from #RecommendationProcessedData RPD
		inner join ZnodeRecommendationBaseProducts ZRBP ON RPD.BaseSKU = ZRBP.SKU 
						 and ISNULL(RPD.PortalId,0) = isnull(ZRBP.PortalId,0) and RPD.RecommendationProcessingLogsId = ZRBP.RecommendationProcessingLogsId
		where isnull(RPD.RecommendationBaseProductsId,0) = 0 and RPD.Id BETWEEN @MinRow AND @MaxRow	 

		----Updating RecommendedProducts data for base SKU
		update ZRBP set Quantity = RPD.Quantity, Occurrence = RPD.Occurrence,ModifiedBy = @UserID, ModifiedDate = @getdate
		from #RecommendationProcessedData RPD
		inner join ZnodeRecommendedProducts ZRBP ON RPD.RecommendedSKU = ZRBP.SKU AND RPD.RecommendationBaseProductsId = ZRBP.RecommendationBaseProductsId
		where RPD.Id BETWEEN @MinRow AND @MaxRow

		----Inserting RecommendedProducts data against base SKU
		insert into ZnodeRecommendedProducts (RecommendationBaseProductsId,SKU,Quantity,Occurrence,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		select distinct RecommendationBaseProductsId, RecommendedSKU, Quantity, Occurrence, @UserID, @getdate, @UserID, @getdate
		from #RecommendationProcessedData RPD
		where not exists(select * from ZnodeRecommendedProducts ZRBP where RPD.RecommendedSKU = ZRBP.SKU AND RPD.RecommendationBaseProductsId = ZRBP.RecommendationBaseProductsId)
	    and RPD.Id BETWEEN @MinRow AND @MaxRow
		
		set @RowId = @RowId+1
		set @ProcessTimeInLoop = (cast(Datediff(ms, @getdate,getdate()) AS bigint))
	end


	if (@ProcessTimeInLoop>@ProcessingTimeLimit)
	begin
		rollback tran
		set @Status = 0  
		--select @Status
	end
	else 
	begin
		commit tran
		set @Status = 1
		--select @Status
	end

end
GO
 if exists(select * from sys.procedures where name = 'Znode_CreateTempTable')
	drop proc Znode_CreateTempTable
 go
CREATE PROCEDURE [dbo].[Znode_CreateTempTable]
(
  @TableName NVARCHAR(150),
  @ColumnList NVARCHAR(MAX)
)
AS
BEGIN
DECLARE @SQLString NVARCHAR(MAX)
iF @TableName <> '' AND @ColumnList <> ''
SET @SQLString = 'CREATE TABLE '+@TableName + '  '+@ColumnList+' '

EXEC (@SQLString)

END
go
USE [master]
GO
ALTER DATABASE [Znode_Multifront_RecommendationEngine] SET  READ_WRITE 
GO
