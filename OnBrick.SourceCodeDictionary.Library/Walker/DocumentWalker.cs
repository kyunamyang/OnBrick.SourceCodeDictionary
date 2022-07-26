using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OnBrick.SourceCodeDictionary.Library.Models;
using System.IO;
using System.Xml;

namespace OnBrick.SourceCodeDictionary.Library.Walker
{
    public class DocumentWalker
    {
        public DocumentWalker()
        {
        }

        public DocumentModel GetDocumentModel(string filePath, string codeText)
        {
            DocumentModel documentModel = new DocumentModel();

            documentModel.OriginalPath = filePath;
            documentModel.OriginalText = codeText;

            CSharpSyntaxTree tree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(codeText);

            if (tree.HasCompilationUnitRoot == true)
            {
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                // [USING]
                var uw = new UsingWalker();
                uw.Visit(root);

                foreach (var u in uw.Usings)
                {
                    string s = u.ToString()
                        .TrimStart(u.UsingKeyword.ToString().ToCharArray())
                        .TrimEnd(u.SemicolonToken.ToString().ToCharArray());
                    documentModel.Usings.Add(new UsingModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpanStart = u.SpanStart,
                        //Text = u.ToString(), // u.Name.ToString(),
                        Text = s,
                        Comments = string.Empty
                    });
                }

                // [TYPE.CLASS]
                var cw = new ClassWalker();
                cw.Visit(root);

                foreach (var c in cw.Classes)
                {
                    string b = c.BaseList == null ? string.Empty : c.BaseList.ToString().TrimStart(c.BaseList.ColonToken.ToString().ToCharArray());
                    string t = c.TypeParameterList == null ? string.Empty : 
                        c.TypeParameterList.ToString()
                            .TrimStart(c.TypeParameterList.LessThanToken.ToString().ToCharArray())
                            .TrimEnd(c.TypeParameterList.GreaterThanToken.ToString().ToCharArray());
                    //string constrains = string.Empty;
                    //if(c.ConstraintClauses.Count > 0)
                    //{
                    //    foreach(var el in c.ConstraintClauses)
                    //    {
                    //        constrains += el.ToString() + 
                    //    }
                    //}
                    var classModel = new ClassModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpanStart = c.Identifier.SpanStart,
                        Identifier = c.Identifier.Text,
                        Namespace = c.Parent as NamespaceDeclarationSyntax == null ? string.Empty : ((NamespaceDeclarationSyntax)c.Parent).Name.ToString(),
                        Modifier = c.Modifiers.ToString(),

                        Public = c.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                        Protected = c.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                        Private = c.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                        Internal = c.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                        Partial = c.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                        Static = c.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                        New = c.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                        Abstract = c.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                        Override = c.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                        //BaseList = c.BaseList == null ? string.Empty : c.BaseList.ToString(),
                        BaseList = b.TrimStart(),
                        ConstraintClauses = c.ConstraintClauses == null ? string.Empty : c.ConstraintClauses.ToString(),
                        //TypeParameter = c.TypeParameterList == null ? string.Empty : c.TypeParameterList.ToString(),
                        TypeParameter = t.TrimStart(),
                        Attributes = GetAttribute(c.AttributeLists),
                        Comments = GetComments(c.GetLeadingTrivia())
                    };

                    foreach (var el in c.Members)
                    {
                        if (el is ConstructorDeclarationSyntax cd)
                        {
                            var construcModel = new ConstructModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = cd.SpanStart,
                                Identifier = cd.Identifier.Text,
                                Modifier = cd.Modifiers.ToString(),

                                Public = cd.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                                Protected = cd.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                                Private = cd.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                                Internal = cd.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                                Partial = cd.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                                Static = cd.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                                New = cd.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                                Abstract = cd.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                                Override = cd.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                                Initializer = cd.Initializer == null ? string.Empty : cd.Initializer.ToString(),
                                Parameter = cd.ParameterList.ToString(),
                                Body = cd.Body == null ? string.Empty : cd.Body.ToString(),
                                Attributes = GetAttribute(cd.AttributeLists),
                                Comments = GetComments(cd.GetLeadingTrivia())
                            };

                            classModel.Constructs.Add(construcModel);

                            foreach (var pl in cd.ParameterList.Parameters)
                            {
                                construcModel.Parameters.Add(new ParameterModel
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    SpanStart = pl.SpanStart,
                                    Type = pl.Type.ToString(),
                                    Identifier = pl.Identifier.Text,
                                    Modifier = pl.Modifiers.ToString()
                                });
                            }
                        }

