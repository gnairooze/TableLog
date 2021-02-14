using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Command
{
    public class Options
    {
        public enum RunMode
        {
            NotSet = 0,
            TestDummy = 1,
            TestReal = 2,
            Real = 4,
            Help = 8
        }

        [Option('m', "mode", Default = RunMode.Help, Required = false, HelpText = "Set run mode: TestDummy, TestReal, Real")]
        public RunMode Mode { get; set; }

        [Option('c', "source-connection-string", Required = false, HelpText = "Set source connection string")]
        public string SourceConnectionString { get; set; }
        
        [Option('w', "source-db", Required = false, HelpText = "Set source database name")]
        public string SourceDB { get; set; }
        
        [Option('e', "source-schema", Default = "dbo", Required = false, HelpText = "Set source schema")]
        public string SourceSchema { get; set; }
        
        [Option('r', "source-table", Required = false, HelpText = "Set source table name")]
        public string SourceTable { get; set; }
        
        [Option('s', "target-db", Required = false, HelpText = "Set target database name")]
        public string TargetDB { get; set; }
        
        [Option('d', "target-schema", Default = "dbo", Required = false, HelpText = "Set target schema")]
        public string TargetSchema { get; set; }

        [Usage(ApplicationAlias = "TableLog.Command")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Generate log scripts for a real table", new Options { 
                    Mode = RunMode.Real,
                    SourceConnectionString = "Data Source=.; Initial Catalog=Draft; Integrated Security=true;",
                    SourceDB = "Draft",
                    SourceSchema = "dbo",
                    SourceTable = "Users",
                    TargetDB = "Logging",
                    TargetSchema = "dbo"
                });
            }
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, new Newtonsoft.Json.Converters.StringEnumConverter());
        }
    }
}
