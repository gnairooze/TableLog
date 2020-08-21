using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileManagerCore
{
    public class Controller
    {
        public event EventHandler<LineReadEventArgs> LineRead;

        public void ReadFile(bool hasHeader, string filename)
        {
            bool firstLine = true;
            string header = string.Empty;

            using (StreamReader sr = new StreamReader(filename))
            {
                string currentLine;
                // currentLine will be null when the StreamReader reaches the end of file
                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (hasHeader && firstLine)
                    {
                        header = currentLine;
                    }

                    //fire event with the line data
                    OnLineRead(new LineReadEventArgs()
                    {
                        CurrentLine = currentLine,
                        Header = header
                    });

                    firstLine = false;
                }
            }
        }

        protected virtual void OnLineRead(LineReadEventArgs e)
        {
            EventHandler<LineReadEventArgs> handler = LineRead;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void SaveFile(bool append, string filename, string line)
        {
            using (StreamWriter file = new StreamWriter(filename, append))
            {
                file.WriteLine(line);
            }
        }
    }
}