                        if (el is DestructorDeclarationSyntax dd)
                        {
                            classModel.Destruct = new DestructModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = dd.SpanStart,
                                Identifier = dd.TildeToken.ToString() + dd.Identifier.Text,
                                Modifier = dd.Modifiers.ToString(),
                                Parameter = dd.ParameterList.ToString(),
                                Body = dd.Body == null ? string.Empty : dd.Body.ToString(),
                                Attributes = GetAttribute(dd.AttributeLists),
                                Comments = GetComments(dd.GetLeadingTrivia())
                            };
                        }

                        #region // Field
                        if (el is FieldDeclarationSyntax f)
                        {
                            foreach(var v in f.Declaration.Variables)
                            {
                                classModel.Fields.Add(new FieldModel
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    SpanStart = f.SpanStart,
                                    //Identifier = f.Declaration.Variables.ToString(),
                                    //Identifier = f.Declaration.Variables[0].Identifier.ToString(),
                                    Identifier = v.Identifier.ToString(),
                                    Modifier = f.Modifiers.ToString(),

                                    Public = f.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                                    Protected = f.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                                    Private = f.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                                    Internal = f.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                                    Partial = f.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                                    Static = f.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                                    New = f.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                                    Abstract = f.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                                    Override = f.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                                    Type = f.Declaration.Type.ToString(),
                                    Attributes = GetAttribute(f.AttributeLists),
                                    Comments = GetComments(f.GetLeadingTrivia())
                                });
                            }
                            
                        }
                        #endregion

                        #region // Property
                        if (el is PropertyDeclarationSyntax prop)
                        {
                            var propertyModel = new PropertyModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = prop.SpanStart,
                                Identifier = prop.Identifier.Text,
                                Modifier = prop.Modifiers.ToString(),

                                Public = prop.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                                Protected = prop.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                                Private = prop.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                                Internal = prop.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                                Partial = prop.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                                Static = prop.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                                New = prop.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                                Abstract = prop.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                                Override = prop.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                                Type = prop.Type.ToString(),
                                Body = prop.AccessorList == null ? string.Empty : prop.AccessorList.ToString(),
                                Attributes = GetAttribute(prop.AttributeLists),
                                Comments = GetComments(prop.GetLeadingTrivia())
                            };

                            classModel.Properties.Add(propertyModel);

                            if (prop.AccessorList != null)
                            {
                                foreach (var a in prop.AccessorList.Accessors)
                                {
                                    propertyModel.Accessors.Add(new AccessorModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        SpanStart = a.SpanStart,
                                        Keyword = a.Keyword.Text,
                                        Body = a.Body == null ? string.Empty : a.Body.ToString(),
                                        ExpressionBody = a.ExpressionBody == null ? string.Empty : a.ExpressionBody.ToString()
                                    });
                                }
                            }
                        }
                        #endregion

                        if (el is MethodDeclarationSyntax m)
                        {
                            string p = m.ParameterList.ToString()
                                .TrimStart(m.ParameterList.OpenParenToken.ToString().ToCharArray())
                                .TrimEnd(m.ParameterList.CloseParenToken.ToString().ToCharArray());
                            var methodModel = new MethodModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = m.SpanStart,
                                Identifier = m.Identifier.Text,
                                Modifier = m.Modifiers.ToString(),

                                Public = m.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                                Protected = m.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                                Private = m.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                                Internal = m.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                                Partial = m.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                                Static = m.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                                New = m.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                                Abstract = m.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                                Override = m.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                                ReturnType = m.ReturnType.ToString(),
                                //Parameter = m.ParameterList.ToString(),
                                Parameter = p,
                                TypeParameter = m.TypeParameterList == null ? string.Empty : m.TypeParameterList.ToString(),
                                Body = m.Body == null ? string.Empty : m.Body.ToString(),
                                Attributes = GetAttribute(m.AttributeLists),
                                Comments = GetComments(m.GetLeadingTrivia())
                            };

                            classModel.Methods.Add(methodModel);

                            foreach (var pl in m.ParameterList.Parameters)
                            {
                                methodModel.Parameters.Add(new ParameterModel
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    SpanStart = pl.SpanStart,
                                    Type = pl.Type.ToString(),
                                    Identifier = pl.Identifier.Text,
                                    Modifier = pl.Modifiers.ToString()
                                });
                            }
                        }
                    }

                    documentModel.Classes.Add(classModel);
                }
                
                // [TYPE.INTERFACE]
                var iw = new InterfaceWalker();
                iw.Visit(root);

                foreach (var i in iw.Interfaces)
                {
                    string b = i.BaseList == null ? string.Empty : i.BaseList.ToString().TrimStart(i.BaseList.ColonToken.ToString().ToCharArray());
                    string t = i.TypeParameterList == null ? string.Empty :
                        i.TypeParameterList.ToString()
                            .TrimStart(i.TypeParameterList.LessThanToken.ToString().ToCharArray())
                            .TrimEnd(i.TypeParameterList.GreaterThanToken.ToString().ToCharArray());
                    //string constrains = string.Empty;
                    //if(i.ConstraintClauses.Count > 0)
                    //{
                    //    foreach(var el in i.ConstraintClauses)
                    //    {
                    //        constrains += el.ToString() + 
                    //    }
                    //}
                    var interfaceModel = new InterfaceModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SpanStart = i.Identifier.SpanStart,
                        Identifier = i.Identifier.Text,
                        Namespace = i.Parent as NamespaceDeclarationSyntax == null ? string.Empty : ((NamespaceDeclarationSyntax)i.Parent).Name.ToString(),
                        Modifier = i.Modifiers.ToString(),

                        Public = i.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                        Protected = i.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                        Private = i.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                        Internal = i.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                        Partial = i.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                        Static = i.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                        New = i.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                        Abstract = i.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                        Override = i.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                        //BaseList = i.BaseList == null ? string.Empty : i.BaseList.ToString(),
                        BaseList = b.TrimStart(),
                        ConstraintClauses = i.ConstraintClauses == null ? string.Empty : i.ConstraintClauses.ToString(),
                        //TypeParameter = i.TypeParameterList == null ? string.Empty : i.TypeParameterList.ToString(),
                        TypeParameter = t.TrimStart(),
                        Attributes = GetAttribute(i.AttributeLists),
                        Comments = GetComments(i.GetLeadingTrivia())
                    };

                    foreach (var el in i.Members)
                    {
                        if (el is ConstructorDeclarationSyntax cd)
                        {
                            var construcModel = new ConstructModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = cd.SpanStart,
                                Identifier = cd.Identifier.Text,
                                Modifier = cd.Modifiers.ToString(),

                                Public = cd.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                                Protected = cd.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                                Private = cd.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                                Internal = cd.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                                Partial = cd.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                                Static = cd.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                                New = cd.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                                Abstract = cd.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                                Override = cd.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                                Initializer = cd.Initializer == null ? string.Empty : cd.Initializer.ToString(),
                                Parameter = cd.ParameterList.ToString(),
                                Body = cd.Body == null ? string.Empty : cd.Body.ToString(),
                                Attributes = GetAttribute(cd.AttributeLists),
                                Comments = GetComments(cd.GetLeadingTrivia())
                            };

                            interfaceModel.Constructs.Add(construcModel);

                            foreach (var pl in cd.ParameterList.Parameters)
                            {
                                construcModel.Parameters.Add(new ParameterModel
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    SpanStart = pl.SpanStart,
                                    Type = pl.Type.ToString(),
                                    Identifier = pl.Identifier.Text,
                                    Modifier = pl.Modifiers.ToString()
                                });
                            }
                        }

                        if (el is DestructorDeclarationSyntax dd)
                        {
                            interfaceModel.Destruct = new DestructModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = dd.SpanStart,
                                Identifier = dd.TildeToken.ToString() + dd.Identifier.Text,
                                Modifier = dd.Modifiers.ToString(),
                                Parameter = dd.ParameterList.ToString(),
                                Body = dd.Body == null ? string.Empty : dd.Body.ToString(),
                                Attributes = GetAttribute(dd.AttributeLists),
                                Comments = GetComments(dd.GetLeadingTrivia())
                            };
                        }

                        #region // Property
                        if (el is PropertyDeclarationSyntax prop)
                        {
                            var propertyModel = new PropertyModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = prop.SpanStart,
                                Identifier = prop.Identifier.Text,
                                Modifier = prop.Modifiers.ToString(),

                                Public = prop.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                                Protected = prop.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                                Private = prop.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                                Internal = prop.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                                Partial = prop.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                                Static = prop.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                                New = prop.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                                Abstract = prop.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                                Override = prop.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                                Type = prop.Type.ToString(),
                                Body = prop.AccessorList == null ? string.Empty : prop.AccessorList.ToString(),
                                Attributes = GetAttribute(prop.AttributeLists),
                                Comments = GetComments(prop.GetLeadingTrivia())
                            };

                            interfaceModel.Properties.Add(propertyModel);

                            if (prop.AccessorList != null)
                            {
                                foreach (var a in prop.AccessorList.Accessors)
                                {
                                    propertyModel.Accessors.Add(new AccessorModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        SpanStart = a.SpanStart,
                                        Keyword = a.Keyword.Text,
                                        Body = a.Body == null ? string.Empty : a.Body.ToString(),
                                        ExpressionBody = a.ExpressionBody == null ? string.Empty : a.ExpressionBody.ToString()
                                    });
                                }
                            }
                        }
                        #endregion

                        if (el is MethodDeclarationSyntax m)
                        {
                            string p = m.ParameterList.ToString()
                                .TrimStart(m.ParameterList.OpenParenToken.ToString().ToCharArray())
                                .TrimEnd(m.ParameterList.CloseParenToken.ToString().ToCharArray());
                            var methodModel = new MethodModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SpanStart = m.SpanStart,
                                Identifier = m.Identifier.Text,
                                Modifier = m.Modifiers.ToString(),

                                Public = m.Modifiers.IndexOf(SyntaxKind.PublicKeyword) > -1 ? "public" : "",
                                Protected = m.Modifiers.IndexOf(SyntaxKind.ProtectedKeyword) > -1 ? "protected" : "",
                                Private = m.Modifiers.IndexOf(SyntaxKind.PrivateKeyword) > -1 ? "private" : "",
                                Internal = m.Modifiers.IndexOf(SyntaxKind.InternalKeyword) > -1 ? "internal" : "",
                                Partial = m.Modifiers.IndexOf(SyntaxKind.PartialKeyword) > -1 ? "partial" : "",
                                Static = m.Modifiers.IndexOf(SyntaxKind.StaticKeyword) > -1 ? "static" : "",
                                New = m.Modifiers.IndexOf(SyntaxKind.NewKeyword) > -1 ? "new" : "",
                                Abstract = m.Modifiers.IndexOf(SyntaxKind.AbstractKeyword) > -1 ? "abstract" : "",
                                Override = m.Modifiers.IndexOf(SyntaxKind.OverrideKeyword) > -1 ? "override" : "",

                                ReturnType = m.ReturnType.ToString(),
                                //Parameter = m.ParameterList.ToString(),
                                Parameter = p,
                                TypeParameter = m.TypeParameterList == null ? string.Empty : m.TypeParameterList.ToString(),
                                Body = m.Body == null ? string.Empty : m.Body.ToString(),
                                Attributes = GetAttribute(m.AttributeLists),
                                Comments = GetComments(m.GetLeadingTrivia())
                            };

                            interfaceModel.Methods.Add(methodModel);

                            foreach (var pl in m.ParameterList.Parameters)
                            {
                                methodModel.Parameters.Add(new ParameterModel
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    SpanStart = pl.SpanStart,
                                    Type = pl.Type.ToString(),
                                    Identifier = pl.Identifier.Text,
                                    Modifier = pl.Modifiers.ToString()
                                });
                            }
                        }
                    }

                    documentModel.Interfaces.Add(interfaceModel);
                }



                // [TYPE.STRUCT]
                // [TYPE.ENUM]
                // [TYPE.DELEGATE]
            }

            return documentModel;
        }

        ////private ProjectModel AddProject(Project p)
        ////{
        ////    var projectModel = new ProjectModel
        ////    {
        ////        Id = p.Id.Id.ToString(),
        ////        Version = p.Version.ToString(),
        ////        Path = p.FilePath,
        ////        Name = p.Name
        ////    };

        ////    foreach (var d in p.Documents)
        ////    {
        ////        projectModel.Documents.Add(DoDocument(d));
        ////    }

        ////    return projectModel;
        ////}

        ////private DocumentModel DoDocument(Document document)
        ////{
        ////    var documentModel = new DocumentModel
        ////    {
        ////        Id = document.Id.Id.ToString(),
        ////        Path = document.FilePath,
        ////        Name = document.Name,
        ////        SourceCodeKind = document.SourceCodeKind.ToString()
        ////    };

        ////    if (document.SourceCodeKind != Microsoft.CodeAnalysis.SourceCodeKind.Regular)
        ////    {
        ////        return documentModel;
        ////    }

        ////    CSharpSyntaxTree tree = (CSharpSyntaxTree)document.GetSyntaxTreeAsync().Result;

        ////    if (tree.HasCompilationUnitRoot == true)
        ////    {
        ////        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        ////        // [USING]
        ////        var uw = new UsingWalker();
        ////        uw.Visit(root);

        ////        foreach (var u in uw.Usings)
        ////        {
        ////            documentModel.Usings.Add(new UsingModel
        ////            {
        ////                Id = Guid.NewGuid().ToString(),
        ////                SpanStart = u.SpanStart,
        ////                Text = u.Name.ToString(),
        ////                Comments = string.Empty
        ////            }); ;
        ////        }

        ////        // [TYPE.CLASS]
        ////        var cw = new ClassWalker();
        ////        cw.Visit(root);

        ////        foreach (var c in cw.Classes)
        ////        {
        ////            var classModel = new ClassModel
        ////            {
        ////                Id = Guid.NewGuid().ToString(),
        ////                SpanStart = c.Identifier.SpanStart,
        ////                Identifier = c.Identifier.Text,
        ////                Namespace = c.Parent as NamespaceDeclarationSyntax == null ? string.Empty : ((NamespaceDeclarationSyntax)c.Parent).Name.ToString(),
        ////                Modifier = c.Modifiers.ToString(),
        ////                BaseList = c.BaseList == null ? string.Empty : c.BaseList.ToString(),
        ////                TypeParameter = c.TypeParameterList == null ? string.Empty : c.TypeParameterList.ToFullString(),
        ////                Attributes = DoAttribute(c.AttributeLists),
        ////                Comments = DoComments(c.GetLeadingTrivia())
        ////            };

        ////            foreach (var el in c.Members)
        ////            {
        ////                if (el is ConstructorDeclarationSyntax cd)
        ////                {
        ////                    var construcModel = new ConstructModel
        ////                    {
        ////                        Id = Guid.NewGuid().ToString(),
        ////                        SpanStart = cd.SpanStart,
        ////                        Identifier = cd.Identifier.Text,
        ////                        Modifier = cd.Modifiers.ToString(),
        ////                        Initializer = cd.Initializer == null ? string.Empty : cd.Initializer.ToString(),
        ////                        Parameter = cd.ParameterList.ToString(),
        ////                        Body = cd.Body == null ? string.Empty : cd.Body.ToString(),
        ////                        Attributes = DoAttribute(cd.AttributeLists),
        ////                        Comments = DoComments(cd.GetLeadingTrivia())
        ////                    };

        ////                    classModel.Constructs.Add(construcModel);

        ////                    foreach (var pl in cd.ParameterList.Parameters)
        ////                    {
        ////                        construcModel.Parameters.Add(new ParameterModel
        ////                        {
        ////                            Id = Guid.NewGuid().ToString(),
        ////                            SpanStart = pl.SpanStart,
        ////                            Type = pl.Type.ToString(),
        ////                            Identifier = pl.Identifier.Text,
        ////                            Modifier = pl.Modifiers.ToString()
        ////                        });
        ////                    }
        ////                }

        ////                if (el is DestructorDeclarationSyntax dd)
        ////                {
        ////                    classModel.Destruct = new DestructModel
        ////                    {
        ////                        Id = Guid.NewGuid().ToString(),
        ////                        SpanStart = dd.SpanStart,
        ////                        Identifier = dd.TildeToken.ToString() + dd.Identifier.Text,
        ////                        Modifier = dd.Modifiers.ToString(),
        ////                        Parameter = dd.ParameterList.ToString(),
        ////                        Body = dd.Body == null ? string.Empty : dd.Body.ToString(),
        ////                        Attributes = DoAttribute(dd.AttributeLists),
        ////                        Comments = DoComments(dd.GetLeadingTrivia())
        ////                    };
        ////                }

        ////                if (el is FieldDeclarationSyntax f)
        ////                {
        ////                    classModel.Fields.Add(new FieldModel
        ////                    {
        ////                        Id = Guid.NewGuid().ToString(),
        ////                        SpanStart = f.SpanStart,
        ////                        Identifier = f.Declaration.Variables.ToString(),
        ////                        Modifier = f.Modifiers.ToString(),
        ////                        Type = f.Declaration.Type.ToString(),
        ////                        Attributes = DoAttribute(f.AttributeLists),
        ////                        Comments = DoComments(f.GetLeadingTrivia())
        ////                    });
        ////                }

        ////                if (el is PropertyDeclarationSyntax p)
        ////                {
        ////                    var propertyModel = new PropertyModel
        ////                    {
        ////                        Id = Guid.NewGuid().ToString(),
        ////                        SpanStart = p.SpanStart,
        ////                        Identifier = p.Identifier.Text,
        ////                        Modifier = p.Modifiers.ToString(),
        ////                        Type = p.Type.ToString(),
        ////                        Body = p.AccessorList == null ? string.Empty : p.AccessorList.ToString(),
        ////                        Attributes = DoAttribute(p.AttributeLists),
        ////                        Comments = DoComments(p.GetLeadingTrivia())
        ////                    };

        ////                    classModel.Properties.Add(propertyModel);

        ////                    if (p.AccessorList != null)
        ////                    {
        ////                        foreach (var a in p.AccessorList.Accessors)
        ////                        {
        ////                            propertyModel.Accessors.Add(new AccessorModel
        ////                            {
        ////                                Id = Guid.NewGuid().ToString(),
        ////                                SpanStart = a.SpanStart,
        ////                                Keyword = a.Keyword.Text,
        ////                                Body = a.Body == null ? string.Empty : a.Body.ToString(),
        ////                                ExpressionBody = a.ExpressionBody == null ? string.Empty : a.ExpressionBody.ToString()
        ////                            });
        ////                        }
        ////                    }
        ////                }

        ////                if (el is MethodDeclarationSyntax m)
        ////                {
        ////                    var methodModel = new MethodModel
        ////                    {
        ////                        Id = Guid.NewGuid().ToString(),
        ////                        SpanStart = m.SpanStart,
        ////                        Identifier = m.Identifier.Text,
        ////                        Modifier = m.Modifiers.ToString(),
        ////                        ReturnType = m.ReturnType.ToString(),
        ////                        Parameter = m.ParameterList.ToString(),
        ////                        TypeParameter = m.TypeParameterList == null ? string.Empty : m.TypeParameterList.ToFullString(),
        ////                        Body = m.Body == null ? string.Empty : m.Body.ToString(),
        ////                        Attributes = DoAttribute(m.AttributeLists),
        ////                        Comments = DoComments(m.GetLeadingTrivia())
        ////                    };

        ////                    classModel.Methods.Add(methodModel);

        ////                    foreach (var pl in m.ParameterList.Parameters)
        ////                    {
        ////                        methodModel.Parameters.Add(new ParameterModel
        ////                        {
        ////                            Id = Guid.NewGuid().ToString(),
        ////                            SpanStart = pl.SpanStart,
        ////                            Type = pl.Type.ToString(),
        ////                            Identifier = pl.Identifier.Text,
        ////                            Modifier = pl.Modifiers.ToString()
        ////                        });
        ////                    }
        ////                }
        ////            }

        ////            documentModel.Classes.Add(classModel);
        ////        }

        ////        ////// [TYPE.INTERFACE]

        ////        ////// [TYPE.STRUCT]

        ////        ////// [TYPE.ENUM]

        ////        ////// [TYPE.DELEGATE]
        ////    }

        ////    return documentModel;
        ////}

        private IList<AttributeModel> GetAttribute(SyntaxList<AttributeListSyntax> @attributes)
        {
            List<AttributeModel> result = new List<AttributeModel>();

            if (@attributes.Count == 0)
            {
                return result;
            }

            foreach (var a in @attributes)
            {
                result.Add(new AttributeModel
                {
                    Id = Guid.NewGuid().ToString(),
                    SpanStart = a.SpanStart,
                    Text = a.Attributes.ToString()
                });
            }

            return result;
        }

        private string GetComments(SyntaxTriviaList list)
        {
            HashSet<SyntaxKind> commentTypes = new HashSet<SyntaxKind>(new[] {
                SyntaxKind.SingleLineCommentTrivia,
                SyntaxKind.MultiLineCommentTrivia,
                SyntaxKind.DocumentationCommentExteriorTrivia,
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxKind.MultiLineDocumentationCommentTrivia
            });

            string result = string.Empty;

            foreach (var el in list)
            {
                if (commentTypes.Contains(el.Kind()))
                {
                    result += el.ToFullString();
                    // [TODO]
                }
            }

            return result;
        }
    }


}
