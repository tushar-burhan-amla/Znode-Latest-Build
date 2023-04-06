
CREATE FUNCTION [dbo].[Fn_RemoveSpecialChar]
(
  @str VARCHAR(2000)
)
RETURNS VARCHAR(2000)
AS
BEGIN
	
    DECLARE @expres  VARCHAR(50) = '%[~,@,#,$,%,&,*,(,),.,!,^,{,},:,",<,>,|,\,?,/,;]%'
      WHILE PATINDEX( @expres, @str ) > 0
          SET @str = REPLACE(Replace(REPLACE( @str, SUBSTRING( @str, PATINDEX( @expres, @str ), 1 ),''),'-',' '),' ', '' )
 
      RETURN @str
END