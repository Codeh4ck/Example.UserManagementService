CREATE TABLE [dbo].[users]
(
	[id] UNIQUEIDENTIFIER NOT NULL,
	[username] NVARCHAR(150) NOT NULL,
	[password] NVARCHAR(32) NOT NULL,
	[email] NVARCHAR(150) NOT NULL,
	[created_at] DATETIME NOT NULL,
	[updated_at] DATETIME NULL
	CONSTRAINT PK__users PRIMARY KEY CLUSTERED ([id] ASC) WITH (FILLFACTOR = 30),
	CONSTRAINT UQ__users_username UNIQUE NONCLUSTERED ([username]),
	CONSTRAINT UQ__users_email UNIQUE NONCLUSTERED ([email])
)

GO
CREATE NONCLUSTERED INDEX IDX__users_id ON [dbo].[users] ([id] ASC)

GO
CREATE NONCLUSTERED INDEX IDX__users_username ON [dbo].[users] ([username] ASC)

GO
CREATE NONCLUSTERED INDEX IDX__users_id_username ON [dbo].[users] ([id] ASC) INCLUDE ([username])

GO
CREATE NONCLUSTERED INDEX IDX__users_id_email ON [dbo].[users] ([id]) INCLUDE ([email])