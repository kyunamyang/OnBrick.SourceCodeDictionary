using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public sealed class UsingModel
    {
        public string Id { get; set; }
        public int SpanStart { get; set; }
        public string Text { get; set; }
        public string Comments { get; set; }
    }
}
