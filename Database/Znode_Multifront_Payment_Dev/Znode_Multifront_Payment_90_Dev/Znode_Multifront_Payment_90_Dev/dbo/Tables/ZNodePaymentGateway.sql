CREATE TABLE [dbo].[ZNodePaymentGateway] (
    [PaymentGatewayId] INT           IDENTITY (1, 1) NOT NULL,
    [GatewayName]      VARCHAR (MAX) NOT NULL,
    [WebsiteURL]       VARCHAR (MAX) NULL,
    [ClassName]        VARCHAR (MAX) NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedDate]     DATETIME      NOT NULL,
    [GatewayCode]      VARCHAR (100) NULL,
    [IsACHEnabled] BIT NULL, 
    CONSTRAINT [PK_SC_Gateway] PRIMARY KEY CLUSTERED ([PaymentGatewayId] ASC)
);



