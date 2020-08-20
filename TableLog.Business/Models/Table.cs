using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Business.Models
{
    public class Table
    {
        public Table()
        {
            this.Columns = new List<Column>();
        }
        public string Name { get; set; }
        public List<Column> Columns { get; private set; }
    }
}
