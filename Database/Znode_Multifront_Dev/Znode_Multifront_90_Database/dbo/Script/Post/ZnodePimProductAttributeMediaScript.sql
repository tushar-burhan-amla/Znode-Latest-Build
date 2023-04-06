
-- Update the records before adding the constraint on ZnodePimProductAttributeMedia 
UPDATE ZnodePimProductAttributeMedia 
SET MediaId= NULL 
FROM ZnodePimProductAttributeMedia 
WHERE MediaId NOT IN (SELECT MediaId FROM ZnodeMedia) AND MediaId IS NOT NULL 

 
 --Adding constraint on ZnodePimProductAttributeMedia

IF NOT EXISTS(select  * from sys.sysconstraints where object_name(constid) = 'FK_ZnodePimProductAttributeMedia_ZnodePimAttributeValueId')
BEGIN 
ALTER TABLE [dbo].[ZnodePimProductAttributeMedia]  WITH CHECK ADD  CONSTRAINT [FK_ZnodePimProductAttributeMedia_ZnodePimAttributeValueId] FOREIGN KEY([PimAttributeValueId])
REFERENCES [dbo].[ZnodePimAttributeValue] ([PimAttributeValueId])

ALTER TABLE [dbo].[ZnodePimProductAttributeMedia] CHECK CONSTRAINT [FK_ZnodePimProductAttributeMedia_ZnodePimAttributeValueId]
end