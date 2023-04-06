CREATE TABLE [dbo].[ZnodeSearchProfileTrigger] (
    [SearchProfileTriggerId] INT             IDENTITY (1, 1) NOT NULL,
    [Keyword]                NVARCHAR (1000) NULL,
    [ProfileId]              INT             NULL,
    [SearchProfileId]        INT             NOT NULL,
    [CreatedBy]              INT             NOT NULL,
    [CreatedDate]            DATETIME        NOT NULL,
    [ModifiedBy]             INT             NOT NULL,
    [ModifiedDate]           DATETIME        NOT NULL,
    CONSTRAINT [PK_ZnodeTriggerSearchProfile] PRIMARY KEY CLUSTERED ([SearchProfileTriggerId] ASC),
    CONSTRAINT [FK_ZnodeSearchProfileTrigger_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId]),
    CONSTRAINT [FK_ZnodeSearchProfileTrigger_ZnodeSearchProfile] FOREIGN KEY ([SearchProfileId]) REFERENCES [dbo].[ZnodeSearchProfile] ([SearchProfileId])
);





