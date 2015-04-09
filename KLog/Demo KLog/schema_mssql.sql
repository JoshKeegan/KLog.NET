-- Schema used in the Demo if the DbLog w/ MS SQL
-- Authors:
--	Josh Keegan 09/04/2015

USE [KLog]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[demo](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[message] [ntext] NOT NULL,
	[logLevel] [varchar](100) NOT NULL,
	[callingMethodFullName] [ntext] NOT NULL,
	[eventDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_demo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO