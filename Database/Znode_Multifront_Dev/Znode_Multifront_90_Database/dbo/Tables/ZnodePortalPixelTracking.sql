CREATE TABLE [dbo].[ZnodePortalPixelTracking](
	[PortalPixelTrackingId] [int] IDENTITY(1,1) NOT NULL,
	[PortalId] [int] NOT NULL,
	[TrackingPixelScriptCode] [nvarchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [int] NULL,
	[ModifiedDate] [datetime] NULL,
    CONSTRAINT [PK_ZnodePortalPixelTracking] PRIMARY KEY CLUSTERED ([PortalPixelTrackingId] ASC),
    CONSTRAINT [FK_ZnodePortalPixelTracking_ZnodePortal] FOREIGN KEY ([PortalId]) REFERENCES [dbo].[ZnodePortal] ([PortalId])
);







