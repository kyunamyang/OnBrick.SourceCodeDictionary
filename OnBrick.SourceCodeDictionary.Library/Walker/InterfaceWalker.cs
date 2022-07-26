using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OnBrick.SourceCodeDictionary.Library.Walker
{
    public class InterfaceWalker : CSharpSyntaxWalker
    {
        public IList<InterfaceDeclarationSyntax> Interfaces { get; }

        public InterfaceWalker()
        {
            this.Interfaces = new List<InterfaceDeclarationSyntax>();
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            //base.VisitClassDeclaration(node);
            Interfaces.Add(node);
        }
    }
}
