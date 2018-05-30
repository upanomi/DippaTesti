using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.DirectedGraph
{
    public class Item
    {

        #region Construction

        public Item() :
            this(string.Empty, new Node())
        { }

        public Item(string name, Node location) :
            this(Guid.NewGuid().ToString(), name, location)
        { }

        public Item(string id, string name, Node location)
        {
            Id = new Guid(id);
            Name = name;
            Location = location;
        }

        #endregion

        #region Properties

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Node Location { get; set; }

        #endregion

        #region Methods
        #endregion

    }
}
