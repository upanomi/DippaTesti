using DataGeneraattori.DataBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DataGeneraattori.RealTime;
using DataGeneraattori.PetriNet;
using DataGeneraattori.DirectedGraph;

namespace DataGeneraattori
{
    class Program
    {

        #region Main

        static void Main(string[] args)
        {
            //Version at Insolution
            //DbConn = new DbConnector("server=192.168.1.14,1433;database=Inspector_Joros_OP;User Id=sa;Password=yJ8N51Jx6Cy4528;connection timeout=30");

            //Local version
            //DbConn = new DbConnector("server=PÖYTÄPÖNTTÖ\\SQLEXPRESS;Trusted_Connection=yes;database=testing;User Id=test;connection timeout=30");
            DbConn = new DbConnector("Data Source=PÖYTÄPÖNTTÖ;Initial Catalog=testing;Integrated Security=True");

            

            //UnComment to generate data
            GenerateShit(100);

            //UnComment to handle existing data
            HandleShit();
        }

        static private void GenerateShit(int count)
        {
            //Dg version
            DirectedGraph = new DgGenerator(DbConn);
            //DirectedGraph.GenerateEventsIntoDb(count);

            //Pn version
            PetriNet = new PnGenerator(DbConn);
            //PetriNet.GenerateEventsIntoDb(count);

            //Generic workplaces
            Workplaces = new List<Workplace>();
            for(var i = 0; i <= 6; i++)
            {
                Workplaces.Add(new Workplace(i));
            }

            foreach(var wp in Workplaces)
            {
                switch (wp.Number)
                {
                    case 0:
                        wp.NextOnes.AddRange(Workplaces.Where(p => p.Number == 1 || p.Number == 2));
                        break;
                    case 1:
                        wp.NextOnes.AddRange(Workplaces.Where(p => p.Number == 3 || p.Number == 4));
                        break;
                    case 2:
                        wp.NextOnes.AddRange(Workplaces.Where(p => p.Number == 4 || p.Number == 5 || p.Number == 6));
                        break;
                    case 3:
                        wp.NextOnes.AddRange(Workplaces.Where(p => p.Number == 6));
                        break;
                    case 4:
                        wp.NextOnes.AddRange(Workplaces.Where(p => p.Number == 3 || p.Number == 6));
                        break;
                    case 5:
                        wp.NextOnes.AddRange(Workplaces.Where(p => p.Number == 6));
                        break;
                    case 6:
                        break;

                }
            }

            Workplaces = Workplaces.OrderBy(wp => wp.Number).ToList();

            for(var i = 0; i < count; i++)
            {
                //New Item
                PetriNet.NewToken(i);
                DirectedGraph.NewItem(i);

                var wp = Workplaces.FirstOrDefault(w => w.Number == 0);

                int nextIndex = GenerateNextMove(i, wp);

                if(nextIndex > 0)
                {
                    wp = wp.NextOnes.ElementAt(nextIndex);

                    DirectedGraph.NewEvent(wp, i);
                    PetriNet.NewEvent(wp, i);
                }
            }

        }

        private static int GenerateNextMove(int item, Workplace wp)
        {
            var rnd = new Random();

            //To continue or not
            if (rnd.Next(1, 4) > 1 || wp.Number == 6)
                return 0;

            return rnd.Next(0, wp.NextOnes.Count);
        }

        static private void HandleShit()
        {
            //Dg

            //Pn

        }

        #endregion

        #region Properties

        private static List<Node> Nodes { get; set; }
        private static List<Transaction> Transactions { get; set; }
        private static List<long> Orders { get; set; }
        private static DbConnector DbConn { get; set; }

        private static DgGenerator DirectedGraph { get; set; }
        private static PnGenerator PetriNet { get; set; }

        private static List<Workplace> Workplaces { get; set; }

        #endregion



        #region Sequence methods
        /*
        private static void GenerateNodes()
        {
            Nodes = new List<Node>
            {
                //Entry
                new Node("0", NodeType.CheckPoint), //0 in the graph

                //Machine 1
                new Node("11", NodeType.ValueAdding), //1 in the graph
                new Node("12", NodeType.Buffer),

                //Machine 2
                new Node("21", NodeType.ValueAdding), //2
                new Node("22", NodeType.Buffer),

                //Machine 3
                new Node("31", NodeType.ValueAdding), //3
                new Node("32", NodeType.Buffer),

                //Machine 4
                new Node("41", NodeType.ValueAdding), //5
                new Node("42", NodeType.Buffer),

                //Storage
                new Node("Storage", NodeType.Buffer), //4

                //Exit
                new Node("Exit", NodeType.Exit) //6 because I can
            };
        }
        */

