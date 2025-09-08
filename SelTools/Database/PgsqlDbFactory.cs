// SPDX-License-Identifier: CC0-1.0

using System.Data;

using Npgsql;

namespace SelTools.Database;

public class PgsqlDbFactory : IDbConnectionFactory
{
    private readonly string connectionString;
    private readonly string connectionName;

    public string GetDatabaseType() => nameof(DatabaseType.Pgsql);

    public PgsqlDbFactory(string connectionString, string? connectionName)
    {
        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        this.connectionString = connectionString;
        this.connectionName   = connectionName ?? "Unknown Application";
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

    private NpgsqlConnection CreateAndOpen()
    {
        var connection = new NpgsqlConnection(this.connectionString);
        connection.Open();

        using var command = new NpgsqlCommand("SET application_name TO $1", connection);
        command.Parameters.Add(new NpgsqlParameter { Value = this.connectionName });

        try
        {
            command.ExecuteNonQuery();
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.AdminShutdown)
        {
            connection.Open();
            command.ExecuteNonQuery();
        }

        return connection;
    }

    private async Task<NpgsqlConnection> CreateAndOpenAsync()
    {
        var connection = new NpgsqlConnection(this.connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("SET application_name TO $1", connection);
        command.Parameters.Add(new NpgsqlParameter { Value = this.connectionName });

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.AdminShutdown)
        {
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        return connection;
    }
}
