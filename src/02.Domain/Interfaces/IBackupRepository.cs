namespace TicketManagement.Domain.Interfaces;

public interface IBackupRepository
{
    /// <summary>
    /// Menjalankan T-SQL BACKUP DATABASE. Return path file .bak yang dihasilkan.
    /// </summary>
    Task<string> BackupDatabaseAsync(string backupFileName);

    /// <summary>
    /// Menjalankan T-SQL RESTORE DATABASE dari file .bak yang di-upload.
    /// </summary>
    Task RestoreDatabaseAsync(string backupFilePath);
}