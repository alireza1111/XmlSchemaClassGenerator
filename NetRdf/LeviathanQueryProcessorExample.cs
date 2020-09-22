using System;
using System.Diagnostics;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Writing.Formatting;

namespace JsonSCH2Cs
{
    public class LeviathanQueryProcessorExample
    {
		public static void Main(String[] args)
		{
			TripleStore store = new TripleStore();

			Graph g = new Graph();
			 g.LoadFromFile("sample.rdf");
			g.BaseUri = null;
			store.Add(g);

			//Assume that we fill our Store with data from somewhere

			//Create a dataset for our queries to operate over
			//We need to explicitly state our default graph or the unnamed graph is used
			//Alternatively you can set the second parameter to true to use the union of all graphs
			//as the default graph
			InMemoryDataset ds = new InMemoryDataset(store, true);

			//Get the Query processor
			ISparqlQueryProcessor processor = new LeviathanQueryProcessor(ds);

			//Use the SparqlQueryParser to give us a SparqlQuery object
			//Should get a Graph back from a CONSTRUCT query
			SparqlQueryParser sparqlparser = new SparqlQueryParser();
			SparqlQuery query = sparqlparser.ParseFromString("CONSTRUCT { ?s ?p ?o } WHERE {?s ?p ?o}");
			var results = processor.ProcessQuery(query);
			if (results is IGraph)
			{
				//Print out the Results
				IGraph ig = (IGraph)results;
				NTriplesFormatter formatter = new NTriplesFormatter();
				foreach (Triple t in ig.Triples)
				{
					Console.WriteLine(t.ToString(formatter));
				}
				Console.WriteLine("Query took " + query.QueryExecutionTime.Value.TotalMilliseconds + " milliseconds");
			}
		}
	}
}
