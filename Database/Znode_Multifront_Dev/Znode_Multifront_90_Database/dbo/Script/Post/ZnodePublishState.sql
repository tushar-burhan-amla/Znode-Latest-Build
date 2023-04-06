
--dt 1-07-2020 --> ZPD-7890>>ZPD-11111
INSERT INTO ZnodePublishState (StateName,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,
PublishStateCode,DisplayName,IsDefaultContentState,IsEnabled,IsContentState,Description)
SELECT 'Aborted', 1, 1, GETDATE(), 1, GETDATE(),
'ABORTED', 'Aborted', 0, 1, 0, null
WHERE NOT EXISTS 
(SELECT * FROM ZnodePublishState WHERE PublishStateCode ='ABORTED')

GO