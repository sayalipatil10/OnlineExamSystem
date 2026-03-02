-- ============================================================
-- Online Exam System - Complete Database Setup Script
-- Run this in SQL Server Management Studio (SSMS)
-- Database: OnlineExam
-- ============================================================

USE master;
GO

SELECT * FROM userInfo;

USE OnlineExam;
GO

INSERT INTO userInfo
(id, name, department, email, semester, gender, password, fatherName, hall)
VALUES
('S001', 'Sayali', 'CSE', 'sayalipatil.smp@gmail.com', '2nd Year 2nd Semester', 'Female', '1234', 'Mr. Patil', 'Hall-A');

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OnlineExam')
BEGIN
    CREATE DATABASE OnlineExam;
END
GO

USE OnlineExam;
GO

-- ============================================================
-- DROP EXISTING TABLES (safe re-run)
-- ============================================================
IF OBJECT_ID('theoryAns',         'U') IS NOT NULL DROP TABLE theoryAns;
IF OBJECT_ID('theoryCourseQueue', 'U') IS NOT NULL DROP TABLE theoryCourseQueue;
IF OBJECT_ID('theoryQueue',       'U') IS NOT NULL DROP TABLE theoryQueue;
IF OBJECT_ID('theoryTaken',       'U') IS NOT NULL DROP TABLE theoryTaken;
IF OBJECT_ID('theoryCourseDetail','U') IS NOT NULL DROP TABLE theoryCourseDetail;
IF OBJECT_ID('theoryQS',          'U') IS NOT NULL DROP TABLE theoryQS;
IF OBJECT_ID('mcqTaken',          'U') IS NOT NULL DROP TABLE mcqTaken;
IF OBJECT_ID('mcqCourseDetail',   'U') IS NOT NULL DROP TABLE mcqCourseDetail;
IF OBJECT_ID('mcqQS',             'U') IS NOT NULL DROP TABLE mcqQS;
IF OBJECT_ID('userInfo',          'U') IS NOT NULL DROP TABLE userInfo;
GO

-- ============================================================
-- TABLE 1: userInfo
-- Stores student registration data, exam stats, and average
-- ============================================================
CREATE TABLE userInfo (
    id          VARCHAR(50)   NOT NULL PRIMARY KEY,
    name        VARCHAR(100)  NOT NULL,
    department  VARCHAR(100),
    email       VARCHAR(150),
    semester    VARCHAR(60),
    gender      VARCHAR(10),
    password    VARCHAR(100)  NOT NULL,
    fatherName  VARCHAR(100),
    hall        VARCHAR(100),
    image       VARCHAR(300),
    no_of_exam  INT           NOT NULL DEFAULT 0,
    total_mark  FLOAT         NOT NULL DEFAULT 0.0,
    abc         FLOAT         NOT NULL DEFAULT 0.0   -- stores running average mark
);
GO

-- ============================================================
-- TABLE 2: mcqQS
-- Stores MCQ questions per course
-- ============================================================
CREATE TABLE mcqQS (
    id      INT           IDENTITY(1,1) PRIMARY KEY,
    course  VARCHAR(20)   NOT NULL,
    qsId    VARCHAR(50),
    qsNo    VARCHAR(10)   NOT NULL,
    qs      NVARCHAR(MAX) NOT NULL,
    op1     NVARCHAR(500) NOT NULL,
    op2     NVARCHAR(500) NOT NULL,
    op3     NVARCHAR(500) NOT NULL,
    op4     NVARCHAR(500) NOT NULL,
    ans     NVARCHAR(500) NOT NULL,
    tag     VARCHAR(100),
    etime   VARCHAR(10)   -- exam duration in minutes
);
GO

