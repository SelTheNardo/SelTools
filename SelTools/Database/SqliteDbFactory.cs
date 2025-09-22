// SPDX-License-Identifier: CC0-1.0

using System.Data;
using Microsoft.Data.Sqlite;

namespace SelTools.Database;

public class SqliteDbFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    /// <inheritdoc/>
    public string GetDatabaseType() => nameof(DatabaseType.Sqlite);

    public SqliteDbFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);
        var builder = new SqliteConnectionStringBuilder(connectionString)
        {
            ForeignKeys = true
        };
        this.connectionString = builder.ToString();
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
        var connection = new SqliteConnection(this.connectionString);
        connection.Open();
        return connection;
    }

    /// <inheritdoc/>
    public async Task<IDbConnection> CreateAndOpenAsync()
    {
        var connection = new SqliteConnection(this.connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
