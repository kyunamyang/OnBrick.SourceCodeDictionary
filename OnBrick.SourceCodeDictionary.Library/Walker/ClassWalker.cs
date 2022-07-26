using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OnBrick.SourceCodeDictionary.Library.Walker
{
    public class ClassWalker : CSharpSyntaxWalker
    {
        public IList<ClassDeclarationSyntax> Classes { get; }

        public ClassWalker()
        {
            this.Classes = new List<ClassDeclarationSyntax>();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            //base.VisitClassDeclaration(node);
            Classes.Add(node);
        }
    }
}
