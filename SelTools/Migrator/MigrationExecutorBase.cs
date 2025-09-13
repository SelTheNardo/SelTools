// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Migrator;

using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

public abstract partial class MigrationExecutorBase
{
    protected abstract void ExecuteMigration(IDbTransaction transaction, Migration migration);
    protected abstract long GetMigrationNumber(IDbConnection? connection);

    public MigrationResult Run(IDbConnection? connection, string basePath)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var currentVersion = this.GetMigrationNumber(connection);
        var migrations = LoadEligibleMigrations(currentVersion, basePath);

        var currentDatabaseVersion = currentVersion;
        var repeatableMigrationsPerformed = 0;
        var sequentialMigrationsPerformed = 0;

        foreach (var migration in migrations)
        {
            using var transaction = connection.BeginTransaction();
            try
            {
                if (!migration.IsRepeatable)
                {
                    Console.Out.WriteLine($"Running migration {migration.Version} ({migration.Name})");
                }

                this.ExecuteMigration(transaction, migration);

                if (migration.IsRepeatable)
                {
                    repeatableMigrationsPerformed++;
                }
                else
                {
                    currentDatabaseVersion = migration.Version;
                    sequentialMigrationsPerformed++;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine($"Error in migration '{migration.Name}':\n{ex.Message}");
                transaction.Rollback();
                throw;
            }

            transaction.Commit();
        }

        return new MigrationResult(
            PreviousDatabaseVersion: currentVersion,
            CurrentDatabaseVersion: currentDatabaseVersion,
            RepeatableMigrationsPerformed: repeatableMigrationsPerformed,
            SequentialMigrationsPerformed: sequentialMigrationsPerformed
        );
    }

    private static List<Migration> LoadEligibleMigrations(long currentVersion, string basePath)
    {
        var currentPath = Path.Combine(basePath, "repeatable_before");
        List<Migration> allPaths = [];
        if (Path.Exists(currentPath))
        {
            allPaths =
                Directory.EnumerateFiles(currentPath)
                    .Where(fn => fn.EndsWith(".SQL", StringComparison.OrdinalIgnoreCase))
                    .Select(fn => new Migration()
                    {
                        Version = StartsWithNumbersRegex().IsMatch(Path.GetFileName(fn))
                            ? long.Parse(Path.GetFileName(fn).Split('_').FirstOrDefault() ?? "-1", NumberStyles.Integer,
                                CultureInfo.InvariantCulture)
                            : -1,
                        Name = Path.GetFileName(fn),
                        Path = fn,
                        IsRepeatable = true,
                    })
                    .OrderBy(m => m.Version)
                    .ToList();
        }

        currentPath = Path.Combine(basePath, "sequential");
        if (Path.Exists(currentPath))
        {
            allPaths.AddRange(
                Directory.EnumerateFiles(currentPath)
                    .Where(fn => fn.EndsWith(".SQL", StringComparison.OrdinalIgnoreCase))
                    .Select(fn => new Migration()
                    {
                        Version = long.Parse(
                            Path.GetFileName(fn).Split('_').FirstOrDefault() ?? throw new FormatException(
                                "Sequential migration filename does not meet expected naming convention (yyyymmddhhmmss_description-here.sql)."),
                            NumberStyles.Integer, CultureInfo.InvariantCulture),
                        Name = Path.GetFileName(Path.GetFileName(fn)),
                        Path = fn,
                        IsRepeatable = false,
                    })
                    .Where(m => m.Version > currentVersion)
                    .OrderBy(m => m.Version)
                    .ToList()
            );
        }

        currentPath = Path.Combine(basePath, "repeatable_after");
        if (Path.Exists(currentPath))
        {
            allPaths.AddRange(
                Directory.EnumerateFiles(currentPath)
                    .Where(fn => fn.EndsWith(".SQL", StringComparison.OrdinalIgnoreCase))
                    .Select(fn => new Migration()
                    {
                        Version = StartsWithNumbersRegex().IsMatch(Path.GetFileName(fn))
                            ? long.Parse(Path.GetFileName(fn).Split('_').FirstOrDefault() ?? "-1", NumberStyles.Integer,
                                CultureInfo.InvariantCulture)
                            : -1,
                        Name = Path.GetFileName(fn),
                        Path = fn,
                        IsRepeatable = true,
                    })
                    .OrderBy(m => m.Version)
                    .ToList()
            );
        }

        var badMigrations = allPaths
            .Where(m => !m.IsRepeatable && (m.Version < 20000000000000) | (m.Version > 99999999999999)).ToList();
        if (badMigrations.Count != 0)
        {
            var err = badMigrations.Aggregate("Incorrectly named migrations found:\n",
                (current, migration) => current + $"{migration.Name}\n");
            throw new FormatException(err);
        }

        return allPaths;
    }

    [GeneratedRegex(@"^\d+_")]
    private static partial Regex StartsWithNumbersRegex();
}
