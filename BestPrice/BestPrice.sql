USE [BestPrice]
GO

select * from BuyCar
select * from ImageNames
select * from TemporaryImageNamesSearch
select * from TemporarySearch
select * from [User]







/****** Object:  Table [dbo].[BuyCar]    Script Date: 8/15/2023 7:17:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BuyCar](
	[CarID] [int] IDENTITY(1,1) NOT NULL,
	[Odometer] [int] NULL,
	[Year] [int] NULL,
	[Price] [int] NULL,
	[BetweenIntSearch] [int] NULL,
	[ImageNumbersSort] [int] NULL,
	[ImageName] [varchar](500) NULL,
	[CarModel] [varchar](100) NULL,
	[CarMaker] [varchar](100) NULL,
	[CarVinNumber] [varchar](50) NULL,
	[CarColor] [varchar](50) NULL,
 CONSTRAINT [PK_Car] PRIMARY KEY CLUSTERED 
(
	[CarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


----------------------------------------------------------------------------------
USE [BestPrice]
GO

/****** Object:  Table [dbo].[ImageNames]    Script Date: 8/15/2023 7:17:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ImageNames](
	[ImageNamesID] [int] IDENTITY(1,1) NOT NULL,
	[CarID] [int] NOT NULL,
	[ImageName] [varchar](500) NOT NULL,
 CONSTRAINT [PK_ImageModel] PRIMARY KEY CLUSTERED 
(
	[ImageNamesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


-------------------------------------------------------------------
USE [BestPrice]
GO

/****** Object:  Table [dbo].[TemporarySearch]    Script Date: 8/15/2023 7:17:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TemporarySearch](
	[CarID] [int] NOT NULL,
	[Odometer] [int] NULL,
	[Year] [int] NULL,
	[Price] [int] NULL,
	[BetweenIntSearch] [int] NULL,
	[ImageNumbersSort] [int] NULL,
	[ImageName] [varchar](500) NULL,
	[CarModel] [varchar](100) NULL,
	[CarMaker] [varchar](100) NULL,
	[CarVinNumber] [varchar](50) NULL,
	[CarColor] [varchar](50) NULL,
 CONSTRAINT [PK_TemporarySearch] PRIMARY KEY CLUSTERED 
(
	[CarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


-------------------------------------------------------------------------
USE [BestPrice]
GO

/****** Object:  Table [dbo].[User]    Script Date: 8/15/2023 7:17:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[AddressLine1] [varchar](50) NOT NULL,
	[AddressLine2] [varchar](50) NULL,
	[City] [varchar](50) NOT NULL,
	[State] [varchar](50) NOT NULL,
	[ZipCode] [int] NOT NULL,
	[Phone] [varchar](50) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[Active] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[UpdatedBy] [int] NULL,
	[DateCreated] [datetime] NULL,
	[DateLastUpdated] [datetime] NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


-----------------------------------------------------------------
USE [BestPrice]
GO

/****** Object:  Table [dbo].[TemporaryImageNamesSearch]    Script Date: 8/15/2023 7:33:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TemporaryImageNamesSearch](
	[ImageNamesID] [int] IDENTITY(1,1) NOT NULL,
	[CarID] [int] NOT NULL,
	[ImageName] [varchar](500) NOT NULL,
 CONSTRAINT [PK_TemporaryImageNamesSearch] PRIMARY KEY CLUSTERED 
(
	[ImageNamesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


