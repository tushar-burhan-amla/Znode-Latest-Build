CREATE TABLE [dbo].[ZnodePaymentGateway] (
    [PaymentGatewayId] INT           IDENTITY (1, 1) NOT NULL,
    [GatewayName]      VARCHAR (300) NOT NULL,
    [WebsiteURL]       VARCHAR (300) NULL,
    [CreatedBy]        INT           NOT NULL,
    [CreatedDate]      DATETIME      NOT NULL,
    [ModifiedBy]       INT           NOT NULL,
    [ModifiedDate]     DATETIME      NOT NULL,
    [GatewayCode]      VARCHAR (100) NULL,
    CONSTRAINT [PK_ZnodePaymentGateway] PRIMARY KEY CLUSTERED ([PaymentGatewayId] ASC)
);



