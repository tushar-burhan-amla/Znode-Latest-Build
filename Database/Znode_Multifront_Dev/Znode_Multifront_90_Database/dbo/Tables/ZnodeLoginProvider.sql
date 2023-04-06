CREATE TABLE [dbo].[ZnodeLoginProvider] (
    [LoginProviderId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [CreatedBy]       INT            NOT NULL,
    [CreatedDate]     DATETIME       NOT NULL,
    [ModifiedBy]      INT            NOT NULL,
    [ModifiedDate]    DATETIME       NOT NULL,
    CONSTRAINT [PK_ZnodeLoginProvider] PRIMARY KEY CLUSTERED ([LoginProviderId] ASC)
);

