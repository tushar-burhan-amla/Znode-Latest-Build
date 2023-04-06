CREATE PROCEDURE Znode_ChangeStatusForIndexCreation
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE  ZnodePublishProductEntity SET ElasticSearchEvent = 1
END