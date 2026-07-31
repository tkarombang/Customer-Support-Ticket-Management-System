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

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TicketHistories]') AND [c].[name] = N'Timestamp');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [TicketHistories] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [TicketHistories] DROP COLUMN [Timestamp];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Users] ADD [Address] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Users] ADD [AvatarUrl] nvarchar(255) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Users] ADD [JobTitle] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Users] ADD [PhoneNumber] nvarchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Users] ADD [Status] nvarchar(10) NOT NULL DEFAULT N'Active';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Users] ADD [Username] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Tickets] ADD [ApplicationSystem] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Tickets] ADD [Category] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Tickets] ADD [DueDate] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Tickets] ADD [Impact] nvarchar(30) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Tickets] ADD [Priority] nvarchar(10) NOT NULL DEFAULT N'Medium';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    ALTER TABLE [Tickets] ADD [Type] nvarchar(20) NOT NULL DEFAULT N'Incident';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TicketHistories]') AND [c].[name] = N'Action');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [TicketHistories] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [TicketHistories] ALTER COLUMN [Action] nvarchar(30) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [AppSettings] (
        [SettingKey] nvarchar(100) NOT NULL,
        [SettingValue] nvarchar(max) NULL,
        [IsEncrypted] bit NOT NULL,
        [UpdatedDate] datetime2 NOT NULL,
        [UpdatedBy] uniqueidentifier NULL,
        CONSTRAINT [PK_AppSettings] PRIMARY KEY ([SettingKey]),
        CONSTRAINT [FK_AppSettings_Users_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users] ([UserId]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [BackupHistories] (
        [BackupId] uniqueidentifier NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [FilePath] nvarchar(500) NOT NULL,
        [FileSizeBytes] bigint NULL,
        [Type] nvarchar(20) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [TriggeredBy] uniqueidentifier NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_BackupHistories] PRIMARY KEY ([BackupId]),
        CONSTRAINT [FK_BackupHistories_Users_TriggeredBy] FOREIGN KEY ([TriggeredBy]) REFERENCES [Users] ([UserId]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [IntegrationConfigs] (
        [IntegrationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [WebhookUrl] nvarchar(500) NULL,
        [ApiKeyEncrypted] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_IntegrationConfigs] PRIMARY KEY ([IntegrationId])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [SystemLogs] (
        [LogId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NULL,
        [Action] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [IpAddress] nvarchar(45) NULL,
        [Timestamp] datetime2 NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_SystemLogs] PRIMARY KEY ([LogId]),
        CONSTRAINT [FK_SystemLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE SET NULL
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [TicketAttachments] (
        [AttachmentId] uniqueidentifier NOT NULL,
        [TicketId] uniqueidentifier NOT NULL,
        [FileName] nvarchar(255) NOT NULL,
        [FilePath] nvarchar(500) NOT NULL,
        [FileSizeBytes] bigint NOT NULL,
        [ContentType] nvarchar(100) NOT NULL,
        [UploadedBy] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_TicketAttachments] PRIMARY KEY ([AttachmentId]),
        CONSTRAINT [FK_TicketAttachments_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Tickets] ([TicketId]) ON DELETE CASCADE,
        CONSTRAINT [FK_TicketAttachments_Users_UploadedBy] FOREIGN KEY ([UploadedBy]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [TicketCc] (
        [TicketId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TicketCc] PRIMARY KEY ([TicketId], [UserId]),
        CONSTRAINT [FK_TicketCc_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Tickets] ([TicketId]) ON DELETE CASCADE,
        CONSTRAINT [FK_TicketCc_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [TicketComments] (
        [CommentId] uniqueidentifier NOT NULL,
        [TicketId] uniqueidentifier NOT NULL,
        [Content] nvarchar(1000) NOT NULL,
        [CreatedBy] uniqueidentifier NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [UpdatedDate] datetime2 NULL,
        CONSTRAINT [PK_TicketComments] PRIMARY KEY ([CommentId]),
        CONSTRAINT [FK_TicketComments_Tickets_TicketId] FOREIGN KEY ([TicketId]) REFERENCES [Tickets] ([TicketId]) ON DELETE CASCADE,
        CONSTRAINT [FK_TicketComments_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE TABLE [TicketSequences] (
        [Id] int NOT NULL IDENTITY,
        [LastSequence] int NOT NULL,
        CONSTRAINT [PK_TicketSequences] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'LastSequence') AND [object_id] = OBJECT_ID(N'[TicketSequences]'))
        SET IDENTITY_INSERT [TicketSequences] ON;
    EXEC(N'INSERT INTO [TicketSequences] ([Id], [LastSequence])
    VALUES (1, 0)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'LastSequence') AND [object_id] = OBJECT_ID(N'[TicketSequences]'))
        SET IDENTITY_INSERT [TicketSequences] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN

                    UPDATE Users
                    SET Username = LEFT(Email, CHARINDEX('@', Email) - 1)
                    WHERE Username = '' OR Username IS NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_AppSettings_UpdatedBy] ON [AppSettings] ([UpdatedBy]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_BackupHistories_TriggeredBy] ON [BackupHistories] ([TriggeredBy]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_SystemLogs_Timestamp] ON [SystemLogs] ([Timestamp]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_SystemLogs_UserId] ON [SystemLogs] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_TicketAttachments_TicketId] ON [TicketAttachments] ([TicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_TicketAttachments_UploadedBy] ON [TicketAttachments] ([UploadedBy]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_TicketCc_UserId] ON [TicketCc] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_TicketComments_CreatedBy] ON [TicketComments] ([CreatedBy]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    CREATE INDEX [IX_TicketComments_TicketId] ON [TicketComments] ([TicketId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260729074340_AddExtendedModulesV2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260729074340_AddExtendedModulesV2', N'10.0.10');
END;

COMMIT;
GO

