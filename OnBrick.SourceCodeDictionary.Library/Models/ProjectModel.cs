using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public class ProjectModel
    {
        public string Id { get; set; }
        public string Version { get; set; } 
        public string Path { get; set; }
        public string Name { get; set; }

        public readonly IList<DocumentModel> Documents;

        public ProjectModel()
        {
            Documents = new List<DocumentModel>();
        }

    }
}
