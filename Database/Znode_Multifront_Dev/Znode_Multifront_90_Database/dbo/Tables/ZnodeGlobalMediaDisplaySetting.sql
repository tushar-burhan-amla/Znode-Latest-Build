CREATE TABLE [dbo].[ZnodeGlobalMediaDisplaySetting](
	[GlobalMediaDisplaySettingsId] [int] IDENTITY(1,1) NOT NULL,
	[MediaId] [int] NULL,
	[MaxDisplayItems] [int] NULL,
	[MaxSmallThumbnailWidth] [int] NULL,
	[MaxSmallWidth] [int] NULL,
	[MaxMediumWidth] [int] NULL,
	[MaxThumbnailWidth] [int] NULL,
	[MaxLargeWidth] [int] NULL,
	[MaxCrossSellWidth] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodeGlobalMediaDisplaySetting] PRIMARY KEY CLUSTERED (	[GlobalMediaDisplaySettingsId] ASC)WITH (FILLFACTOR = 90)
) 