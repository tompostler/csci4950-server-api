
CREATE TABLE [users] (
  [id] int NOT NULL IDENTITY (1, 1) ,
  [fname] nvarchar(50) NOT NULL ,
  [lname] nvarchar(50) NOT NULL ,
  [email] nvarchar(50) NOT NULL ,
  [password] char(128) NOT NULL  -- SHA512, 
 PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [activities] (
  [id] int NOT NULL IDENTITY (1, 1) ,
  [user_id] int NOT NULL ,
  [course_id] varchar(12) NOT NULL ,
  [name] nvarchar(50) NOT NULL ,
  [description] nvarchar(100) ,
  [ddate] datetime ,
  [mdate] datetime NOT NULL , 
 PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [tags] (
  [id] tinyint NOT NULL IDENTITY (1, 1) ,
  [name] nvarchar(50) NOT NULL ,
  [default_color] char(6) NOT NULL , 
 PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [tags_activities] (
  [tag_id] tinyint NOT NULL ,
  [activity_id] int NOT NULL , 
 PRIMARY KEY ([tag_id], [activity_id])
) ON [PRIMARY]
GO

CREATE TABLE [locations] (
  [id] int NOT NULL IDENTITY (1, 1) ,
  [user_id] int NOT NULL ,
  [name] nvarchar(50) ,
  [content] nvarchar(100) NOT NULL , 
 PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [activityunits] (
  [id] bigint NOT NULL IDENTITY (1, 1) ,
  [activity_id] int NOT NULL ,
  [location_id] int NOT NULL ,
  [name] nvarchar(50) NOT NULL ,
  [description] nvarchar(100) ,
  [stime] datetime NOT NULL ,
  [etime] datetime NOT NULL , 
 PRIMARY KEY ([id])
) ON [PRIMARY]
GO

CREATE TABLE [tags_activityunits] (
  [tag_id] tinyint NOT NULL ,
  [activityunit_id] bigint NOT NULL , 
 PRIMARY KEY ([tag_id], [activityunit_id])
) ON [PRIMARY]
GO

CREATE TABLE [auths] (
  [user_id] int NOT NULL ,
  [token] char(64) NOT NULL , -- SHA 256
  [expire] datetime NOT NULL  -- Tokens last 24hrs, 
 PRIMARY KEY ([user_id])
) ON [PRIMARY]
GO

CREATE TABLE [settings] (
  [user_id] int NOT NULL ,
  [value] ntext NOT NULL ,
  [mdate] datetime NOT NULL , 
 PRIMARY KEY ([user_id])
) ON [PRIMARY]
GO

CREATE TABLE [courses] (
  [id] varchar(12) NOT NULL ,
  [name] nvarchar(32) NOT NULL , 
 PRIMARY KEY ([id])
) ON [PRIMARY]
GO

ALTER TABLE [activities] ADD FOREIGN KEY (user_id) REFERENCES [users] ([id]);
				
ALTER TABLE [activities] ADD FOREIGN KEY (course_id) REFERENCES [courses] ([id]);
				
ALTER TABLE [tags_activities] ADD FOREIGN KEY (tag_id) REFERENCES [tags] ([id]);
				
ALTER TABLE [tags_activities] ADD FOREIGN KEY (activity_id) REFERENCES [activities] ([id]);
				
ALTER TABLE [locations] ADD FOREIGN KEY (user_id) REFERENCES [users] ([id]);
				
ALTER TABLE [activityunits] ADD FOREIGN KEY (activity_id) REFERENCES [activities] ([id]);
				
ALTER TABLE [activityunits] ADD FOREIGN KEY (location_id) REFERENCES [locations] ([id]);
				
ALTER TABLE [tags_activityunits] ADD FOREIGN KEY (tag_id) REFERENCES [tags] ([id]);
				
ALTER TABLE [tags_activityunits] ADD FOREIGN KEY (activityunit_id) REFERENCES [activityunits] ([id]);
				
ALTER TABLE [auths] ADD FOREIGN KEY (user_id) REFERENCES [users] ([id]);
				
ALTER TABLE [settings] ADD FOREIGN KEY (user_id) REFERENCES [users] ([id]);
				
ALTER TABLE [auths] ADD CONSTRAINT DF_auths DEFAULT DATEADD(day, 1, GETDATE()) FOR [expire];
