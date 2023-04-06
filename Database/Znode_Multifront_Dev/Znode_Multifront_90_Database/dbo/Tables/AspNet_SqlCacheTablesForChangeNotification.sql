CREATE TABLE [dbo].[AspNet_SqlCacheTablesForChangeNotification] (
    [tableName]           NVARCHAR (450) NOT NULL,
    [notificationCreated] DATETIME       CONSTRAINT [DF__AspNet_Sq__notif__44867965] DEFAULT (getdate()) NOT NULL,
    [changeId]            INT            CONSTRAINT [DF__AspNet_Sq__chang__457A9D9E] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK__AspNet_SqlCacheT__4392552C] PRIMARY KEY CLUSTERED ([tableName] ASC)
);

