// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Database;

using System.Data;
using MySqlConnector;

public class MysqlDbFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public string GetDatabaseType() => nameof(DatabaseType.Mysql);

    public MysqlDbFactory(string connectionString)
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

    public async Task UseAsync(Func<IDbConnection, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        await using var connection = await this.CreateAndOpenAsync();
        await action.Invoke(connection, cancellationToken);
    }

    public T Use<T>(Func<IDbConnection, T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        using var connection = this.CreateAndOpen();
        return action.Invoke(connection);
    }

    public async Task<T> UseAsync<T>(Func<IDbConnection, CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        await using var connection = await this.CreateAndOpenAsync();
        return await action.Invoke(connection, cancellationToken);
    }

    private MySqlConnection CreateAndOpen()
    {
        var connection = new MySqlConnection(this.connectionString);
        connection.Open();
        return connection;
    }

    private async Task<MySqlConnection> CreateAndOpenAsync()
    {
        var connection = new MySqlConnection(this.connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
