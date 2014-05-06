DROP TABLE [dbo].[Rating]
DROP TABLE [dbo].[AccessRight]
DROP TABLE [dbo].[AccessRightType]
DROP TABLE [dbo].[ClientAdmin]
DROP TABLE [dbo].[EntityInfo]
DROP TABLE [dbo].[EntityInfoType]
DROP TABLE [dbo].[UserInfo]
DROP TABLE [dbo].[UserInfoType]
DROP TABLE [dbo].[Entity]
DROP TABLE [dbo].[EntityType]
DROP TABLE [dbo].[Client]
DROP TABLE [dbo].[UserAcc]


CREATE TABLE Client(
	Id Int IDENTITY Primary Key,
	Name varchar(256) NOT NULL,
	Token varchar(256) NOT NULL
);


CREATE TABLE EntityInfoType(
	Id Int IDENTITY Primary Key,
	Name varchar(256) NOT NULL,
);


CREATE TABLE EntityType(
	Id Int IDENTITY Primary Key,
	Type varchar(256) NOT NULL UNIQUE
);

CREATE TABLE Entity(
	Id Int IDENTITY Primary Key,
	FilePath varchar(256) NOT NULL,
	ClientId Int NOT NULL REFERENCES Client(Id) ON DELETE CASCADE,
	TypeId Int REFERENCES EntityType(Id) ON DELETE SET NULL
);

CREATE TABLE EntityInfo(
	Data varchar(max),
	EntityId Int NOT NULL REFERENCES Entity(Id) ON DELETE CASCADE,
	EntityInfoTypeId Int NOT NULL REFERENCES EntityInfoType(Id) ON DELETE CASCADE,
	Id INT IDENTITY Primary Key
);

CREATE TABLE UserAcc(
	Id INT IDENTITY Primary Key,
	Username varchar(20) NOT NULL UNIQUE,
	Password varchar(50) NOT NULL
);

CREATE TABLE UserInfoType(
	Id INT IDENTITY Primary Key,
	Type varchar(256) NOT NULL UNIQUE
);

CREATE TABLE UserInfo(
	Id Int IDENTITY Primary Key,
	Data varchar(max),
	UserId Int NOT NULL REFERENCES UserAcc(Id) ON DELETE CASCADE,
	UserInfoType Int NOT NULL REFERENCES UserInfoType(Id) ON DELETE CASCADE
);

CREATE TABLE AccessRightType(
	Id Int IDENTITY Primary Key,
	Name varchar(256) NOT NULL UNIQUE
);

CREATE TABLE AccessRight(
	Id Int IDENTITY Primary Key,
	Expiration datetime,
	EntityId Int NOT NULL REFERENCES Entity(Id) ON DELETE CASCADE,
	UserId Int NOT NULL REFERENCES UserAcc(Id) ON DELETE CASCADE,
	AccessRightTypeId INT NOT NULL REFERENCES AccessRightType(Id) ON DELETE CASCADE,
);

CREATE TABLE ClientAdmin(
	Id Int IDENTITY Primary Key,
	ClientId Int NOT NULL REFERENCES Client(Id) ON DELETE CASCADE,
	UserId Int NOT NULL REFERENCES UserAcc(Id) ON DELETE CASCADE	
);

CREATE TABLE Rating(
	Id int IDENTITY Primary Key,
	Value int NOT NULL,
	UserId Int NOT NULL REFERENCES UserAcc(Id) ON DELETE CASCADE,
	EntityId Int NOT NULL REFERENCES Entity(Id) ON DELETE CASCADE,
);