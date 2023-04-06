
--dt ZPD-9892 --> ZPD-9979
GO
disable trigger Trg_ZnodeLocale_GlobalSetting on znodelocale
go
update znodelocale set Code = 'ja-JP',ModifiedDate=getdate()
where Name = 'Japanese' and code = 'ja'

update znodelocale set Code = 'ja-JP',ModifiedDate=getdate()
where Name = 'Japanese' and code = 'ja-jp'
go
enable trigger Trg_ZnodeLocale_GlobalSetting on znodelocale