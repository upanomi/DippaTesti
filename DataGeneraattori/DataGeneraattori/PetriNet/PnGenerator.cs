using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGeneraattori.DataBase;

namespace DataGeneraattori.PetriNet
{
    class PnGenerator
    {

        #region Construction

        public PnGenerator(DbConnector dbConn)
        {
            DbConn = dbConn;
            Tokens = new List<Token>();
            Events = new List<Event>();

            List<Place> tempPlaces;
            List<Transition> tempTransitions;
            List<Connector> tempConnectors;

            DbConn.InitPetriNets(out tempPlaces, out tempTransitions, out tempConnectors);

            Places = tempPlaces;
            Transitions = tempTransitions;
            Connectors = tempConnectors;
        }

        #endregion

        #region Properties

        private DbConnector DbConn { get; set; }

        private List<Event> Events { get; set; }
        private List<Place> Places { get; set; }
        private List<Transition> Transitions { get; set; }
        private List<Token> Tokens { get; set; }
        private List<Connector> Connectors { get; set; }

        #endregion

        #region Methods

        public void GenerateEventsIntoDb(int count)
        {
            //Creating an amount of items on the entry
            int indexOfFirstPlace = Places.IndexOf(Places.FirstOrDefault(p => p.Name == "p0"));
            for (var i = 0; i < count; i++)
            {
                var tk = new Token("Token_" + i.ToString(), Places.ElementAt(indexOfFirstPlace));
                Tokens.Add(tk);
                Places.ElementAt(indexOfFirstPlace).Capacity++;

                DbConn.Pn_InsertNewToken(tk);

                Event ev = GenerateNextEvent(Tokens.Last());
                //DbConn.Pn_InsertEvent(ev);

                while (ev != null)
                {
                    DbConn.Pn_InsertEvent(ev);
                    ev = GenerateNextEvent(Tokens.Last());
                }

            }


        }

        private Event GenerateNextEvent(Token tk)
        {
            Event retVal;
            var rnd = new Random();

            //To continue, or not to continue
            if (rnd.Next(1, 3) < 1 || tk.Location.Name == "p6")
                return null;

            //The place were at
            var pl = Places.FirstOrDefault(p => p.IsEqual(tk.Location));
            Places.FirstOrDefault(p => p.IsEqual(tk.Location)).Capacity--;

            //Getting the connectors out of the place
            var conns = Connectors.Where(c => c.Pla.IsEqual(pl) && c.Direction == ConnectorDirection.PlTr);
            //Drafting the connector to select
            var firstConn = conns.ElementAt(rnd.Next(0, conns.Count() - 1));
            var tr = Transitions.Single(t => t.IsEqual(firstConn.Trans));
            //Getting the connector leaving from the transition (should be single)
            var secoConn = Connectors.Single(c => c.Trans.IsEqual(tr) && c.Direction == ConnectorDirection.TrPl);

            //Increasing the capacity of the target place, and updating the location of the token
            tk.Location = Places.Single(p => p.IsEqual(secoConn.Pla));
            Places.Single(p => p.IsEqual(secoConn.Pla)).Capacity++;

            //Sending the updated data to the DB
            DbConn.Pn_UpdateToken(tk);
            DbConn.Pn_UpdateLocation(Places.Single(p => p.IsEqual(secoConn.Pla)));

            retVal = new Event(firstConn, secoConn, tk);

            return retVal;
        }

        #region Universal model methods

        public bool NewToken(int number)
        {
            var tk = new Token(number.ToString(), Places.FirstOrDefault(p => p.Name.Contains("0")));
            Tokens.Add(tk);

            return DbConn.Pn_InsertNewToken(tk);
        }

        //An item has moved to the wp in question
        public bool NewEvent(Workplace wp, int ItemNumber)
        {
            var tk = Tokens.FirstOrDefault(t => t.Name.Split('_')[1] == ItemNumber.ToString());
            var pl = Places.FirstOrDefault(p => p.Name.Contains(wp.Number.ToString()));
            var prevPl = tk.Location;

            //Update Place
            prevPl.Capacity--;
            if (!DbConn.Pn_UpdateLocation(prevPl))
                return false;
            pl.Capacity++;
            if (DbConn.Pn_UpdateLocation(pl))
                return false;

            //Update Token
            tk.Location = pl;
            if (!DbConn.Pn_UpdateToken(tk))
                return false;

            //Finding the transition and connectors between the places
            Connector first = null, second = null;
            foreach(var tr in Transitions)
            {
                var conns = Connectors.Where(c => c.Trans.IsEqual(tr));

                if(conns.Any(c => c.Pla.IsEqual(prevPl)) && conns.Any(c => c.Pla.IsEqual(pl)))
                {
                    first = conns.Single(c => c.Pla.IsEqual(prevPl));
                    second = conns.Single(c => c.Pla.IsEqual(pl));
                }
            }

            if (first == null || second == null)
                return false;

            //Create the event
            var ev = new Event(first, second, tk, DateTime.Now);
            if (!DbConn.Pn_InsertEvent(ev))
                return false;

            return true;
        }

        #endregion

        #endregion

    }
}
