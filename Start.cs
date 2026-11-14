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

namespace neanea
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void SkipButton_Click(object sender, EventArgs e) //you don't need to login so take you to home
        { 
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }

        private void RegisterButton_Click(object sender, EventArgs e) //takes you to sign up page
        {
            SignUp Form = new SignUp();
            Form.Show();
            this.Hide();
        }

        private void LoginButton_Click(object sender, EventArgs e) //takes you to login page
        {
            Login Form = new Login();
            Form.Show();
            this.Hide();
        }

        private void CloseButton_Click(object sender, EventArgs e) //closes prorgam 
        {
            Application.Exit();
        }

        private void Start_Load(object sender, EventArgs e) //calls create database command
        {
            DatabaseUtils.CreateDatabase();
        }
    }
}
