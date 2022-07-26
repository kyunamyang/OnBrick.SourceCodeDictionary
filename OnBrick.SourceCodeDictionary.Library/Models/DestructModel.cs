using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public sealed class DestructModel
    {
        public string Id { get; set; }
        public int SpanStart { get; set; }
        public string Identifier { get; set; }
        public string Modifier { get; set; }
        public string Parameter { get; set; } //
        //public string TypeParameter { get; set; }
        public string Body { get; set; }
        public string Comments { get; set; }

        public IList<AttributeModel> Attributes;

        public DestructModel()
        {
            Attributes = new List<AttributeModel>();
        }
    }
}
