using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TicketManagement.Domain.Interfaces;

namespace TicketManagement.Infrastructure.Repositories;

public class BackupRepository(IConfiguration configuration) : IBackupRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<string> BackupDatabaseAsync(string backupFileName)
    {
        var databaseName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;
        var backupFilePath = Path.Combine(GetBackupFolder(), backupFileName);

        var sql = $"BACKUP DATABASE [{databaseName}] TO DISK = @FilePath WITH FORMAT, INIT";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FilePath", backupFilePath);
        command.CommandTimeout = 300; // backup bisa lama untuk database besar

        await command.ExecuteNonQueryAsync();

        return backupFilePath;
    }

    public async Task RestoreDatabaseAsync(string backupFilePath)
    {
        var databaseName = new SqlConnectionStringBuilder(_connectionString).InitialCatalog;

        // Restore butuh koneksi ke database 'master', bukan database target
        // (karena database target harus di-set OFFLINE/SINGLE_USER dulu)
        var masterConnectionString = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "master"
        }.ConnectionString;

        var sql = $@"
            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{databaseName}] FROM DISK = @FilePath WITH REPLACE;
            ALTER DATABASE [{databaseName}] SET MULTI_USER;";

        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync();

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@FilePath", backupFilePath);
        command.CommandTimeout = 300;

        await command.ExecuteNonQueryAsync();
    }

    private static string GetBackupFolder()
    {
        // Sesuai structure.md: App_Data/uploads, di luar wwwroot (tidak public)
        var folder = Path.Combine(AppContext.BaseDirectory, "App_Data", "backups");
        Directory.CreateDirectory(folder);
        return folder;
    }
}