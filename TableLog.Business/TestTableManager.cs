using System;
using System.Collections.Generic;
using System.Text;
using TableLog.Business.Models;

namespace TableLog.Business
{
    public class TestTableManager : ITableManager
    {
        public List<string> ListTables(string connectionString, string tableName)
        {
            return new List<string>() { "Users", "CM_Users" };
        }

        public Table ReadTableSchema(string connectionString, string tableName)
        {
            Table table = new Table() { Name = tableName };

            table.Columns.AddRange(new List<Column> (){
               new Column(){ Name="ID", CollationName=null, DataType="int", IsPrimary = true, IsRequired = true, MaxLength = null},
               new Column(){ Name="Create_Date", CollationName=null, DataType="datetime", IsPrimary=false, IsRequired=true, MaxLength=null},
               new Column(){ Name="Name", CollationName="SQL_Latin1_General_CP1_CI_AS", DataType="nvarchar", IsPrimary=false, IsRequired=false, MaxLength=50},
               new Column(){ Name="Username", CollationName="SQL_Latin1_General_CP1_CI_AS", DataType="nvarchar", IsPrimary=false, IsRequired=true, MaxLength=50}
            });
            return table;
        }
    }
}
