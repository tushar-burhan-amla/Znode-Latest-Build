CREATE TABLE [dbo].[ZnodeManageConditionalDefaultData] 
(
    [ZnodeManageConditionalDefaultDataId]	INT	IDENTITY (1, 1) NOT NULL,
    [ConditionalCode]						NVARCHAR (100)  NOT NULL,
    [ConditionalName]						NVARCHAR (MAX) NULL,
	[DataSource]							NVARCHAR (20) NULL,
    [IsActive]								BIT NOT NULL DEFAULT (1),
    [CreatedBy]								INT NOT NULL,
    [CreatedDate]							DATETIME NOT NULL,
    [ModifiedBy]							INT NOT NULL,
    [ModifiedDate]							DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeManageConditionalDefaultData] PRIMARY KEY CLUSTERED ([ZnodeManageConditionalDefaultDataId] ASC),
	CONSTRAINT [UK_ZnodeManageConditionalDefaultData] UNIQUE (ConditionalCode),
);