using System;
using System.Collections.Generic;
using System.Linq;

namespace TableLog.Business
{
    public class Manager
    {
        protected ITableManager _TableManager = null;
        protected List<string> _ReservedNames = new List<string>() { "ID", "Create_Date", "Correlation_ID", "Is_Old", "Action" };

        protected string CalculateColumnName(string tableName, Models.Column column)
        {
            //if the column name is a reserved name regardless of the case, then we need to prefix it with the table name
            if (_ReservedNames.Contains(column.Name, StringComparer.OrdinalIgnoreCase))
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
