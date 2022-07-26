using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public class ParameterModel
    {
        public string Id { get; set; }
        public int SpanStart { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string Modifier { get; set; }
    }
}
