// SPDX-License-Identifier: CC0-1.0

using Dapper;

using System.Data;
using System.Diagnostics.CodeAnalysis;
using SelTools.Database;

namespace SelTools.Migrator;

public sealed class MysqlExecutor : MigrationExecutorBase
{
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
    public static string GetDatabaseType => nameof(DatabaseType.Mysql).ToLowerInvariant();

    protected override long GetMigrationNumber(IDbConnection? connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var sql = """
                  CREATE TABLE IF NOT EXISTS `migrations` (
                      `version`   bigint       NOT NULL,
                      `performed` timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP,
                      `name`      varchar(255) NOT NULL,
                      PRIMARY KEY (`version`, `name`)
                  )
                  """;
        connection.Execute(sql);

        sql = "SELECT COALESCE(MAX(`version`), -1) AS `version` FROM `migrations`";
        return connection.QuerySingle<long>(sql);
    }

    protected override void ExecuteMigration(IDbTransaction transaction, Migration migration)
    {
        ArgumentNullException.ThrowIfNull(transaction.Connection);

        transaction.Connection.Execute(migration.Sql, transaction);

        var sql = """
                  INSERT INTO `migrations` (`version`, `name`) VALUES (@version, @name)
                  ON DUPLICATE KEY UPDATE `performed` = CURRENT_TIMESTAMP;
                  """;

        transaction.Connection.Execute(sql, new { version = migration.Version, name = migration.Name }, transaction);
    }
}
