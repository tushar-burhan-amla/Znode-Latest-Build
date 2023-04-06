CREATE TABLE [dbo].[ZnodeActivityLogType] (
    [ActivityLogTypeId] INT         NOT NULL,
    [Name]              NCHAR (255) NULL,
    [TypeCategory]      NCHAR (255) NULL,
    CONSTRAINT [PK_ZnodeActivityLogType] PRIMARY KEY CLUSTERED ([ActivityLogTypeId] ASC)
);

