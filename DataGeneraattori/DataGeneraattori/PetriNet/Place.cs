using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.PetriNet
{
    public class Place
    {
        #region Contruction

        public Place(string id, string name, int capacity)
        {
            Id = new Guid(id);
            Name = name;
            Capacity = capacity;
        }

        public Place(string name = "", int capacity = 0)
        {
            Id = Guid.NewGuid();
            Name = name;
            Capacity = capacity;
        }

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        #endregion

        #region Methods

        public bool IsEqual(Place pl)
        {
            if (pl.Id == Id)
                return true;

            return false;
        }

        #endregion
    }
}
