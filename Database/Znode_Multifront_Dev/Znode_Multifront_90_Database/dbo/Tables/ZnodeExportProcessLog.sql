CREATE TABLE ZnodeExportProcessLog
(
	ExportProcessLogId INT IDENTITY(1,1) NOT NULL,
	ExportType NVARCHAR (100) NOT NULL,
	FileType NVARCHAR(100) NOT NULL,
	[Status] NVARCHAR (50) NULL,
	ProcessStartedDate DATETIME NOT NULL,
	ProcessCompletedDate DATETIME NULL,
	TableName NVARCHAR (100) NULL,
	CreatedBy INT NOT NULL,
	CreatedDate DATETIME NOT NULL,
	ModifiedBy INT NOT NULL,
	ModifiedDate DATETIME NOT NULL,
	CONSTRAINT [PK_ZnodeExportProcessLog] PRIMARY KEY CLUSTERED ([ExportProcessLogId] ASC)
)