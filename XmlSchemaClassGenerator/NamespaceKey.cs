﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlSchemaClassGenerator
{
    public class NamespaceKey : IComparable<NamespaceKey>, IEquatable<NamespaceKey>, IComparable
    {
        private const UriComponents CompareComponents = UriComponents.Host | UriComponents.Scheme | UriComponents.Path;
        private const UriFormat CompareFormat = UriFormat.Unescaped;

        public string XmlSchemaNamespace { get; private set; }
        public Uri Source { get; private set; }

        public NamespaceKey(Uri source, string xmlSchemaNamespace)
        {
            Source = source;
            XmlSchemaNamespace = xmlSchemaNamespace;
        }

        public bool Equals(NamespaceKey other)
        {
            return CompareTo(other) == 0;
        }

        public int CompareTo(NamespaceKey other)
        {
            if (Source == null && other.Source != null)
                return -1;
            if (Source != null && other.Source == null)
                return 1;
            int result;
            if (Source != null)
            {
                result = Uri.Compare(Source, other.Source, CompareComponents, CompareFormat,
                    StringComparison.OrdinalIgnoreCase);
                if (result != 0)
                    return result;
            }
            result = string.Compare(XmlSchemaNamespace, other.XmlSchemaNamespace, StringComparison.Ordinal);
            return result;
        }

        public override bool Equals(object obj)
        {
            return Equals((NamespaceKey)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (Source != null)
                hashCode = Source.GetComponents(CompareComponents, CompareFormat).ToLower().GetHashCode();
            return hashCode
                   ^ XmlSchemaNamespace.GetHashCode();
        }

        int IComparable.CompareTo(object obj)
        {
            return CompareTo((NamespaceKey)obj);
        }
    }
}