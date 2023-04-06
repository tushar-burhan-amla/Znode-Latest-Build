CREATE FUNCTION [dbo].[Split] (
      @InputString                  VARCHAR(max),
      @Delimiter                    VARCHAR(50)
)

RETURNS @Items TABLE ( Id int Identity(1,1)
      ,Item                          VARCHAR(8000)
)

AS
BEGIN
      IF @Delimiter = ' '
      BEGIN
            SET @Delimiter = ','
            SET @InputString = REPLACE(@InputString, ' ', @Delimiter)
      END

	  DECLARE @StartIndex INT, @EndIndex INT
 
      SET @StartIndex = 1
      IF SUBSTRING(@InputString, LEN(@InputString) - 1, LEN(@InputString)) <> @Delimiter
      BEGIN
            SET @InputString = @InputString + @Delimiter
      END
 
      WHILE CHARINDEX(@Delimiter, @InputString) > 0
      BEGIN
            SET @EndIndex = CHARINDEX(@Delimiter, @InputString)
           
            INSERT INTO @Items(Item)
            SELECT SUBSTRING(@InputString, @StartIndex, @EndIndex - 1)
           
            SET @InputString = SUBSTRING(@InputString, @EndIndex + 1, LEN(@InputString))
      END
 
      RETURN
END -- End Function
