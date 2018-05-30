using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.PetriNet
{
    public enum ConnectorDirection
    {
        PlTr = 0,
        TrPl = 1
    }


    public class Connector
    {
        #region Contruction

        public Connector(string id, ConnectorDirection direction, Transition trans, Place pla)
        {
            Id = new Guid(id);
            Direction = direction;
            Trans = trans;
            Pla = pla;
        }

        public Connector(ConnectorDirection direction, Transition trans = null, Place pla = null)
        {
            Id = Guid.NewGuid();
            Direction = direction;
            Trans = trans ?? new Transition();
            Pla = pla ?? new Place();
        }

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public Transition Trans { get; set; }

        public Place Pla { get; set; }

        public ConnectorDirection Direction { get; set; }

        #endregion

        #region Methods

        public bool IsEqual(Connector cn)
        {
            if (cn.Id == Id)
                return true;

            return false;
        }

        #endregion
    }
}
