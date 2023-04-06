CREATE FUNCTION [dbo].[Fn_CheckGlobalAttributeTransactionUsed]
(
   @CheckType Varchar(1000) 
  ,@Id        int
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
   if @CheckType ='GlobalAttribute'
   begin 
		   If exists (select 1 From ZnodePortalGlobalAttributeValue gg where @Id=gg.GlobalAttributeId )
						or    exists (select 1 From ZnodeUserGlobalAttributeValue gg where @Id=gg.GlobalAttributeId )
						or    exists (select 1 From ZnodeAccountGlobalAttributeValue gg where @Id =gg.GlobalAttributeId )
		   Begin
			     RETURN 1
		   End
		   Else
		   Begin
			     RETURN 0
		   End
   End
   Else  if @CheckType ='GlobalAttributeDefaultValue'
   begin 
		   If exists (select 1 From ZnodePortalGlobalAttributeValueLocale gg where @Id=gg.GlobalAttributeDefaultValueId )
						or    exists (select 1 From ZnodeUserGlobalAttributeValueLocale gg where @Id=gg.GlobalAttributeDefaultValueId )
						or    exists (select 1 From ZnodeAccountGlobalAttributeValueLocale gg where @Id =gg.GlobalAttributeDefaultValueId )
		   Begin
			     RETURN 1
		   End
		   Else
		   Begin
			     RETURN 0
		   End
   End


	RETURN 0

END