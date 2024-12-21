
CREATE DATABASE QuestionsAnswersDB;
GO

USE QuestionsAnswersDB;
GO


--SCHEMA AND SP

CREATE TABLE UserQA (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Salt NVARCHAR(255) NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);
GO

CREATE TABLE Question (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    UserQAId UNIQUEIDENTIFIER NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE(),
    IsClosed BIT DEFAULT 0,
    CONSTRAINT FK_Question_UserQA FOREIGN KEY (UserQAId) REFERENCES UserQA(Id)
);
GO

CREATE TABLE Answer (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    QuestionId UNIQUEIDENTIFIER NOT NULL,
    UserQAId UNIQUEIDENTIFIER NOT NULL,
    Response NVARCHAR(MAX) NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Answer_Question FOREIGN KEY (QuestionId) REFERENCES Question(Id),
    CONSTRAINT FK_Answer_UserQA FOREIGN KEY (UserQAId) REFERENCES UserQA(Id)
);
GO



--STORE PROCEDURES FOR UserQA TABLE 

-- 1. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_InsertUserQA')
BEGIN
    DROP PROCEDURE dbo.sp_InsertUserQA;
END
GO

-- Insert a new user into UserQA
CREATE PROCEDURE sp_InsertUserQA
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @Salt NVARCHAR(255)
AS
BEGIN
    -- Check if the username or email already exists
    IF EXISTS (SELECT 1 FROM UserQA WHERE Username = @Username)
    BEGIN
        RAISERROR('Username already exists.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM UserQA WHERE Email = @Email)
    BEGIN
        RAISERROR('Email already registered.', 16, 1);
        RETURN;
    END

    -- Generate a new GUID for the Id and insert the new user into the database
    DECLARE @NewUserId UNIQUEIDENTIFIER = NEWID();

    INSERT INTO UserQA (Id, Username, Email, PasswordHash, Salt, CreationDate, IsActive)
    VALUES (@NewUserId, @Username, @Email, @PasswordHash, @Salt, GETDATE(), 1);
    
    -- Return the Id of the newly created user
    SELECT @NewUserId AS UserId;
END
GO

-- 2. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetUserQAById')
BEGIN
    DROP PROCEDURE dbo.sp_GetUserQAById;
END
GO

-- Get a user by Id
CREATE PROCEDURE sp_GetUserQAById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    -- Retrieve the user by their Id
    SELECT Id, Username, Email, PasswordHash, Salt, CreationDate, IsActive
    FROM UserQA
    WHERE Id = @Id;
END
GO

-- 3. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetUserQAByUsername')
BEGIN
    DROP PROCEDURE dbo.sp_GetUserQAByUsername;
END
GO

-- Get a user by Username
CREATE PROCEDURE sp_GetUserQAByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    -- Retrieve the user by their Username
    SELECT Id, Username, Email, PasswordHash, Salt, CreationDate, IsActive
    FROM UserQA
    WHERE Username = @Username;
END
GO

-- 4. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_UpdateUserQA')
BEGIN
    DROP PROCEDURE dbo.sp_UpdateUserQA;
END
GO

-- Update user details
CREATE PROCEDURE sp_UpdateUserQA
    @Id UNIQUEIDENTIFIER,
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @IsActive BIT
AS
BEGIN
    -- Check if the user exists
    IF NOT EXISTS (SELECT 1 FROM UserQA WHERE Id = @Id)
    BEGIN
        RAISERROR('User not found.', 16, 1);
        RETURN;
    END

    -- Update the user's information
    UPDATE UserQA
    SET Username = @Username,
        Email = @Email,
        IsActive = @IsActive,
        CreationDate = GETDATE()
    WHERE Id = @Id;
END
GO

-- 5. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_DeleteUserQA')
BEGIN
    DROP PROCEDURE dbo.sp_DeleteUserQA;
END
GO

-- Delete a user
CREATE PROCEDURE sp_DeleteUserQA
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    -- Check if the user exists
    IF NOT EXISTS (SELECT 1 FROM UserQA WHERE Id = @Id)
    BEGIN
        RAISERROR('User not found.', 16, 1);
        RETURN;
    END

    -- Delete the user from the database
    DELETE FROM UserQA WHERE Id = @Id;
END
GO

-- 6. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_UpdateUserQAStatus')
BEGIN
    DROP PROCEDURE dbo.sp_UpdateUserQAStatus;
END
GO

-- Activate or Deactivate a user
CREATE PROCEDURE sp_UpdateUserQAStatus
    @Id UNIQUEIDENTIFIER,
    @IsActive BIT
AS
BEGIN
    -- Check if the user exists
    IF NOT EXISTS (SELECT 1 FROM UserQA WHERE Id = @Id)
    BEGIN
        RAISERROR('User not found.', 16, 1);
        RETURN;
    END

    -- Update the user's active status
    UPDATE UserQA
    SET IsActive = @IsActive
    WHERE Id = @Id;
END
GO


--STORE PROCEDURES FOR Question TABLE


-- 1. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_InsertQuestion')
BEGIN
    DROP PROCEDURE dbo.sp_InsertQuestion;
END
GO

-- Insert a new question
CREATE PROCEDURE sp_InsertQuestion
    @UserQAId UNIQUEIDENTIFIER,
    @Title NVARCHAR(200)
AS
BEGIN
    -- Insert the new question into the database
    INSERT INTO Question (UserQAId, Title, CreationDate, IsClosed)
    VALUES (@UserQAId, @Title, GETDATE(), 0);
    
    -- Return the Id of the newly created question
    SELECT SCOPE_IDENTITY() AS QuestionId;
END
GO

-- 2. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetQuestionById')
BEGIN
    DROP PROCEDURE dbo.sp_GetQuestionById;
END
GO

-- Get a question by Id
CREATE PROCEDURE sp_GetQuestionById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    -- Retrieve the question by their Id
    SELECT Id, UserQAId, Title, CreationDate, IsClosed
    FROM Question
    WHERE Id = @Id;
END
GO

-- 3. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_UpdateQuestion')
BEGIN
    DROP PROCEDURE dbo.sp_UpdateQuestion;
END
GO

-- Update the question details
CREATE PROCEDURE sp_UpdateQuestion
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(200),
    @IsClosed BIT
AS
BEGIN
    -- Check if the question exists
    IF NOT EXISTS (SELECT 1 FROM Question WHERE Id = @Id)
    BEGIN
        RAISERROR('Question not found.', 16, 1);
        RETURN;
    END

    -- Update the question
    UPDATE Question
    SET Title = @Title,
        IsClosed = @IsClosed,
        CreationDate = GETDATE()
    WHERE Id = @Id;
END
GO

-- 4. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_DeleteQuestion')
BEGIN
    DROP PROCEDURE dbo.sp_DeleteQuestion;
END
GO

-- Delete a question
CREATE PROCEDURE sp_DeleteQuestion
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    -- Check if the question exists
    IF NOT EXISTS (SELECT 1 FROM Question WHERE Id = @Id)
    BEGIN
        RAISERROR('Question not found.', 16, 1);
        RETURN;
    END

    -- Delete the question
    DELETE FROM Question WHERE Id = @Id;
END
GO

-- 5. Check if the procedure exists, and drop it if it does
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllQuestionsDesc')
BEGIN
    DROP PROCEDURE dbo.sp_GetAllQuestionsDesc;
END
GO

-- Get all questions ordered by CreationDate in descending order
CREATE PROCEDURE sp_GetAllQuestionsDesc
AS
BEGIN
    -- Retrieve all questions ordered by CreationDate in descending order
    SELECT Id, UserQAId, Title, CreationDate, IsClosed
    FROM Question
    ORDER BY CreationDate DESC;
END
GO


--STORE PROCEDURES FOR Answer TABLE



--CREATING DB USER

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'UserQAA')
BEGIN
    CREATE LOGIN UserQAA WITH PASSWORD = 'Wr12azqo+';
END

USE QuestionsAnswersDb;

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'UserQAA')
BEGIN
    CREATE USER UserQAA FOR LOGIN UserQAA;
    
    ALTER ROLE db_datareader ADD MEMBER UserQAA;
    
    ALTER ROLE db_datawriter ADD MEMBER UserQAA;
    
END

--Grant permision to execute SPs to DB User

USE QuestionsAnswersDb;
GO

GRANT EXECUTE ON SCHEMA::dbo TO UserQAA;


