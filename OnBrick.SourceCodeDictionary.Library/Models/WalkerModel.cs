using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library.Models
{
    public class WalkerModel
    {
        List<ProjectModel> projects;

        public WalkerModel(ProjectModel p)
        {
            projects = new List<ProjectModel>();
            projects.Add(p);
        }

        public void Add(ProjectModel p)
        {
            projects.Add(p);
        }
    }
}
