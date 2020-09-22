/*using System;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace NetRdf
{
    class Program
    {
        static void Main(string[] args)
        {
            // For simple queries:
            //First we need an instance of the SparqlQueryParser
            SparqlQueryParser parser = new SparqlQueryParser();

            //For complex queries
            //Create a Parameterized String
            SparqlParameterizedString queryString = new SparqlParameterizedString();

            //Add a namespace declaration
            queryString.Namespaces.AddNamespace("ex", new Uri("http://www.w3.org/ns/dcat#"));

            //Set the SPARQL command
            //For more complex queries we can do this in multiple lines by using += on the
            //CommandText property
            //Note we can use @name style parameters here
            queryString.CommandText = "SELECT * WHERE { ?s ex:property @value }";


            //Inject a Value for the parameter
            queryString.SetUri("value", new Uri("http://example.org/value"));

            //When we call ToString() we get the full command text with namespaces appended as PREFIX
            //declarations and any parameters replaced with their declared values
            Console.WriteLine(queryString.ToString());

            //We can turn this into a query by parsing it as in our previous example
            SparqlQueryParser p = new SparqlQueryParser();
            SparqlQuery query = p.ParseFromString(queryString);





            *//*            IGraph g = new Graph();
                        g.LoadFromFile("dcat-ap_2.0.1.rdf");

                        foreach (Triple t in g.Triples)
                        {
                            Console.WriteLine(t.Predicate);
                            Console.WriteLine(t.Subject);
                            Console.WriteLine(t.Object);
                        }*/



/*            TurtleParser turtleParser = new TurtleParser();
            Graph graph = new Graph();

            graph.LoadFromFile("dcat-ap_2.0.1.rdf");
            var ds = graph.GetUriNode("dct:title");*//*

        }
    }
}
*/