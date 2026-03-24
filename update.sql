BEGIN TRANSACTION;
GO

CREATE TABLE [DeliveryAddresses] (
    [AddressId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ReceiverName] nvarchar(100) NOT NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [AddressLine] nvarchar(500) NOT NULL,
    [IsDefault] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_DeliveryAddresses] PRIMARY KEY ([AddressId]),
    CONSTRAINT [FK_DeliveryAddresses_UserAccounts_UserId] FOREIGN KEY ([UserId]) REFERENCES [UserAccounts] ([UserId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_DeliveryAddresses_UserId] ON [DeliveryAddresses] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324025253_AddDeliveryAddress', N'8.0.23');
GO

COMMIT;
GO

