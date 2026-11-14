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
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace neanea
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, EventArgs e) //Registers acccount if they do not exist inside the database 
        {
            if (EmailTextBox.Text == "" || EmailTextBox.Text == null || UsernameTextBox.Text == "" || UsernameTextBox.Text == null || PasswordTextBox.Text == "" || PasswordTextBox.Text == null) //Checks details are no spaces or left blank
            {
                MessageBox.Show("Fill all blank fields.", "Error");
            }
            else if (!ValidatePassword(PasswordTextBox.Text.Trim()) || !ValidateUsername(UsernameTextBox.Text.Trim()) || !ValidateEmail(EmailTextBox.Text.Trim())) //validates inputs 
            {
            }
            else
            {
                using (OleDbConnection connection = new OleDbConnection(DatabaseUtils.CONNECTION_STRING))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Connected To The Database Successfully!");

                        string selectaccount = "SELECT * " +
                                               "FROM Account " +
                                               "WHERE Username = ?";
                        using (OleDbCommand selectaccountcmd = new OleDbCommand(selectaccount, connection))
                        {
                            selectaccountcmd.Parameters.AddWithValue("?", UsernameTextBox.Text.Trim());

                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(selectaccountcmd))
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);
                                if (table.Rows.Count > 0)
                                {
                                    MessageBox.Show(UsernameTextBox.Text + " already exists.", "Error");
                                }
                                else
                                {
                                    string insertaccount = "INSERT INTO Account (Username, [Password], Email, DateCreated) "
                                                         + "VALUES (?, ?, ?, ?)";

                                    using (OleDbCommand insertaccountcmd = new OleDbCommand(insertaccount, connection))
                                    {
                                        insertaccountcmd.Parameters.AddWithValue("?", UsernameTextBox.Text.Trim());
                                        string hashpassword = Login.HashingAlgorithm(PasswordTextBox.Text.Trim());
                                        insertaccountcmd.Parameters.AddWithValue("?", hashpassword);
                                        insertaccountcmd.Parameters.AddWithValue("?", EmailTextBox.Text.Trim());
                                        insertaccountcmd.Parameters.AddWithValue("?", DateTime.Today); 
                                        insertaccountcmd.ExecuteNonQuery();

                                        MessageBox.Show("Account Registered Successfully.");
                                        Terms_Conditions Form = new Terms_Conditions();
                                        Form.Show();
                                        this.Hide();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Account registration was unsuccessful.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) //closes program 
        {
            Application.Exit();
        }

        private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) //Toggles password visability is characters are replaced 
        {
            if(ShowPasswordCheckBox.Checked)
            {
                PasswordTextBox.PasswordChar = '\0';
            }
            else
            {
                PasswordTextBox.PasswordChar = '*';
            }
        }

        private void EmailTextBox_Enter(object sender, EventArgs e) //Shows example 
        {
            if (EmailTextBox.Text == "username@domain.com")
            {
                EmailTextBox.Text = "";
                EmailTextBox.ForeColor = Color.Black;
            }
        }

        private void EmailTextBox_Leave(object sender, EventArgs e) //removes example so you can type 
        {
            if (EmailTextBox.Text == "" || EmailTextBox.Text == null)
            {
                EmailTextBox.Text = "username@domain.com";
                EmailTextBox.ForeColor = Color.Gray;
            }
        }

        private void UsernameTextBox_Enter(object sender, EventArgs e) //Shows example
        {
            if (UsernameTextBox.Text == "username123")
            {
                UsernameTextBox.Text = "";
                UsernameTextBox.ForeColor = Color.Black;
            }
        }

        private void UsernameTextBox_Leave(object sender, EventArgs e) //removes example so you can type 
        {
            if (UsernameTextBox.Text == "" || UsernameTextBox.Text == null)
            {
                UsernameTextBox.Text = "username123";
                UsernameTextBox.ForeColor = Color.Gray;
            }
        }

        private void PasswordTextBox_Enter(object sender, EventArgs e) //show example
        {
            if (PasswordTextBox.Text == "Pa55w0rd1?")
            {
                PasswordTextBox.Text = "";
                PasswordTextBox.ForeColor = Color.Black;
            }
        }

        private void PasswordTextBox_Leave(object sender, EventArgs e) //removes example so you can type
        {
            if (PasswordTextBox.Text == "" || PasswordTextBox.Text == null)
            {
                PasswordTextBox.Text = "Pa55w0rd1?";
                PasswordTextBox.ForeColor = Color.Gray;
            }
        }

        private void SignUp_Load(object sender, EventArgs e) //loads the textboxes when loaded to show example inputs
        {
            UsernameTextBox.Text = "username123";
            UsernameTextBox.ForeColor = Color.Gray;
            PasswordTextBox.Text = "Pa55w0rd1?";
            PasswordTextBox.ForeColor = Color.Gray;
            EmailTextBox.Text = "username@domain.com";
            EmailTextBox.ForeColor = Color.Gray; ;
        }

        private bool ValidatePassword(string password) //validates password 
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
                MessageBox.Show("Password must contain at least 1 symbol.", "Invalid Password");
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

        private bool ValidateUsername(string username) //validates username 
        {
            bool valid = true;
            const int maxlength = 15;
            const int minlength = 5;
            if (!(Regex.IsMatch(username, @"[a-zA-Z\d._]")))
            {
                MessageBox.Show("Username must not contain any special characaters.", "Invalid Username");
                return false;
            }
            else if (username.Length < minlength)
            {
                MessageBox.Show("Username is too short.", "Invalid Username");
                return false;
            }
            else if (username.Length > maxlength)
            {
                MessageBox.Show("Username is too long.", "Invalid Username");
                return false;
            }
            else if ((Regex.IsMatch(username, @"\s")))
            {
                MessageBox.Show("Username must not contain spaces.", "Invalid Username");
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

        private void AlreadyAccount_Click(object sender, EventArgs e) //take you to login if you think you aleady have an account
        {
            Login Form = new Login();
            Form.Show();
            this.Hide();
        }
    }
}
