using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGeneraattori
{
    interface IFlowAnalyticsCore
    {
        #region Getters
        //Gets the average inventory size, sorted by item types (in the dict) in the given timeperiod.
        //Can be refined to include only certain buffers or itemtypes
        Dictionary<Guid, List<float>> GetAverageInventory(DateTime startTime, DateTime endTime, string type = "",
            List<Guid> buffers = null, TimeResolution resolution = 0);

        //Calculates the amount of items scrapped in a timespan
        //Can be refined to include only certain nodes after which the scrapping has been done, or the item type
        float GetScrapPercentage(DateTime startTime, DateTime endTime, string type = "", List<Guid> nodes = null);

        //Calculates the average time a item has been in the whole system. Can be fetched for a single item or for specific itemtypes or orders
        //Can be refined with the nodes between which the calculation takes place, and by the timeslot
        long GetThroughputTimeForSingleUnit(string serialNumber, Guid startNode = new Guid(),
            Guid endNode = new Guid());

        Dictionary<Guid, List<long>> GetThroughputTImeForMultipleItems(List<string> itemTypes, DateTime startTime,
            DateTime endTime, Guid startNode = new Guid(), Guid endNode = new Guid());

        //Calculating how much time a item (or all items of a type on average) have spent in a ValueAddingNode (being machined etc)
        //Is returned as a dictionary where the time is presenteded individually for the nodes
        Dictionary<Guid, long> GetProcessTimeSingleUnit(string serialNumber);

        Dictionary<Guid, Dictionary<Guid, long>> GetProcessTimeForMultipleItems(List<string> type, DateTime startTime, DateTime endTime);

        //Caculating the time a item or items of type on average, have spent in a buffer
        long GetBufferTimeSingleItem(string serialNumber);

        Dictionary<Guid, List<long>> GetBufferTimeMultipleItems(DateTime startTime, DateTime endTime, string type);


        //Calculates the amount of items that have been sent out in a given timespan. They can be divided into sections,
        //according to the resolution
        Dictionary<Guid, List<int>> GetOutputRate(DateTime startTime, DateTime endTime, TimeResolution resolution = 0,
            string type = "");
        #endregion

        #region Configs
        
        bool UpdateNode(Node updatedNode);

        bool DeleteNode(Node toDelNode);



        #endregion
    }
}
