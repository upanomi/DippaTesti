using DataGeneraattori.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.DirectedGraph
{
    public class DgGenerator
    {
        #region Construction

        public DgGenerator(DbConnector dbConn)
        {
            DbConn = dbConn;

            Nodes = new List<Node>();
            Transactions = new List<Transaction>();
            Edges = new List<Edge>();
            Items = new List<Item>();
        }

        #endregion

        #region Properties

        private DbConnector DbConn { get; set; }

        private List<Node> Nodes { get; set; }
        private List<Transaction> Transactions { get; set; }
        private List<Edge> Edges { get; set; }
        private List<Item> Items { get; set; }

        #endregion

        #region Methods

       public bool NewItem(int Number)
        {
            var it = new Item(Number.ToString(), Nodes.FirstOrDefault(n => n.Name.Contains("0")));
            Items.Add(it);

            return DbConn.Dg_InsertNewItem(it);
        }

        private bool NewTransaction(Transaction tr, Item it)
        {
            return DbConn.Dg_InsertTransaction(tr, FindEdge(tr), it);
        }

        private Edge FindEdge(Transaction tr)
        {
            var res = Edges.Find(e => e.Start.Id == tr.StartNode.Id && e.End.Id == tr.EndNode.Id);
            if(res == null)
            {
                res = new Edge(tr.StartNode, tr.EndNode);
                Edges.Add(res);
                DbConn.Dg_InsertNewEdge(res);
            }

            return res;
        }

        #endregion
    }
}
