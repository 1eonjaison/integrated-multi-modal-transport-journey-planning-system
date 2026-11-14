using prjDatabaseCreation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using GMap.NET.MapProviders;

namespace neanea
{
    public partial class ViewTickets : Form
    {
        static string filename;

        private List<DataView> datalist = new List<DataView>();
        public ViewTickets()
        {
            InitializeComponent();
            LoadDelete();
            LoadTickets.Refresh();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ViewTickets_Load(object sender, EventArgs e) //Calls a load method
        {
            LoadTicketDGV();
        }

        private void LoadTicketDGV() //Loads datagridview with database table data for tickets
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selecttickets = "SELECT Tickets.TicketID, Routes.StartLocation, Routes.FinalLocation, Routes.Mode, Routes.Specific, Tickets.TicketPrice, Tickets.PurchaseDate " +
                                           "FROM Routes " +
                                           "INNER JOIN Tickets " +
                                           "ON Routes.RouteID = Tickets.RouteID " +
                                           "WHERE Tickets.Username = ?";
                    using (OleDbCommand cmd = new OleDbCommand(selecttickets, connection))
                    {
                        cmd.Parameters.AddWithValue("?", Login.username.Trim());

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            LoadTickets.DataSource = table;
                            LoadTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error loading tickets.", "Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e) //Deletes record in datagrid view and database table of ticket of a ticket id
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                connection.Open();
                string deleterecord = "DELETE " +
                                      "FROM Tickets " +
                                      "WHERE TicketID = ?";
                using (OleDbCommand cmd = new OleDbCommand(deleterecord, connection))
                {
                    cmd.Parameters.AddWithValue("?", DeleteTextBox.Text.Trim());

                    int rowsaffected = cmd.ExecuteNonQuery();
                    if (rowsaffected > 0)
                    {
                        DeleteTextFile();
                        MessageBox.Show("Ticket has been Cancelled.");
                        LoadTicketDGV();
                        LoadDelete();
                    }
                    else
                    {
                        MessageBox.Show("Enter a valid Ticket ID.");
                    }
                }
            }
        }

        private void DeleteTextFile() //Gets filename then deletws the text file if it exists
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                connection.Open();
                string selectfile = "SELECT FilePath " +
                                    "FROM Tickets " +
                                    "WHERE TicketID = ?";
                using (OleDbCommand findfilecmd = new OleDbCommand(selectfile, connection))
                {
                    findfilecmd.Parameters.AddWithValue("?", DeleteTextBox.Text.Trim());
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(findfilecmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            filename = Convert.ToString(table.Rows[0]["FilePath"]);
                        }
                        else
                        {
                            Console.WriteLine("File was not found.");
                            return;
                        }
                    }
                }
            }
            if (File.Exists(filename)) //Checks that it is an actual text file
            {
                File.Delete(filename); //Deletes textfile
                Console.WriteLine("File deleted.");
            }
        }

        private void HomeButton_Click(object sender, EventArgs e) //Home button
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }

        private void LoadDelete() //Loads combobox for the inputs that are accepted 
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selecttickets = "SELECT TicketID " +
                                           "FROM Tickets " +
                                           "WHERE Username = ?";
                    using (OleDbCommand cmd = new OleDbCommand(selecttickets, connection))
                    {
                        cmd.Parameters.AddWithValue("?", Login.username.Trim());
                        OleDbDataReader reader = cmd.ExecuteReader();
                        {
                            DeleteTextBox.Items.Clear(); 
                            while (reader.Read())
                            {
                                DeleteTextBox.Items.Add(reader["TicketID"].ToString());
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Tickets could not found.");
                }
                finally
                {
                    connection.Close();
                }
            }
            LoadTicketDGV();
        }

        private void OpenButton_Click(object sender, EventArgs e) //Gets file path of file
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                connection.Open();
                string selectfile = "SELECT FilePath " +
                                    "FROM Tickets " +
                                    "WHERE TicketID = ?";
                using (OleDbCommand cmd = new OleDbCommand(selectfile, connection))
                {
                    cmd.Parameters.AddWithValue("?", DeleteTextBox.Text.Trim());
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            filename = Convert.ToString(table.Rows[0]["FilePath"]);
                        }
                        else
                        {
                            MessageBox.Show("Enter a valid Ticket ID.");
         
                            Console.WriteLine("Textfile could not found.");
                            return;
                        }
                    }
                }
            }
            OpenFile(filename);
        }

        public static void OpenFile(string filename) //Opens textfile 
        {
            try
            {
                if (File.Exists(filename)) 
                {
                    Process.Start(filename);
                }
                else
                {
                    MessageBox.Show("Ticket was not found.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("An error occurred while opening the ticket.");
            }
        }
    }
}
