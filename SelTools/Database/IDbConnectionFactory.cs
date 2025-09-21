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
    /// <summary>
    /// Automatically executes OpenAndCreate(), passing the connection to <paramref name="action"/>, handles disposal. Doesn't return a value.
    /// </summary>
    /// <param name="action"></param>
    void Use(Action<IDbConnection> action);

    /// <summary>
    /// Automatically executes OpenAndCreate(), passing the connection to <paramref name="action"/>, handles disposal. Doesn't return a value.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UseAsync(Func<IDbConnection, CancellationToken, Task> action, CancellationToken cancellationToken);

    /// <summary>
    /// Automatically executes OpenAndCreate(), passing the connection to <paramref name="action"/>, handles disposal.
    /// </summary>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Use<T>(Func<IDbConnection, T> action);

    /// <summary>
    /// Automatically executes OpenAndCreate(), passing the connection to <paramref name="action"/>, handles disposal.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> UseAsync<T>(Func<IDbConnection, CancellationToken, Task<T>> action, CancellationToken cancellationToken);

    /// <summary>
    /// Allows the caller to directly obtain a usable IDbConnection. Useful for working with connections "raw".
    /// Be sure to dispose of the IDbConnection object returned.
    /// </summary>
    /// <returns></returns>
    IDbConnection CreateAndOpen();

    /// <summary>
    /// Allows the caller to directly obtain a usable IDbConnection. Useful for working with connections "raw".
    /// Be sure to dispose of the IDbConnection object returned.
    /// </summary>
    /// <returns></returns>
    Task<IDbConnection> CreateAndOpenAsync();

    /// <summary>
    /// Returns the current database type, as a string. Possible values declared in <see cref="DatabaseType"/>
    /// </summary>
    /// <returns></returns>
    string GetDatabaseType();
}
