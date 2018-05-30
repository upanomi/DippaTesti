using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.DirectedGraph
{

    public enum NodeType
    {
        CheckPoint = 1,
        ValueAdding = 2,
        Buffer = 3,
        Exit = 4
    }

    public class Node
    {

        #region Construction

        public Node()
        {
            Id = Guid.NewGuid();
        }

        public Node(string name, NodeType type, int capacity = 0, List<Node> nexts = null) :
            this()
        {
            Name = name;
            Type = type;
            Capacity = capacity;
            if (nexts == null)
            {
                NextNodes = new List<Node>();
            }
            else
            {
                NextNodes = nexts;
            }
        }

        public Node(string name, int type, int capacity = 0, List<Node> nexts = null)
        {
            Name = name;
            Capacity = capacity;
            
            if(nexts == null)
            {
                NextNodes = new List<Node>();
            }
            else
            {
                NextNodes = nexts;
            }

            switch (type)
            {
                case 1:
                    Type = NodeType.CheckPoint;
                    break;
                case 2:
                    Type = NodeType.ValueAdding;
                    break;
                case 3:
                    Type = NodeType.Buffer;
                    break;
                case 4:
                    Type = NodeType.Exit;
                    break;
            }

        }

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public string Name { get; set; }

        public NodeType Type { get; set; }

        public int Capacity { get; set; }

        public int Group { get; set; }

        public List<Node> NextNodes { get; set; }

        #endregion

        #region Methods

        #region Updating

        public bool Update(Node updated)
        {


            return true;
        }

        #endregion

        #region Speculating

        //Calculates the transaction to the next node
        public Transaction CalculateNextNode(ref Transaction trans, int min, int max)
        {
            var rnd = new Random();
            trans.Duration = TimeSpan.FromMinutes(rnd.Next(min, max));
            trans.EndTime = trans.StartTime + trans.Duration;

            int target;

            if(NextNodes.Count > 0)
            {
                target = rnd.Next(0, NextNodes.Count - 1);
                trans.EndNode = NextNodes.ElementAt(target);
            }
            else
            {
                return null;
            }
            //Returning a new transaction starting from the end of the previous node
            return new Transaction(trans.EndNode, trans.EndTime, trans.Order);
        }

        //Randomly chooses a node from the list of next nodes
        public Node SpeculateNextNode()
        {
            if (NextNodes == null || NextNodes.Count <= 0)
                return null;

            var rnd = new Random();
            int target = rnd.Next(0, NextNodes.Count - 1);
            return NextNodes.ElementAt(target);
        }

        #endregion

        #endregion

    }
}
