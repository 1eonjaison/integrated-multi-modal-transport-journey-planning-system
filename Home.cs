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
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void JourneyPlannerButton_Click(object sender, EventArgs e) //Opens Journey Planner Form
        {
            Map Form = new Map();
            Form.Show();
            this.Hide();
        }

        private void CloseButton_Click(object sender, EventArgs e) //Closes Program 
        {
            Application.Exit();
        }

        private void AccountButton_Click(object sender, EventArgs e) //Opens account form
        {
            Account Form = new Account();
            Form.Show();
            this.Hide();
        }

        private void HomeButton_Click(object sender, EventArgs e) //Home button
        {
            Home Form = new Home();
            Form.Show();
            this.Hide();
        }
    }
}
