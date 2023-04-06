CREATE TABLE [dbo].[ZnodeReportSetting] (
    [ReportSettingId]  INT           IDENTITY (1, 1) NOT NULL,
    [ReportCode]       VARCHAR (100) NULL,
    [SettingXML]       TEXT          NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedBy]       INT           NOT NULL,
    [ModifiedDate]     DATETIME      NOT NULL,
    [DisplayMode]      BIT           NULL,
    [StyleSheetId]     INT           NULL,
    [DefaultLayoutXML] NTEXT         NULL,
    CONSTRAINT [PK_ZNnodeReportSetting] PRIMARY KEY CLUSTERED ([ReportSettingId] ASC)
);

