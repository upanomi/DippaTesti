using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori
{
    public class Order
    {

        #region Construction

        public Order() { }

        #endregion

        #region Properties

        public long OrderNumber { get; set; }

        public int BatchSize { get; set; }

        public DateTime CreateTimestamp { get; set; }

        public DateTime UpdateTimeStamp { get; set; }

        public int Priority { get; set; }

        public string ProductionLine { get; set; }

        public DateTime DueDate { get; set; }

        public int OrderStatus { get; set; }

        public float CriticalValue { get; set; }

        public long TimeNeeded { get; set; }

        public string Comment { get; set; }

        #endregion

        #region Methods

        public string GetInsertQuery()
        {
            var res = "INSERT INTO us_work_order ";


            return res;
        }

        #endregion

    }
}
