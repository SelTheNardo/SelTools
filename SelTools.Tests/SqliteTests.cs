// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Tests;

using Microsoft.Data.Sqlite;
using SelTools.Database;
using Xunit;

public class SqliteTests
{
    private readonly SqliteDbFactory sqliteFactory = new SqliteDbFactory("Data Source=:memory:");

    [Fact]
    public void SqliteFkShouldFail()
    {
        sqliteFactory.Use(conn =>
        {
            using var sqliteCommand = conn.CreateCommand();
            sqliteCommand.CommandText = """
                                        CREATE TABLE a (
                                            id INTEGER PRIMARY KEY
                                        );
                                        """;
            Assert.Equal(0, sqliteCommand.ExecuteNonQuery());

            sqliteCommand.CommandText = """
                                        CREATE TABLE b (
                                           id INTEGER PRIMARY KEY,
                                           col1 INTEGER,
                                           FOREIGN KEY (col1) REFERENCES a (id)
                                        );
                                        """;
            Assert.Equal(0, sqliteCommand.ExecuteNonQuery());

            sqliteCommand.CommandText = "INSERT INTO a (id) VALUES (1)";
            Assert.Equal(1, sqliteCommand.ExecuteNonQuery());

            sqliteCommand.CommandText = "INSERT INTO b (id, col1) VALUES (1, 1)";
            Assert.Equal(1, sqliteCommand.ExecuteNonQuery());

            sqliteCommand.CommandText = "INSERT INTO b (id, col1) VALUES (2, 2)";
            var exception = Assert.Throws<SqliteException>(() => sqliteCommand.ExecuteNonQuery());
            Assert.Equal(19, exception.SqliteErrorCode);
        });
    }
}
