using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori.PetriNet
{
    public class Token
    {
        #region Contruction

        public Token(string id, string name, Place location = null)
        {
            Id = new Guid(id);
            Name = name;
            Location = location ?? new Place();
        }

        public Token(string name = "", Place location = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location ?? new Place();
        }

        #endregion

        #region Properties

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Place Location { get; set; }

        #endregion

        #region Methods

        public bool IsEqual(Token tk)
        {
            if (tk.Id == Id)
                return true;

            return false;
        }

        #endregion
    }
}
