// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Database;

using System.Data;
using Microsoft.Data.Sqlite;

public class SqliteDbFactory : IDbConnectionFactory
{
    private readonly string connectionString;
    private readonly bool disableForeignKeys;

    public string GetDatabaseType() => nameof(DatabaseType.Sqlite);

    public SqliteDbFactory(string connectionString, bool disableForeignKeys = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        this.connectionString = connectionString;
        this.disableForeignKeys = disableForeignKeys;
    }

    public void Use(Action<IDbConnection> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        using var connection = this.CreateAndOpen();
        action.Invoke(connection);
    }

    public async Task UseAsync(Func<IDbConnection, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        await using var connection = await this.CreateAndOpenAsync();
        await action.Invoke(connection);
    }

    public T Use<T>(Func<IDbConnection, T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        using var connection = this.CreateAndOpen();
        return action.Invoke(connection);
    }

    public async Task<T> UseAsync<T>(Func<IDbConnection, Task<T>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        await using var connection = await this.CreateAndOpenAsync();
        return await action.Invoke(connection);
    }

    private SqliteConnection CreateAndOpen()
    {
        var connection = new SqliteConnection(this.connectionString);
        connection.Open();
        if (disableForeignKeys)
        {
            return connection;
        }

        using var command = new SqliteCommand("PRAGMA foreign_keys = ON", connection);
        command.ExecuteNonQuery();
        return connection;
    }

    private async Task<SqliteConnection> CreateAndOpenAsync()
    {
        var connection = new SqliteConnection(this.connectionString);
        await connection.OpenAsync();
        if (disableForeignKeys)
        {
            return connection;
        }

        await using var command = new SqliteCommand("PRAGMA foreign_keys = ON", connection);
        await command.ExecuteNonQueryAsync();
        return connection;
    }
}
