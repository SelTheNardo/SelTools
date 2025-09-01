// SPDX-License-Identifier: CC0-1.0

using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;
using SelTools.Database;
using SelTools.Migrator;

namespace SelTools.Tests.Migrations;

public static class Program
{
    private static readonly JsonSerializerOptions writeIndentedJsonSerializerOptions = new() { WriteIndented = true };

    public static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("SelTools Integrated Tests");
        var dbTypeArg = new Argument<string>("Database Type");
        var connStringArg = new Argument<string>("Connection String");
        var migrationBase = new Argument<string>("Migrations Base Path");

        dbTypeArg.Validators.Add(result =>
        {
            switch (result.GetValue(dbTypeArg)?.ToUpperInvariant())
            {
                case "PGSQL":
                case "MYSQL":
                case "SQLITE":
                    break;
                default:
                    result.AddError("Unsupported database type.");
                    break;
            }
        });

        migrationBase.Validators.Add(result =>
        {
            if (!Directory.Exists(result.GetValue(migrationBase)))
            {
                result.AddError("Migration base path doesn't exist.");
            }
        });

        rootCommand.Add(dbTypeArg);
        rootCommand.Add(migrationBase);
        rootCommand.Add(connStringArg);

        rootCommand.SetAction(async (parseResult, token) =>
        {
            return parseResult.GetRequiredValue(dbTypeArg).ToUpperInvariant() switch
            {
                "PGSQL" => await DoPgsql(parseResult.GetRequiredValue(migrationBase), parseResult.GetRequiredValue(connStringArg), token),
                "MYSQL" => await DoMysql(parseResult.GetRequiredValue(migrationBase), parseResult.GetRequiredValue(connStringArg), token),
                "SQLITE" => await DoSqlite(parseResult.GetRequiredValue(migrationBase), parseResult.GetRequiredValue(connStringArg), token),
                _ => throw new UnreachableException()
            };
        });

        var result = rootCommand.Parse(args);
        return await result.InvokeAsync();
    }

    private static async Task<int> DoPgsql(string migrationBase, string connString, CancellationToken token)
    {
        var databaseFactory = new PgsqlDbFactory(connString, "integrated tests");
        var result = databaseFactory.Use(connection =>
        {
            var migrator = new PgsqlExecutor();
            return migrator.Run(connection, Path.Combine(migrationBase, databaseFactory.GetDatabaseType()));
        });

        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(result, writeIndentedJsonSerializerOptions));
        return result.CurrentDatabaseVersion == 20221029000000 ? 0 : 1;
    }

    private static async Task<int> DoMysql(string migrationBase, string connString, CancellationToken token)
    {
        var databaseFactory = new MysqlDbFactory(connString);
        var result = databaseFactory.Use(connection =>
        {
            var migrator = new MysqlExecutor();
            return migrator.Run(connection, Path.Combine(migrationBase, databaseFactory.GetDatabaseType()));
        });

        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(result, writeIndentedJsonSerializerOptions));
        return result.CurrentDatabaseVersion == 20221029000000 ? 0 : 1;
    }

    private static async Task<int> DoSqlite(string migrationBase, string connString, CancellationToken token)
    {
        var databaseFactory = new SqliteDbFactory(connString);
        var result = databaseFactory.Use(connection =>
        {
            var migrator = new SqliteExecutor();
            return migrator.Run(connection, Path.Combine(migrationBase, databaseFactory.GetDatabaseType()));
        });

        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(result, writeIndentedJsonSerializerOptions));
        return result.CurrentDatabaseVersion == 20221029000000 ? 0 : 1;
    }
}
