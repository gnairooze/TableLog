![.NET Core](https://github.com/gnairooze/TableLog/workflows/.NET%20Core/badge.svg)

# TableLog
Auto Generate Log Tables from original table schema read from Sql DB.

# Sample

TableLog.Test.exe --real --source-connection-string"Data Source=192.168.1.110;Initial Catalog=Draft;UID=sqladmin;PWD=87654321;" --source-dbDraft --source-schemadbo --source-tableUsers --target-dbLogging --target-schemadbo
