# Sel's Tools

All source code in this directory tree is CC0 unless otherwise annotated.

For example, non-CC0:

* some methods may by linked to Stack Overflow answers
* some files may include a different SPDX header

# Usage

Download the repo somewhere outside of your project. Then copy/paste the files you
want to use into your project as-needed. It is not intended to be used as a library
wholesale (even though that is possible with various git trickery.)

Reasons not to pull it into your project:

* No version/stability/anything is given, implied, or otherwise.
* The repo's git history may be rewritten at any time.
* The repo could disappear at any time.

This repo may eventually be re-arranged in such a way to make this more obvious.

# Database Tooling

Sel has been using this [database tooling](./SelTools/Database) for many years (and
the migrator has gone through many iterations) However, be aware this may _no longer_
a good way to handle database(s) in a modern .NET application. If your project uses
Dapper, then this might remain a reasonable approach.

In any case, I've been slowly moving my projects away from this factory pattern for
my database access layer.

A better approach on a modern application might be to use the DbDataSource abstraction
provided by that database's vendor. For example, with Postgres, this is [NpgsqlDataSource](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSource.html).

I strongly encourage you do this instead of using my old Dapper-centric tooling.

The implementation necessary for Microsoft.Data.Sqlite to have a SQLite DbDataSource
is extremely thin. Be aware there are concurrency issues to think through, which
might be a good reason to stick with the DbFactory pattern(s) represented in this
library.

You should, however, wrap them appropriately to fit your usecase.

# Contributions

Issues, PRs, etc are not accepted and will be closed without comment.

# Tests

The test suite is non-comprehensive and the sql migrator test setup is barely
functional and likely test's Sel's ability to write a compose project more than
database migrations.

Use anything in this repo at your own peril.

Testing database:

    podman compose up --build -d
    podman compose logs tests
    podman compose down -v

# AI Disclosure

The author has avoided LLM usage for this project, with the following exception:

* JetBrains [local model for code completion](https://www.jetbrains.com/help/rider/Full_Line_code_completion.html)
* Google/Bing searches unavoidably produce (sometimes useful) snippets when searching programming topics.