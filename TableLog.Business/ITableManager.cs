using System.Collections.Generic;
using TableLog.Business.Models;

namespace TableLog.Business
{
    public interface ITableManager
    {
        List<string> ListTables(string connectionString, string tableName);
        Table ReadTableSchema(string connectionString, string tableName);
    }
}