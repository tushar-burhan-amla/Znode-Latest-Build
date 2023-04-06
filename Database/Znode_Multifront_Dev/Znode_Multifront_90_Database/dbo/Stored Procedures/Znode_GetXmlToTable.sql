Create  procedure [dbo].[Znode_GetXmlToTable]
(
@XMLDOC varchar(8000)
,@qry nvarchar(max)
)
AS
begin
DECLARE @hdoc INT
DECLARE @xml VARCHAR(MAX)
SELECT @xml = @XMLDOC

DECLARE  @param NVARCHAR(50)
SELECT @param = N'@hdoc INT'

EXEC sp_xml_preparedocument @hdoc OUTPUT, @xml
EXEC sp_executesql @qry, @param, @hdoc
EXEC sp_xml_removedocument @hdoc

end