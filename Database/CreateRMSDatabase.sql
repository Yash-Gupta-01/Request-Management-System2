-- SQL script to create Request Management System database tables

CREATE TABLE User_Details (
    Username NVARCHAR(100) PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'Guest'
);

CREATE TABLE Request_Details (
    RequestId INT IDENTITY(1,1) PRIMARY KEY,
    RequestType NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    DateOfIssue DATETIME NOT NULL,
    CurrentStatus NVARCHAR(100) NOT NULL DEFAULT 'Yet to Start',
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Request_User FOREIGN KEY (Username) REFERENCES User_Details(Username)
);

CREATE TABLE Request_Status_History (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RequestId INT NOT NULL,
    UpdatedBy NVARCHAR(100) NOT NULL,
    NewStatus NVARCHAR(100) NOT NULL,
    Remarks NVARCHAR(MAX) NOT NULL,
    Timestamp DATETIME NOT NULL,
    CONSTRAINT FK_Status_Request FOREIGN KEY (RequestId) REFERENCES Request_Details(RequestId)
);
