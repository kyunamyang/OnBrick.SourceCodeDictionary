using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public class AccessorModel
    {
        public string Id { get; set; }
        public int SpanStart { get; set; }
        public string Keyword { get; set; }
        public string Body { get; set; }
        public string ExpressionBody { get; set; }
    }
}