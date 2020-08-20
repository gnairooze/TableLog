using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Business
{
    public class LogTableManager:Manager
    {
        public LogTableManager(ITableManager tableManager)
        {
            this._TableManager = tableManager;
        }

        public string GenerateLogTableSchema(string originalConnectionString, string originalTableName, string targetDbName, string targetSchema)
        {
            StringBuilder result = new StringBuilder();
            string logTableName = $"{originalTableName}_Log";
            string logTableFullName = $"[{ targetSchema}].[{ logTableName}]";

            Models.Table table = _TableManager.ReadTableSchema(originalConnectionString, originalTableName);

            result.AppendLine($"use [{targetDbName}]");
            result.AppendLine($"go");
            result.AppendLine();
            result.AppendLine($"create table {logTableFullName}");
            result.AppendLine($"(");
            result.AppendLine($"\t[ID] [bigint] identity(1,1) not null,");
            result.AppendLine($"\t[Is_Old] [bit] not null,");
            result.AppendLine($"\t[Action] [varchar](8) not null,");
            result.AppendLine($"\t[Create_Date] [datetime] not null,");
            result.AppendLine($"\t[Correlation_ID] [uniqueidentifier] not null,");

            foreach (var column in table.Columns)
            {
                result.AppendLine($"\t[{CalculateColumnName(originalTableName, column)}] [{column.DataType}]{CalcualteFieldLength(column)} {CalculateNullableField(column)},");
            }
            result.AppendLine($"\tCONSTRAINT [PK_{logTableName}] PRIMARY KEY CLUSTERED");
            result.AppendLine($"\t(");
            result.AppendLine($"\t\t[ID] asc");
            result.AppendLine($"\t)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]");
            result.AppendLine($") ON [PRIMARY]");
            result.AppendLine($"go");
            result.AppendLine();
            result.AppendLine($"alter table {logTableFullName} ADD  CONSTRAINT [DF_{logTableName}_Create_Date]  DEFAULT (getdate()) FOR [Create_Date]");
            result.AppendLine($"go");
            result.AppendLine();

            return result.ToString();
        }

        protected string CalcualteFieldLength(Models.Column column)
        {
            if (column.DataType.ToLower().Contains("char"))
            {
                return $"({column.MaxLength})";
            }
            else
            {
                return string.Empty;
            }
        }
        protected string CalculateNullableField(Models.Column column)
        {
            if (column.IsRequired)
            {
                return "not null";
            }
            else
            {
                return "null";
            }
        }
    }
}
