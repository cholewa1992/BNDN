INSERT INTO AccessRightType (Name) VALUES  ('Buyer')
INSERT INTO AccessRightType (Name) VALUES  ('Owner')


INSERT INTO UserInfoType (Type) VALUES  ('Email')
INSERT INTO UserInfoType (Type) VALUES  ('Firstname')
INSERT INTO UserInfoType (Type) VALUES  ('Lastname')
INSERT INTO UserInfoType (Type) VALUES  ('Location')


INSERT INTO EntityInfoType (Name) VALUES  ('Title')
INSERT INTO EntityInfoType (Name) VALUES  ('Description')
INSERT INTO EntityInfoType (Name) VALUES  ('Price')
INSERT INTO EntityInfoType (Name) VALUES  ('Picture')
INSERT INTO EntityInfoType (Name) VALUES  ('KeywordTag')
INSERT INTO EntityInfoType (Name) VALUES  ('Genre')
INSERT INTO EntityInfoType (Name) VALUES  ('TrackLength')
INSERT INTO EntityInfoType (Name) VALUES  ('Runtime')
INSERT INTO EntityInfoType (Name) VALUES  ('NumberOfPages')
INSERT INTO EntityInfoType (Name) VALUES  ('Author')
INSERT INTO EntityInfoType (Name) VALUES  ('Director')
INSERT INTO EntityInfoType (Name) VALUES  ('Artist')
INSERT INTO EntityInfoType (Name) VALUES  ('CastMember')
INSERT INTO EntityInfoType (Name) VALUES  ('ReleaseDate')
INSERT INTO EntityInfoType (Name) VALUES  ('Language')
INSERT INTO EntityInfoType (Name) VALUES  ('ExpirationDate')
INSERT INTO EntityInfoType (Name) VALUES  ('AverageRating')
INSERT INTO EntityInfoType (Name) VALUES  ('Thumbnail')


INSERT INTO Client (Name,Token) VALUES  ('ArtShare', '7dac496c534911c0ef47bce1de772502b0d6a6c60b1dbd73c1d3f285f36a0f61')
INSERT INTO Client (Name,Token) VALUES  ('SMU', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855')


INSERT INTO UserAcc (Username, Password) VALUES  ('Jacob', '1234')
INSERT INTO UserAcc (Username, Password) VALUES  ('Loh', '2143')
INSERT INTO UserAcc (Username, Password) VALUES  ('Mathias', '4321')


INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('jacob@cholewa.dk', 1, 1)
INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('Jacob', 1, 2)
INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('Cholewa', 1, 3)
INSERT INTO UserInfo (Data, UserId, UserInfoType) VALUES  ('Denmark', 1, 4)


INSERT INTO ClientAdmin (ClientId, UserId) VALUES  (1, 1)
INSERT INTO ClientAdmin (ClientId, UserId) VALUES  (2, 2)


INSERT INTO EntityType (Type) VALUES  ('Movie')
INSERT INTO EntityType (Type) VALUES  ('Book')
INSERT INTO EntityType (Type) VALUES  ('Music')


INSERT INTO Entity (FilePath, ClientId, TypeId) VALUES  ('C:/lol', 1, 1)
INSERT INTO EntityInfo (Data, EntityId , EntityInfoTypeId) VALUES  ('Lord of the Rings', 1, 1)
INSERT INTO EntityInfo (Data, EntityId , EntityInfoTypeId) VALUES  ('A movie about a ring', 1, 2)
INSERT INTO EntityInfo (Data, EntityId , EntityInfoTypeId) VALUES  ('$1000', 1, 3)