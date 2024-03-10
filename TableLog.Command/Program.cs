using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TableLog.Business;

namespace TableLog.Command
{
    class Program
    {
        static readonly FileManagerCore.Controller _FileManager = new ();
        static ParserResult<Options> _ParserResult;

        static void Main(string[] args)
        {
            _ParserResult = Parser.Default.ParseArguments<Options>(args);
            _ParserResult.WithParsed(RunOptions).WithNotParsed(HandleParseError);
        }

        static void RunOptions(Options opts)
        {
            //Console.WriteLine(opts.ToString());

            Start(opts);
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            bool invalid = false;

            if(errs != null && errs.Any())
            {
                bool helpRequested = errs.Any(e => 
                e.Tag == ErrorType.HelpRequestedError 
                || e.Tag == ErrorType.HelpVerbRequestedError
                || e.Tag == ErrorType.VersionRequestedError
                );

                if(!helpRequested)
                {
                    invalid = true;
                    Console.WriteLine("!!!!! invalid argument(s) !!!!!");
                }
            }

            if (!invalid)
            {
                return;
            }

            foreach (var err in errs)
            {
                Console.WriteLine($"{err.Tag} | Stopped Processing: {err.StopsProcessing}");
            }
        }

        private static void Start(Options opts)
        {
            switch (opts.Mode)
            {
                case Options.RunMode.NotSet:
                case Options.RunMode.Help:
                    Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(_ParserResult, h => h, e => e));
                    return;
                case Options.RunMode.TestDummy:
                    RunTests(opts, true);
                    return;
                case Options.RunMode.TestReal:
                    RunTests(opts, false);
                    break;
                case Options.RunMode.Real:
                    RunReal(opts);
                    break;
            }
        }

        private static bool IsValidForRealOperation(Options opts, out string result)
        {
            bool isValid = true;
            StringBuilder reason = new ();

            if (String.IsNullOrEmpty(opts.SourceConnectionString))
            {
                reason.AppendLine("source connection string not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(opts.SourceTable))
            {
                reason.AppendLine("source table not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(opts.TargetDB))
            {
                reason.AppendLine("target database not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(opts.TargetSchema))
            {
                reason.AppendLine("target schema not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(opts.SourceSchema))
            {
                reason.AppendLine("source schema not set");
                isValid = false;
            }

            if (String.IsNullOrEmpty(opts.SourceDB))
            {
                reason.AppendLine("source database not set");
                isValid = false;
            }

            result = reason.ToString();
            return isValid;
        }
        private static void RunReal(Options opts)
        {

            #region validate
            bool isValid = IsValidForRealOperation(opts, out string reason);

            if (!isValid)
            {
                bool moreThan1 = reason.Split(Environment.NewLine).Length > 1;
                string pluralNoun = moreThan1 ? "s" : string.Empty;
                string pluralVerb = moreThan1 ? "are" : "is";
                Console.WriteLine($"mode set to real but the following argument{pluralNoun} {pluralVerb} missing:");
                Console.WriteLine(reason);
                return;
            }
            #endregion

            TableManager tableManager = new ();
            LogTableManager logTableManager = new (tableManager);
            TriggerManager triggerManager = new (tableManager);

            string tableScript = logTableManager.GenerateLogTableSchema(opts.SourceConnectionString, opts.SourceTable, opts.TargetDB, opts.TargetSchema);
            
            HandleFolderStructure();

            #region save files
            _FileManager.SaveFile(false, "result\\target\\" + opts.SourceTable + "_Log_Create_Table.sql", tableScript);
            string deleteTriggerScript = triggerManager.GenerateTriggerOnDelete(opts.SourceConnectionString, opts.SourceDB, opts.SourceSchema, opts.SourceTable, opts.TargetDB, opts.TargetSchema);
            _FileManager.SaveFile(false, "result\\source\\" + opts.SourceTable + "_Trigger_LogOnDelete.sql", deleteTriggerScript);
            string insertTriggerScript = triggerManager.GenerateTriggerOnInsert(opts.SourceConnectionString, opts.SourceDB, opts.SourceSchema, opts.SourceTable, opts.TargetDB, opts.TargetSchema);
            _FileManager.SaveFile(false, "result\\source\\" + opts.SourceTable + "_Trigger_LogOnInsert.sql", insertTriggerScript);
            string updateTriggerScript = triggerManager.GenerateTriggerOnUpdate(opts.SourceConnectionString, opts.SourceDB, opts.SourceSchema, opts.SourceTable, opts.TargetDB, opts.TargetSchema);
            _FileManager.SaveFile(false, "result\\source\\" + opts.SourceTable + "_Trigger_LogOnUpdate.sql", updateTriggerScript);
            #endregion
        }

        private static void HandleFolderStructure()
        {
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
        }

        private static void RunTests(Options opts, bool isDummy)
        {
            if (!isDummy && string.IsNullOrEmpty(opts.SourceConnectionString))
            {
                Console.WriteLine("Set to Test-Real but source connection string not set. test aborted");
                return;
            }

            TestLogTableManager managerTable = new ();
            TestTriggerManager managerTrigger = new ();

            if (!isDummy)
            {
                managerTable.ConnectionString = opts.SourceConnectionString;
                managerTrigger.ConnectionString = opts.SourceConnectionString;
            }

            if (isDummy)
            {
                Console.WriteLine("===Test Dummy Mode=================================");
            }
            else
            {
                Console.WriteLine("===Test Real Mode=================================");
            }
            Console.WriteLine("===Start Test Log Table Schema Script Generation===");
            if (isDummy)
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

        
    }
}
