using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using System;
using System.IO;

namespace JsonSCH2Cs
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = File.ReadAllText("dcat-ap_2.0.0 (1).json");
            var schemaFromFile = JsonSchema.FromSampleJson(json);
            var classGenerator = new CSharpGenerator(schemaFromFile, new CSharpGeneratorSettings
            {
                ClassStyle = CSharpClassStyle.Poco,
            });
            var codeFile = classGenerator.GenerateFile();
            File.WriteAllText("DcatAp.cs", codeFile);
        }
    }
}
