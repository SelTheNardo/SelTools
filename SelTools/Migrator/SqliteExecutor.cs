// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Migrator;

using System.Data;
using System.Diagnostics.CodeAnalysis;
using SelTools.Database;

public class SqliteExecutor : MigrationExecutorBase
{
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
    public static string GetDatabaseType => nameof(DatabaseType.Sqlite).ToLowerInvariant();

    protected override long GetMigrationNumber(IDbConnection? connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS [migrations] (
                [version]   INTEGER,
                [performed] DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                [name]      TEXT,
                CONSTRAINT migrations_pk PRIMARY KEY ([version], [name])
            )
            """;
        command.ExecuteNonQuery();

        command.CommandText = "SELECT COALESCE(MAX(version), -1) AS [version] FROM [migrations];";
        return (long)command.ExecuteScalar()!;
    }

    protected override void ExecuteMigration(IDbTransaction transaction, Migration migration)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        using var command = transaction.Connection.CreateCommand();
        command.CommandText = migration.Sql;
        command.Transaction = transaction;
        command.ExecuteNonQuery();

        command.CommandText = @"REPLACE INTO [migrations] ([version], [name]) VALUES (@version, @name);";
        var version = command.CreateParameter();
        version.ParameterName = "version";
        version.DbType = DbType.Int64;
        version.Value = migration.Version;

        var name = command.CreateParameter();
        name.ParameterName = "name";
        name.DbType = DbType.String;
        name.Value = migration.Name;

        command.Parameters.Add(version);
        command.Parameters.Add(name);
        command.ExecuteNonQuery();
    }
}
