﻿	CREATE  FUNCTION [dbo].[FN_GetEncodedURL]  
   (
       @url NVARCHAR(max) 
   )
   RETURNS NVARCHAR(max)
   AS
    BEGIN  
   DECLARE @count INT, @c NCHAR(1), @i INT, @urlReturn NVARCHAR(max)
   SET @count = LEN(@url)
   SET @i = 1
   SET @urlReturn = ''    
   WHILE (@i <= @count)
    BEGIN
       SET @c = SUBSTRING(@url, @i, 1)
       IF @c LIKE N'[A-Za-z0-9()''*\-._!~]' COLLATE Latin1_General_BIN ESCAPE N'\' COLLATE Latin1_General_BIN
        BEGIN
           SET @urlReturn = @urlReturn + @c
        END
       ELSE
        BEGIN
           SET @urlReturn = 
                  @urlReturn + '%'
                  + SUBSTRING(sys.fn_varbintohexstr(CAST(@c AS VARBINARY(MAX))),3,2)
                  + ISNULL(NULLIF(SUBSTRING(sys.fn_varbintohexstr(CAST(@c AS VARBINARY(MAX))),5,2), '00'), '')
        END
       SET @i = @i +1
    END
   RETURN @urlReturn
   END