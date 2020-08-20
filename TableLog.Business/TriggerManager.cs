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

        private string generateTriggerStart(string originalDbName, string originalTableFullName, string triggerName, string triggerAction)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"use [{originalDbName}]");
            result.AppendLine($"go");
            result.AppendLine();
            result.AppendLine($"create trigger {triggerName}");
            result.AppendLine($"\ton {originalTableFullName}");
            result.AppendLine($"\tafter {triggerAction}");
            result.AppendLine($"as");
            result.AppendLine($"begin");
            result.AppendLine($"\tset nocount on;");
            result.AppendLine();
            result.AppendLine($"\tdeclare @Correlation_ID uniqueidentifier");
            result.AppendLine($"\tset @Correlation_ID = newid()");
            result.AppendLine();

            return result.ToString();
        }
        private string generateCommonInsert(Models.Table table, string targetDbName, string logTableFullName, int isOld, string triggerAction, string triggerTable)
        {
            StringBuilder result = new StringBuilder();
            string originalTableName = table.Name;


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
            result.AppendLine($"\t\t{isOld},");
            result.AppendLine($"\t\t@Correlation_ID,");
            result.AppendLine($"\t\t'{triggerAction}',");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Models.Column column = table.Columns[i];

                result.AppendLine($"\t\t[{column.Name}]{DetermineLastComma(i, table.Columns.Count)}");
            }
            result.AppendLine($"\tfrom {triggerTable}");

            return result.ToString();
        }
        private string generateTriggerEnd()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"end");
            result.AppendLine();
            result.AppendLine($"go");
            result.AppendLine();

            return result.ToString();
        }
        public string GenerateTriggerOnInsert(string originalConnectionString, string originalDbName, string originalSchema, string originalTableName, string targetDbName, string targetSchema)
        {
            StringBuilder result = new StringBuilder();
            string logTableName = $"{originalTableName}_Log";
            string logTableFullName = $"[{ targetSchema}].[{ logTableName}]";
            string triggerName = $"[{originalSchema}].[{originalTableName}_Log_On_Insert]";
            string originalTableFullName = $"[{ originalSchema}].[{ originalTableName}]";
            string triggerAction = "insert";
            string triggerTable = "inserted";
            int isOld = 0;

            Models.Table table = _TableManager.ReadTableSchema(originalConnectionString, originalTableName);

            result.AppendLine(generateTriggerStart(originalDbName, originalTableFullName, triggerName, triggerAction));
            result.AppendLine(generateCommonInsert(table, targetDbName, logTableFullName, isOld, triggerAction, triggerTable));
            result.AppendLine(generateTriggerEnd());
            

            return result.ToString();
        }

        public string GenerateTriggerOnDelete(string originalConnectionString, string originalDbName, string originalSchema, string originalTableName, string targetDbName, string targetSchema)
        {
            StringBuilder result = new StringBuilder();
            string logTableName = $"{originalTableName}_Log";
            string logTableFullName = $"[{ targetSchema}].[{ logTableName}]";
            string triggerName = $"[{originalSchema}].[{originalTableName}_Log_On_Delete]";
            string originalTableFullName = $"[{ originalSchema}].[{ originalTableName}]";
            string triggerAction = "delete";
            string triggerTable = "deleted";
            int isOld = 1;

            Models.Table table = _TableManager.ReadTableSchema(originalConnectionString, originalTableName);

            result.AppendLine(generateTriggerStart(originalDbName, originalTableFullName, triggerName, triggerAction));
            result.AppendLine(generateCommonInsert(table, targetDbName, logTableFullName, isOld, triggerAction, triggerTable));
            result.AppendLine(generateTriggerEnd());

            return result.ToString();
        }

        public string GenerateTriggerOnUpdate(string originalConnectionString, string originalDbName, string originalSchema, string originalTableName, string targetDbName, string targetSchema)
        {
            StringBuilder result = new StringBuilder();
            string logTableName = $"{originalTableName}_Log";
            string logTableFullName = $"[{ targetSchema}].[{ logTableName}]";
            string triggerName = $"[{originalSchema}].[{originalTableName}_Log_On_Update]";
            string originalTableFullName = $"[{ originalSchema}].[{ originalTableName}]";
            string triggerAction = "update";
            string triggerTable = "deleted";
            int isOld = 1;

            Models.Table table = _TableManager.ReadTableSchema(originalConnectionString, originalTableName);

            result.AppendLine(generateTriggerStart(originalDbName, originalTableFullName, triggerName, triggerAction));
            result.AppendLine(generateCommonInsert(table, targetDbName, logTableFullName, isOld, triggerAction, triggerTable));
            triggerTable = "inserted";
            isOld = 0;
            result.AppendLine(generateCommonInsert(table, targetDbName, logTableFullName, isOld, triggerAction, triggerTable));
            result.AppendLine(generateTriggerEnd());

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
