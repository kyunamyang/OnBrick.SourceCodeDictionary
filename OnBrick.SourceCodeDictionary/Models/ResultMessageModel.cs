using OnBrick.SourceCodeDictionary.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnBrick.SourceCodeDictionary.Models
{
    public class ResultMessageModel
    {
        public string Message { get; set; }
        public ErrorTypeEnum Category { get; set; }
        public string Position { get; set; }

        public ResultMessageModel()
        {
            Message = string.Empty;
            Category = ErrorTypeEnum.Spec;
            Position = string.Empty;
        }
    }
}
