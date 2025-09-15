// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Database;

using System.Data;

public enum DatabaseType
{
    Undefined,
    Sqlite,
    Pgsql,
    Mysql
}

public interface IDbConnectionFactory
{
    void Use(Action<IDbConnection> action);
    Task UseAsync(Func<IDbConnection, CancellationToken, Task> action, CancellationToken cancellationToken);
    T Use<T>(Func<IDbConnection, T> action);
    Task<T> UseAsync<T>(Func<IDbConnection, CancellationToken, Task<T>> action, CancellationToken cancellationToken);

    string GetDatabaseType();
}
