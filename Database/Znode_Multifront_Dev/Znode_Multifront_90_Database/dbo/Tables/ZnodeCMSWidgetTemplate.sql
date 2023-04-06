CREATE TABLE ZnodeCMSWidgetTemplate (
    CMSWidgetTemplateId int NOT NULL  IDENTITY(1,1),
	Code varchar(200) not null,
    Name nvarchar(100) NOT NULL,
	FileName nvarchar(2000) NOT NULL,
	MediaId int,
    CreatedBy int NOT NULL,
    CreatedDate datetime NOT NULL, 
    ModifiedBy int NOT NULL,
	ModifiedDate datetime  NOT NULL ,
	CONSTRAINT [PK_ZnodeCMSWidgetTemplate] PRIMARY KEY CLUSTERED ([CMSWidgetTemplateId] ASC) WITH (FILLFACTOR = 90)
);