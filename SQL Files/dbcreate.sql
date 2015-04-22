USE [csci4950s15]
GO

/****** Drop all the constraints ******/
ALTER TABLE [dbo].[tags_activityunits] DROP CONSTRAINT [FK__tags_acti__tag_i__2EA5EC27]
GO
ALTER TABLE [dbo].[tags_activityunits] DROP CONSTRAINT [FK__tags_acti__activ__2DB1C7EE]
GO
ALTER TABLE [dbo].[tags_activities] DROP CONSTRAINT [FK__tags_acti__tag_i__2F9A1060]
GO
ALTER TABLE [dbo].[tags_activities] DROP CONSTRAINT [FK__tags_acti__activ__4589517F]
GO
ALTER TABLE [dbo].[settings] DROP CONSTRAINT [FK__settings__user_i__2CBDA3B5]
GO
ALTER TABLE [dbo].[locations] DROP CONSTRAINT [FK__locations__user___318258D2]
GO
ALTER TABLE [dbo].[auths] DROP CONSTRAINT [FK__auths__user_id__32767D0B]
GO
ALTER TABLE [dbo].[activityunits] DROP CONSTRAINT [FK__activityu__locat__3B0BC30C]
GO
ALTER TABLE [dbo].[activityunits] DROP CONSTRAINT [FK__activityu__activ__467D75B8]
GO
ALTER TABLE [dbo].[activities] DROP CONSTRAINT [FK__activitie__user___44952D46]
GO
ALTER TABLE [dbo].[activities] DROP CONSTRAINT [FK__activitie__cours__4D2A7347]
GO

/****** Drop all the tables ******/
DROP TABLE [dbo].[users]
GO
DROP TABLE [dbo].[tags_activityunits]
GO
DROP TABLE [dbo].[tags_activities]
GO
DROP TABLE [dbo].[tags]
GO
DROP TABLE [dbo].[settings]
GO
DROP TABLE [dbo].[locations]
GO
DROP TABLE [dbo].[courses]
GO
DROP TABLE [dbo].[auths]
GO
DROP TABLE [dbo].[activityunits]
GO
DROP TABLE [dbo].[activities]
GO

/****** Create all the tables, alphabetically ******/
CREATE TABLE [dbo].[activities](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[course_id] [varchar](12) NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](100) NULL,
	[ddate] [datetime2](3) NULL,
	[eduration] [real] NULL,
	[pduration] [real] NULL,
	[importance] [tinyint] NULL,
	[mdate] [datetime2](3) NOT NULL DEFAULT (getutcdate()),
 PRIMARY KEY ([id])
)
GO

CREATE TABLE [dbo].[activityunits](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[activity_id] [int] NOT NULL,
	[location_id] [int] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](100) NULL,
	[stime] [datetime2](3) NOT NULL,
	[etime] [datetime2](3) NOT NULL,
	[mdate] [datetime2](3) NOT NULL DEFAULT (getutcdate()),
 PRIMARY KEY ([id])
)
GO

CREATE TABLE [dbo].[auths](
	[user_id] [int] NOT NULL,
	[token] [char](64) NOT NULL,
	[expire] [datetime2](3) NOT NULL,
 PRIMARY KEY ([user_id])
)
GO

CREATE TABLE [dbo].[courses](
	[id] [varchar](12) NOT NULL,
	[name] [varchar](100) NOT NULL,
 PRIMARY KEY ([id])
)
GO

CREATE TABLE [dbo].[locations](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[name] [nvarchar](50) NULL,
	[content] [nvarchar](100) NOT NULL,
	[mdate] [datetime2](3) NOT NULL DEFAULT (getutcdate()),
 PRIMARY KEY ([id])
)
GO

CREATE TABLE [dbo].[settings](
	[user_id] [int] NOT NULL,
	[value] [ntext] NOT NULL,
	[mdate] [datetime2](3) NOT NULL DEFAULT (getutcdate()),
 PRIMARY KEY ([user_id])
)
GO

CREATE TABLE [dbo].[tags](
	[id] [tinyint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[default_color] [char](6) NOT NULL,
 PRIMARY KEY ([id])
)
GO

CREATE TABLE [dbo].[tags_activities](
	[tag_id] [tinyint] NOT NULL,
	[activity_id] [int] NOT NULL,
 PRIMARY KEY ([tag_id], [activity_id])
)
GO

CREATE TABLE [dbo].[tags_activityunits](
	[tag_id] [tinyint] NOT NULL,
	[activityunit_id] [bigint] NOT NULL,
 PRIMARY KEY ([tag_id], [activityunit_id])
)
GO

CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[fname] [nvarchar](50) NOT NULL,
	[lname] [nvarchar](50) NOT NULL,
	[email] [nvarchar](50) NOT NULL,
	[password] [char](60) NOT NULL,
	[mdate] [datetime2](3) NOT NULL DEFAULT (getutcdate()),
 PRIMARY KEY ([id]),
 CONSTRAINT [AK_email] UNIQUE ([email])
)
GO

/****** Add all the constraints ******/
ALTER TABLE [dbo].[activities] ADD FOREIGN KEY([course_id]) REFERENCES [dbo].[courses] ([id])
GO

ALTER TABLE [dbo].[activities] ADD FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[activityunits] ADD FOREIGN KEY([activity_id]) REFERENCES [dbo].[activities] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[activityunits] ADD FOREIGN KEY([location_id]) REFERENCES [dbo].[locations] ([id])
GO

ALTER TABLE [dbo].[auths] ADD FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[locations] ADD FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[settings] ADD FOREIGN KEY([user_id]) REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tags_activities] ADD FOREIGN KEY([activity_id]) REFERENCES [dbo].[activities] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tags_activities] ADD FOREIGN KEY([tag_id]) REFERENCES [dbo].[tags] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tags_activityunits] ADD FOREIGN KEY([activityunit_id]) REFERENCES [dbo].[activityunits] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tags_activityunits] ADD FOREIGN KEY([tag_id]) REFERENCES [dbo].[tags] ([id])
ON DELETE CASCADE
GO
