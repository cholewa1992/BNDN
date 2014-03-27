DROP TABLE [dbo].[AccessRight]
GO
DROP TABLE [dbo].[AccessRightType]
GO
DROP TABLE [dbo].[ClientAdmin]
GO
DROP TABLE [dbo].[EntityInfo]
GO
DROP TABLE [dbo].[EntityInfoType]
GO
DROP TABLE [dbo].[UserInfo]
GO
DROP TABLE [dbo].[UserInfoType]
GO
DROP TABLE [dbo].[Entity]
GO
DROP TABLE [dbo].[EntityExtesion]
GO
DROP TABLE [dbo].[EntityType]
GO
DROP TABLE [dbo].[Client]
GO
DROP TABLE [dbo].[UserAcc]
GO


CREATE TABLE Client(
	Id Int IDENTITY Primary Key,
	Name varchar(256) NOT NULL,
	Token varchar(256) NOT NULL
);

GO

CREATE TABLE EntityInfoType(
	Id Int IDENTITY Primary Key,
	Name varchar(256) NOT NULL,
);

GO

CREATE TABLE EntityType(
	Id Int IDENTITY Primary Key,
	Type varchar(256) NOT NULL UNIQUE
);

GO 

CREATE TABLE EntityExtesion(
	Id Int IDENTITY Primary Key,
	Extension varchar(256) NOT NULL UNIQUE
);

GO 

CREATE TABLE Entity(
	Id Int IDENTITY Primary Key,
	FilePath varchar(256) NOT NULL,
	ClientId Int NOT NULL REFERENCES Client(Id) ON DELETE CASCADE,
	TypeId Int REFERENCES EntityType(Id) ON DELETE SET NULL,
	ExtensionId Int REFERENCES EntityExtesion(Id) ON DELETE SET NULL
);

GO

CREATE TABLE EntityInfo(
	Data varchar(max),
	EntityId Int NOT NULL REFERENCES Entity(Id) ON DELETE CASCADE,
	EntityInfoTypeId Int NOT NULL REFERENCES EntityInfoType(Id) ON DELETE CASCADE,
	Id INT IDENTITY Primary Key
);

GO

CREATE TABLE UserAcc(
	Id INT IDENTITY Primary Key,
	Username varchar(20) NOT NULL UNIQUE,
	Password varchar(50) NOT NULL
);

GO

CREATE TABLE UserInfoType(
	Id INT IDENTITY Primary Key,
	Type varchar(256) NOT NULL UNIQUE
);

GO

CREATE TABLE UserInfo(
	Id Int IDENTITY Primary Key,
	Data varchar(max),
	UserId Int NOT NULL REFERENCES UserAcc(Id) ON DELETE CASCADE,
	UserInfoType Int NOT NULL REFERENCES UserInfoType(Id) ON DELETE CASCADE
);

GO

CREATE TABLE AccessRightType(
	Id Int IDENTITY Primary Key,
	Name varchar(256) NOT NULL UNIQUE
);

GO

CREATE TABLE AccessRight(
	Id Int IDENTITY Primary Key,
	Expiration datetime,
	EntityId Int NOT NULL REFERENCES Entity(Id) ON DELETE CASCADE,
	UserId Int NOT NULL REFERENCES UserAcc(Id) ON DELETE CASCADE,
	AccessRightTypeId INT NOT NULL REFERENCES AccessRightType(Id) ON DELETE CASCADE,
);

GO

CREATE TABLE ClientAdmin(
	Id Int IDENTITY Primary Key,
	ClientId Int NOT NULL REFERENCES Client(Id) ON DELETE CASCADE,
	UserId Int NOT NULL REFERENCES UserAcc(Id) ON DELETE CASCADE	
);

GO