using prjDatabaseCreation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neanea
{
    public partial class UserDetails : Form
    {
        public UserDetails()
        {
            InitializeComponent();
        }

        private void LoadAccountDGV() //Loads datagridview but getting account details from database
        {
            using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
            {
                try
                {
                    connection.Open();
                    string selectaccount = "SELECT Account.Username, Account.Password, Account.Email " +
                                           "FROM Account " +
                                           "WHERE Account.Username = ? ";
                    using (OleDbCommand cmd = new OleDbCommand(selectaccount, connection))
                    {
                        cmd.Parameters.AddWithValue("?", Login.username.Trim());

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);
                            LoadAccount.DataSource = table;
                            LoadAccount.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Error loading account.", "Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void UpdateAccountDetails() //Updates record in database 
        {
            if (!ValidatePassword(PasswordTextBox.Text.Trim()) || !ValidateEmail(EmailTextBox.Text.Trim()))
            {
            }
            else
            {
                using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
                {
                    try
                    {
                        connection.Open();
                        string updateaccount = "UPDATE Account " +
                                               "SET [Password] = ?, Email = ? " +
                                               "WHERE Username = ? ";
                        using (OleDbCommand cmd = new OleDbCommand(updateaccount, connection))
                        {
                            string hashpassword = Login.HashingAlgorithm(PasswordTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("?", hashpassword.Trim());
                            cmd.Parameters.AddWithValue("?", EmailTextBox.Text.Trim());
                            cmd.Parameters.AddWithValue("?", Login.username.Trim());

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Account Updated Successfully");
                            Account login = new Account();
                            login.Show();
                            this.Hide();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error updating account.", "Error");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private bool ValidatePassword(string password) //Validates password
        {
            bool valid = true;
            const int minlength = 10;

            if (!(Char.IsUpper(password[0])))
            {
                MessageBox.Show("Password must start with a capital letter.", "Invalid Password");
                return false;
            }
            else if (password.Length < minlength)
            {
                MessageBox.Show("Password must be at least 10 characters long.", "Invalid Password");
                return false;
            }
            else if (!(Regex.IsMatch(password, @"[!£$%^&*()#@~?]")))
            {
                MessageBox.Show("Password must contain at least 1 symbol.", "Invalid Passoword");
                return false;
            }
            else if (!(Regex.IsMatch(password, @"\d")))
            {
                MessageBox.Show("Password must contain a digit.", "Invalid Password");
                return false;
            }
            else if ((Regex.IsMatch(password, @"\s")))
            {
                MessageBox.Show("Password must not contain spaces.", "Invalid Password");
                return false;
            }
            return valid;
        }

        private bool ValidateEmail(string email) //validates email
        {
            bool valid = true;
            string emailformat = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailformat))
            {
                MessageBox.Show("Incorrect Email Format, should be 'username@domain.com'.", "Invalid Email");
                return false;
            }
            else if ((Regex.IsMatch(email, @"\s")))
            {
                MessageBox.Show("Email must not contain spaces.", "Invalid Email");
                return false;
            }
            return valid;
        }

        private void UpdateButton_Click(object sender, EventArgs e) //calls record to updated 
        {
            UpdateAccountDetails();
        }

        private void UserDetails_Load(object sender, EventArgs e) //calls data grid view subroutine
        {
            LoadAccountDGV();
        }

        private void CloseButton_Click(object sender, EventArgs e) //Closes program
        {
            Application.Exit();
        }

        private void HomeButton_Click(object sender, EventArgs e) //home button
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) //toggles password visavility whre charcters are not replaced 
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
    }
}
