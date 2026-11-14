using prjDatabaseCreation;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace neanea
{
    public partial class BuyTickets : Form
    {
        public static string routeid;
        public static string specific;
        public static int ticketprice;
        public static string filename;
        private bool ticketbought = false; //check if the ticket has been bought
        private static Random rnd = new Random();
        public BuyTickets()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, EventArgs e) //Closes program
        {
            Application.Exit();
        }

        private void BuyTickets_Load(object sender, EventArgs e) //Loads datadgrid view with a data from different database tables into one table.
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectrouteid = "SELECT RouteID " +
                                           "FROM Routes " +
                                           "WHERE StartLocation = ? AND FinalLocation = ?";
                    using (OleDbCommand findrouteidcmd = new OleDbCommand(selectrouteid, connection))
                    {
                        findrouteidcmd.Parameters.AddWithValue("?", Map.startaddress.Trim());
                        findrouteidcmd.Parameters.AddWithValue("?", Map.endaddress.Trim());

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
                    }

                    string selectroute = "SELECT * " +
                                         "FROM Routes " +
                                         "WHERE RouteID = ?";
                    using (OleDbCommand findroutecmd = new OleDbCommand(selectroute, connection))
                    {
                        findroutecmd.Parameters.AddWithValue("?", routeid.Trim());
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(findroutecmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            LoadRoute.DataSource = table;
                            LoadRoute.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                        }
                    }

                    string selectspecific = "SELECT Specific " +
                                            "FROM Routes " +
                                            "WHERE StartLocation = ? AND FinalLocation = ?";
                    using (OleDbCommand findspecificcmd = new OleDbCommand(selectspecific, connection))
                    {
                        findspecificcmd.Parameters.AddWithValue("?", Map.startaddress.Trim());
                        findspecificcmd.Parameters.AddWithValue("?", Map.endaddress.Trim());

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(findspecificcmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table); 

                            if (table.Rows.Count > 0)
                            {
                                specific = Convert.ToString(table.Rows[0]["Specific"]);
                            }
                            else
                            {
                                Console.WriteLine("Specific transport was not found.");
                                return;
                            }
                        }
                    }

                    string selectticketprice = "SELECT TicketPrice " +
                                               "FROM Transport " +
                                               "WHERE Specific = ?";
                    using (OleDbCommand findticketpricecmd = new OleDbCommand(selectticketprice, connection))
                    {
                        findticketpricecmd.Parameters.AddWithValue("?", specific.Trim());
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(findticketpricecmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table); 

                            if (table.Rows.Count > 0)
                            {
                                ticketprice = Convert.ToInt32(table.Rows[0]["TicketPrice"]);
                            }
                            else
                            {
                                Console.WriteLine("Ticket price was not found.");
                                return;
                            }
                        }
                    }

                    string selecttransport = "SELECT * " +
                                             "FROM Transport " +
                                             "WHERE Specific = ?";
                    using (OleDbCommand findticketpricecmd = new OleDbCommand(selecttransport, connection))
                    {
                        findticketpricecmd.Parameters.AddWithValue("?", specific.Trim());
                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(findticketpricecmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            LoadPrice.DataSource = table;
                            LoadPrice.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error creating ticket.", "Error");
                }
            }
        }

        private void BuyButton_Click(object sender, EventArgs e) //Allows ticket to bought only once
        {
            if (ticketbought == true)
            {
                MessageBox.Show("Ticket has already been bought.");
                return;
            }
            ticketbought = true;
            SaveToDB();
            CreateTicket();
        }

        public static void SaveToDB() //Saves to database that a ticket was bought and which ticket by assiging data to variables.
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectroute  = "SELECT RouteID " +
                                          "FROM Routes " +
                                          "WHERE StartLocation = ? AND FinalLocation = ?";
                    using (OleDbCommand findrouteidcmd = new OleDbCommand(selectroute, connection))
                    {
                        findrouteidcmd.Parameters.AddWithValue("?", Map.startaddress.Trim());
                        findrouteidcmd.Parameters.AddWithValue("?", Map.endaddress.Trim());
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

                        string selectspecific = "SELECT Specific " +
                                                "FROM Routes " +
                                                "WHERE StartLocation = ? AND FinalLocation = ?";
                        using (OleDbCommand findspecificcmd = new OleDbCommand(selectspecific, connection))
                        {
                            findspecificcmd.Parameters.AddWithValue("?", Map.startaddress.Trim());
                            findspecificcmd.Parameters.AddWithValue("?", Map.endaddress.Trim());

                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(findspecificcmd))
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                if (table.Rows.Count > 0)
                                {
                                    specific = Convert.ToString(table.Rows[0]["Specific"]); 
                                }
                                else
                                {
                                    Console.WriteLine("Specific transport was not found.");
                                    return;
                                }
                            }
                        }

                        string selectticketprice = "SELECT TicketPrice " +
                                                   "FROM Transport " +
                                                   "WHERE Specific = ?";
                        using (OleDbCommand findticketpricecmd = new OleDbCommand(selectticketprice, connection))
                        {
                            findticketpricecmd.Parameters.AddWithValue("?", specific.Trim());
                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(findticketpricecmd))
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                if (table.Rows.Count > 0)
                                {
                                    ticketprice = Convert.ToInt32(table.Rows[0]["TicketPrice"]); 
                                }
                                else
                                {
                                    Console.WriteLine("Ticket price was not found.");
                                    return;
                                }
                            }
                        }

                        string insertticket = "INSERT INTO Tickets (RouteID, Username, TicketPrice, PurchaseDate) "
                                            + "VALUES (?, ?, ?, ?)";
                        using (OleDbCommand cmd = new OleDbCommand(insertticket, connection))
                        {
                            cmd.Parameters.AddWithValue("?", routeid.Trim());
                            cmd.Parameters.AddWithValue("?", Login.username.Trim());
                            cmd.Parameters.AddWithValue("?", ticketprice);
                            cmd.Parameters.AddWithValue("?", DateTime.Today);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Ticket has been saved.");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error saving ticket.", "Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static void CreateTicket() //Creates textfile tickets by collecting data from differnet tables 
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectroute = "SELECT Routes.StartLocation, Routes.FinalLocation, Routes.Mode, Routes.Specific, Tickets.TicketPrice, Tickets.PurchaseDate " +
                                         "FROM Routes " +
                                         "INNER JOIN Tickets " +
                                         "ON Routes.RouteID = Tickets.RouteID " +
                                         "WHERE Tickets.Username = ?";
                    using (OleDbCommand cmd = new OleDbCommand(selectroute, connection))
                    {
                        cmd.Parameters.AddWithValue("?", Login.username.Trim());
                        OleDbDataReader reader = cmd.ExecuteReader();
                        {
                            string datetime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            filename = string.Format("tickets{0}.txt", datetime);
                            while (reader.Read())
                            {
                                using (StreamWriter sw = new StreamWriter(filename))
                                {
                                    sw.WriteLine("Ticket Information");
                                    sw.WriteLine("_________________________________________________________________________________");
                                    string ticketid = GenerateTicketID();
                                    sw.WriteLine("Ticket ID: {0}", ticketid);
                                    sw.WriteLine("Start Location: {0}", reader["StartLocation"].ToString());
                                    sw.WriteLine("Final Location: {0}", reader["FinalLocation"].ToString());
                                    sw.WriteLine("Mode: {0}", reader["Mode"].ToString());
                                    sw.WriteLine("Specific: {0}", reader["Specific"].ToString());
                                    sw.WriteLine("Ticket Price: £{0}", Convert.ToDecimal(reader["TicketPrice"]));
                                    sw.WriteLine("Purchase Date: {0}", Convert.ToDateTime(reader["PurchaseDate"]).ToString("d"));
                                    sw.WriteLine("_________________________________________________________________________________");
                                }
                            }
                        }
                        
                    }
                    string updateticket = "UPDATE Tickets " +
                                          "SET FilePath = ?" +
                                          "WHERE RouteID = ?";
                    using (OleDbCommand cmd = new OleDbCommand(updateticket, connection))
                    {
                        cmd.Parameters.AddWithValue("?",filename.Trim());
                        cmd.Parameters.AddWithValue("?", routeid.Trim());
                        cmd.ExecuteNonQuery();
                    }
                        Console.WriteLine("Ticket file has been saved.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Ticket could not be saved.", "Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static string GenerateTicketID() //Generates a random ticket id 
        {
            int length = 5;
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            char[] id = new char[length];
            for (int i = 0; i < length; i++)
            {
                id[i] = characters[rnd.Next(characters.Length)];
            }
            return new string(id);
        }

        private void HomeButton_Click(object sender, EventArgs e) //Takes you to home form.
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }

        private void PreviewButton_Click(object sender, EventArgs e) //Opens the textfile chosen if the file exists
        {
            try
            {
               if (ticketbought == false)
               {
                    MessageBox.Show("Ticket must be bought before being previewed.");
                }
                if (ticketbought == true)
                {
                    if (File.Exists(filename))
                    {
                        Process.Start(filename);
                    }
                    else
                    {
                        MessageBox.Show("Ticket cannot be previewed right now.");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred while finding your ticket.");
            }
        }
    }
}
