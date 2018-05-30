using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.PetriNet
{
    public class Transition
    {
        #region Contruction

        public Transition(string id, string name)
        {
            Id = new Guid(id);
            Name = name;
        }

        public Transition(string name = "")
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public string Name { get; set; }

        #endregion

        #region Methods

        public bool IsEqual(Transition tr)
        {
            if (tr.Id == Id)
                return true;

            return false;
        }

        #endregion
    }
}
