using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public sealed class DocumentModel
    {
        public string Id { get; set; }

        public string OriginalPath { get; set; }
        public string OriginalText { get; set; }

        public string Path { get; set; }
        public string Name { get; set; }
        public string SourceCodeKind { get; set; }
        public readonly IList<UsingModel> Usings;
        public readonly IList<ClassModel> Classes;
        public readonly IList<InterfaceModel> Interfaces;

        public DocumentModel()
        {
            Usings = new List<UsingModel>();
            Classes = new List<ClassModel>();
            Interfaces = new List<InterfaceModel>();
        }
    }
}