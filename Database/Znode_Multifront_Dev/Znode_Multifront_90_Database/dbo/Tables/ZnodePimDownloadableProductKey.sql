CREATE TABLE [dbo].[ZnodePimDownloadableProductKey] (
    [PimDownloadableProductKeyId] INT             IDENTITY (1, 1) NOT NULL,
    [PimDownloadableProductId]    INT             NOT NULL,
    [DownloadableProductKey]      NVARCHAR (250)  NULL,
    [DownloadableProductURL]      NVARCHAR (2000) NULL,
    [IsUsed]                      BIT             CONSTRAINT [DF_ZnodePimDownloadableProductKey_IsUsed] DEFAULT ((0)) NOT NULL,
    [WarehouseId]                 INT             NULL,
    [CreatedBy]                   INT             NOT NULL,
    [CreatedDate]                 DATETIME        NOT NULL,
    [ModifiedBy]                  INT             NOT NULL,
    [ModifiedDate]                DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodePimDownloadableProductKey] PRIMARY KEY CLUSTERED ([PimDownloadableProductKeyId] ASC),
    CONSTRAINT [FK_ZnodePimDownloadableProductKey_ZnodePimDownloadableProduct] FOREIGN KEY ([PimDownloadableProductId]) REFERENCES [dbo].[ZnodePimDownloadableProduct] ([PimDownloadableProductId]),
    CONSTRAINT [IX_ZnodePimDownloadableProductKey] UNIQUE NONCLUSTERED ([DownloadableProductKey] ASC)
);










GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[Trg_ZnodePimDownloadableProductKey] 
   ON  [dbo].[ZnodePimDownloadableProductKey]
   AFTER INSERT,DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	Declare @Quantity int 
	Declare @InsertedQuantity TABLE (SKU Nvarchar(300), Quantity numeric(28,6))
	insert into @InsertedQuantity (SKU , Quantity) 
	Select ZPDP.SKU , count(ID.DownloadableProductKey) from inserted ID 
	Inner join ZnodePimDownloadableProduct ZPDP On 
	ID.PimDownloadableProductId  =  ZPDP.PimDownloadableProductId   GROUP BY ZPDP.SKU 

	Update ZI SET ZI.Quantity = ZI.Quantity  + IQ.Quantity
	from ZnodeInventory ZI Inner join @InsertedQuantity  IQ on ZI.SKU = IQ.SKU
	 
	delete from @InsertedQuantity 
	insert into @InsertedQuantity (SKU , Quantity) 
	Select ZPDP.SKU , count(ID.DownloadableProductKey) from DELETED ID Inner join ZnodePimDownloadableProduct ZPDP On 
	ID.PimDownloadableProductId  =  ZPDP.PimDownloadableProductId   GROUP BY ZPDP.SKU 
	
	Update ZI SET ZI.Quantity = ZI.Quantity  - IQ.Quantity
	from ZnodeInventory ZI Inner join @InsertedQuantity  IQ on ZI.SKU = IQ.SKU 

    -- Insert statements for trigger here

END