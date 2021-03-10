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
    public partial class FormDeleteAccount : Form
    {
        SqlConnection sqlcon = null;//sql connection variable
        public FormDeleteAccount()
        {
            InitializeComponent();

            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function

            update_combobox();


        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            SqlCommand query = new SqlCommand("DELETE FROM Login WHERE Username = @Username;", sqlcon);//delete product from database
            SqlCommand query1 = new SqlCommand("SELECT OrderID FROM OrderRecord WHERE ClientID = @ClientID;", sqlcon);
            SqlCommand query2;
            SqlCommand query3;
            SqlCommand query4;
            SqlCommand query5 = new SqlCommand("SELECT loginID FROM Login WHERE Username = @Username;", sqlcon);
            SqlCommand query6 = new SqlCommand("SELECT ClientID FROM Client WHERE loginID = @loginID;", sqlcon);
            SqlCommand query7 = new SqlCommand("DELETE FROM Client WHERE ClientID = @Client;", sqlcon);
   
            if (userCombo.SelectedIndex > -1)//check if something is selected in combobox1
            {//if true
                sqlcon.Open();//open database
                query5.Parameters.AddWithValue("@Username", userCombo.SelectedItem);
                string id = query5.ExecuteScalar().ToString();
                query6.Parameters.AddWithValue("@loginID", id);
                string cid = query6.ExecuteScalar().ToString();
                query1.Parameters.AddWithValue("@loginID", cid);
                SqlDataReader read1 = query1.ExecuteReader();
                while (read1.Read())
                {
                    if(read1.GetString(0) != null)
                    {
                        query2 = new SqlCommand("SELECT OrderListID FROM OrderList WHERE OrderID = '" + read1.GetString(0) + "';", sqlcon);
                        SqlDataReader read2 = query2.ExecuteReader();
                        while (read2.Read())
                        {
                            if(read2.GetString(0) != null)
                            {
                                query3 = new SqlCommand("DELETE FROM OrderList WHERE OrderListID = '" + read2.GetString(0) + "';", sqlcon);
                                query3.ExecuteNonQuery();
                            }
                        }
                        read2.Close();
                        query4 = new SqlCommand("DELETE FROM OrderRecord WHERE OrderID = '" + read1.GetString(0) + "';", sqlcon);
                        query4.ExecuteNonQuery();
                    }
                }
                read1.Close();
                query7.Parameters.AddWithValue("@Client", cid);
                query7.ExecuteNonQuery();
                query.Parameters.AddWithValue("@Username", userCombo.SelectedItem);//set username to text in combobox
                query.ExecuteNonQuery();//execute query
                MessageBox.Show("User account " + userCombo.SelectedItem + " deleted.");//show message box
                sqlcon.Close();//close database
                update_combobox();//update combobox
                userCombo.Text = string.Empty;//reset combobox
            }
            else
            {
                MessageBox.Show("Product not selected");//show message box
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void update_combobox()//update combobox
        {
            userCombo.Items.Clear();//clear combobox
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT Username FROM Login;", sqlcon);//get login id and username from login entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                string s = read.GetString(0);
                userCombo.Items.Add(s);//add items to usercombo
            }
            read.Close();//close data reader
            sqlcon.Close();//close database
        }
    }
}
