-- Create database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UsersDb')
BEGIN
    CREATE DATABASE UsersDb;
END
GO

USE UsersDb;
GO

-- Create Users table if not exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Login NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(100) NOT NULL,
        Role NVARCHAR(20) NOT NULL,
        IsBlocked BIT NOT NULL DEFAULT 0,
        FailedLoginAttempts INT NOT NULL DEFAULT 0,
        LastLoginDate DATETIME NULL,
        CreationDate DATETIME NOT NULL,
        NeedPasswordChange BIT NOT NULL DEFAULT 1
    );
END
GO

-- Add admin user if not exists (password 'admin')
IF NOT EXISTS (SELECT * FROM Users WHERE Login = 'admin')
BEGIN
    INSERT INTO Users (Login, PasswordHash, Role, IsBlocked, FailedLoginAttempts, LastLoginDate, CreationDate, NeedPasswordChange)
    VALUES ('admin', 'admin', 'admin', 0, 0, NULL, GETDATE(), 0);
END
GO

-- Add test user if not exists (password 'user')
IF NOT EXISTS (SELECT * FROM Users WHERE Login = 'user')
BEGIN
    INSERT INTO Users (Login, PasswordHash, Role, IsBlocked, FailedLoginAttempts, LastLoginDate, CreationDate, NeedPasswordChange)
    VALUES ('user', 'user', 'user', 0, 0, NULL, GETDATE(), 1);
END
GO 