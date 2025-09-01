// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Migrator;

public record MigrationResult(
    int RepeatableMigrationsPerformed,
    int SequentialMigrationsPerformed,
    long PreviousDatabaseVersion,
    long CurrentDatabaseVersion
)
{
    public static MigrationResult FailedMigrationResult => new(-1, -1, -1, -1);
}
