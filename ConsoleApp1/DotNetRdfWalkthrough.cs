/**
 * This is a set of unit tests that provide a simple walkthrough of dotNetRdf. 
 * Many samples are taken from or inspired by the User Guide Basic Tutorial.
 * The full website is here http://www.dotnetrdf.org/default.asp
 * 
 * This code is made available under the following license:
 * http://creativecommons.org/licenses/by/3.0/
 * 
 * Author: Graham Moore
 * gra@brightstardb.com
 */

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Update;
using VDS.RDF.Writing;
using VDS.RDF.Writing.Formatting;

namespace ConsoleApp1
{
    /// <summary>
    /// dotNetRDF is a .NET library for working with RDF data. It's strength lies in it having a very simple, easy to use API
    /// for working with RDF in memory, along with support for the latest RDF standards, and serialisation formats. It also provides 
    /// abstractions to connect to RDF data stores. While dotNetRDF does provide a persistence model in SQL Server this
    /// is not its focus.
    /// </summary>
    [TestClass]
    public class DotNetRdfWalkthrough
    {
        /// <summary>
        /// The dotNetRDF data model consists of 3 main interfaces and one concrete class: INode, Triple, IGraph and ITripleStore.
        /// INode represents a node in an RDF Graph, such as Resource (URI) or a Literal value. In the model 
        /// there are three specialised interfaces IBlankNode, IUriNode and ILiternalNode.
        /// 
        /// Triple is a model of an RDF Statement, it consists of three components a subject, predicate and object.
        /// 
        /// IGraph represents a collection of triples.
        /// 
        /// ITripleStore represents a collection of named graphs.  
        /// </summary>
        [TestMethod]
        public void CoreDataModel()
        {
            // 1. A Graph--------------------------------------------------------------------
            // We start off by needing a container for our triples so we create a new Graph.           
            IGraph g = new Graph { BaseUri = new Uri("http://www.example.org/") };
            // ------------------------------------------------------------------------------

            // 2. URI Nodes -----------------------------------------------------------------
            // Triples are composed of INode things so we create some of these aswell.
            // Create a URI Node that refers to some specific URI
            IUriNode dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));

            //Create a URI Node using a QName but we need to define a Namespace first
            g.NamespaceMap.AddNamespace("ex", UriFactory.Create("http://example.org/namespace/"));
            IUriNode demo = g.CreateUriNode("ex:demo");

            // we use the Create() method of the UriFactory class to create URIs since this takes advantage of dotNetRDF's 
            // URI interning feature to reduce memory usage and speed up equality comparisons on URIs. 
            // ------------------------------------------------------------------------------

            // 3. Blank Nodes ---------------------------------------------------------------
            // As well as URI nodes triples can also contain Blank nodes. A Blank node represents
            // an anonymous resource.

            // Create an anonymous Blank Node
            // Each call to this constructor generates a Blank Node with a new unique identifier within the Graph
            IBlankNode anon = g.CreateBlankNode();

            // Create a named Blank Node
            // Reusing the same ID results in the same Blank Node within the Graph
            // Note that if the ID refers to an automatically assigned ID that is already in use the returned
            // Blank Node will be given an alternative ID
            IBlankNode named = g.CreateBlankNode("ID");

            // name clash with auto assigned ID
            IBlankNode namedAgain = g.CreateBlankNode(anon.InternalID);
            Assert.AreEqual("ID", named.InternalID);
            Assert.AreNotEqual(anon.InternalID, namedAgain.InternalID);

            // ------------------------------------------------------------------------------

            // 4. Literal Nodes -------------------------------------------------------------
            // Literal nodes consist of a value, a datatype and a language code. The second two 
            // are optional.

            // Create a Plain Literal           
            ILiteralNode plain = g.CreateLiteralNode("some value");
            Assert.IsNull(plain.DataType);
            Assert.IsTrue(string.IsNullOrEmpty(plain.Language));
            Assert.AreEqual("some value", plain.Value);

            // Create some Language Specified Literal
            ILiteralNode hello = g.CreateLiteralNode("hello", "en");
            ILiteralNode bonjour = g.CreateLiteralNode("bonjour", "fr");

