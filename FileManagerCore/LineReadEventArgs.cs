using System;

namespace FileManagerCore
{
    public class LineReadEventArgs:EventArgs
    {
        public string CurrentLine { get; set; }
        public string Header { get; set; }
    }
}
