CREATE TABLE [dbo].[ZnodeOmsSavedCartLineItemDetails] (
    [ZnodeOmsSavedCartLineItemDetailId] INT            IDENTITY (1, 1) NOT NULL,
    [OmsSavedCartLineItemId]            INT            NULL,
    [OmsSavedCartId]                    INT            NULL,
    [Key]                               NVARCHAR (200) NULL,
    [Value]                             NVARCHAR (MAX) NULL,
    [CreatedBy]                         INT            NULL,
    [CreatedDate]                       DATETIME       NULL,
    [ModifiedBy]                        INT            NULL,
    [ModifiedDate]                      DATETIME       NULL,
    CONSTRAINT [PK_ZnodeOmsSavedCartLineItemDetails] PRIMARY KEY CLUSTERED ([ZnodeOmsSavedCartLineItemDetailId] ASC)
);

