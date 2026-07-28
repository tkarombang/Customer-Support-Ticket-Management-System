-- ============================================================
-- Seed Data: Customer Support Ticket Management System
-- Catatan: Data ini SUDAH otomatis ter-include di schema.sql
-- (hasil dari EF Core HasData). File ini disediakan terpisah
-- hanya untuk referensi cepat / dijalankan ulang manual jika perlu.
-- ============================================================

USE TicketManagementDb;
GO

-- Default Manager (password: Manager123!)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'manager@ticket.com')
BEGIN
    INSERT INTO Users (UserId, Name, Email, PasswordHash, Role, CreatedDate)
    VALUES (1, 'Default Manager', 'manager@ticket.com',
            '$2a$11$REPLACE_WITH_ACTUAL_BCRYPT_HASH', 'Manager', '2026-01-01');
END
GO

-- Default Support Agent (password: Agent123!)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'agent1@ticket.com')
BEGIN
    INSERT INTO Users (UserId, Name, Email, PasswordHash, Role, CreatedDate)
    VALUES (2, 'Agent One', 'agent1@ticket.com',
            '$2a$11$REPLACE_WITH_ACTUAL_BCRYPT_HASH', 'SupportAgent', '2026-01-01');
END
GO
