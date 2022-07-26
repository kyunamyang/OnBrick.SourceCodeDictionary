using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OnBrick.SourceCodeDictionary.Library.Walker
{
    public class UsingWalker : CSharpSyntaxWalker
    {
        public IList<UsingDirectiveSyntax> Usings { get; }

        public UsingWalker()
        {
            Usings = new List<UsingDirectiveSyntax>();
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            Usings.Add(node);
        }
    }
}
