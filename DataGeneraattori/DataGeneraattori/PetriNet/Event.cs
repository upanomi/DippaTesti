using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.PetriNet
{
    public class Event
    {
        #region Contruction

        public Event(Connector first, Connector second, Token item, DateTime timestamp = default(DateTime))
        {
            Id = Guid.NewGuid();
            FirstConnector = first;
            SecondConnector = second;
            Item = item;
            Timestamp = timestamp != default(DateTime) ? timestamp : DateTime.Now;
        }

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public Connector FirstConnector { get; set; }

        public Connector SecondConnector { get; set; }

        public Token Item { get; set; }

        #endregion

        #region Methods
        #endregion
    }
}
