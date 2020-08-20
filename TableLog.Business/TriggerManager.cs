using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Business
{
    public class TriggerManager:Manager
    {
        public TriggerManager(ITableManager tableManager)
        {
            this._TableManager = tableManager;
        }

        public string GenerateTriggerOnInsert(string originalConnectionString, string originalDbName, string originalSchema, string originalTableName, string targetDbName, string targetSchema)
        {
            StringBuilder result = new StringBuilder();
            string logTableName = $"{originalTableName}_Log";
            string logTableFullName = $"[{ targetSchema}].[{ logTableName}]";
            string triggerName = $"[{originalSchema}].[{originalTableName}_Log_On_Insert]";
            string originalTableFullName = $"[{ originalSchema}].[{ originalTableName}]";

            Models.Table table = _TableManager.ReadTableSchema(originalConnectionString, originalTableName);

            result.AppendLine($"use [{originalDbName}]");
            result.AppendLine($"go");
            result.AppendLine();
            result.AppendLine($"create trigger {triggerName}");
            result.AppendLine($"\ton {originalTableFullName}");
            result.AppendLine($"\tafter insert");
            result.AppendLine($"as");
            result.AppendLine($"begin");
            result.AppendLine($"\tset nocount on;");
            result.AppendLine();
            result.AppendLine($"\tdeclare @Correlation_ID uniqueidentifier");
            result.AppendLine($"\tset @Correlation_ID = newid()");
            result.AppendLine();
            result.AppendLine($"\tinsert [{targetDbName}].{logTableFullName}");
            result.AppendLine($"\t(");
            result.AppendLine($"\t\t[Is_Old],");
            result.AppendLine($"\t\t[Correlation_ID],");
            result.AppendLine($"\t\t[Action],");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{CalculateColumnName(originalTableName, column)}]{DetermineLastComma(i,table.Columns.Count)}");
            }
            result.AppendLine($"\t)");
            result.AppendLine($"\tselect");
            result.AppendLine($"\t\t0,");
            result.AppendLine($"\t\t@Correlation_ID,");
            result.AppendLine($"\t\t'insert',");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{column.Name}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\tfrom inserted");
            result.AppendLine($"end");
            result.AppendLine();
            result.AppendLine($"go");
            result.AppendLine();

            return result.ToString();
        }

        public string GenerateTriggerOnDelete(string originalConnectionString, string originalDbName, string originalSchema, string originalTableName, string targetDbName, string targetSchema)
        {
            StringBuilder result = new StringBuilder();
            string logTableName = $"{originalTableName}_Log";
            string logTableFullName = $"[{ targetSchema}].[{ logTableName}]";
            string triggerName = $"[{originalSchema}].[{originalTableName}_Log_On_Delete]";
            string originalTableFullName = $"[{ originalSchema}].[{ originalTableName}]";

            Models.Table table = _TableManager.ReadTableSchema(originalConnectionString, originalTableName);

            result.AppendLine($"use [{originalDbName}]");
            result.AppendLine($"go");
            result.AppendLine();
            result.AppendLine($"create trigger {triggerName}");
            result.AppendLine($"\ton {originalTableFullName}");
            result.AppendLine($"\tafter delete");
            result.AppendLine($"as");
            result.AppendLine($"begin");
            result.AppendLine($"\tset nocount on;");
            result.AppendLine();
            result.AppendLine($"\tdeclare @Correlation_ID uniqueidentifier");
            result.AppendLine($"\tset @Correlation_ID = newid()");
            result.AppendLine();
            result.AppendLine($"\tinsert [{targetDbName}].{logTableFullName}");
            result.AppendLine($"\t(");
            result.AppendLine($"\t\t[Is_Old],");
            result.AppendLine($"\t\t[Correlation_ID],");
            result.AppendLine($"\t\t[Action],");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{CalculateColumnName(originalTableName, column)}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\t)");
            result.AppendLine($"\tselect");
            result.AppendLine($"\t\t1,");
            result.AppendLine($"\t\t@Correlation_ID,");
            result.AppendLine($"\t\t'delete',");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{column.Name}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\tfrom deleted");
            result.AppendLine($"end");
            result.AppendLine();
            result.AppendLine($"go");
            result.AppendLine();

            return result.ToString();
        }

        public string GenerateTriggerOnUpdate(string originalConnectionString, string originalDbName, string originalSchema, string originalTableName, string targetDbName, string targetSchema)
        {
            StringBuilder result = new StringBuilder();
            string logTableName = $"{originalTableName}_Log";
            string logTableFullName = $"[{ targetSchema}].[{ logTableName}]";
            string triggerName = $"[{originalSchema}].[{originalTableName}_Log_On_Update]";
            string originalTableFullName = $"[{ originalSchema}].[{ originalTableName}]";

            Models.Table table = _TableManager.ReadTableSchema(originalConnectionString, originalTableName);

            result.AppendLine($"use [{originalDbName}]");
            result.AppendLine($"go");
            result.AppendLine();
            result.AppendLine($"create trigger {triggerName}");
            result.AppendLine($"\ton {originalTableFullName}");
            result.AppendLine($"\tafter update");
            result.AppendLine($"as");
            result.AppendLine($"begin");
            result.AppendLine($"\tset nocount on;");
            result.AppendLine();
            result.AppendLine($"\tdeclare @Correlation_ID uniqueidentifier");
            result.AppendLine($"\tset @Correlation_ID = newid()");
            result.AppendLine();
            result.AppendLine($"\tinsert [{targetDbName}].{logTableFullName}");
            result.AppendLine($"\t(");
            result.AppendLine($"\t\t[Is_Old],");
            result.AppendLine($"\t\t[Correlation_ID],");
            result.AppendLine($"\t\t[Action],");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{CalculateColumnName(originalTableName, column)}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\t)");
            result.AppendLine($"\tselect");
            result.AppendLine($"\t\t1,");
            result.AppendLine($"\t\t@Correlation_ID,");
            result.AppendLine($"\t\t'update',");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{column.Name}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\tfrom deleted");
            result.AppendLine();
            result.AppendLine($"\tinsert [{targetDbName}].{logTableFullName}");
            result.AppendLine($"\t(");
            result.AppendLine($"\t\t[Is_Old],");
            result.AppendLine($"\t\t[Correlation_ID],");
            result.AppendLine($"\t\t[Action],");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{CalculateColumnName(originalTableName, column)}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\t)");
            result.AppendLine($"\tselect");
            result.AppendLine($"\t\t0,");
            result.AppendLine($"\t\t@Correlation_ID,");
            result.AppendLine($"\t\t'update',");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{column.Name}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\tfrom inserted");
            result.AppendLine($"end");
            result.AppendLine();
            result.AppendLine($"go");
            result.AppendLine();

            return result.ToString();
        }
        private string DetermineLastComma(int i, int count)
        {
            if(i != count - 1)
            {
                return ",";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
