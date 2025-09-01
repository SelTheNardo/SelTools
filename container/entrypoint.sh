#!/usr/bin/env bash

set -euo pipefail

afailure=0
function test:pass {
  printf "\e[1;37m[\e[32mPASS\e[1;37m]\e[0m %s\n" "$1"
}

function test:fail {
  afailure=1
  printf "\e[1;37m[\e[31mFAIL\e[1;37m]\e[0m %s\n" "$1"
}

dotnet restore || exit 1

dotnet build || exit 1

dotnet test || test:fail "unit tests"

cmd="dotnet run --no-build --project /app/SelTools.Tests.Migrations/SelTools.Tests.Migrations.csproj --"

connstring="Server=pgsql_db;Port=5432;Database=integratedtest;User Id=integratedtest;Password=integratedtest;"
$cmd pgsql "/app/SelTools.Tests.Migrations/migrations" "${connstring}" \
  && test:pass "pgsql migration 1" \
  || test:fail "pgsql migration 1"

$cmd pgsql "/app/SelTools.Tests.Migrations/migrations" "${connstring}" \
  && test:pass "pgsql migration 2" \
  || test:fail "pgsql migration 2"

connstring="Server=mysql_db;Database=integratedtest;Uid=integratedtest;Pwd=integratedtest;"
$cmd mysql "/app/SelTools.Tests.Migrations/migrations" "${connstring}" \
  && test:pass "mysql migration 1" \
  || test:fail "mysql migration 1"

$cmd mysql "/app/SelTools.Tests.Migrations/migrations" "${connstring}" \
  && test:pass "mysql migration 2" \
  || test:fail "mysql migration 2"

connstring="Data Source=/app/database.sqlite;Version=3;"
$cmd sqlite "/app/SelTools.Tests.Migrations/migrations" "${connstring}" \
  && test:pass "sqlite migration 1" \
  || test:fail "sqlite migration 1"

$cmd sqlite "/app/SelTools.Tests.Migrations/migrations" "${connstring}" \
  && test:pass "sqlite migration 2" \
  || test:fail "sqlite migration 2"

exit "${afailure}"
