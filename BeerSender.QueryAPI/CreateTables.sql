CREATE TABLE [dbo].[OpenBoxes](
	[BoxId] [uniqueidentifier] NOT NULL,
	[Capacity] [int] NOT NULL,
	[NumberOfBottles] [int] NOT NULL DEFAULT (0),
    CONSTRAINT [PK_OpenBoxes] PRIMARY KEY CLUSTERED
    (
        [BoxId] ASC
    )
) 

CREATE TABLE [dbo].[UnsentBoxes](
	[BoxId] [uniqueidentifier] NOT NULL,
	[Status] [varchar](64) NOT NULL,
    CONSTRAINT [PK_UnsentBoxes] PRIMARY KEY CLUSTERED
    (
        [BoxId] ASC
    )
) 

CREATE TABLE [dbo].[ProjectionCheckpoints](
	[ProjectionName] [varchar](256) NOT NULL,
	[EventVersion] [binary](8) NOT NULL DEFAULT 0x0000000000000000,
    CONSTRAINT [PK_ProjectionCheckpoints] PRIMARY KEY CLUSTERED 
    (
        [ProjectionName] ASC
    )
) 