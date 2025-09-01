// SPDX-License-Identifier: CC0-1.0

using System.Data;

namespace SelTools.Database;

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
    Task UseAsync(Func<IDbConnection, Task> action);
    T Use<T>(Func<IDbConnection, T> action);
    Task<T> UseAsync<T>(Func<IDbConnection, Task<T>> action);

    string GetDatabaseType();
}
