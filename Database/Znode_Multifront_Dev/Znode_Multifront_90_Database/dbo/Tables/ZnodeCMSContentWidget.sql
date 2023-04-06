CREATE TABLE ZnodeCMSContentWidget (
    CMSContentWidgetId int NOT NULL  IDENTITY(1,1),
	Name nvarchar(100) NOT NULL,
	WidgetKey nvarchar(50) NOT NULL ,
	PortalId int ,
	FamilyId int NOT NULL,
    Tags nvarchar(1000),
    CreatedBy int NOT NULL,
    CreatedDate datetime NOT NULL, 
    ModifiedBy int NOT NULL,
	ModifiedDate datetime  NOT NULL ,
	CONSTRAINT [PK_ZnodeCMSContentWidget] PRIMARY KEY CLUSTERED ([CMSContentWidgetId] ASC) WITH (FILLFACTOR = 90),

);