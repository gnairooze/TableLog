using System;
using System.Collections.Generic;

namespace TableLog.Business
{
    public class Manager
    {
        protected ITableManager _TableManager = null;
        protected List<string> _ReservedNames = new List<string>() { "ID", "Create_Date", "Correlation_ID", "Is_Old", "Action" };

        protected string CalculateColumnName(string tableName, Models.Column column)
        {
            if (_ReservedNames.Contains(column.Name)) //reserved for columns in the log table
            {
                return $"{tableName}_{column.Name}";
            }
            else
            {
                return column.Name;
            }
        }
    }
}
