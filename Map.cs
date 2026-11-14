using GMap.NET.WindowsForms;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using OsmSharp.Streams;
using OsmSharp;
using System.IO;
using GMap.NET.WindowsForms.Markers;
using System.Collections;
using prjDatabaseCreation;
using System.Data.OleDb;
using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;
using ADOX;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using static neanea.Map;
using OsmSharp.IO.PBF;

namespace neanea
{
    public partial class Map : Form
    {
        public static string startaddress = "";
        public static string endaddress = "";
        public static double totaldistance = 0;
        public static bool pathcalculated = false;
        private static GMapOverlay markersoverlay = new GMapOverlay("markers");
        private GMapControl gmap; //initialises map call
        private Dictionary<long, PointLatLng> nodes = new Dictionary<long, PointLatLng>();
        private Dictionary<long, List<(long node, double distance)>> graph = new Dictionary<long, List<(long, double)>>(); //Nodes then list of previous nodes and distance
        private GMapOverlay pathoverlay = new GMapOverlay("path");

        public Map()
        {
            InitializeComponent();
            InitializeMap();
            LoadInitialLocation();
            LoadFinallLocation();
        }

        private void InitializeMap() //Sets up what the map shoul look like when loaded 
        {
            gmap = new GMapControl
            {
                Size = new Size(250, 150),
                MapProvider = GMapProviders.GoogleMap,
                Position = new GMap.NET.PointLatLng(50.904968, -1.403230),
                MinZoom = 2,
                MaxZoom = 25,
                Zoom = 8,
            };
            int adjustvertical = 55;
            gmap.Location = new Point(
                (this.ClientSize.Width - gmap.Width) / 2,
                (this.ClientSize.Height - gmap.Height) / 2 - adjustvertical);

            this.Controls.Add(gmap);
            gmap.Overlays.Add(pathoverlay);
            ParseOSMFile("hampshire-latest.osm.pbf"); //Osm file to be parsed
        }

        private void FindPathButton_Click(object sender, EventArgs e) //Inital and location are found then sent to dijkstras algorthm
        {
            markersoverlay.Markers.Clear();
            startaddress = InitialLocationTextBox.Text;
            endaddress = FinalLocationTextBox.Text;

            if (InitialLocationTextBox.Text == "" || InitialLocationTextBox.Text == null || FinalLocationTextBox.Text == "" || FinalLocationTextBox.Text == null) 
            {
                MessageBox.Show("Please enter both start and end addresses.", "Invalid Input");
            }
            else if (InitialLocationTextBox.Text == FinalLocationTextBox.Text) 
            {
                MessageBox.Show("The start and end location are the same.", "No Distance Travelled");
            }
            PointLatLng? startpoint = GetCoordinates(startaddress); 
            PointLatLng? endpoint = GetCoordinates(endaddress);

            if (startpoint.HasValue && endpoint.HasValue)
            {
                PointLatLng start = startpoint.Value;
                AddMarker(start, "Start Point", Color.Blue);
                PointLatLng end = endpoint.Value;
                AddMarker(end, "End Point", Color.Blue);
                long? startnode = FindNearestNode(start); 
                long? endnode = FindNearestNode(end); 

                if (startnode.HasValue && endnode.HasValue)
                {
                    List<long> path = DijkstraAlgorithm(startnode.Value, endnode.Value, graph);

                    if (path.Count > 0)
                    {
                        ShowRoute(path); 
                        pathcalculated = true;
                    }
                    else
                    {
                        MessageBox.Show("No path found between the nodes selected.", "Path Not Found");
                    }
                    startnode = endnode = null; //Resets nodes to stop using old values 
                }
            }
        }

