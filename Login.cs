using prjDatabaseCreation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.ServiceModel.Security;
using System.CodeDom;
using System.Text.RegularExpressions;

namespace neanea
{
    public partial class Login : Form
    {
        public static bool loggedin = false;
        public static string username = "";
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, EventArgs e) //Check if username and password exists in the database tabl ethne if there is an account number assigned the username
        {
            if (UsernameTextBox.Text == "" || PasswordTextBox.Text == "")
            {
                MessageBox.Show("Fill all blank fields.", "Error");
            }
            else
            {
                using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
                {
                    try
                    {
                        connection.Open();
                        String selectaccount = "SELECT * " +
                                               "FROM Account " +
                                               "WHERE Username = ? AND Password = ?";
                        using (OleDbCommand cmd = new OleDbCommand(selectaccount, connection))
                        {
                            cmd.Parameters.AddWithValue("?", UsernameTextBox.Text.Trim());
                            string hashpassword = HashingAlgorithm(PasswordTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("?", hashpassword.Trim());
                            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                if (table.Rows.Count > 0)
                                {
                                    string selectaccountnumber = "SELECT AccountNumber " +
                                                                 "FROM BankDetails " +
                                                                 "WHERE Username = ?";
                                    using (OleDbCommand checkaccountcmd = new OleDbCommand(selectaccountnumber, connection))
                                    {
                                        checkaccountcmd.Parameters.AddWithValue("?", UsernameTextBox.Text.Trim());

                                        MessageBox.Show("Logged in Successfully.");
                                        BankDetails.bankdetails = true;
                                        OleDbDataAdapter adapter2 = new OleDbDataAdapter(checkaccountcmd);
                                        {
                                            DataTable table2 = new DataTable();
                                            adapter2.Fill(table2);
                                            if (table2.Rows.Count == 0)
                                            {
                                                MessageBox.Show("Remember to update your bank details.");
                                                BankDetails.bankdetails = false;
                                            }
                                            loggedin = true;
                                            username = UsernameTextBox.Text.Trim();
                                            Home Form = new Home();
                                            Form.Show();
                                            this.Hide();
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect Username or Password.", "Error");
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Login was unsuccessful.", "Error");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) //Closes program
        {
            Application.Exit();
        }

        public static string HashingAlgorithm(string input) //Hashing algorthm
        {
            int hashvalue = 17;
            foreach (char c in input)
            {
                hashvalue = (hashvalue * (23 + c) - 52) % 100000;
            }
            return hashvalue.ToString("X"); //Converts to hexadecimal 
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) //Toggles visability of password 
        {
            if (ShowPasswordCheckBox.Checked)
            {
                PasswordTextBox.PasswordChar = '\0';
            }
            else
            {
                PasswordTextBox.PasswordChar = '*';
            }
        }

        private void Login_Load(object sender, EventArgs e) //Shows example input 
        {
            UsernameTextBox.Text = "username123";
            UsernameTextBox.ForeColor = Color.Gray;
            PasswordTextBox.Text = "Pa55w0rd1?";
            PasswordTextBox.ForeColor = Color.Gray;
        }

        private void UsernameTextBox_Enter(object sender, EventArgs e) //Shows example input for username 
        {
            if (UsernameTextBox.Text == "username123")
            {
                UsernameTextBox.Text = "";
                UsernameTextBox.ForeColor = Color.Black;
            }
        }

        private void UsernameTextBox_Leave(object sender, EventArgs e) //Removes example to allow for typing
        {
            if (UsernameTextBox.Text == "" || UsernameTextBox.Text == null)
            {
                UsernameTextBox.Text = "username123";
                UsernameTextBox.ForeColor = Color.Gray;
            }
        }

        private void PasswordTextBox_Enter(object sender, EventArgs e) //Shows example input for password
        {
            if (PasswordTextBox.Text == "Pa55w0rd1?")
            {
                PasswordTextBox.Text = "";
                PasswordTextBox.ForeColor = Color.Black;
            }
        }

        private void PasswordTextBox_Leave(object sender, EventArgs e) //Removes example to allow for typing
        {
            if (PasswordTextBox.Text == "" || PasswordTextBox.Text == null)
            {
                PasswordTextBox.Text = "Pa55w0rd1?";
                PasswordTextBox.ForeColor = Color.Gray;
            }
        }

        private void NoAccount_Click(object sender, EventArgs e) //No account means the user can switch to signup form
        {
            SignUp Form = new SignUp();
            Form.Show();
            this.Hide();
        }
    }
}
