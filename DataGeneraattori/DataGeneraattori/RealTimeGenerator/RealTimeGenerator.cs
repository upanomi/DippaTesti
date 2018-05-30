using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using DataGeneraattori.DataBase;

namespace DataGeneraattori.RealTime
{
    public class RealTimeGenerator
    {

        #region Const values

        private const int MaxMinutes = 45;
        private const int MinMinutes = 10;

        #endregion

        #region Construction

        public RealTimeGenerator(List<Node> nodes = null, List<long> orders = null)
        {
            Nodes = nodes ?? new List<Node>();
            Orders = orders ?? new List<long>();
            Running = false;
            Interval = new System.Timers.Timer();
            Rnd = new Random();

            Interval.Elapsed += StartItem;
        }

        #endregion

        #region Properties

        private List<Node> Nodes { get; set; }
        private List<long> Orders { get; set; }
        private bool Running { get; set; }
        private System.Timers.Timer Interval { get; set; }
        private Random Rnd { get; set; }
        private DbConnector DbConn { get; set; }

        #endregion

        #region Public methods

        public void Start()
        {
            Running = true;
            MainCycle();
        }

        public void Stop()
        {
            Running = false;
        }

        #endregion

        #region Private methods

        #region MainFrame

        private void MainCycle()
        {
            if (!Running) return;

            //Interval.Interval = Rnd.Next(MinMinutes, MaxMinutes) * 60000;
            Interval.Interval = Rnd.Next(MinMinutes, MaxMinutes) * 1000;
            Interval.Start();
        }

        private void StartItem(object obj, ElapsedEventArgs args)
        {
            if (Running)
            {
                //Interval.Interval = Rnd.Next(MinMinutes, MaxMinutes) * 60000;
                Interval.Interval = Rnd.Next(MinMinutes, MaxMinutes) * 1000;
            }
            else
            {
                Interval.Stop();
                return;
            }

            //ItemThreads.Add(new Thread(new ThreadStart(StartOrder)));

            var lanka = new Thread(EmulateOrder);
            lanka.Start();
        }



        #endregion

        #region SingleItem

        private void EmulateOrder()
        {
            Node startNode = Nodes.FirstOrDefault(n => n.Name == "1");

            var mins = Rnd.Next(1, 15);
            //TimeSpan delay = TimeSpan.FromMinutes(mins);
            TimeSpan delay = TimeSpan.FromSeconds(mins); //For testing

            Thread.Sleep(delay);


            //var ord = Rnd.Next(0, Orders.Count - 1);

            //var order = Orders.ElementAt(Rnd.Next(0, Orders.Count - 1));
            var order = Rnd.Next(0, 1000);

            Transaction currentTrans = new Transaction(startNode, DateTimeOffset.Now, order);
            
            NewEvent(currentTrans);

            while (Running)
            {
                mins = Rnd.Next(MinMinutes, MaxMinutes);
                //delay = TimeSpan.FromMinutes(mins);
                delay = TimeSpan.FromSeconds(mins); //For testing
                Thread.Sleep(delay);

                //The next event occurs

                currentTrans.EndNode = currentTrans.StartNode.SpeculateNextNode();
                currentTrans.EndTime = DateTimeOffset.Now;
                currentTrans.Duration = delay;
                UpdateEvent(currentTrans);

                if (currentTrans.EndNode == null || currentTrans.EndNode.NextNodes.Count <= 0)
                {
                    break;
                }

                var nextTrans = new Transaction(currentTrans.EndNode, DateTimeOffset.Now, order);
                NewEvent(nextTrans);
            }

        }

        #endregion

        #region Helpers

        private void NewEvent(Transaction trans)
        {
            Console.WriteLine("New transaction");
            Console.WriteLine(trans.GetString());
        }

        private void UpdateEvent(Transaction trans)
        {
            Console.WriteLine("Transaction is updated");
            Console.WriteLine(trans.GetString());
        }

        #endregion

        #endregion

    }
}
