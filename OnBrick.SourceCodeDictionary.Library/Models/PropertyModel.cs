﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public sealed class PropertyModel
    {
        public string Id { get; set; }
        public int SpanStart { get; set; }
        public string Identifier { get; set; }
        public string Modifier { get; set; }

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
        //public string TypeParameter { get; set; }
        public string Body { get; set; }
        public string Comments { get; set; }

        public IList<AccessorModel> Accessors;
        public IList<AttributeModel> Attributes;

        public PropertyModel()
        {
            Accessors = new List<AccessorModel>();
            Attributes = new List<AttributeModel>();
        }
    }
}
