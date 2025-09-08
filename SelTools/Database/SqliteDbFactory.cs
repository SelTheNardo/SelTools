// SPDX-License-Identifier: CC0-1.0

using System.Data;

using Microsoft.Data.Sqlite;

namespace SelTools.Database;

public class SqliteDbFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public string GetDatabaseType() => nameof(DatabaseType.Sqlite);

    public SqliteDbFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        this.connectionString = connectionString;
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
        return connection;
    }

    private async Task<SqliteConnection> CreateAndOpenAsync()
    {
        var connection = new SqliteConnection(this.connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
