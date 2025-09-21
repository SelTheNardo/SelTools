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

    /// <inheritdoc/>
    public void Use(Action<IDbConnection> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        using var connection = this.CreateAndOpen();
        action.Invoke(connection);
    }

    /// <inheritdoc/>
    public async Task UseAsync(Func<IDbConnection, CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        using var connection = await this.CreateAndOpenAsync();
        await action.Invoke(connection, cancellationToken);
    }

    /// <inheritdoc/>
    public T Use<T>(Func<IDbConnection, T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        using var connection = this.CreateAndOpen();
        return action.Invoke(connection);
    }

    /// <inheritdoc/>
    public async Task<T> UseAsync<T>(Func<IDbConnection, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        using var connection = await this.CreateAndOpenAsync();
        return await action.Invoke(connection, cancellationToken);
    }

    /// <inheritdoc/>
    public IDbConnection CreateAndOpen()
    {
        var connection = new MySqlConnection(this.connectionString);
        connection.Open();
        return connection;
    }

    /// <inheritdoc/>
    public async Task<IDbConnection> CreateAndOpenAsync()
    {
        var connection = new MySqlConnection(this.connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