        private static void GenerateWithOrders(long count)
        {
            Transactions = new List<Transaction>();

            var rnd = new Random();
            var yearStart = new DateTime(2018, 1, 1, 0, 0, 0);

            var startNode = Nodes.Find(n => n.Name == "33"); //0 was in my own data model. In real one, 33 is the beginning

            for (var i = 0; i < count; i++)
            {
                //Selecting the order to link this item to
                var orderIndex = rnd.Next(0, Orders.Count - 1);


                //Starting point for the current item
                var startTime = yearStart + TimeSpan.FromHours(rnd.Next(0, 720));
                var trans = new Transaction(startNode, startTime, Orders.ElementAt(orderIndex));

                //The first move
                //var nextTrans = CalculateNextNode(ref trans);

                var nextTrans = startNode.CalculateNextNode(ref trans, 15, 60);

                Transactions.Add(trans);

                trans = nextTrans;

                while (trans != null)
                {
                    //nextTrans = CalculateNextNode(ref trans);
                    nextTrans = trans.StartNode.CalculateNextNode(ref trans, 15, 60);

                    if (nextTrans == null)
                    {
                        break;
                    }

                    Transactions.Add(trans);
                    trans = nextTrans;
                }


            }
        }

        private static void Generate(long count)
        {
            Transactions = new List<Transaction>();

            var rnd = new Random();
            var yearStart = new DateTime(2018, 1, 1, 0, 0, 0);
            
            var startNode = Nodes.Find(n => n.Name == "0");

            for (var i = 0; i < count; i++)
            {
                //Starting point for the current item
                var startTime = yearStart + TimeSpan.FromHours(rnd.Next(0, 720));
                var trans = new Transaction(startNode, startTime, i);

                //The first move
                //var nextTrans = CalculateNextNode(ref trans);

                var nextTrans = startNode.CalculateNextNode(ref trans, 15, 60);

                Transactions.Add(trans);
                
                trans = nextTrans;

                while (trans != null)
                {
                    //nextTrans = CalculateNextNode(ref trans);
                    nextTrans = trans.StartNode.CalculateNextNode(ref trans, 15, 60);

                    if (nextTrans == null)
                    {
                        break;
                    }

                    Transactions.Add(trans);
                    trans = nextTrans;
                }


            }
        }
        
