using System;
using System.Collections.Generic;
using System.Text;

namespace TableLog.Business.Models
{
    public class Column
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsRequired { get; set; }
        public string CollationName { get; set; }
    }
}
