CREATE TABLE [dbo].[ZnodeCMSContainerTemplate](
	[CMSContainerTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](200) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[FileName] [nvarchar](2000) NOT NULL,
	[MediaId] [int] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [int] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ZnodeCMSContainerTemplate] PRIMARY KEY CLUSTERED 
(
	[CMSContainerTemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO