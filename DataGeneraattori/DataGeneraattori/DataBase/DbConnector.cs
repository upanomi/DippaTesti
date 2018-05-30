using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DataGeneraattori.PetriNet;
using Microsoft.Win32;
using DataGeneraattori.DirectedGraph;

namespace DataGeneraattori.DataBase
{
    public class DbConnector
    {

        #region Construction

        public DbConnector(string conn = "server=PÖYTÄPÖNTTÖ\\SQLEXPRESS; Trusted_Connection=yes; database=Inspector_Joros; connection timeout=30")
        {
            //Connection = new SqlConnection("server=PÖYTÄPÖNTTÖ\\SQLEXPRESS; Trusted_Connection=yes; database=Inspector_Joros; connection timeout=30");
            //Connection.Open();
            ConnectionString = conn;
            
        }

        #endregion

        #region Properties

        //private SqlConnection Connection { get; set; }
        private string ConnectionString { get; set; }

        #endregion

        #region Methods for initializing the system

        #region DgInit

        public List<Node> GetNodes()
        {

            using ( var connection = new SqlConnection(ConnectionString))
            {
                var res = new List<Node>();

                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }

                Console.WriteLine("Connection open");
                //var cmd = new SqlCommand("SELECT un_id, un_type, un_name FROM us_nodes WHERE un_id >= 33", connection);
                var cmd = new SqlCommand("SELECT un_id, un_type, un_name FROM us_nodes", connection);

                using (var reader = cmd.ExecuteReader())
                {
                   while (reader.Read())
                    {
                        //Console.WriteLine(reader[0].ToString() + reader[1].ToString());
                        res.Add(new Node(reader[0].ToString(), (int)reader[1]));
                    }
                }

                /*cmd = new SqlCommand("SELECT DISTINCT dt_node_start, dt_node_end FROM dy_transactions WHERE dt_node_start >= 33 ORDER BY dt_node_start ASC",
                    connection);*/
                /*
                cmd = new SqlCommand("SELECT DISTINCT dt_node_start, dt_node_end FROM dy_transactions ORDER BY dt_node_start ASC",
                    connection);

                using (var reader = cmd.ExecuteReader())
                {
                    Node startN;
                    Node endN;
                    while (reader.Read())
                    {
                        
                        startN = res.FirstOrDefault(n => n.Name == reader[0].ToString());
                        endN = res.FirstOrDefault(n => n.Name == reader[1].ToString());

                        if (startN == null || endN == null)
                            continue;

                        startN.NextNodes.Add(endN);
                        
                    }
                }*/

                foreach(var nd in res)
                {
                    switch (nd.Name)
                    {
                        case "Node0":
                            nd.NextNodes.AddRange(res.Where(n => n.Name == "Node1" || n.Name == "Node2"));
                            break;

                        case "Node1":
                            nd.NextNodes.AddRange(res.Where(n => n.Name == "Node3" || n.Name == "Node4"));
                            break;

                        case "Node2":
                            nd.NextNodes.AddRange(res.Where(n => n.Name == "Node4" || n.Name == "Node5" || n.Name == "Node6"));
                            break;

                        case "Node3":
                            nd.NextNodes.AddRange(res.Where(n => n.Name == "Node6"));
                            break;

                        case "Node4":
                            nd.NextNodes.AddRange(res.Where(n => n.Name == "Node3" || n.Name == "Node6"));
                            break;

                        case "Node5":
                            nd.NextNodes.AddRange(res.Where(n => n.Name == "Node6"));
                            break;

                        case "Node6":
                            break;

                        default:
                            break;
                    }
                }

                return res;

            }
        }

        public List<long> GetOrders()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var res = new List<long>();
                connection.Open();

                var cmd = new SqlCommand("SELECT DISTINCT uwo_id FROM us_work_order", connection);

