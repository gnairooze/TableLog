using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Test
{
    class TestLogTableManager
    {
        public string ConnectionString { get; set; }
        public void Test1() 
        {
            TableLog.Business.LogTableManager manager = new Business.LogTableManager(new TableLog.Business.TestTableManager());
            string result = manager.GenerateLogTableSchema("dummy", "CM_Users", "Logs", "dbo");

            Console.WriteLine(result);
        }

        public void Test2()
        {
            TableLog.Business.LogTableManager manager = new Business.LogTableManager(new TableLog.Business.TableManager());
            string result = manager.GenerateLogTableSchema(this.ConnectionString, "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }
    }
}
