using System;
using VDS.RDF;

namespace NetRdf
{
    class Program
    {
        static void Main(string[] args)
        {
            IGraph g = new Graph();
            g.LoadFromFile("dcat-ap_2.0.0.rdf");
        }
    }
}
