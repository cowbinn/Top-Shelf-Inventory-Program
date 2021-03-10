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
    public partial class FormDeleteProduct : Form
    {
        SqlConnection sqlcon = null;//sql connection variable

        public FormDeleteProduct(FormMain form)//constructor
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            update_combobox();//update combobox
        }

        public delegate void UpdateDelegate(object sender, UpdateEventArgs args);
        public event UpdateDelegate UpdateEventHandler;

        public class UpdateEventArgs : EventArgs
        {
            public string Data { get; set; }
        }

        protected void update_main_form()//update gridview in main form
        {
            UpdateEventArgs args = new UpdateEventArgs();//create new update event args object
            UpdateEventHandler.Invoke(this, args);
        }

        private void update_combobox()//update combobox
        {
            comboBox1.Items.Clear();//clear combobox
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT ProductBrand, ProductName FROM Product;", sqlcon);//get product brand and product name from Product entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                comboBox1.Items.Add(read.GetString(0) + " " + read.GetString(1));//add items to combobox1
            }
            read.Close();//close data reader
            sqlcon.Close();//close database
        }

        private void Button2_Click(object sender, EventArgs e)//exit delete product form
        {   
            Close();//close delete product form
        }

        private void Button1_Click(object sender, EventArgs e)//delete product
        {
            SqlCommand query = new SqlCommand("DELETE FROM Product WHERE productID = @productID", sqlcon);//delete product from database
            SqlCommand query1 = new SqlCommand("SELECT ProductID FROM Product WHERE ProductBrand + ' ' + ProductName = '" + comboBox1.SelectedItem + "';", sqlcon);//get product id from product brand + product name in combobox1
            SqlCommand query2 = new SqlCommand("DELETE FROM IncomingProduct WHERE ProductID = @ProductID;", sqlcon);
            SqlCommand query3 = new SqlCommand("DELETE FROM OrderList WHERE ProductID = @ProductID;", sqlcon);
            if(comboBox1.SelectedIndex > -1)//check if something is selected in combobox1
            {//if true
                sqlcon.Open();//open database
                string output = query1.ExecuteScalar().ToString();////set output to value output from executing the query
                query2.Parameters.AddWithValue("@ProductID", output);
                query2.ExecuteNonQuery();
                query3.Parameters.AddWithValue("@ProductID", output);
                query3.ExecuteNonQuery();
                query.Parameters.AddWithValue("@productID", output);//set product id to text in value in output
                query.ExecuteNonQuery();//execute query
                update_main_form();//update grid view in main form
                MessageBox.Show("Product Deleted.");//show message box
                sqlcon.Close();//close database
                update_combobox();//update combobox1
                comboBox1.Text = string.Empty;//reset combobox1
            }
            else
            {
                MessageBox.Show("Product not selected");//show message box
            }
        }
    }
}