-- ============================================================
-- TABLE 3: theoryQS
-- Stores theory questions per course (2 parts: A and B)
-- ============================================================
CREATE TABLE theoryQS (
    id      INT           IDENTITY(1,1) PRIMARY KEY,
    course  VARCHAR(20)   NOT NULL,
    qsId    VARCHAR(50),
    qsNo    VARCHAR(10)   NOT NULL,
    qsA     NVARCHAR(MAX) NOT NULL,
    markA   VARCHAR(10)   NOT NULL,
    qsB     NVARCHAR(MAX) NOT NULL,
    markB   VARCHAR(10)   NOT NULL,
    eTime   VARCHAR(10)   -- exam duration in minutes
);
GO

-- ============================================================
-- TABLE 4: mcqCourseDetail
-- Tracks how many MCQ exams exist per course
-- ============================================================
CREATE TABLE mcqCourseDetail (
    id       INT         IDENTITY(1,1) PRIMARY KEY,
    courseID VARCHAR(20) NOT NULL,
    examNo   VARCHAR(10) NOT NULL
);
GO

-- ============================================================
-- TABLE 5: theoryCourseDetail
-- Tracks how many theory exams exist per course
-- ============================================================
CREATE TABLE theoryCourseDetail (
    id       INT         IDENTITY(1,1) PRIMARY KEY,
    courseID VARCHAR(20) NOT NULL,
    examNo   VARCHAR(10) NOT NULL
);
GO


-- ============================================================
-- TABLE 6: mcqTaken
-- Records MCQ exam submissions by students
-- ============================================================
CREATE TABLE mcqTaken (
    id          INT         IDENTITY(1,1) PRIMARY KEY,
    studentID   VARCHAR(50) NOT NULL,
    courseID    VARCHAR(20) NOT NULL,
    examNo      VARCHAR(10),
    mark        FLOAT       NOT NULL DEFAULT 0
);
GO

-- ============================================================
-- TABLE 7: theoryTaken
-- Records theory exam submissions (pending admin grading)
-- ============================================================
CREATE TABLE theoryTaken (
    id          INT         IDENTITY(1,1) PRIMARY KEY,
    studentID   VARCHAR(50) NOT NULL,
    courseID    VARCHAR(20) NOT NULL,
    examNo      VARCHAR(10)
);
GO

-- ============================================================
-- TABLE 8: theoryAns
-- Stores student theory answer sheets (5 question sets)
-- Admin reviews and assigns marks here
-- ============================================================
CREATE TABLE theoryAns (
    id          INT           IDENTITY(1,1) PRIMARY KEY,
    studentID   VARCHAR(50)   NOT NULL,
    courseID    VARCHAR(20)   NOT NULL,
    qsNo        VARCHAR(10)   NOT NULL,
    qsA         NVARCHAR(MAX),
    ansA        NVARCHAR(MAX),
    markA       VARCHAR(10),
    isAprove    VARCHAR(5)    NOT NULL DEFAULT 'No',  -- 'Yes' or 'No'
    qsB         NVARCHAR(MAX),
    markB       VARCHAR(10),
    ansB        NVARCHAR(MAX),
    mark        FLOAT         NOT NULL DEFAULT 0.0    -- filled by admin when grading
);
GO

-- ============================================================
-- TABLE 9: theoryCourseQueue
-- Tracks which students have submitted theory exams pending grading
-- ============================================================
CREATE TABLE theoryCourseQueue (
    id          INT         IDENTITY(1,1) PRIMARY KEY,
    student_ID  VARCHAR(50) NOT NULL,   -- NOTE: underscore matches code exactly
    courseID    VARCHAR(20) NOT NULL
);
GO

-- ============================================================
-- TABLE 10: theoryQueue
-- Admin queue showing which courses have pending theory submissions
-- ============================================================
CREATE TABLE theoryQueue (
    id          INT          IDENTITY(1,1) PRIMARY KEY,
    courseID    VARCHAR(20)  NOT NULL,
    courseName  VARCHAR(200)
);
GO

-- ============================================================
-- VERIFY: Show all created tables
-- ============================================================
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;
GO

PRINT 'Database setup complete! All 10 tables created successfully.';
GO
