using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Test
{
    class TestTriggerManager
    {
        public string ConnectionString { get; set; }
        public void TestInsertDummy()
        {
            TableLog.Business.TriggerManager manager = new Business.TriggerManager(new TableLog.Business.TestTableManager());
            string result = manager.GenerateTriggerOnInsert("dummy", "Draft", "dbo", "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }

        public void TestInsertReal()
        {
            TableLog.Business.TriggerManager manager = new Business.TriggerManager(new TableLog.Business.TableManager());
            string result = manager.GenerateTriggerOnInsert(this.ConnectionString, "Draft", "dbo", "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }

        public void TestDeleteDummy()
        {
            TableLog.Business.TriggerManager manager = new Business.TriggerManager(new TableLog.Business.TestTableManager());
            string result = manager.GenerateTriggerOnDelete("dummy", "Draft", "dbo", "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }

        public void TestDeleteReal()
        {
            TableLog.Business.TriggerManager manager = new Business.TriggerManager(new TableLog.Business.TableManager());
            string result = manager.GenerateTriggerOnDelete(this.ConnectionString, "Draft", "dbo", "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }

        public void TestUpdateDummy()
        {
            TableLog.Business.TriggerManager manager = new Business.TriggerManager(new TableLog.Business.TestTableManager());
            string result = manager.GenerateTriggerOnUpdate("dummy", "Draft", "dbo", "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }

        public void TestUpdateReal()
        {
            TableLog.Business.TriggerManager manager = new Business.TriggerManager(new TableLog.Business.TableManager());
            string result = manager.GenerateTriggerOnUpdate(this.ConnectionString, "Draft", "dbo", "CM_Users", "Logging", "dbo");

            Console.WriteLine(result);
        }
    }
}