            // Create some typed Literals
            ILiteralNode number = g.CreateLiteralNode("1", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger));
            ILiteralNode tr = g.CreateLiteralNode("true", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeBoolean));

            // Literal Node equality is somewhat more complex but basically follows the following rules:
            //    If a Language Specifier is present both Nodes must have an identical Language Specifier
            //    If a Data Type URI is present both Nodes must have an identical Data Type URI
            //    The String value of the Literal must match on a character by character basis (using Ordinal comparison)

            ILiteralNode one1 = g.CreateLiteralNode("1", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger));
            ILiteralNode one2 = g.CreateLiteralNode("0001", UriFactory.Create(XmlSpecsHelper.XmlSchemaDataTypeInteger));

            // Use Options.LiteralEqualityMode = LiteralEqualityMode.Loose; to perform equality based on typed values 
            // rather than the string representation. This is a global flag and has overhead so should be used sparingly.

            Assert.IsFalse(one1.Equals(one2));
            Options.LiteralEqualityMode = LiteralEqualityMode.Loose;
            Assert.IsTrue(one1.Equals(one2));
            // ------------------------------------------------------------------------------

            // 5. Triple --------------------------------------------------------------------
            g = new Graph();

            // Create some Nodes
            dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));
            IUriNode createdBy = g.CreateUriNode(UriFactory.Create("http://example.org/createdBy"));
            ILiteralNode robVesse = g.CreateLiteralNode("Rob Vesse");

            // Assert this Triple
            Triple t = new Triple(dotNetRDF, createdBy, robVesse);
            g.Assert(t);
            Assert.AreEqual(1, g.Triples.Count);
            // ------------------------------------------------------------------------------
        }

        /// <summary>
        /// To recap on the model we will do a simple hello world example. This 
        /// example creates a few triples, prints them to the console and then
        /// writes the data out in some well known RDF formats.
        /// </summary>
        [TestMethod]
        public void HelloWorld()
        {
            //Fill in the code shown on this page here to build your hello world application
            Graph g = new Graph();

            IUriNode dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));
            IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
            ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
            ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");

            g.Assert(new Triple(dotNetRDF, says, helloWorld));
            g.Assert(new Triple(dotNetRDF, says, bonjourMonde));

            Console.WriteLine();
            Console.WriteLine("Raw Output");
            Console.WriteLine();
            foreach (Triple t in g.Triples)
            {
                Console.WriteLine(t.ToString());
            }

            // RDF is written out using one of the Writer types.
            // Use the Save method on the graph to serialise the triples in
            // the specified format to the provided write.
            Console.WriteLine();
            Console.WriteLine("NTriples");
            Console.WriteLine();
            NTriplesWriter ntwriter = new NTriplesWriter();
            var sw = new System.IO.StringWriter();
            ntwriter.Save(g, sw);
            Console.WriteLine(sw.ToString());

            Console.WriteLine();
            Console.WriteLine("RDF XML");
            Console.WriteLine();
            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            sw = new System.IO.StringWriter();
            rdfxmlwriter.Save(g, sw);
            Console.WriteLine(sw.ToString());

            // view the Test Results output to see the different serialisations.
        }

        /// <summary>
        /// Graphs are container for triples. Triples can be added, removed and looked up.
        /// </summary>
        [TestMethod]
        public void WorkingWithGraphs()
        {
            // 1. Insert Triples ------------------------------------------------------------
            Graph g = new Graph();

            IUriNode dotNetRDF = g.CreateUriNode(UriFactory.Create("http://www.dotnetrdf.org"));
            IUriNode says = g.CreateUriNode(UriFactory.Create("http://example.org/says"));
            IUriNode label = g.CreateUriNode(UriFactory.Create("http://example.org/label"));

            ILiteralNode dotNetRdfName = g.CreateLiteralNode("dotNetRdf");
            ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
            ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");

            g.Assert(new Triple(dotNetRDF, says, helloWorld));
            g.Assert(new Triple(dotNetRDF, says, bonjourMonde));
            g.Assert(new Triple(dotNetRDF, label, dotNetRdfName));

            Assert.AreEqual(3, g.Triples.Count);

            // asserting an existing triple will not add a new one
            g.Assert(new Triple(dotNetRDF, says, bonjourMonde));
            Assert.AreEqual(3, g.Triples.Count);

            // We can also create a triple by just passing in the subject, predicate and object
            g.Assert(dotNetRDF, says, g.CreateLiteralNode("Welcome"));

            Assert.AreEqual(4, g.Triples.Count);
            // ------------------------------------------------------------------------------


            // 2. Remove Triples ------------------------------------------------------------
            // Triple removal is done BY VALUE. 
            g.Retract(dotNetRDF, says, g.CreateLiteralNode("Welcome"));

            Assert.AreEqual(3, g.Triples.Count);

            // There is no notion of updating a triple, they can be removed and added. That's it.
            // e.g. we can't update a Node's value.
            // says.Uri = new Uri(""); // no setter for Uri

            // ------------------------------------------------------------------------------

            // 3. Lookup Triples ------------------------------------------------------------
            // We quite often need to find a set of triples to display, delete, process.

            // get all triples with a given predicate
            var matches = g.GetTriplesWithPredicate(says);
            Assert.AreEqual(2, matches.Count());

            // get all the triples whose subject matches a given INode
            matches = g.GetTriplesWithSubject(dotNetRDF);
            Assert.AreEqual(3, matches.Count());

            // get all the triples whose subject and predicate matches the given INodes
            matches = g.GetTriplesWithSubjectPredicate(dotNetRDF, says);
            Assert.AreEqual(2, matches.Count());

            // Note: Nodes have no identity beyond their value.
            var n = g.CreateLiteralNode("Hello World");
            Assert.AreNotSame(n, helloWorld);
            matches = g.GetTriplesWithObject(n);
            Assert.AreEqual(1, matches.Count());

            // ------------------------------------------------------------------------------

            // 4. Loading Data --------------------------------------------------------------
            // RDF data comes in many different syntaxes. Data can be loaded into graphs by using 
            // the appropriate parser. Many different syntaxes are supported.
            g = new Graph();
            NTriplesParser parser = new NTriplesParser();
            parser.Load(g, new StringReader("<http://example.org/a> <http://example.org/b> <http://example.org/c>."));

            Assert.AreEqual(1, g.Triples.Count);
            // ------------------------------------------------------------------------------

            // 5. Loading Data from SPARQL --------------------------------------------------
            // RDF data can also be loaded into a graph from the result of a SPARQL query that returns RDF XML.
            // DESCRIBE or CONSTRUCT return RDF.
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"));

            // Ask DBPedia to describe the first thing it finds which is a Person
            var query = "DESCRIBE ?person WHERE {?person a <http://dbpedia.org/ontology/Person>} LIMIT 1";

            //Get the result
            var dbpGraph = endpoint.QueryWithResultGraph(query);

            Assert.IsTrue(dbpGraph.Triples.Count > 0);
            // ------------------------------------------------------------------------------


            // 6. Writing Data --------------------------------------------------------------
            // Again there are writers for different syntaxes. The data can be written to files,
            // streams, strings as needed.

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(dbpGraph, Console.Out); // view test result details for output.

            // ------------------------------------------------------------------------------
        }

        /// <summary>
        /// Looking up triples with the simple graph methods becomes impractical once
        /// any level of complexity is reached. RDF has a query language called SPARQL
        /// that can be used to find data in a graph. 
        /// </summary>
        [TestMethod]
        public void SparqlQuery()
        {
            TripleStore store = new TripleStore();
            var g = new Graph();

            var parser = new NTriplesParser();
            parser.Load(g, new StringReader(
@"<http://example.org/a> <http://example.org/b> <http://example.org/c>.
  <http://example.org/a> <http://example.org/b> <http://example.org/d>.
  <http://example.org/a> <http://example.org/b> <http://example.org/e>.
  <http://example.org/d> <http://example.org/f> <http://example.org/g>."));

            store.Add(g);

            // Normal SPARQL results ARE NOT rdf data. But a rows of bindings.
            Object results = store.ExecuteQuery("SELECT * WHERE {?s ?p ?o}");
            if (results is SparqlResultSet)
            {
                var rset = (SparqlResultSet)results;
                Assert.AreEqual(4, rset.Count);

                foreach (SparqlResult result in rset)
                {
                    Console.WriteLine(result.ToString());
                }
            }

            // SPARQL can be used to construct RDF as a result of the query. This can be loaded into a graph
            SparqlQueryParser sparqlparser = new SparqlQueryParser();
            SparqlQuery query = sparqlparser.ParseFromString("CONSTRUCT { ?s ?p ?o } WHERE {?s ?p ?o}");
            results = store.ExecuteQuery(query);
            if (results is IGraph)
            {
                IGraph gr = (IGraph)results;
                foreach (Triple t in gr.Triples)
                {
                    Console.WriteLine(t.ToString());
                }
            }

            // SPARQL works by matching patterns
            results = store.ExecuteQuery("SELECT * WHERE {<http://example.org/a> ?p ?o}");
            if (results is SparqlResultSet)
            {
                var rset = (SparqlResultSet)results;
                Assert.AreEqual(3, rset.Count);

                foreach (SparqlResult result in rset)
                {
                    Console.WriteLine(result.ToString());
                }
            }
        }

        /// <summary>
        /// The Graph API with Retract and Assert is OK for small changes but is not very sophisticated
        /// and requires a lot of code to achieve things. SPARQL UPDATE provides for batch updates 
        /// to a graph based on query patterns.
        /// </summary>
        [TestMethod]
        public void SparqlUpdate()
        {
            TripleStore store = new TripleStore();
            SparqlUpdateParser parser = new SparqlUpdateParser();

            //Generate a Command
            SparqlParameterizedString cmdString = new SparqlParameterizedString();
            cmdString.CommandText = "LOAD <http://dbpedia.org/resource/Southampton> INTO <http://example.org/Soton>";

            //Parse the command into a SparqlUpdateCommandSet
            SparqlUpdateCommandSet cmds = parser.ParseFromString(cmdString);

            //Create an Update Processor using our dataset and apply the updates
            LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(store);
            processor.ProcessCommandSet(cmds);

            //We should now have a Graph in our dataset as a result of the LOAD update
            //So we'll retrieve this and print it to the Console
            IGraph g = store.Graphs[new Uri("http://example.org/Soton")];
            NTriplesFormatter formatter = new NTriplesFormatter();
            foreach (Triple t in g.Triples)
            {
                Console.WriteLine(t.ToString(formatter));
            }
        }

    }
}
