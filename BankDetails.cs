using prjDatabaseCreation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace neanea
{
    public partial class BankDetails : Form
    {
        public static bool bankdetails = false; //To check if bank details are inputted for account
        public BankDetails()
        {
            InitializeComponent();
        }

        private void PersonalInformation_Load(object sender, EventArgs e) //Loads account details into datagrid view
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

        private void SaveButton_Click(object sender, EventArgs e) //Saves bank detailsa to database if they do not already exist
        {
            if (FirstNameTextBox.Text == "" || FirstNameTextBox.Text == null || LastNameTextBox.Text == "" || LastNameTextBox.Text == null || AccountNumberTextBox.Text == "" || AccountNumberTextBox.Text == null) //Checks if null of empty space
            {
                MessageBox.Show("Fill all blank fields.", "Error");
            }
            else if (!ValidateFirstName(FirstNameTextBox.Text.Trim()) || !ValidateLastName(LastNameTextBox.Text.Trim()) || !ValidateAccountNumber(AccountNumberTextBox.Text.Trim())) //Calls the validation subroutines 
            {
            }
            else
            {
                using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING)) 
                {
                    try
                    {
                        connection.Open();
                        string checkaccount = "SELECT * " +
                                              "FROM BankDetails " +
                                              "WHERE Username = ?";
                        using (OleDbCommand checkaccountcmd = new OleDbCommand(checkaccount, connection)) 
                        {
                            checkaccountcmd.Parameters.AddWithValue("?", Login.username.Trim());

                            OleDbDataAdapter adapter = new OleDbDataAdapter(checkaccountcmd); 
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                if (table.Rows.Count > 0)
                                {
                                    MessageBox.Show("Bank details are already registered.");
                                }
                                else
                                {
                                    string insertbankdetails = "INSERT INTO BankDetails (Username, FirstName, LastName, AccountNumber) "
                                                             + "VALUES (?, ?, ?, ?)";

                                    using (OleDbCommand cmd = new OleDbCommand(insertbankdetails, connection))
                                    {
                                        cmd.Parameters.AddWithValue("?", Login.username.Trim());
                                        cmd.Parameters.AddWithValue("?", FirstNameTextBox.Text.Trim());
                                        cmd.Parameters.AddWithValue("?", LastNameTextBox.Text.Trim());
                                        string hashaccountnumber = Login.HashingAlgorithm(AccountNumberTextBox.Text.Trim());
                                        cmd.Parameters.AddWithValue("?", hashaccountnumber);

                                        cmd.ExecuteNonQuery(); //Used to execute SQL queries that don't return anything

                                        MessageBox.Show("Account Updated Successfully");
                                        bankdetails = true;
                                        Account login = new Account();
                                        login.Show();
                                        this.Hide();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error updating account.", "Error ");
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

        private void HomeButton_Click(object sender, EventArgs e) //Home button
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }

        private bool ValidateFirstName(string firstname) //Validates firstname
        {
            bool valid = true;
            const int maxlength = 20;
            const int minlength = 3;
            if (!(Regex.IsMatch(firstname, @"^[a-zA-Z]+$")))
            {
                MessageBox.Show("Firstname should only include alphabet characters.", "Invalid Firstname");
                return false;
            }
            if (!(Char.IsUpper(firstname[0])))
            {
                MessageBox.Show("Firstname must start with a capital letter.", "Invalid Firstname");
                return false;
            }
            else if (firstname.Length < minlength)
            {
                MessageBox.Show("Firstname is too short.", "Invalid Firstname");
                return false;
            }
            else if (firstname.Length > maxlength)
            {
                MessageBox.Show("Firstname is too long.", "Invalid Firstname");
                return false;
            }
            return valid;
        }

        private bool ValidateLastName(string lastname) //Validates lastname
        {
            bool valid = true;
            const int maxlength = 20;
            const int minlength = 3;
            if (!(Regex.IsMatch(lastname, @"^[a-zA-Z]+$")))
            {
                MessageBox.Show("Lastname should only include alphabet characteres.", "Invalid Lastname");
                return false;
            }
            if (!(Char.IsUpper(lastname[0]))) 
            {
                MessageBox.Show("Lastname must start with a capital letter.", "Invalid Lastname");
                return false;
            }
            else if (lastname.Length < minlength)
            {
                MessageBox.Show("Lastname is too short.", "Invalid Lastname");
                return false;
            }
            else if (lastname.Length > maxlength)
            {
                MessageBox.Show("Lastname is too long.", "Invalid Lastname");
                return false;
            }
            return valid;
        }

        private bool ValidateAccountNumber(string accountnumber) //Validates account number
        {
            bool valid = true;
            const int accountnumberlength = 8;
            if (!(Regex.IsMatch(accountnumber, @"^\d+$")))
            {
                MessageBox.Show("Account number should only include numeric characters.", "Invalid Account Number");
                return false;
            }
            else if (!(accountnumber.Length == accountnumberlength))
            {
                MessageBox.Show("Account number must be only 8 digits.", "Invalid Account Number");
                return false;
            }
            return valid;
        }

        private void ShowAccountNumberCheckBox_CheckedChanged(object sender, EventArgs e) //Account number visablity which is toggled wherr chracters are not replaced.
        {
            if (ShowAccountNumberCheckBox.Checked)
            {
                AccountNumberTextBox.PasswordChar = '\0'; 
            }
            else
            {
                AccountNumberTextBox.PasswordChar = '*';
            }
        }
    }
}
