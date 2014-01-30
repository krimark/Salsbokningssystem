BEGIN TRAN

USE [SalsbokningsDB]

GO

CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](30) UNIQUE NOT NULL,
	[Email] [nvarchar](50) NULL,		
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Room](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Capacity] [int] NULL,			
 CONSTRAINT [PK_Room] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Availability](
	[ID] [int] IDENTITY(1,1) NOT NULL,	
	[Weekday] [nvarchar](10) NOT NULL,
	[StartTime] [time] NOT NULL,
	[EndTime] [time] NOT NULL,
 CONSTRAINT [PK_Availability] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Booking](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,		
	[UserID] [int] NOT NULL,
	[RoomID] [int] NOT NULL,	
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[RoomAvailability](
	[ID] [int] IDENTITY(1,1) NOT NULL,	
	[RoomID] [int] NOT NULL,
	[AvailabilityID] [int] NOT NULL, 			
 CONSTRAINT [PK_RoomAvailability] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([UserID])
	REFERENCES [dbo].[User] ([ID])

ALTER TABLE [dbo].[Booking]  WITH CHECK ADD FOREIGN KEY([RoomID])
	REFERENCES [dbo].[Room] ([ID])

ALTER TABLE [dbo].[RoomAvailability]  WITH CHECK ADD FOREIGN KEY([RoomID])
	REFERENCES [dbo].[Room] ([ID])

ALTER TABLE [dbo].[RoomAvailability]  WITH CHECK ADD FOREIGN KEY([AvailabilityID])
	REFERENCES [dbo].[Availability] ([ID])

GO

SET NOCOUNT ON

GO

INSERT INTO [dbo].[Room]
			([Name]
			,[Capacity])
VALUES
			('Projektrum 1'
			,6),
			('Projektrum 2'
			,6),
			('Projektrum 3'
			,6),
			('Projektrum 4'
			,6),
			('Projektrum 5'
			,6),
			('Projektrum 6'
			,6)

GO

INSERT INTO [dbo].[Availability]
			([Weekday]
			,[StartTime]
			,[EndTime])
VALUES
			('Måndag'
			,'08:00:00'
			,'09:00:00'),
			('Måndag'
			,'09:00:00'
			,'10:00:00'),
			('Måndag'
			,'10:00:00'
			,'11:00:00'),
			('Måndag'
			,'11:00:00'
			,'12:00:00'),
			('Måndag'
			,'12:00:00'
			,'13:00:00'),
			('Måndag'
			,'13:00:00'
			,'14:00:00'),
			('Måndag'
			,'14:00:00'
			,'15:00:00'),
			('Måndag'
			,'15:00:00'
			,'16:00:00'),

			('Tisdag'
			,'08:00:00'
			,'09:00:00'),
			('Tisdag'
			,'09:00:00'
			,'10:00:00'),
			('Tisdag'
			,'10:00:00'
			,'11:00:00'),
			('Tisdag'
			,'11:00:00'
			,'12:00:00'),
			('Tisdag'
			,'12:00:00'
			,'13:00:00'),
			('Tisdag'
			,'13:00:00'
			,'14:00:00'),
			('Tisdag'
			,'14:00:00'
			,'15:00:00'),
			('Tisdag'
			,'15:00:00'
			,'16:00:00'),

			('Onsdag'
			,'08:00:00'
			,'09:00:00'),
			('Onsdag'
			,'09:00:00'
			,'10:00:00'),
			('Onsdag'
			,'10:00:00'
			,'11:00:00'),
			('Onsdag'
			,'11:00:00'
			,'12:00:00'),
			('Onsdag'
			,'12:00:00'
			,'13:00:00'),
			('Onsdag'
			,'13:00:00'
			,'14:00:00'),
			('Onsdag'
			,'14:00:00'
			,'15:00:00'),
			('Onsdag'
			,'15:00:00'
			,'16:00:00'),

			('Torsdag'
			,'08:00:00'
			,'09:00:00'),
			('Torsdag'
			,'09:00:00'
			,'10:00:00'),
			('Torsdag'
			,'10:00:00'
			,'11:00:00'),
			('Torsdag'
			,'11:00:00'
			,'12:00:00'),
			('Torsdag'
			,'12:00:00'
			,'13:00:00'),
			('Torsdag'
			,'13:00:00'
			,'14:00:00'),
			('Torsdag'
			,'14:00:00'
			,'15:00:00'),
			('Torsdag'
			,'15:00:00'
			,'16:00:00'),

			('Fredag'
			,'08:00:00'
			,'09:00:00'),
			('Fredag'
			,'09:00:00'
			,'10:00:00'),
			('Fredag'
			,'10:00:00'
			,'11:00:00'),
			('Fredag'
			,'11:00:00'
			,'12:00:00'),
			('Fredag'
			,'12:00:00'
			,'13:00:00'),
			('Fredag'
			,'13:00:00'
			,'14:00:00'),
			('Fredag'
			,'14:00:00'
			,'15:00:00'),
			('Fredag'
			,'15:00:00'
			,'16:00:00')

INSERT INTO [dbo].[RoomAvailability]
			([RoomID]
			,[AvailabilityID])
VALUES
			(1
			,1),
			(1
			,2),
			(1
			,3),
			(1
			,4),
			(1
			,5),

			(2
			,1),
			(2
			,2),
			(2
			,3),
			(2
			,4),
			(2
			,5),

			(3
			,1),
			(3
			,2),
			(3
			,3),
			(3
			,4),
			(3
			,5),

			(4
			,1),
			(4
			,2),
			(4
			,3),
			(4
			,4),
			(4
			,5),

			(5
			,1),
			(5
			,2),
			(5
			,3),
			(5
			,4),
			(5
			,5),

			(6
			,1),
			(6
			,2),
			(6
			,3),
			(6
			,4),
			(6
			,5)
			



SET NOCOUNT OFF

COMMIT