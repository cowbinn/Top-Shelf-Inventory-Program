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
using login.classes;

namespace login
{
    public partial class FormAccount : Form
    {
        SqlConnection sqlcon = null;//sql connection object
        User user;
        Client client;

        public FormAccount(User u, Client c)
        {
            InitializeComponent();
            Connection open = new Connection();//create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            user = u;
            client = c;

            textBox1.Text = client.ClientID;
            textBox2.Text = client.PhoneNumber;
            textBox3.Text = client.EMail;
            textBox4.Text = client.Address;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Update")
            {
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = false;
                textBox4.ReadOnly = false;
                label5.Visible = true;
                textBox5.Visible = true;
                textBox2.BackColor = Color.White;
                textBox3.BackColor = Color.White;
                textBox4.BackColor = Color.White;
                textBox2.ForeColor = Color.Black;
                textBox3.ForeColor = Color.Black;
                textBox4.ForeColor = Color.Black;
                button1.Text = "Save";
                button2.Text = "Cancel";
            }
            else if(button1.Text == "Save")
            {
                if (string.IsNullOrEmpty(textBox5.Text))
                {
                    MessageBox.Show("Please enter the password.");
                }
                else if(textBox5.Text == user.Password)
                {
                    sqlcon.Open();
                    SqlCommand query = new SqlCommand("UPDATE Client SET PhoneNumber = '" + textBox2.Text + "', EMail = '" + textBox3.Text + "', Address = '" + textBox4.Text + "' WHERE ClientID = '" + textBox1.Text + "';", sqlcon);
                    query.ExecuteNonQuery();
                    sqlcon.Close();
                    client.PhoneNumber = textBox2.Text;
                    client.EMail = textBox3.Text;
                    client.Address = textBox4.Text;
                    MessageBox.Show("Saved.");

                    textBox2.ReadOnly = true;
                    textBox3.ReadOnly = true;
                    textBox4.ReadOnly = true;
                    label5.Visible = false;
                    textBox5.Visible = false;
                    textBox2.BackColor = Color.FromArgb(26, 32, 40);
                    textBox3.BackColor = Color.FromArgb(26, 32, 40);
                    textBox4.BackColor = Color.FromArgb(26, 32, 40);
                    textBox2.ForeColor = Color.White;
                    textBox3.ForeColor = Color.White;
                    textBox4.ForeColor = Color.White;
                    button1.Text = "Update";
                    button2.Text = "Exit";
                }
                else
                {
                    MessageBox.Show("Incorrect password.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(button2.Text == "Exit")
            {
                Close();
            }
            else if(button2.Text == "Cancel")
            {
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
                label5.Visible = false;
                textBox5.Visible = false;
                textBox2.BackColor = Color.FromArgb(26, 32, 40);
                textBox3.BackColor = Color.FromArgb(26, 32, 40);
                textBox4.BackColor = Color.FromArgb(26, 32, 40);
                textBox2.ForeColor = Color.White;
                textBox3.ForeColor = Color.White;
                textBox4.ForeColor = Color.White;
                button1.Text = "Update";
                button2.Text = "Exit";
            }
        }
    }
}
