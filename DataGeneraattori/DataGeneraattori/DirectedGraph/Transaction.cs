using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.DirectedGraph
{
    public class Transaction
    {
        #region Construction

        public Transaction(long order, long numId = 0)
        {
            //Id = new Guid();
            Id = Guid.NewGuid();
            Order = order;
            NumId = numId;
        }

        public Transaction(Node startNode, DateTimeOffset startTime, long order, long numId = 0)
            :this(order, numId)
        {
            StartNode = startNode;
            StartTime = startTime;
            Type = StartNode.Type;
        }

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public long NumId { get; set; }

        public Node StartNode { get; set; }

        public Node EndNode { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public TimeSpan Duration { get; set; }

        public long Order { get; set; }

        public NodeType Type { get; set; }

        #endregion

        #region Methods

        public string GetString()
        {
            var res = "";
            if(EndNode != null)
                res = String.Format("Transaction: {0}, from: {1}, to: {2}", NumId.ToString(), StartNode.Name, EndNode.Name);
            else
                res = String.Format("Transaction: {0}, from: {1}, to: {2}", NumId.ToString(), StartNode.Name, "");
            return res;
        }

        #endregion

        #region Helpers

        #endregion
    }
}
