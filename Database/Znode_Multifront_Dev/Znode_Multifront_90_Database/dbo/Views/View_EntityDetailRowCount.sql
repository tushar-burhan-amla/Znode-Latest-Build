CREATE VIEW dbo.View_EntityDetailRowCount
AS
     SELECT CAST(ISNULL(0, 1) AS BIGINT) AS ROWID,
            '' AS [RowCount],
            '' AS IndexId,
            '' AS Leadvalue,
            '' AS Lagvalue;
