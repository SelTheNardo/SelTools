// SPDX-License-Identifier: CC0-1.0

namespace SelTools.Migrator;

public class Migration
{
    private string? sql { get; set; }

    public long Version { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public bool IsRepeatable { get; init; }

    public string Sql
    {
        get => this.sql ??= File.ReadAllText(this.Path);

        set => this.sql = value;
    }
}
