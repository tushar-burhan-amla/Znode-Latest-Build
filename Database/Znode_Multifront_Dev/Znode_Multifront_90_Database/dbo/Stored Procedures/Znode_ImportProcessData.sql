--EXEC Znode_ImportProcessData 1
CREATE  PROCEDURE [dbo].[Znode_ImportProcessData] 
(

	@ImportHeadId INT , @TblGUID nvarchar(255),@TouchPointName nvarchar(200) 

)
AS 
BEGIN

	Declare @ERPTaskSchedulerId int 
	SET NOCOUNT ON;
	
	select @ERPTaskSchedulerId = ERPTaskSchedulerId from ZnodeERPTaskScheduler a
	inner join ZnodeERPConfigurator b on(a.ERPConfiguratorId = b.ERPConfiguratorId and TouchPointName = @TouchPointName)


	IF EXISTS ( SELECT ImportHeadId From ZnodeImportHead where ImportHeadId = @ImportHeadId AND Name = 'Product' )
	Begin
		EXEC [dbo].[Znode_ImportProcessProductData] @TblGUID=@TblGUID,@ERPTaskSchedulerId= @ERPTaskSchedulerId
	End
	Else If EXISTS ( SELECT ImportHeadId From ZnodeImportHead where ImportHeadId = @ImportHeadId AND Name = 'Pricing' )
	Begin
		EXEC [dbo].[Znode_ImportProcessPriceData] @TblGUID=@TblGUID,@ERPTaskSchedulerId= @ERPTaskSchedulerId
	End
	Else If EXISTS ( SELECT ImportHeadId From ZnodeImportHead where ImportHeadId = @ImportHeadId AND Name = 'Inventory' )
	Begin
		EXEC [dbo].[Znode_ImportProcessInventoryData] @TblGUID=@TblGUID,@ERPTaskSchedulerId= @ERPTaskSchedulerId
	End

	Else If EXISTS ( SELECT ImportHeadId From ZnodeImportHead where ImportHeadId = @ImportHeadId AND Name IN ( 'ProductAttribute' ))
	Begin
		EXEC [dbo].[Znode_ImportProcessAttributeData] @TblGUID=@TblGUID,@ERPTaskSchedulerId= @ERPTaskSchedulerId
	End
	Else If EXISTS ( SELECT ImportHeadId From ZnodeImportHead where ImportHeadId = @ImportHeadId AND Name in ('CustomerAddress', 'Customer' ))
	Begin
		EXEC [dbo].[Znode_ImportProcessCustomer] @TblGUID=@TblGUID,@ERPTaskSchedulerId= @ERPTaskSchedulerId
	End
	Else If EXISTS ( SELECT ImportHeadId From ZnodeImportHead where ImportHeadId = @ImportHeadId AND Name in ('Category' ))
	Begin
		EXEC [dbo].[Znode_ImportProcessCategoryData] @TblGUID=@TblGUID,@ERPTaskSchedulerId= @ERPTaskSchedulerId
	End

	


END