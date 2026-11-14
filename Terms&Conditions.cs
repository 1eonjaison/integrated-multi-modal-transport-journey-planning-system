using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace neanea
{
    public partial class Terms_Conditions : Form
    {
        public static string filename = "T&C.txt"; //terms and conditions in text file
        public static bool agreed = false;
        public Terms_Conditions()
        {
            InitializeComponent();
        }

        private void AgreeButton_Click(object sender, EventArgs e) //if you agree it takes you to login
        {
            agreed = true;
            Login Form = new Login();
            Form.Show();
            this.Hide();
        }

        private void DisagreeButton_Click(object sender, EventArgs e) //if you disagree it takes you to start implying you to not use system 
        {
            agreed = false;
            Start Form = new Start();
            Form.Show();
            this.Hide();
        }

        private void CloseButton_Click(object sender, EventArgs e) //closes program 
        {
            Application.Exit();
        }

        private void LoadTermsAndCondtions() //reads textfile than outputs in textbox
        {
            if (File.Exists(filename))
            {
                string filecontent = File.ReadAllText(filename);
                TermsAndCondtions.Text = filecontent;
            }
        }

        private void Terms_Conditions_Load(object sender, EventArgs e) //When form loads it calls the subroutine to load textbox
        {
            LoadTermsAndCondtions();
        }
    }
}
