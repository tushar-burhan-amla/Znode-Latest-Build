CREATE PROCEDURE [dbo].[Znode_DeleteHighlights]
(
@HighlightId VARCHAR(300) = NULL ,
@Status         INT OUT
)
AS
/*
    Summary:  Remove Highlights with their respective details and from reference tables              
    Unit Testing  
   
     DECLARE @Status INT
EXEC Znode_DeleteHighlights @HighlightId = 3 ,@Status=@Status OUT  
    */
BEGIN
BEGIN TRY
SET NOCOUNT ON;
BEGIN TRAN A;
DECLARE @DeletdHighlightId TABLE (HighlightCode VARCHAR(300));

INSERT INTO @DeletdHighlightId
SELECT b.HighlightCode
FROM dbo.split ( @HighlightId , ',') AS a
INNER JOIN ZnodeHighlight AS B ON ( a.item = b.HighlightId )

UPDATE ZPP SET PublishStateId = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName = 'Draft')
FROM ZnodePimProduct ZPP
INNER JOIN ZnodePimAttributeValue ZPAV ON ZPP.PimProductId = ZPAV.PimProductId
INNER JOIN ZnodePimProductAttributeDefaultValue ZPPADV ON ZPAV.PimAttributeValueId = ZPPADV.PimAttributeValueId
inner join ZnodePimAttributeDefaultValue ZPDV ON ZPPADV.PimAttributeDefaultValueId = ZPDV.PimAttributeDefaultValueId and ZPAV.PimAttributeId = ZPDV.PimAttributeId
INNER JOIN ZnodeHighlight ZHL ON ZPDV.AttributeDefaultValueCode = ZHL.HighlightCode
WHERE ZPDV.PimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute ZPD WHERE AttributeCode = 'Highlights')
and ZHL.HighlightCode IN ( SELECT HighlightCode FROM  @DeletdHighlightId)
     
DELETE FROM ZnodePimProductAttributeDefaultValue
WHERE EXISTS ( SELECT TOP 1 1 FROM ZnodePimAttributeDefaultValue ZD
INNER JOIN @DeletdHighlightId h on h.HighlightCode =zd.AttributeDefaultValueCode
INNER JOIN ZnodePimAttributeValue za ON zd.PimAttributeDefaultValueId = ZA.PimAttributeDefaultValueId
WHERE ZA.PimAttributeValueId =ZnodePimProductAttributeDefaultValue.PimAttributeValueId and ZA.PimAttributeDefaultValueId =ZnodePimProductAttributeDefaultValue.PimAttributeDefaultValueId);
 
DELETE FROM ZnodePimAttributeValue
WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeDefaultValue ZD
INNER JOIN @DeletdHighlightId h ON h.HighlightCode =zd.AttributeDefaultValueCode and zd.PimAttributeId = dbo.Fn_GetProductHighlightsAttributeId()
WHERE zd.PimAttributeDefaultValueId =ZnodePimAttributeValue.PimAttributeDefaultValueId)
AND PimAttributeValueId not in ( SELECT PimAttributeValueId FROM ZnodePimProductAttributeDefaultValue);

----Not to delete from attribute default data while deleting the highlight
--DELETE FROM ZnodePimAttributeDefaultValueLocale
--WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeDefaultValue ZD
--INNER JOIN @DeletdHighlightId h ON h.HighlightCode =zd.AttributeDefaultValueCode and zd.PimAttributeId = dbo.Fn_GetProductHighlightsAttributeId()
--WHERE zd.PimAttributeDefaultValueId =ZnodePimAttributeDefaultValueLocale.PimAttributeDefaultValueId);

--DELETE FROM ZnodePimAttributeDefaultValue
--WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdHighlightId H where H.HighlightCode= ZnodePimAttributeDefaultValue.AttributeDefaultValueCode
--AND ZnodePimAttributeDefaultValue.PimAttributeId = dbo.Fn_GetProductHighlightsAttributeId());
 
DELETE FROM ZnodeHighlightLocale
WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdHighlightId h INNER JOIN ZnodeHighlight ZH ON h.HighlightCode =ZH.HighlightCode
WHERE ZH.HighlightId  = ZnodeHighlightLocale.HighlightId);
 
DELETE FROM ZnodeHighlight
WHERE EXISTS (SELECT TOP 1 1 FROM @DeletdHighlightId h WHERE  h.HighlightCode =ZnodeHighlight.HighlightCode);

SET @Status = 1;

COMMIT TRAN A;                        
END TRY
BEGIN CATCH
ROLLBACK TRAN A;
SET @Status = 0;
DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteHighlights @HighlightId = '+@HighlightId+',@Status='+CAST(@Status AS VARCHAR(10));
             
        SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
   
        EXEC Znode_InsertProcedureErrorLog
@ProcedureName = 'Znode_DeleteHighlights',
@ErrorInProcedure = @Error_procedure,
@ErrorMessage = @ErrorMessage,
@ErrorLine = @ErrorLine,
@ErrorCall = @ErrorCall;

SELECT  ERROR_MESSAGE(),ERROR_LINE()
END CATCH
END
