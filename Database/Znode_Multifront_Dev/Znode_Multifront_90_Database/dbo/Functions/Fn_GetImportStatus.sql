-- =============================================
-- Description:	Get staus of import 
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetImportStatus](
               @ImportStatus INT = 0)
RETURNS VARCHAR(100)
AS
     BEGIN
         RETURN CASE
                    WHEN @ImportStatus = 0
                    THEN 'Started'
                    WHEN @ImportStatus = 1
                    THEN 'In Process'
                    WHEN @ImportStatus = 2
                    THEN 'Completed Successfully'
                    WHEN @ImportStatus = 3
                    THEN 'Failed'
					WHEN @ImportStatus = 4
                    THEN 'Completed With Errors'
                END;
     END;