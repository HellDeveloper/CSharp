USE [UnitTest]
GO

/****** Object:  Table [dbo].[Letter]    Script Date: 03/22/2015 14:24:51 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Letter]') AND type in (N'U'))
DROP TABLE [dbo].[Letter]
GO

USE [UnitTest]
GO

/****** Object:  Table [dbo].[Letter]    Script Date: 03/22/2015 14:24:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Letter](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FromName] [nvarchar](50) NULL,
	[ToName] [nvarchar](50) NULL,
	[Title] [nvarchar](50) NULL,
	[Contents] [nvarchar](50) NULL,
	[SendTime] [datetime] NULL,
	[ReadTime] [datetime] NULL,
	[Category] [nvarchar](50) NULL,
 CONSTRAINT [PK_Letter] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
)

GO


