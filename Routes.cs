using prjDatabaseCreation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace immtjps
{
    public partial class Routes : Form
    {
        public static bool sort;
        public Routes()
        {
            InitializeComponent();
        }

        private void Routes_Load(object sender, EventArgs e) //Loads routes datagridview by getting routes data from different database tables
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectroute = "SELECT Routes.StartLocation, Routes.FinalLocation, Routes.Distance, Routes.TimeTaken, Routes.Mode, Routes.Specific " +
                                         "FROM Routes " +
                                         "INNER JOIN UserRoutes " +
                                         "ON Routes.RouteID = UserRoutes.RouteID " +
                                         "WHERE UserRoutes.Username = ?";
                    using (OleDbCommand cmd = new OleDbCommand(selectroute, connection))
                    {
                        cmd.Parameters.AddWithValue("?", Login.username.Trim());

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            LoadRoutes.DataSource = table;
                            LoadRoutes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error loading routes.", "Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) //Toggles asednding or descenddiing - naming is differnt due to not being able to change as it will cause errors
        {
            if (AscendingOrDescending.Checked)
            {
                sort = true;
            }
            else
            {
                sort = false;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) //Closes program 
        {
            Application.Exit();
        }

        private void SortButton_Click(object sender, EventArgs e) //Sorts by chosen sort which is ascending or descending 
        {
            SortingAlgorithms sorts = new SortingAlgorithms();

            if (!LoadRoutes.Columns.Contains(SortTextBox.Text.Trim())) 
            {
                MessageBox.Show("Column not found.", "Error");
                return;
            }

            try
            {
                if (sort == true)
                {
                    sorts.BubbleSortAscending(LoadRoutes, LoadRoutes.Columns[SortTextBox.Text.Trim()].Index);
                }
                else if (sort == false)
                {
                    sorts.BubbleSortDescending(LoadRoutes, LoadRoutes.Columns[SortTextBox.Text.Trim()].Index); 
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please enter a valid input for sort.", "Invalid Input");
            }
        }
        
        private void HomeButton_Click(object sender, EventArgs e) //Takes you to home form
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }
    }
}
