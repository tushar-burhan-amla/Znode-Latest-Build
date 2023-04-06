CREATE TABLE [dbo].[ZNodeGateway] (
    [GatewayTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [GatewayName]   VARCHAR (MAX) NOT NULL,
    [WebsiteURL]    VARCHAR (MAX) NULL,
    [ClassName]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_SC_Gateway] PRIMARY KEY CLUSTERED ([GatewayTypeID] ASC)
);

