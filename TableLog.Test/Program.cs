using System;

namespace TableLog.Test
{
    class Program
    {
        static string _ConnectionString = "Data Source=192.168.1.110;Initial Catalog=Draft;UID=sqladmin;PWD=87654321;";
        static void Main(string[] args)
        {
            TestTriggerManager test = new TestTriggerManager() { ConnectionString = _ConnectionString};
            test.TestUpdateDummy();
        }
    }
}