                using(var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            res.Add((long)reader[0]);
                        }
                        catch(Exception ex)
                        {
                            continue;
                        }
                    }
                }

                return res;
            }
        }

        #endregion

        #region PnInit

        public bool InitPetriNets(out List<Place> places, out List<Transition> transitions, out List<Connector> connectors)
        {
            places = new List<Place>();
            transitions = new List<Transition>();
            connectors = new List<Connector>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                Console.WriteLine("Connection open for initializing petri nets");

                try
                {
                    //Places
                    var cmd = new SqlCommand("SELECT id, name, capacity FROM pn_Places ORDER BY name ASC", connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            places.Add(new Place(reader[0].ToString(), reader[1].ToString(), int.Parse(reader[2].ToString())));
                        }
                    }

                    //Transitions
                    cmd = new SqlCommand("SELECT Id, Name FROM pn_Transitions ORDER BY Name ASC", connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transitions.Add(new Transition(reader[0].ToString(), reader[1].ToString()));
                        }
                    }

                    //Connectors
                    cmd = new SqlCommand("SELECT Id, Transition, Place, Direction FROM pn_Connectors", connection);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tempPla = places.FirstOrDefault(p => p.Id == new Guid(reader[2].ToString()));
                            var tempTrans = transitions.FirstOrDefault(p => p.Id == new Guid(reader[1].ToString()));
                            var dir = (ConnectorDirection)(int.Parse(reader[3].ToString()));

                            connectors.Add(new Connector(reader[0].ToString(), dir, tempTrans, tempPla));
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

            }

            return true;
        }

        public bool InsertNewToken(Token tk)
        {
            using(var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                var cmd = new SqlCommand(string.Format("INSERT INTO [dbo].[pn_Tokens] (Id, Name, Location) VALUES (newid(), {0}, {1}",
                    tk.Name, tk.Location.Id.ToString()), connection);

                try
                {
                    if(cmd.ExecuteNonQuery() <= 0)
                    {
                        return false;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }


            return true;
        }

        #endregion

        #endregion

        #region Population

        #region Dg methods

        public bool Dg_InsertTransaction(Transaction tr, Edge ed, Item it)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(string.Format("INSERT INTO [dbo].[dg_Transactions] VALUES"), connection);

                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

            return true;
        }

        public bool Dg_InsertNewItem(Item it)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(string.Format("INSERT INTO [dbo].[dg_Items] (Id, Name, Location) VALUES ('{0}', '{1}', '{2}');",
                    it.Id.ToString(), it.Name, it.Location.Id.ToString()), connection);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
            return true;
        }

        public bool Dg_InsertNewEdge(Edge ed)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand(string.Format("INSERT INTO [dbo].[dg_Edges] (Id, StartNode, EndNode, Count, Duration) VALUES ('{0}', '{1}', '{2}', '{3}', @dur ",
                    ed.Id.ToString(), ed.Start.Id.ToString(), ed.End.Id.ToString(), 0), connection);
                cmd.Parameters.AddWithValue("@dur", new TimeSpan(0));

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

                return true;
        }

        #endregion

        #region Pn methods

        public bool Pn_InsertEvent(Event ev)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                var query = string.Format("INSERT INTO [dbo].[pn_Events] (Id, Timestamp, FirstConnector, SecondConnector, Token) VALUES ( newid(), @value, '{0}', '{1}', '{2}');",
                        ev.FirstConnector.Id.ToString(), ev.SecondConnector.Id.ToString(), ev.Item.Id.ToString());
                var cmd = new SqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@value", ev.Timestamp);

                try
                {
                    var affected = cmd.ExecuteNonQuery();
                    if (affected <= 0)
                        return false;

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

            }

            return true;
        }

        public bool Pn_InsertNewToken(Token tk)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                var cmd = new SqlCommand(string.Format("INSERT INTO [dbo].[pn_Tokens] (Id, Name, Location) VALUES ('{0}', '{1}', '{2}' )",
                    tk.Id.ToString(), tk.Name, tk.Location.Id.ToString()),
                    connection);

                try
                {
                    if (cmd.ExecuteNonQuery() <= 0)
                        return false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

            return true;
        }

        #region Updating the tables

        public bool Pn_UpdateToken(Token tk)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                var cmd = new SqlCommand(string.Format("ALTER [dbo].[pn_Tokens] SET Location = {0} WHERE Id = {1}", tk.Location.Id.ToString(), tk.Id.ToString()), 
                    connection);

                try
                {
                    if (cmd.ExecuteNonQuery() <= 0)
                        return false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

                return true;
        }

        public bool Pn_UpdateLocation(Place pl)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }

                var cmd = new SqlCommand(string.Format("ALTER [dbo].[pn_Places] SET Capacity = {0} WHERE Id = {1}",
                    pl.Capacity.ToString(), pl.Id.ToString()),
                    connection);

                try
                {
                    if (cmd.ExecuteNonQuery() <= 0)
                        return false;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

            return true;
        }

        #endregion

        #endregion

        #endregion

        #region Real time methods

        public bool Dg_NewEvent(Transaction trans)
        {
            return true;
        }

        public bool Dg_UpdateEvent(Transaction trans)
        {
            return true;
        }

        #endregion

        #region Simulated system

        #region Dg

        public bool Dg_GetWholeGraph()
        {
            return true;
        }

        #endregion

        #region PN

        public bool Pn_GetWholeGraph(out List<Place> pl, out List<Transition> tr, out List<Connector> cn, out List<Token> tk)
        {
            pl = new List<Place>();
            tr = new List<Transition>();
            cn = new List<Connector>();
            tk = new List<Token>();

            using (var connection = new SqlConnection(ConnectionString))
            {
                var plCmd = new SqlCommand("SELECT (Id, Name, Capacity) FROM [dbo].[pn_Places]", connection);
                var trCmd = new SqlCommand("SELECT (Id, Name) FROM [dbo].[pn_Transitions]", connection);
                var cnCmd = new SqlCommand("SELECT (Id, Transition, Place, Direction) FROM [dbo].[pn_Connectors]", connection);
                var tkCmd = new SqlCommand("SELECT (Id, Name, Location) FROM [dbo].[pn_Tokens]", connection);

                try
                {
                    connection.Open();

                    using(var reader = plCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pl.Add(new Place(reader[0].ToString(), reader[1].ToString(), int.Parse(reader[2].ToString())));
                        }
                    }

                    using (var reader = trCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tr.Add(new Transition(reader[0].ToString(), reader[1].ToString()));
                        }
                    }

                    using(var reader = cnCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tempPla = pl.FirstOrDefault(p => p.Id == new Guid(reader[2].ToString()));
                            var tempTrans = tr.FirstOrDefault(p => p.Id == new Guid(reader[1].ToString()));
                            var dir = (ConnectorDirection)(int.Parse(reader[3].ToString()));

                            cn.Add(new Connector(reader[0].ToString(), dir, tempTrans, tempPla));
                        }
                    }

                    using (var reader = tkCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tempPla = pl.FirstOrDefault(p => p.Id == new Guid(reader[2].ToString()));

                            tk.Add(new Token(reader[0].ToString(), reader[1].ToString(), tempPla));
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }

                return true;
        }

        #endregion

        #endregion
    }
}
