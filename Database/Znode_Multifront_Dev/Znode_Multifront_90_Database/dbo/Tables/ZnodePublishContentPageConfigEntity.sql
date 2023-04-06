CREATE TABLE [dbo].[ZnodePublishContentPageConfigEntity] (
    [PublishContentPageConfigEntityId] INT            IDENTITY (1, 1) NOT NULL,
    [VersionId]                        INT            NOT NULL,
    [PublishStartTime]                 DATETIME       NULL,
    [ContentPageId]                    INT            NOT NULL,
    [PortalId]                         INT            NOT NULL,
    [FileName]                         VARCHAR (300)  NULL,
    [ProfileId]                        VARCHAR (300)  NULL,
    [LocaleId]                         INT            NOT NULL,
    [PageTitle]                        VARCHAR (300)  NULL,
    [PageName]                         NVARCHAR (400) NULL,
    [ActivationDate]                   DATETIME       NULL,
    [ExpirationDate]                   DATETIME       NULL,
    [IsActive]                         BIT            NOT NULL,
    CONSTRAINT [PK_ZnodePublishContentPageConfigEntity] PRIMARY KEY CLUSTERED ([PublishContentPageConfigEntityId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [Ind_ZnodePublishContentPageConfigEntityVersionId]
    ON [dbo].[ZnodePublishContentPageConfigEntity]([VersionId] ASC);

