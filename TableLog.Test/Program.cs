using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using TableLog.Business;

namespace TableLog.Test
{
    class Program
    {
        static FileManagerCore.Controller _FileManager = new FileManagerCore.Controller();

        #region properties
        static string ConnectionString { get; set; }
        static RunMode Mode { get; set; }
        static string SourceDB { get; set; }
        static string SourceSchema { get; set; }
        static string SourceTable { get; set; }
        static string TargetDB { get; set; }
        static string TargetSchema { get; set; }
        static List<string> EnhancedArgs { get; set; }
        #endregion

        /// <summary>
        /// test or generate log table schema and related triggers
        /// </summary>
        /// <param name="args">
        /// no args is the same as --help
        /// 1. --test-dummy
        /// 2. --test-real
        /// 3. --real
        /// 4. --source-connection-string
        /// 5. --source-db
        /// 6. --source-schema
        /// 5. --source-table
        /// 6. --target-db
        /// 7. --target-schema
        /// 8. --help
        /// </param>
        static void Main(string[] args)
        {
            EnhancedArgs = args.ToList();

            DecideMode();

            Start();
        }

        private static void DecideMode()
        {
            if (EnhancedArgs.Count == 0 || EnhancedArgs.Any(e=> e.StartsWith("--help", StringComparison.InvariantCultureIgnoreCase)))
            {
                Mode = RunMode.Help;
                return;
            }

            if (EnhancedArgs.Any(e=> e.StartsWith("--real", StringComparison.InvariantCultureIgnoreCase)))
            {
                Mode = RunMode.Real;
                return;
            }

            if (EnhancedArgs.Any(e=> e.StartsWith("--test-dummy", StringComparison.InvariantCultureIgnoreCase)))
            {
                Mode = RunMode.TestDummy;
                return;
            }

            if (EnhancedArgs.Any(e=> e.StartsWith("--test-real", StringComparison.InvariantCultureIgnoreCase)))
            {
                Mode = RunMode.TestReal;
                return;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Generate Log Table Schema And Triggers Script");
            Console.WriteLine("=============================================");
            Console.WriteLine("--help\tshow this help. This help also shows when no arguments added");
            Console.WriteLine();
            Console.WriteLine("--real\tconnect to real db to generate script based on other arguments");
            Console.WriteLine();
            Console.WriteLine("--test-dummy\ttest functionalties without connecting to DB. No other arguments needed");
            Console.WriteLine();
            Console.WriteLine("--test-real\ttest functionalties by connecting to DB using --conn-string");
            Console.WriteLine();
            Console.WriteLine("--source-connection-string\tconnection string for the database of the table to generate log for");
            Console.WriteLine();
            Console.WriteLine("--source-db\tname of the database for the table to generate log for");
            Console.WriteLine();
            Console.WriteLine("--source-schema\tname of the schema for the table to generate log for");
            Console.WriteLine();
            Console.WriteLine("--source-table\tname of the table to generate log for");
            Console.WriteLine();
            Console.WriteLine("--target-db\tname of the database for the log table");
            Console.WriteLine();
            Console.WriteLine("--target-schema\tname of the schema for the log table");
            Console.WriteLine();
            Console.WriteLine("=============================================");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("TableLog.Test.exe --real --source-connection-string\"Data Source = 192.168.1.110; Initial Catalog = Draft; UID = sqladmin; PWD = 87654321;\" --source-dbDraft --source-schemadbo --source-tableUsers --target-dbLogging --target-schemadbo");
            Console.WriteLine();
            Console.WriteLine("=============================================");
        }

        private static void Start()
        {
            switch (Mode)
            {
                case RunMode.NotSet:
                    ShowHelp();
                    return;
                case RunMode.TestDummy:
                    RunTests(true);
                    return;
                case RunMode.TestReal:
                    SetConnectionString();
                    RunTests(false);
                    break;
                case RunMode.Real:
                    SetConnectionString();
                    SetRealArguments();
                    RunReal();
                    break;
                case RunMode.Help:
                    ShowHelp();
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }

        private static void SetRealArguments()
        {
            string arg = "--source-table";
            if (EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string value = EnhancedArgs.First(a => a.StartsWith(arg));
                SourceTable = value.Substring(arg.Length);
            }

            arg = "--target-db";
            if (EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string value = EnhancedArgs.First(a => a.StartsWith(arg));
                TargetDB = value.Substring(arg.Length);
            }

            arg = "--target-schema";
            if (EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string value = EnhancedArgs.First(a => a.StartsWith(arg));
                TargetSchema = value.Substring(arg.Length);
            }

            arg = "--source-schema";
            if (EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string value = EnhancedArgs.First(a => a.StartsWith(arg));
                SourceSchema = value.Substring(arg.Length);
            }

            arg = "--source-db";
            if (EnhancedArgs.Any(e => e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string value = EnhancedArgs.First(a => a.StartsWith(arg));
                SourceDB = value.Substring(arg.Length);
            }
        }

        private static bool IsValidForRealOperation(out string result)
        {
            bool isValid = true;
            StringBuilder reason = new StringBuilder();

            if (String.IsNullOrEmpty(ConnectionString))
            {
                reason.AppendLine("source connection string not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(SourceTable))
            {
                reason.AppendLine("source table not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(TargetDB))
            {
                reason.AppendLine("target database not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(TargetSchema))
            {
                reason.AppendLine("target schema not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(SourceSchema))
            {
                reason.AppendLine("source schema not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(SourceDB))
            {
                reason.AppendLine("source database not set");
                isValid = false;
            }

            result = reason.ToString();
            return isValid;
        }

        private static void RunReal()
        {
            string reason;
            bool isValid = IsValidForRealOperation(out reason);

            if (!isValid)
            {
                bool moreThan1 = reason.Split(Environment.NewLine).Length > 1;
                string pluralNoun = moreThan1 ? "s" : string.Empty;
                string pluralVerb = moreThan1 ? "are" : "is";
                Console.WriteLine($"mode set to real but the following argument{pluralNoun} {pluralVerb} missing:");
                Console.WriteLine(reason);
                return;
            }

            TableManager tableManager = new TableManager();
            LogTableManager logTableManager = new LogTableManager(tableManager);
            TriggerManager triggerManager = new TriggerManager(tableManager);

            string tableScript = logTableManager.GenerateLogTableSchema(ConnectionString, SourceTable, TargetDB, TargetSchema);
            if (!System.IO.Directory.Exists("result"))
            {
                System.IO.Directory.CreateDirectory("result");
                System.IO.Directory.CreateDirectory("result\\source");
                System.IO.Directory.CreateDirectory("result\\target");
            }

            if (!System.IO.Directory.Exists("result\\source"))
            {
                System.IO.Directory.CreateDirectory("result\\source");
            }

            if (!System.IO.Directory.Exists("result\\target"))
            {
                System.IO.Directory.CreateDirectory("result\\target");
            }


            _FileManager.SaveFile(false, "result\\target\\"+SourceTable + "_Log_Create_Table.sql", tableScript);
            string deleteTriggerScript = triggerManager.GenerateTriggerOnDelete(ConnectionString, SourceDB, SourceSchema, SourceTable, TargetDB, TargetSchema);
            _FileManager.SaveFile(false, "result\\source\\" + SourceTable + "_Trigger_LogOnDelete.sql", deleteTriggerScript);
            string insertTriggerScript = triggerManager.GenerateTriggerOnInsert(ConnectionString, SourceDB, SourceSchema, SourceTable, TargetDB, TargetSchema);
            _FileManager.SaveFile(false, "result\\source\\" + SourceTable + "_Trigger_LogOnInsert.sql", insertTriggerScript);
            string updateTriggerScript = triggerManager.GenerateTriggerOnUpdate(ConnectionString, SourceDB, SourceSchema, SourceTable, TargetDB, TargetSchema);
            _FileManager.SaveFile(false, "result\\source\\" + SourceTable + "_Trigger_LogOnUpdate.sql", updateTriggerScript);
        }

        private static void SetConnectionString()
        {
            string arg = "--source-connection-string";
            if (EnhancedArgs.Any(e=> e.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase)))
            {
                string conn = EnhancedArgs.First(a => a.StartsWith(arg));
                ConnectionString = conn.Substring(arg.Length);
                return;
            }
        }

        private static void RunTests(bool isDummy)
        {
            if (!isDummy && string.IsNullOrEmpty(ConnectionString))
            {
                Console.WriteLine("Set to Test-Real but source connection string not set. test aborted");
                return;
            }

            TestLogTableManager managerTable = new TestLogTableManager();
            TestTriggerManager managerTrigger = new TestTriggerManager();

            if (!isDummy)
            {
                managerTable.ConnectionString = ConnectionString;
                managerTrigger.ConnectionString = ConnectionString;
            }

            if(isDummy)
            {
                Console.WriteLine("===Test Dummy Mode=================================");
            }
            else
            {
                Console.WriteLine("===Test Real Mode=================================");
            }
            Console.WriteLine("===Start Test Log Table Schema Script Generation===");
            if(isDummy)
            {
                managerTable.TestDummy();
            }
            else
            {
                managerTable.TestReal();
            }
            Console.WriteLine("===End Test Log Table Schema Script Generation=====");
            Console.WriteLine("===================================================");
            Console.WriteLine("====Start Test Delete Trigger Script Generation====");
            if (isDummy)
            {
                managerTrigger.TestDeleteDummy();
            }
            else
            {
                managerTrigger.TestDeleteReal();
            }
            Console.WriteLine("====End Test Delete Trigger Script Generation======");
            Console.WriteLine("===================================================");
            Console.WriteLine("====Start Test Insert Trigger Script Generation====");
            if (isDummy)
            {
                managerTrigger.TestInsertDummy();
            }
            else
            {
                managerTrigger.TestInsertReal();
            }
            Console.WriteLine("====End Test Insert Trigger Script Generation======");
            Console.WriteLine("===================================================");
            Console.WriteLine("====Start Test Update Trigger Script Generation====");
            if (isDummy)
            {
                managerTrigger.TestUpdateDummy();
            }
            else
            {
                managerTrigger.TestUpdateReal();
            }
            Console.WriteLine("====End Test Update Trigger Script Generation======");
            Console.WriteLine("===================================================");
        }

        enum RunMode
        {
            NotSet = 0,
            TestDummy = 1,
            TestReal = 2,
            Real = 4,
            Help = 8
        }
    }
}
