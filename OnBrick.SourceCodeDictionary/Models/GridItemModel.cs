using OnBrick.SourceCodeDictionary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnBrick.SourceCodeDictionary.Models
{
    public class GridItemModel : BindableBase
    {
        public int Sequence { get; set; }

        public string File { get; set; }
        public string Namespace { get; set; }
        public string FQDN { get; set; }
        public string MemberType { get; set; }

        public string Public { get; set; }
        public string Protected { get; set; }
        public string Private { get; set; }
        public string Internal { get; set; }
        public string Partial { get; set; }
        public string Static { get; set; }
        public string New { get; set; }
        public string Abstract { get; set; }
        public string Override { get; set; }

        public string Type { get; set; }
        public string ReturnType { get; set; }
        public string Identifier { get; set; }

        public GridItemModel()
        {
        }
    }
}