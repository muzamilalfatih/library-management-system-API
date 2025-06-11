
use LibraryManagementSystem

INSERT INTO People (NationalNo, FirstName, SecondName, ThirdName, LastName, Gender, Email, Phone, Address)
VALUES ('admin01', 'Admin', 'Admin', NULL, 'Admin', 1, 'admin@example.com', '0000000000', 'HQ');

declare @AdminPersonID int = @@Identity;

INSERT INTO Users (PersonID, UserName, Password, UserRoles, IsActive)
VALUES (@AdminPersonID, 'admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 0, 1);
