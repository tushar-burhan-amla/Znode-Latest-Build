create function [dbo].[Fn_RandomString]
(
    @pStringLength int = 10 --set desired string length

) returns varchar(max)

/* Requires View create view dbo.View_NewID as select newid() as NewIDValue;

select [dbo].[Fn_RandomString] (10)
 */

as begin
 declare  @RandomString varchar(max);

    with
    a1 as (select 1 as N 
		   union all
           select 1 
		   union all
           select 1 
		   union all
           select 1 
		   union all
           select 1 
		   union all
           select 1 
		   union all
		   select 1 
		   union all
		   select 1 
		   union all
           select 1 
		   union all
           select 1),
    a2 as (select 1 as N
           from a1 as a
		   cross join a1 as b),
    a3 as (select 1 as N
           from a2 as a cross join a2 as b),
    a4 as (select 1 as N
           from a3 as a
		   cross join a2 as b),
    Tally as (select row_number() over (order by N) as N
				from a4)
    , cteRandomString (RandomString
    ) as ( select top (@pStringLength)
        substring(x,(abs(checksum((select NewIDValue from View_NewID)))%36)+1,1)

    from Tally cross join (select x='0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ') a

    )
     select @RandomString =replace((select '','' + RandomString
     from cteRandomString
     for xml path ('')),'','');

	 
    return (@RandomString);



end