        private static Transaction CalculateNextNode(ref Transaction trans)
        {
            var rnd = new Random();
            trans.Duration = TimeSpan.FromMinutes(rnd.Next(15, 60));
            trans.EndTime = trans.StartTime + trans.Duration;

            int target;

            switch (trans.StartNode.Name)
            {
                //Entry
                case "0":
                    target = rnd.Next(1, 2);
                    if (target == 1)
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "11");
                    }
                    else
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "21");
                    }

                    break;

                //Machines
                case "11":
                    trans.EndNode = Nodes.Find(n => n.Name == "12");
                    break;
                case "12":
                    target = rnd.Next(3, 4);
                    if (target == 3)
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "31");
                    }
                    else
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "Storage");
                    }

                    break;

                case "21":
                    trans.EndNode = Nodes.Find(n => n.Name == "22");
                    break;
                case "22":
                    target = rnd.Next(4, 6);
                    if (target == 4)
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "Storage");
                    }
                    else if (target == 5)
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "41");
                    }
                    else
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "Exit");
                    }
                    break;
                case "31":
                    trans.EndNode = Nodes.Find(n => n.Name == "32");
                    break;
                case "32":
                    trans.EndNode = Nodes.Find(n => n.Name == "Exit");
                    break;
                case "41":
                    trans.EndNode = Nodes.Find(n => n.Name == "42");
                    break;
                case "42":
                    trans.EndNode = Nodes.Find(n => n.Name == "Exit");
                    break;

                //Storage
                case "Storage":
                    target = rnd.Next(3, 4);
                    if (target == 3)
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "31");
                    }
                    else
                    {
                        trans.EndNode = Nodes.Find(n => n.Name == "Exit");
                    }
                    break;

                //Exit
                case "Exit":
                    return null;

                //Error
                default:
                    break;
            }
            
            //Returning a new transaction starting from the end of the previous node
            return new Transaction(trans.EndNode, trans.EndTime, trans.Order);
        }

        private static void SaveTransactions()
        {
            foreach (var trans in Transactions)
            {
                Console.WriteLine((trans.ToString()));
            }
        }
        
        #endregion

        #region FileWriter

        private static void TransactionsToFile()
        {
            //var file = new StreamWriter(@"F:\DGSql\trans.sql");
            using (var file = new StreamWriter(@"F:\DGSql\trans.sql"))
            {

                file.WriteLine("INSERT INTO nodes VALUES ");

                foreach (var node in Nodes)
                {
                    file.WriteLine("( {0}, '{1}', {2}, {3}, {4} )", node.Id, node.Name, node.Type, node.Capacity,
                        node.Group);
                }

                file.WriteLine(";");

                file.WriteLine("INSERT INTO transactions VALUES");

                foreach (var trans in Transactions)
                {
                    file.WriteLine("( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} )", trans.Id, trans.StartNode.Id,
                        trans.EndNode.Id, trans.StartTime, trans.EndTime, trans.Duration, trans.Order, trans.Type);
                }

                file.WriteLine(";");
            }
        }

        private static void TransactionsToFileVersionTwo()
        {
            using (var file = new StreamWriter(@"F:\DGSql\trans2.sql"))
            {
                file.WriteLine("INSERT INTO [Inspector_Joros_OP].[dbo].[dy_transactions] (dt_time_start, dt_time_end, dt_node_start, dt_node_end, dt_duration, dt_work_order, dt_type)");
                file.WriteLine("VALUES");

                int index = 0;
                foreach(var trans in Transactions)
                {
                    if (index <= 990)
                    {
                        file.WriteLine("( '{0}', '{1}', {2}, {3}, {4}, {5}, {6} ),", trans.StartTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"), trans.EndTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"),
                            trans.StartNode.Name, trans.EndNode.Name, trans.Duration.Seconds.ToString(), trans.Order.ToString(), (int)trans.Type);
                    }
                    else
                    {
                        file.WriteLine("( '{0}', '{1}', {2}, {3}, {4}, {5}, {6} )", trans.StartTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"), trans.EndTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"),
                            trans.StartNode.Name, trans.EndNode.Name, trans.Duration.Seconds.ToString(), trans.Order.ToString(), (int)trans.Type);

                        file.WriteLine("GO");
                        file.WriteLine("INSERT INTO [Inspector_Joros].[dbo].[dy_transactions] (dt_time_start, dt_time_end, dt_node_start, dt_node_end, dt_duration, dt_work_order, dt_type)");
                        file.WriteLine("VALUES");

                        index = -1;
                    }

                    ++index;
                }
            }
        }

        private static void DgTransactionsToFile()
        {
            using (var file = new StreamWriter(@"F:\DGSql\PopulateDq.sql"))
            {
                file.WriteLine("INSERT INTO [Inspector_Joros_OP].[dbo].[dy_transactions] (dt_time_start, dt_time_end, dt_node_start, dt_node_end, dt_duration, dt_work_order, dt_type)");
                file.WriteLine("VALUES");

                int index = 0;
                foreach (var trans in Transactions)
                {
                    if (index <= 990)
                    {
                        file.WriteLine("( '{0}', '{1}', {2}, {3}, {4}, {5}, {6} ),", trans.StartTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"), trans.EndTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"),
                            trans.StartNode.Name, trans.EndNode.Name, trans.Duration.Seconds.ToString(), trans.Order.ToString(), (int)trans.Type);
                    }
                    else
                    {
                        file.WriteLine("( '{0}', '{1}', {2}, {3}, {4}, {5}, {6} )", trans.StartTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"), trans.EndTime.ToString("yyyy-MM-dd HH':'mm':'ss'.'fff"),
                            trans.StartNode.Name, trans.EndNode.Name, trans.Duration.Seconds.ToString(), trans.Order.ToString(), (int)trans.Type);

                        file.WriteLine("GO");
                        file.WriteLine("INSERT INTO [Inspector_Joros].[dbo].[dy_transactions] (dt_time_start, dt_time_end, dt_node_start, dt_node_end, dt_duration, dt_work_order, dt_type)");
                        file.WriteLine("VALUES");

                        index = -1;
                    }

                    ++index;
                }
            }
        }

        #endregion
    }

    public class Workplace
    {
        public Workplace(int number)
        {
            Number = number;
            NextOnes = new List<Workplace>();
        }

        public int Number { get; set; }
        public List<Workplace> NextOnes { get; set; }

        public Workplace SpeculateNext()
        {
            var rnd = new Random();

            //To continue or to not
            if (rnd.Next(1, 4) < 4)
                return null;

            if (NextOnes == null || NextOnes.Count <= 0)
                return null;

            return NextOnes.ElementAt(rnd.Next(0, NextOnes.Count - 1));
        }

        public bool Equals(Workplace comp)
        {
            if (Number == comp.Number)
                return true;
            return false;
        }

    }
}
