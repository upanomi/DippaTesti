using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.DirectedGraph
{
    public class Edge
    {

        #region Construction

        //public Edge() : 
        //    this(new Node(), new Node())
        //{ }

        public Edge(Node start, Node end) :
            this(Guid.NewGuid().ToString(), start, end, 0, new TimeSpan(0,0,0))
        { }

        public Edge(string id, Node start, Node end, int count, TimeSpan avgDuration)
        {
            Id = new Guid(id);
            Start = start;
            End = end;
            Count = count;
            AvgDuration = avgDuration;
        }

        #endregion

        #region Properties

        public Guid Id { get; private set; }
        public Node Start { get; set; }
        public Node End { get; set; }
        public int Count { get; set; }
        public TimeSpan AvgDuration { get; set; }

        #endregion

        #region Methods

        public bool Update(TimeSpan newTime)
        {

            return true;
        }

        #endregion

    }
}
