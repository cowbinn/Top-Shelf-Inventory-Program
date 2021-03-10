using login.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace login
{

    public partial class FormCreateAccount : Form
    {
        SqlConnection sqlcon = null;//sql connection variable
        public FormCreateAccount() //constructor
        {
            InitializeComponent();

            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function

            groupCombo.Items.Add("Admin"); //fill combo box
            groupCombo.Items.Add("Clerk");
            groupCombo.Items.Add("Client");
            groupCombo.Items.Add("Stocker");
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            SqlCommand query, query1;
            if (string.IsNullOrEmpty(usernameBox.Text) || string.IsNullOrEmpty(passwordBox.Text))//check if any of the text box is empty
            {
                MessageBox.Show("Need more information!");//show message
            }
            else if (usernameExists(usernameBox.Text))
            {
                MessageBox.Show("Username already exists.");//show message
            }
            else
            {
                if (groupCombo.SelectedIndex > -1)//check if something is selected in groupCombo
                {//if true
                    if (groupCombo.SelectedItem.ToString() == "Client")
                    {
                        if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                        {
                            MessageBox.Show("Need more information from client!");
                        }
                        else
                        {
                            query = new SqlCommand("INSERT INTO Login VALUES (@loginID, @Username, @Password, @Usergroup);", sqlcon);//insert user into database
                            query1 = new SqlCommand("INSERT INTO Client VALUES (@ClientID, @PhoneNumber, @EMail, @Address, @loginID);", sqlcon);
                            //get vals from text boxes
                            query.Parameters.AddWithValue("@LoginID", get_id());
                            query.Parameters.AddWithValue("@Username", usernameBox.Text);
                            query.Parameters.AddWithValue("@Password", passwordBox.Text);
                            query.Parameters.AddWithValue("@Usergroup", groupCombo.SelectedItem);
                            query1.Parameters.AddWithValue("@ClientID", get_cid());
                            query1.Parameters.AddWithValue("@PhoneNumber", textBox1.Text);
                            query1.Parameters.AddWithValue("@EMail", textBox2.Text);
                            query1.Parameters.AddWithValue("@Address", textBox3.Text);
                            query1.Parameters.AddWithValue("@loginID", get_id());
                            sqlcon.Open();//open database
                            query.ExecuteNonQuery();//execute query
                            query1.ExecuteNonQuery();
                            MessageBox.Show("User " + usernameBox.Text + " inserted.");//show message box   
                            sqlcon.Close();//close database

                            usernameBox.Text = String.Empty;//empty textbox2
                            passwordBox.Text = String.Empty;//empty textbox3

                            groupCombo.SelectedIndex = -1;//reset combobox1
                            textBox1.Text = String.Empty;
                            textBox2.Text = String.Empty;
                            textBox3.Text = String.Empty;
                        }
                    }
                    else
                    {
                        query = new SqlCommand("INSERT INTO Login (loginID, Username, Password, Usergroup) VALUES (@loginID, @Username, @Password, @Usergroup);", sqlcon);//insert user into database
                        //get vals from text boxes
                        query.Parameters.AddWithValue("@LoginID", get_id());
                        query.Parameters.AddWithValue("@Username", usernameBox.Text);
                        query.Parameters.AddWithValue("@Password", passwordBox.Text);
                        query.Parameters.AddWithValue("@Usergroup", groupCombo.SelectedItem);
                        sqlcon.Open();//open database
                        query.ExecuteNonQuery();//execute query
                        MessageBox.Show("User " + usernameBox.Text + " inserted.");//show message box   
                        sqlcon.Close();//close database

                        usernameBox.Text = String.Empty;//empty textbox2
                        passwordBox.Text = String.Empty;//empty textbox3

                        groupCombo.SelectedIndex = -1;//reset combobox1
                    }
                }
                else
                {//if false
                    MessageBox.Show("Group not selected!");//show message box
                }
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private int get_id()//auto increment login id
        {
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT MAX(LoginID) FROM Login;", sqlcon);//get the highest login id
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            sqlcon.Close();//close database
            if(output == "")
            {
                return 1;
            }
            int id = Int32.Parse(output);//convert output to integer and set it to id
            id++;//increment id
            return id;
        }

        private int get_cid()
        {
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT MAX(ClientID) FROM Client;", sqlcon);//get the highest client id
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            sqlcon.Close();//close database
            if (output == "")
            {
                return 1;
            }
            int id = Int32.Parse(output);//convert output to integer and set it to id
            id++;//increment id
            return id;
        }

        public bool usernameExists(string username) //return true if the username already exists in the database
        {
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT Username FROM Login WHERE Username = @Username;", sqlcon);
            query.Parameters.AddWithValue("@Username", username); //check function parameter

            bool name = false; //false by default (assume the username does not exist)

            if (query.ExecuteScalar() != null) //if something is found in the table with parameter username
                name = true; //set bool to true, username exists

            sqlcon.Close();

            return name; //return bool
        }
    }
}
