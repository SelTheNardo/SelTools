// SPDX-License-Identifier: CC0-1.0

using System.Data;
using System.Diagnostics.CodeAnalysis;
using SelTools.Database;

namespace SelTools.Migrator;

public class PgsqlExecutor : MigrationExecutorBase
{
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
    public static string GetDatabaseType => nameof(DatabaseType.Pgsql).ToLowerInvariant();

    protected override long GetMigrationNumber(IDbConnection? connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS migrations (
                "version"    bigint    NOT NULL,
                "performed"  timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
                "name"       text      NOT NULL,
                PRIMARY KEY ("version", "name")
            )
            """;
        command.ExecuteNonQuery();

        command.CommandText = """SELECT COALESCE(MAX("version"), -1) AS "version" FROM migrations""";
        return (long)command.ExecuteScalar()!;
    }

    protected override void ExecuteMigration(IDbTransaction transaction, Migration migration)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        using var command = transaction.Connection.CreateCommand();
        command.CommandText = migration.Sql;
        command.Transaction = transaction;
        command.ExecuteNonQuery();

        command.CommandText =
            """
            INSERT INTO migrations ("version", "name") VALUES (@version, @name)
            ON CONFLICT ("version", "name") DO UPDATE SET performed=CURRENT_TIMESTAMP;
            """;

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
