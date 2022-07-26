using System;
using System.Collections.Generic;
using System.Text;

namespace OnBrick.SourceCodeDictionary.Library
{
    public enum LanguageEnum
    {
        CSharp, Java
    }

    public enum MemberTypeEnum
    {
        Namespace, Struct, Enum, Class, Interface, Array, Delegate, Ctor, Field, Property, Method, Destructor
    }

    public enum AccessibilityEnum
    {
        Public, Protected, Internal, ProtectedInternal, Private
    }

    public enum ErrorTypeEnum
    {
        Spec, Required, ValueDomain, DataRelation, Others
    }
}
