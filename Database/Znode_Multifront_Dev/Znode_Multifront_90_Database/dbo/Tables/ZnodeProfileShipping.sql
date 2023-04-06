CREATE TABLE [dbo].[ZnodeProfileShipping] (
    [ProfileShippingId] INT      IDENTITY (1, 1) NOT NULL,
    [ProfileId]         INT      NULL,
    [ShippingId]        INT      NULL,
    [CreatedBy]         INT      NOT NULL,
    [CreatedDate]       DATETIME NOT NULL,
    [ModifiedBy]        INT      NOT NULL,
    [ModifiedDate]      DATETIME NOT NULL,
    [DisplayOrder]      INT      NULL,
    [PublishStateId]    TINYINT  NULL,
    CONSTRAINT [PK_ZnodeProfileShipping] PRIMARY KEY CLUSTERED ([ProfileShippingId] ASC),
    CONSTRAINT [FK_ZnodeProfileShipping_ZnodeProfile] FOREIGN KEY ([ProfileId]) REFERENCES [dbo].[ZnodeProfile] ([ProfileId]),
    CONSTRAINT [FK_ZnodeProfileShipping_ZnodePublishState] FOREIGN KEY ([PublishStateId]) REFERENCES [dbo].[ZnodePublishState] ([PublishStateId]),
    CONSTRAINT [FK_ZnodeProfileShipping_Znodeshipping] FOREIGN KEY ([ShippingId]) REFERENCES [dbo].[ZnodeShipping] ([ShippingId])
);





