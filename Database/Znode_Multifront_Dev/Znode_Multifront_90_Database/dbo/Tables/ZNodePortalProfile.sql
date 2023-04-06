CREATE TABLE [dbo].[ZnodePortalProfile] (
    [PortalProfileID]           INT           IDENTITY (1, 1) NOT NULL,
    [PortalId]                  INT           NOT NULL,
    [ProfileId]                 INT           NOT NULL,
    [IsDefaultAnonymousProfile] BIT           CONSTRAINT [DF_ZnodePortalProfile_IsDefaultAnonymousProfile] DEFAULT ((0)) NOT NULL,
    [IsDefaultRegistedProfile]  BIT           CONSTRAINT [DF_ZnodePortalProfile_IsDefaultRegistedProfile] DEFAULT ((1)) NOT NULL,
    [CreatedBy]                 INT           NOT NULL,
    [CreatedDate]               DATETIME      NOT NULL,
    [ModifiedBy]                INT           NOT NULL,
    [ModifiedDate]              DATETIME      NOT NULL,
    [ProfileNumber]             VARCHAR (100) NULL,
    CONSTRAINT [PK_ZNodePortalProfile] PRIMARY KEY CLUSTERED ([PortalProfileID] ASC),
    CONSTRAINT [FK_ZnodeActivityLog_ZnodePortalProfile] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId]),
    CONSTRAINT [FK_ZNodePortalProfile_ZNodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId])
);







