CREATE TABLE [dbo].[ZnodeSearchProfile] (
    [SearchProfileId]      INT            IDENTITY (1, 1) NOT NULL,
    [ProfileName]          NVARCHAR (200) NULL,
    [SearchQueryTypeId]    INT            NOT NULL,
    [SearchSubQueryTypeId] INT            NULL,
    [Operator]             VARCHAR (20)   NOT NULL,
    [IsDefault]            BIT            CONSTRAINT [DF_ZnodeSearchProfile_IsDefault] DEFAULT ((0)) NOT NULL,
    [CreatedBy]            INT            NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL,
    [ModifiedBy]           INT            NOT NULL,
    [ModifiedDate]         DATETIME       NOT NULL,
    [PublishStateId]       INT            NULL, 
    CONSTRAINT [PK_ZnodeSearchProfile] PRIMARY KEY CLUSTERED ([SearchProfileId] ASC),
    CONSTRAINT [FK_ZnodeSearc hProfile_ZnodeSearchQueryType] FOREIGN KEY ([SearchQueryTypeId]) REFERENCES [dbo].[ZnodeSearchQueryType] ([SearchQueryTypeId])
);







