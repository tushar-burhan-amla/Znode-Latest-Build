
-- dt-10-12-2019 ZPD-7021 --> ZPD-8262 
insert into ZnodeSearchFeature(ParentSearchFeatureId,FeatureCode,FeatureName,IsAdvanceFeature,ControlType,HelpDescription
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select null,'DfsQueryThenFetch','Enable Accurate Scoring',0,'Yes/No','Find all the matching documents from all the shards from the index and apply the relevance score to get a more relevant result. Note: Enabling this feature may hamper the performance of the query by 20%',
2,getdate(),2,getdate()
where not exists(select * from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch' )

insert into ZnodeSearchQueryTypeFeature (SearchFeatureId,SearchQueryTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch'),
(select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Match'),
2,getdate(),2,GETDATE()
where not exists(select * from ZnodeSearchQueryTypeFeature
where SearchFeatureId = (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch')
and SearchQueryTypeId = (select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Match')
)

insert into ZnodeSearchQueryTypeFeature (SearchFeatureId,SearchQueryTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch'),
(select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Match Phrase'),
2,getdate(),2,GETDATE()
where not exists(select * from ZnodeSearchQueryTypeFeature
where SearchFeatureId = (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch')
and SearchQueryTypeId = (select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Match Phrase')
)

insert into ZnodeSearchQueryTypeFeature (SearchFeatureId,SearchQueryTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch'),
(select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Match Phrase Prefix'),
2,getdate(),2,GETDATE()
where not exists(select * from ZnodeSearchQueryTypeFeature
where SearchFeatureId = (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch')
and SearchQueryTypeId = (select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Match Phrase Prefix')
)

insert into ZnodeSearchQueryTypeFeature (SearchFeatureId,SearchQueryTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch'),
(select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match'),
2,getdate(),2,GETDATE()
where not exists(select * from ZnodeSearchQueryTypeFeature
where SearchFeatureId = (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'DfsQueryThenFetch')
and SearchQueryTypeId = (select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match')
)

---dt 02-04-2020 ZPD-9689
DELETE A  
from  ZnodeSearchQueryTypeFeature A 
inner join ZnodeSearchFeature B  ON A.SearchFeatureId= B.SearchFeatureId
inner Join ZnodeSearchQueryType C ON A.SearchQueryTypeId= C.SearchQueryTypeId where C.QueryTypeName in ('Match Phrase','Match Phrase Prefix') 
AND B.FeatureCode='AutoCorrect'

-- dt 25-11-2021 ZPD-16534
Update ZnodeSearchFeature 
set HelpDescription = 'Edit the characters in a word to make it same as another word. Number of edits is calculated basis the character count. Auto Correct will not function when Multi Match-> Cross option is selected.' 
where FeatureCode='AutoCorrect'

-- dt 22-12-2021 ZPD-16988
Update ZnodeSearchFeature set HelpDescription = 'When an entered keyword is misspelled, Autocorrect helps in identifying the correct spelling to display the associated product.' 
where FeatureCode = 'AutoCorrect' and HelpDescription not like 'When an entered%'

-- dt 04-01-2022 ZPD-16988
Update ZnodeSearchFeature set HelpDescription = 'When an entered keyword is misspelled, Autocorrect helps in identifying the correct spelling to display the associated product. Autocorrect does not work with query type Multimatch-Cross and therefore it’s value should be set as No for such scenario.' 
where FeatureCode = 'AutoCorrect' and HelpDescription not like '%No for such scenario.'

-- dt 26-05-2022 ZPD-19939
insert into ZnodeSearchFeature(ParentSearchFeatureId,FeatureCode,FeatureName,IsAdvanceFeature,ControlType,HelpDescription
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select null,'MinGram','Min Gram',0,'Text','Used to break a text into words, this is helpful to create tokens(set of words) that can be used to search the desired output in search results. 
Define minimum character length for the search tokens to be generated here. Default N-gram setting should be used for better results. In case if the default setting is changed then unexpected results can occur.',
2,getdate(),2,getdate()
where not exists(select * from ZnodeSearchFeature where FeatureCode = 'MinGram' )

insert into ZnodeSearchQueryTypeFeature (SearchFeatureId,SearchQueryTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'MinGram'),
(select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match'),
2,getdate(),2,GETDATE()
where not exists(select * from ZnodeSearchQueryTypeFeature
where SearchFeatureId = (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'MinGram')
and SearchQueryTypeId = (select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match')
)

insert into ZnodeSearchFeature(ParentSearchFeatureId,FeatureCode,FeatureName,IsAdvanceFeature,ControlType,HelpDescription
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select null,'MaxGram','Max Gram',0,'Text','Used to break a text into words, this is helpful to create tokens(set of words) that can be used to search the desired output in search results. 
Define maximum character length for the search tokens to be generated here. Default N-gram setting should be used for better results. In case if the default setting is changed then unexpected results can occur.',
2,getdate(),2,getdate()
where not exists(select * from ZnodeSearchFeature where FeatureCode = 'MaxGram' )

insert into ZnodeSearchQueryTypeFeature (SearchFeatureId,SearchQueryTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'MaxGram'),
(select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match'),
2,getdate(),2,GETDATE()
where not exists(select * from ZnodeSearchQueryTypeFeature
where SearchFeatureId = (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'MaxGram')
and SearchQueryTypeId = (select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match')
)

insert into ZnodeSearchFeature(ParentSearchFeatureId,FeatureCode,FeatureName,IsAdvanceFeature,ControlType,HelpDescription
,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select null,'CharacterFilter','Character Filter',0,'Text','Define a mapping for special characters where the format should be “original special character => alternate/replaced by character, i.e. “- => &”',
2,getdate(),2,getdate()
where not exists(select * from ZnodeSearchFeature where FeatureCode = 'CharacterFilter' )

insert into ZnodeSearchQueryTypeFeature (SearchFeatureId,SearchQueryTypeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
select (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'CharacterFilter'),
(select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match'),
2,getdate(),2,GETDATE()
where not exists(select * from ZnodeSearchQueryTypeFeature
where SearchFeatureId = (select top 1 SearchFeatureId from ZnodeSearchFeature where FeatureCode = 'CharacterFilter')
and SearchQueryTypeId = (select top 1 SearchQueryTypeId from ZnodeSearchQueryType where QueryTypeName = 'Multi Match')
)
--ZPD-19834 Dt.15-June-2022
SET IDENTITY_INSERT dbo.ZnodeSearchProfile ON
INSERT INTO dbo.ZnodeSearchProfile (SearchProfileId, ProfileName, SearchQueryTypeId, SearchSubQueryTypeId, 
	Operator, IsDefault, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) 
SELECT 1, N'ZnodeSearchProfile', 5, 7, N'AND', 1, 2, GETDATE(), 2, GETDATE()
WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
SET IDENTITY_INSERT dbo.ZnodeSearchProfile OFF

--ZPD-19834 Dt.14-June-2022
INSERT INTO ZnodeSearchProfileFeatureMapping (SearchProfileId,SearchFeatureId,SearchFeatureValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile'),
(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='AutoCorrect'),
'False',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (	SELECT TOP 1 1 FROM ZnodeSearchProfileFeatureMapping
					WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
						AND SearchFeatureId=(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='AutoCorrect')
				)
	AND EXISTS (SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='AutoCorrect')

INSERT INTO ZnodeSearchProfileFeatureMapping (SearchProfileId,SearchFeatureId,SearchFeatureValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile'),
(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='MinGram'),
'1',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (	SELECT TOP 1 1 FROM ZnodeSearchProfileFeatureMapping
					WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
						AND SearchFeatureId=(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='MinGram')
				)
	AND EXISTS (SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='MinGram')

INSERT INTO ZnodeSearchProfileFeatureMapping (SearchProfileId,SearchFeatureId,SearchFeatureValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile'),
(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='MaxGram'),
'40',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (	SELECT TOP 1 1 FROM ZnodeSearchProfileFeatureMapping
					WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
						AND SearchFeatureId=(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='MaxGram')
				)
	AND EXISTS (SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='MaxGram')

INSERT INTO ZnodeSearchProfileFeatureMapping (SearchProfileId,SearchFeatureId,SearchFeatureValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
SELECT 
(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile'),
(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='CharacterFilter'),
'"_ => ", "- => "',2,GETDATE(),2,GETDATE()
WHERE NOT EXISTS (	SELECT TOP 1 1 FROM ZnodeSearchProfileFeatureMapping
					WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
						AND SearchFeatureId=(SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='CharacterFilter')
				)
	AND EXISTS (SELECT TOP 1 SearchFeatureId FROM ZnodeSearchFeature WHERE FeatureCode='CharacterFilter')

--ZPD-20503 Dt.16-June-2022
DELETE FROM ZnodeSearchProfileAttributeMapping
WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
	AND AttributeCode NOT IN ('ProductName','Brand','SKU')

INSERT INTO ZnodeSearchProfileAttributeMapping 
	(SearchProfileId,AttributeCode,IsFacets,IsUseInSearch,BoostValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNgramEnabled)
SELECT 
(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile'),
'ProductName','false','true',NULL,2,GETDATE(),2,GETDATE(),'true'
WHERE NOT EXISTS (	SELECT TOP 1 1 FROM ZnodeSearchProfileAttributeMapping
					WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
						AND AttributeCode='ProductName'
				)

INSERT INTO ZnodeSearchProfileAttributeMapping 
	(SearchProfileId,AttributeCode,IsFacets,IsUseInSearch,BoostValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNgramEnabled)
SELECT 
(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile'),
'Brand','true','false',NULL,2,GETDATE(),2,GETDATE(),'true'
WHERE NOT EXISTS (	SELECT TOP 1 1 FROM ZnodeSearchProfileAttributeMapping
					WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
						AND AttributeCode='Brand'
				)

INSERT INTO ZnodeSearchProfileAttributeMapping 
	(SearchProfileId,AttributeCode,IsFacets,IsUseInSearch,BoostValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsNgramEnabled)
SELECT 
(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile'),
'SKU','false','true',NULL,2,GETDATE(),2,GETDATE(),'true'
WHERE NOT EXISTS (	SELECT TOP 1 1 FROM ZnodeSearchProfileAttributeMapping
					WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
						AND AttributeCode='SKU'
				)

-- Dt.30-06-2022 -Updated on Dt.21-07-2022
UPDATE ZnodeSearchProfileAttributeMapping
SET IsNgramEnabled='true', ModifiedDate=GETDATE()
WHERE SearchProfileId=(SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile')
	AND AttributeCode IN ('ProductName','Brand','SKU')

DECLARE @SearchProfileId INT= (SELECT TOP 1 SearchProfileId FROM ZnodeSearchProfile WHERE ProfileName='ZnodeSearchProfile');

IF EXISTS (	SELECT * FROM ZnodePublishSearchProfileEntity 
			WHERE SearchProfileId = @SearchProfileId AND SearchProfileAttributeMappingJson NOT LIKE '%IsNgramEnabled%')
BEGIN
	DELETE FROM ZnodePublishSearchProfileEntity
	WHERE SearchProfileId = @SearchProfileId AND SearchProfileAttributeMappingJson NOT LIKE '%IsNgramEnabled%'
END

IF NOT EXISTS (SELECT TOP 1 1 FROM ZnodePublishSearchProfileEntity WHERE SearchProfileId = @SearchProfileId)
BEGIN
	EXEC Znode_PublishSearchProfileEntity @SearchProfileId=@SearchProfileId, @UserId=2;
END
