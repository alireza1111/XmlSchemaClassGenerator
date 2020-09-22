using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace XsltStudy
{
    class Program
    {
        static void Main(string[] args)
        {
            var trans = new XslCompiledTransform();
            var xml = new XmlDocument();


            var assembly = Assembly.GetExecutingAssembly();
            var assemblyPath = Path.GetDirectoryName(assembly.Location);
            var assemblyName = assembly.GetName().Name;
            var namespacePath = Path.Combine(
                typeof(Program).Namespace
                    .Substring(assemblyName.Length)
                    .Split('.'));
            var xsltPath = Path.Combine(assemblyPath, namespacePath, "Books.xsl");
            var xmlPath = Path.Combine(assemblyPath, namespacePath, "Books.xml");

            trans.Load(xsltPath);
            xml.Load(xmlPath);
            
            var result = new XDocument();

            using (XmlWriter writer = result.CreateWriter())
            {
                trans.Transform(xml, writer);
            }

            Console.WriteLine(result);
        }
    }
}
