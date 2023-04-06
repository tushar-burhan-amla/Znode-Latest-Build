CREATE TABLE [dbo].[ZnodePublishState] (
    [PublishStateId]        TINYINT        IDENTITY (1, 1) NOT NULL,
    [StateName]             VARCHAR (32)   NOT NULL,
    [IsActive]              BIT            NOT NULL,
    [CreatedBy]             INT            NOT NULL,
    [CreatedDate]           DATETIME       NOT NULL,
    [ModifiedBy]            INT            NOT NULL,
    [ModifiedDate]          DATETIME       NOT NULL,
    [PublishStateCode]      VARCHAR (30)   NULL,
    [DisplayName]           VARCHAR (50)   NULL,
    [IsDefaultContentState] BIT            NOT NULL,
    [IsEnabled]             BIT            NOT NULL,
    [IsContentState]        BIT            NOT NULL,
    [Description]           NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([PublishStateId] ASC)
);

