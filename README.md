![.NET Core](https://github.com/gnairooze/TableLog/workflows/.NET%20Core/badge.svg)

# TableLog
Auto Generate Log Tables from original table schema read from Sql DB.

# Help

## Example

TableLog.Command --mode Real --source-connection-string "Data Source=.; Initial Catalog=Draft; Integrated Security=true;" --source-db Draft  --source-schema dbo --source-table Users --target-db Logging --target-schema dbo

## Arguments

  -m, --mode                        (Default: Help) Set run mode: TestDummy,
                                    TestReal, Real

  -c, --source-connection-string    Set source connection string

  -w, --source-db                   Set source database name

  -e, --source-schema               (Default: dbo) Set source schema

  -r, --source-table                Set source table name

  -s, --target-db                   Set target database name

  -d, --target-schema               (Default: dbo) Set target schema

  --help                            Display this help screen.

  --version                         Display version information.
  