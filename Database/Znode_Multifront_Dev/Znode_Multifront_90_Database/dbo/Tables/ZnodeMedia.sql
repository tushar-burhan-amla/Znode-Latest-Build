CREATE TABLE [dbo].[ZnodeMedia] (
    [MediaId]              INT           IDENTITY (1, 1) NOT NULL,
    [MediaConfigurationId] INT           NULL,
    [Path]                 VARCHAR (300) NULL,
    [FileName]             VARCHAR (300) NULL,
    [Size]                 VARCHAR (30)  NULL,
    [Height]               CHAR (10)     NULL,
    [Width]                CHAR (10)     NULL,
    [Length]               CHAR (10)     NULL,
    [Type]                 VARCHAR (300) NULL,
    [CreatedBy]            INT           NOT NULL,
    [CreatedDate]          DATETIME      NOT NULL,
    [ModifiedBy]           INT           NOT NULL,
    [ModifiedDate]         DATETIME      NOT NULL,
    [Version]              INT           CONSTRAINT [DF_ZnodeMedia_Version] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ZnodeMedia] PRIMARY KEY CLUSTERED ([MediaId] ASC),
    CONSTRAINT [FK_ZnodeMedia_ZnodeMediaConfiguration] FOREIGN KEY ([MediaConfigurationId]) REFERENCES [dbo].[ZnodeMediaConfiguration] ([MediaConfigurationId])
);











