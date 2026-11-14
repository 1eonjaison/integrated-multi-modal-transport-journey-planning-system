using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neanea
{
    public partial class Account : Form
    {
        public bool bankdetailsinputted = false; //Checker if bank details have been enetered - is not used currently 
        public Account()
        {
            InitializeComponent();
        }

        private void SignOutButton_Click(object sender, EventArgs e) //Signs out of account which toggles login and bankdetails to false then takes back to start form
        {
            if (Login.loggedin == true)
            {
                MessageBox.Show("Your account has been successfully signed out. See you next time!");
            }
            Login.loggedin = false;
            BankDetails.bankdetails = false;
            Start Form = new Start();
            Form.Show();
            this.Hide();
        }

        private void HomeButton_Click(object sender, EventArgs e) //Home Button
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }

        private void TicketsButton_Click(object sender, EventArgs e) //Takes you to Tickets form only if logged in
        {
            if (Login.loggedin == true)
            {
                ViewTickets Form = new ViewTickets();
                Form.Show();
                this.Hide();
            }
            else if (Login.loggedin == false)
            {
                MessageBox.Show("You must login first");
            }
        }

        private void AccountDetailsButton_Click(object sender, EventArgs e) //Takes you to Bank Details form - naming is a bit off due to form name changes made
        {
            if (BankDetails.bankdetails == false)
            {
                if (Login.loggedin == true)
                {
                    BankDetails Form = new BankDetails();
                    Form.Show();
                    this.Hide();
                }
                else if (Login.loggedin == false)
                {
                    MessageBox.Show("You must login first");
                }
            }
            else if (BankDetails.bankdetails == true )
            {
                MessageBox.Show("You have already inputted bank details.");
            }
        }

        private void RoutesButton_Click(object sender, EventArgs e) //Takes you to routes form which needs an account to be logged into to work.
        {
            if (Login.loggedin == true)
            {
                Routes Form = new Routes();
                Form.Show();
                this.Hide();
            }
            else if(Login.loggedin == false)
            {
                MessageBox.Show("You must login first");
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) //Closes program
        {
            Application.Exit();
        }

        private void AccountDetails_Click(object sender, EventArgs e) //Takes you to account details form if there is an account which is logged into
        {
            if (Login.loggedin == true)
            {
                UserDetails Form = new UserDetails();
                Form.Show();
                this.Hide();
            }
            else if (Login.loggedin == false)
            {
                MessageBox.Show("You must login first");
            }
        }
    }
}
