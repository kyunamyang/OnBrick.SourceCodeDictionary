using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public sealed class InterfaceModel
    {
        public string Id { get; set; }
        public int SpanStart { get; set; }
        public string Identifier { get; set; }
        public string Namespace { get; set; }
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

        public string BaseList { get; set; }
        public string ConstraintClauses { get; set; }
        public string TypeParameter { get; set; }
        public string Comments { get; set; }

        public readonly IList<ConstructModel> Constructs;
        public DestructModel Destruct;
        //public readonly IList<FieldModel> Fields;
        public readonly IList<PropertyModel> Properties;
        public readonly IList<MethodModel> Methods;
        public IList<AttributeModel> Attributes;

        public InterfaceModel()
        {
            Constructs = new List<ConstructModel>();
            //Fields = new List<FieldModel>();
            Properties = new List<PropertyModel>();
            Methods = new List<MethodModel>();
            Attributes = new List<AttributeModel>();
        }
    }
}
