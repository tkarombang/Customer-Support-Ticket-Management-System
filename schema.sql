IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [UserId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [PasswordHash] nvarchar(255) NOT NULL,
        [Role] nvarchar(20) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE TABLE [Tickets] (
        [TicketId] uniqueidentifier NOT NULL,
        [TicketNumber] nvarchar(20) NOT NULL,
        [CustomerName] nvarchar(100) NOT NULL,
        [CustomerEmail] nvarchar(150) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Status] nvarchar(20) NOT NULL DEFAULT N'Open',
        [AssignedTo] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_Tickets] PRIMARY KEY ([TicketId]),
        CONSTRAINT [FK_Tickets_Users_AssignedTo] FOREIGN KEY ([AssignedTo]) REFERENCES [Users] ([UserId]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE TABLE [TicketHistories] (
        [HistoryId] uniqueidentifier NOT NULL,
        [TicketId] uniqueidentifier NOT NULL,
        [Action] nvarchar(50) NOT NULL,
        [PreviousStatus] nvarchar(20) NULL,
        [NewStatus] nvarchar(20) NULL,
        [ChangedBy] uniqueidentifier NOT NULL,
        [Timestamp] datetime2 NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_TicketHistories] PRIMARY KEY ([HistoryId]),
        CONSTRAINT [FK_TicketHistories_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Tickets] ([TicketId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TicketHistories_Users_ChangedBy] FOREIGN KEY ([ChangedBy]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TicketHistories_ChangedBy] ON [TicketHistories] ([ChangedBy]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_TicketHistories_TicketId] ON [TicketHistories] ([TicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Tickets_AssignedTo] ON [Tickets] ([AssignedTo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Tickets_CreatedDate_Status_AssignedTo] ON [Tickets] ([CreatedDate], [Status], [AssignedTo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Tickets_TicketNumber] ON [Tickets] ([TicketNumber]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260725142552_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260725142552_InitialCreate', N'10.0.10');
END;

COMMIT;
GO

