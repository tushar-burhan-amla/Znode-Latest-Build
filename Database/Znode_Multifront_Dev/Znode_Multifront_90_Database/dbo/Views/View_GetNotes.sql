
CREATE VIEW [dbo].[View_GetNotes] AS
SELECT        ZA.NoteId,ZA.UserId,ZA.AccountId,Za.CaseRequestId,Za.NoteTitle,ZA.NoteBody,ZA.CreatedDate,APZU.UserName
FROM            dbo.ZnodeNote AS ZA 
					LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZA.CreatedBy)
					LEFT JOIN AspNetUsers APNU ON (APNU.Id = ZU.AspNetUserId )
					LEFT JOIN ASpNetZnodeUser APZU ON (APZU.AspNetZnodeUserId = APNU.UserName );