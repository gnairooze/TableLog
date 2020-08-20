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
    }
}