        private PointLatLng? GetCoordinates(string address) //Gets latitude and longitude values from table
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Latitude, Longitude " +
                                   "FROM Locations " +
                                   "WHERE Address = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("?", address);
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            if (table.Rows.Count > 0)
                            {
                                double latitude = Convert.ToDouble(table.Rows[0]["Latitude"]); 
                                double longitude = Convert.ToDouble(table.Rows[0]["Longitude"]); 
                                return new PointLatLng(latitude, longitude); //Returns both values as coordinate
                            }
                            else
                            {
                                MessageBox.Show(string.Format("Address '{0}' could not be found.", address), "Error");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error connecting to the database.");
                }
                finally
                {
                    connection.Close();
                }
            }
            return null;
        }

        private long FindNearestNode(PointLatLng point) //Finds the closest node for the start and end node 
        {
            long nearestnode = -1;
            double maxdistance = 10000000000000000000;

            foreach (KeyValuePair<long, PointLatLng> node in nodes)
            {
                double distance = CalculateDistance(node.Value, point);
                if (distance < maxdistance)
                {
                    maxdistance = distance; 
                    nearestnode = node.Key; 
                }
            }
            return nearestnode;
        }

        //Code below is not mine but I have made changes to it to work for my system
        private void AddMarker(PointLatLng point, string title, Color colour) //Adds markers onto map
        {
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red);
            marker.ToolTipText = title;
            markersoverlay.Markers.Add(marker);
            gmap.Overlays.Add(markersoverlay);
        }

        private void ParseOSMFile(string osmfilepath)
        {
            if (!File.Exists(osmfilepath)) //Checks file exists 
            {
                Console.WriteLine("OSM file could not be found.");
                return;
            }
            //From here to..
            using (FileStream filestream = File.OpenRead(osmfilepath)) 
            {
                PBFOsmStreamSource source = new PBFOsmStreamSource(filestream); 
                foreach (OsmGeo element in source) 
                {
                    //...here is not my code 
                    //The below if statement was inspired by a github example but is coded in a different way but the code within is mine
                    if (element.Type == OsmGeoType.Node) 
                    {
                        switch(element)
                        {
                            case OsmSharp.Node node: //casts to node automatically
                                if (node != null)
                                {
                                    nodes[node.Id.Value] = new PointLatLng(node.Latitude.Value, node.Longitude.Value);
                                }
                                break;
                            default:
                                Console.WriteLine("Error, the element is not of type node.");
                                break;
                        }
                    }
                    else if (element.Type == OsmGeoType.Way)
                    {
                        switch (element)
                        {
                            case OsmSharp.Way way:
                                CreateGraph(way);
                                break;

                            default:
                                Console.WriteLine("Error, the element is not of type way.");
                                break;
                        }//trys to cast eleement as node 
                    }
                }
            }
        }

        private void CreateGraph(OsmSharp.Way way) //Creates a graph dictionary
        {
            {
                if (way.Nodes.Length > 1)
                {
                    for (int i = 0; i < way.Nodes.Length - 1; i++)
                    {
                        long currentnode = way.Nodes[i];
                        long nextnode = way.Nodes[i + 1];
                        List<(long, double)> nodeanddistancetonodes;

                        if (nodes.TryGetValue(currentnode, out PointLatLng point1) && nodes.TryGetValue(nextnode, out PointLatLng point2)) 
                        {
                            double distance = CalculateDistance(point1, point2);
                            if (!graph.ContainsKey(currentnode))
                            {
                                nodeanddistancetonodes = new List<(long, double)>();
                                graph[currentnode] = nodeanddistancetonodes;
                            }
                            graph[currentnode].Add((nextnode, distance));
                            if (!graph.ContainsKey(nextnode))
                            {
                                nodeanddistancetonodes = new List<(long, double)>();
                                graph[nextnode] = nodeanddistancetonodes;
                            }
                            graph[nextnode].Add((currentnode, distance));
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Graph does not have enough nodes.");
                }
            }
        }

        private double CalculateDistance(PointLatLng point1, PointLatLng point2) //Not my code - Haversines formula - calculates distance betweeen two points
        {
            const double R = 6371000;
            double dLat = (point2.Lat - point1.Lat) * (Math.PI / 180.0);
            double dLon = (point2.Lng - point1.Lng) * (Math.PI / 180.0);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(point1.Lat * (Math.PI / 180.0)) * Math.Cos(point2.Lat * (Math.PI / 180.0)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return distance;
        }

        private List<long> DijkstraAlgorithm(long startnode, long endnode, Dictionary<long, List<(long, double)>> graph) //Coded using pseudocode that is not mine - basicaly I tried implementing the logic shown in psudeocode in c# environment 
        {
            Dictionary<long, double> distances = new Dictionary<long, double>(); //visted nodes
            Dictionary<long, long> previousnodes = new Dictionary<long, long>();
            PriorityQueue priorityqueue = new PriorityQueue();

            distances[startnode] = 0;
            priorityqueue.Enqueue(0, startnode);
            while (!priorityqueue.IsEmpty())
            {
                double currentdistance;
                long currentnode;
                double newdistance;
                priorityqueue.Dequeue(out currentdistance, out currentnode);

                if (currentnode == endnode)
                {
                    break;
                }
                if (graph.TryGetValue(currentnode, out List<(long, double)> neighbours))
                {
                    foreach ((long neighbour, double edgedistance) in neighbours)
                    {
                        newdistance = currentdistance + edgedistance;
                        if (!distances.ContainsKey(neighbour) || newdistance < distances[neighbour]) 
                        {
                            distances[neighbour] = newdistance;
                            previousnodes[neighbour] = currentnode;
                            priorityqueue.Enqueue(newdistance, neighbour);
                        }
                    }
                }
            }
            //from here is fully my code 
            List<long> path = new List<long>();
            totaldistance = Math.Round((distances[endnode]));
            long? current = endnode;
            while (current != null && previousnodes.TryGetValue(current.Value, out long previous))
            {
                path.Add(current.Value);
                current = previous;
            }
            SaveDistance();
            MessageBox.Show("Path has been calculated.");
            return path;
        }


        private void SaveDistance() //Saves distance calculated
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectroute = "SELECT RouteID " +
                                         "FROM Routes " +
                                         "WHERE StartLocation = ? AND FinalLocation = ?";
                    using (OleDbCommand findrouteidcmd = new OleDbCommand(selectroute, connection))
                    {
                        findrouteidcmd.Parameters.AddWithValue("?", InitialLocationTextBox.Text.Trim());
                        findrouteidcmd.Parameters.AddWithValue("?", FinalLocationTextBox.Text.Trim());
                        string routeid;

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(findrouteidcmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            if (table.Rows.Count > 0)
                            {
                                routeid = Convert.ToString(table.Rows[0]["RouteID"]);
                            }
                            else
                            {
                                Console.WriteLine("Route was not found");
                                return;
                            }
                        }
                        string insertData = "UPDATE Routes " +
                                            "SET Distance = ?" +
                                            "WHERE RouteID = ?";
                        using (OleDbCommand cmd = new OleDbCommand(insertData, connection))
                        {
                            cmd.Parameters.AddWithValue("?", totaldistance);
                            cmd.Parameters.AddWithValue("?", routeid);
                            cmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("Distance was saved.");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error saving distance.");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) //Closes program
        {
            Application.Exit();
        }

        private void SaveButton_Click(object sender, EventArgs e) //Saves record of route values found and saves in database but only if logged in
        {
            if (Login.loggedin == false)
            {
                MessageBox.Show("You must log in first.", "Log in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Start Form = new Start();
                Form.Show();
                this.Hide();
            }
            else if (pathcalculated == false)
            {
                MessageBox.Show("You must press find path first.");
            }
            else if (Login.loggedin == true && pathcalculated == true)
            {
                using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
                {
                    try
                    {
                        connection.Open();
                        string selectroute = "SELECT RouteID " +
                                             "FROM Routes " +
                                             "WHERE StartLocation = ? AND FinalLocation = ?";
                        using (OleDbCommand findrouteidcmd = new OleDbCommand(selectroute, connection))
                        {
                            findrouteidcmd.Parameters.AddWithValue("?", InitialLocationTextBox.Text.Trim());
                            findrouteidcmd.Parameters.AddWithValue("?", FinalLocationTextBox.Text.Trim());
                            string routeid;
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(findrouteidcmd))
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                if (table.Rows.Count > 0)
                                {
                                    routeid = Convert.ToString(table.Rows[0]["RouteID"]);
                                }
                                else
                                {
                                    Console.WriteLine("Route was not found.");
                                    return;
                                }
                            }
                            string checkroute = "SELECT * " +
                                                "FROM UserRoutes " +
                                                "WHERE RouteID = ? AND Username = ?";
                            using (OleDbCommand checkroutecmd = new OleDbCommand(checkroute, connection))
                            {
                                checkroutecmd.Parameters.AddWithValue("?", routeid.Trim());
                                checkroutecmd.Parameters.AddWithValue("?", Login.username.Trim());

                                OleDbDataAdapter adapter2 = new OleDbDataAdapter(checkroutecmd);
                                DataTable table2 = new DataTable();
                                adapter2.Fill(table2);

                                if (table2.Rows.Count > 0)
                                {
                                    MessageBox.Show("This route has already been saved.");
                                }
                                else
                                {
                                    string insertData = "INSERT INTO UserRoutes (RouteID, Username) " +
                                                "VALUES (?, ?)";
                                    using (OleDbCommand cmd = new OleDbCommand(insertData, connection))
                                    {
                                        cmd.Parameters.AddWithValue("?", routeid.Trim());
                                        cmd.Parameters.AddWithValue("?", Login.username.Trim());
                                        cmd.ExecuteNonQuery();
                                    }
                                    MessageBox.Show("Route has been saved");
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error saving route.", "Error");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void PaymentButton_Click(object sender, EventArgs e) //Takes you to buy tickts form if criteria is met
        {
            if (Login.loggedin == false)
            {
                MessageBox.Show("You must log in first.", "Log in");
                Start Form = new Start();
                Form.Show();
                this.Hide();
            }
            else if (BankDetails.bankdetails == false)
            {
                MessageBox.Show("You must input your bank details first.", "Bank Details");
                Account Form = new Account();
                Form.Show();
                this.Hide();
            }
            else if (pathcalculated == false)
            {
                MessageBox.Show("You must press find path first.");
            }
            else if (Login.loggedin == true && BankDetails.bankdetails == true && pathcalculated == true)
            {
                BuyTickets Form = new BuyTickets();
                Form.Show();
                this.Hide();
            }
        }

        private void HomeButton_Click(object sender, EventArgs e) //Home button
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }

        private void LoadInitialLocation() //Loads combo box for initial location
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectaddress = "SELECT Address " +
                                           "FROM Locations";
                    using (OleDbCommand cmd = new OleDbCommand(selectaddress, connection))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        InitialLocationTextBox.Items.Clear();
                        while (reader.Read())
                        {
                            InitialLocationTextBox.Items.Add(reader["Address"].ToString());
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Location could not be loaded.", "Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void LoadFinallLocation() //Loads final location combo boc
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectaddress = "SELECT Address " +
                                           "FROM Locations";
                    using (OleDbCommand cmd = new OleDbCommand(selectaddress, connection))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        FinalLocationTextBox.Items.Clear();
                        while (reader.Read())
                        {
                            FinalLocationTextBox.Items.Add(reader["Address"].ToString());
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Location could not be loaded.", "Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void ShowRoute(List<long> path) //Displays route on map
        {
            //My code
            List<PointLatLng> points = new List<PointLatLng>();
            pathoverlay.Routes.Clear();
            foreach (long node in path)
            {
                if (nodes.TryGetValue(node, out PointLatLng point))
                {
                    points.Add(point);
                }
            }
            //.too here 
            try
            {
                //This code is not mine
                if (points.Count > 1)
                {
                    GMapRoute route = new GMapRoute(points, "Shortest Path")
                    {
                        Stroke = new Pen(Color.Red, 5)
                    };

                    pathoverlay.Routes.Add(route);
                    gmap.Refresh();
                }
                //...to here
            }
            catch
            {
                MessageBox.Show("Invalid Path.", "Error");
            }
        }
    }
}
