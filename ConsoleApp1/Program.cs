using System;
using VDS.RDF;
using VDS.RDF.Writing;

namespace ConsoleApp1
{
    class Program
    {
        public static void Main(String[] args)
        {
            //Fill in the code shown on this page here to build your hello world application
            Graph g = new Graph();

            IUriNode dotNetRDF = g.CreateUriNode(new Uri("http://www.dotnetrdf.org"));
            IUriNode says = g.CreateUriNode(new Uri("http://example.org/says"));
            ILiteralNode helloWorld = g.CreateLiteralNode("Hello World");
            ILiteralNode bonjourMonde = g.CreateLiteralNode("Bonjour tout le Monde", "fr");

            g.Assert(new Triple(dotNetRDF, says, helloWorld));
            g.Assert(new Triple(dotNetRDF, says, bonjourMonde));

            foreach (Triple t in g.Triples)
            {
                Console.WriteLine(t.ToString());
            }

/*            NTriplesWriter ntwriter = new NTriplesWriter();
            ntwriter.Save(g, "HelloWorld.nt");*/

            RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
            rdfxmlwriter.Save(g, "HelloWorld.rdf");

        }

            protected void addRDFTriple()
    {
        var list = (List<string[]>)Session["PersonalList"];

        int numberOfRows = 0;

        if (list.Count() > 0)
            numberOfRows = int.Parse(list[list.Count() - 1][0].ToString().Split('/')[6].ToString());

        for (int cv01 = 0; cv01 < list.Count; cv01++)
        {
            if (numberOfRows < int.Parse(list[cv01][0].ToString().Split('/')[6].ToString()))
            {
                numberOfRows = int.Parse(list[cv01][0].ToString().Split('/')[6].ToString());

            }
        }

        //System.Diagnostics.Debug.WriteLine(list.Count() + " " + numberOfRows);

        //int numberOfRows = GridView2.Rows.Count;
        numberOfRows++;
        String dataid = numberOfRows.ToString();
                    Session["Sdataid"] = dataid;
                    Graph g = (Graph)Session["ggraph"];
            var g = new Graph();

        IUriNode personID = g.CreateUriNode(UriFactory.Create("http://www.phonebook.co.uk/Person/" + numberOfRows));
        IUriNode Title = g.CreateUriNode(UriFactory.Create("http://xmlns.com/foaf/0.1/Title"));
        IUriNode Name = g.CreateUriNode(UriFactory.Create("http://xmlns.com/foaf/0.1/Name"));
        IUriNode Surname = g.CreateUriNode(UriFactory.Create("http://xmlns.com/foaf/0.1/Surname"));
        IUriNode Email = g.CreateUriNode(UriFactory.Create("http://xmlns.com/foaf/0.1/Email"));
        IUriNode Phone = g.CreateUriNode(UriFactory.Create("http://xmlns.com/foaf/0.1/Phone"));
        IUriNode Comments = g.CreateUriNode(UriFactory.Create("http://xmlns.com/foaf/0.1/Comments"));

        ILiteralNode NTitle = g.CreateLiteralNode(Session["Stitle"].ToString());
        ILiteralNode NName = g.CreateLiteralNode(Session["Sname"].ToString());
        ILiteralNode NSurname = g.CreateLiteralNode(Session["Ssurname"].ToString());
        ILiteralNode NEmail = g.CreateLiteralNode(Session["Semail"].ToString().Split('>')[1].Split('<')[0].ToString());
        ILiteralNode NPhone = g.CreateLiteralNode(Session["Snumber"].ToString());
        ILiteralNode NComments = g.CreateLiteralNode(" ");

        g.Assert(personID, Title, NTitle);
        g.Assert(personID, Name, NName);
        g.Assert(personID, Surname, NSurname);
        g.Assert(personID, Email, NEmail);
        g.Assert(personID, Phone, NPhone);
        g.Assert(personID, Comments, NComments);

        RdfXmlWriter rdfxmlwriter = new RdfXmlWriter();
        rdfxmlwriter.Save(g, "C:/Users/panayiotis/master/MSC PROJECT/db/" + Session["UserId"].ToString() + ".rdf");
    }
    }
}
