using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Test
{
    class TestLogTableManager
    {
        public void Test1() 
        {
            TableLog.Business.LogTableManager manager = new Business.LogTableManager(new TableLog.Business.TestTableManager());
            string result = manager.GenerateLogTableSchema("dummy", "CM_Users", "Logs", "dbo");

            Console.WriteLine(result);
        }

        public void Test2()
        {
            TableLog.Business.LogTableManager manager = new Business.LogTableManager(new TableLog.Business.TableManager());
            string result = manager.GenerateLogTableSchema("Data Source=192.168.1.110;Initial Catalog=Draft;UID=sqladmin;PWD=87654321;", "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }
    }
}
