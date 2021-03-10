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
    public partial class FormSearchShipmentTime : Form
    {

        SqlConnection sqlcon = null;//sql connection variable

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
            SqlCommand query = new SqlCommand("SELECT ShippingID FROM ShippingRecord;", sqlcon);//get shipping id from Shipping Record entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                comboBox1.Items.Add(read.GetString(0));//add items to combobox1
            }
            read.Close();//close data reader
            sqlcon.Close();//close database;
        }

        public FormSearchShipmentTime(FormMain form)//constructor
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            update_combobox();
        }

        private void viewButton_Click(object sender, EventArgs e)//look up expected arrival time
        {
            string message = "";//initilize string 
            if (comboBox1.SelectedIndex > -1)//check if something is selected in combobox1
            {
                sqlcon.Open();//open database
                SqlCommand query1 = new SqlCommand("SELECT ArrivalTime FROM ShippingRecord WHERE ShippingID = @ShippingID;", sqlcon);//get arrival time from shipping record
                query1.Parameters.AddWithValue("@ShippingID", comboBox1.Text);//set shipping id to text in value in combobox1
                SqlCommand query2 = new SqlCommand("SELECT P.ProductName, IP.IncomingQuantity FROM Product AS P, IncomingProduct AS IP WHERE IP.ShippingID = @ShippingID AND IP.ProductID = P.ProductID;", sqlcon);//get product name and incoming quantity from product and incoming product entity
                query2.Parameters.AddWithValue("@ShippingID", comboBox1.Text);//set shipping to text in value in combobox1
                string time = query1.ExecuteScalar().ToString();//set time to value output from executing the query
                SqlDataReader read = query2.ExecuteReader();//execute query and store values to data reader
                while (read.Read())//while reading data from data reader
                {
                    message = message + read.GetString(1) + " cases of " + read.GetString(0) + "\n";//add data to message string
                }
                read.Close();//close data reader
                MessageBox.Show("Shippment " + comboBox1.Text + " is expected to arrive on " + time + " with\n\n" + message + "\n.");//show message box
                sqlcon.Close();//close database
            }
            else
            {
                MessageBox.Show("Shipping ID Not Selected.");//show message box
            }
        }



        private DataTable dt = new DataTable();//data table object
        private DataSet ds = new DataSet();//data set object

        public DataTable Source()
        {
            sqlcon.Open();//open database
            SqlCommand cmd = sqlcon.CreateCommand();//create a sql command object
            cmd.CommandText = "SELECT * FROM ShippingRecord";//set sql command to look for everything in shippingrecord entity
            SqlDataAdapter adap = new SqlDataAdapter(cmd);//create a sql data adapter object with sql command
            ds.Clear();//clear data set
            adap.Fill(ds);//fill adapter with dataset
            dt = ds.Tables[0];//set data table to everything in data set index 0
            sqlcon.Close();//close database
            return dt;//return data table
        }

        private void exitButton_Click(object sender, EventArgs e)//exit search shipping time form
        {
            Close();//close search shipping time form
        }

        private void FormSearchShipmentTime_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.ShippingRecord' table. You can move, or remove it, as needed.
            this.shippingRecordTableAdapter.Fill(this.loginDataSet.ShippingRecord);
            shipmentGrid.DataSource = Source();
        }

        private void receiveButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)//check if something is selected in combobox1
            {
                sqlcon.Open();

                SqlCommand query1 = new SqlCommand("SELECT ProductID, IncomingQuantity FROM IncomingProduct WHERE ShippingID = @ShippingID ", sqlcon); //get products 
                query1.Parameters.AddWithValue("@ShippingID", comboBox1.Text);//set shipping id to text in value in combobox1
                SqlDataReader read = query1.ExecuteReader(); //execute query and store values to data reader
                List<Product> products = new List<Product>();

                while (read.Read())//while reading data from data reader
                {
                    Product p = new Product(); //create a new product object and put in list
                    p.ProductID = read.GetString(0);
                    p.Quantity = read.GetString(1);
                    products.Add(p);
                }

                foreach (var p in products) //update each product
                {
                    SqlCommand tempQuery = new SqlCommand("UPDATE Product SET Quantity = CAST(Quantity as INT) + CAST(@Quantity as INT) WHERE ProductID = @ProductID", sqlcon);
                    tempQuery.Parameters.AddWithValue("@ProductID", p.ProductID);
                    tempQuery.Parameters.AddWithValue("@Quantity", p.Quantity);

                    tempQuery.ExecuteNonQuery();
                }

                SqlCommand deleteQuery1 = new SqlCommand("DELETE FROM IncomingProduct WHERE ShippingID = @ShippingID", sqlcon); //delete the incoming product after update
                deleteQuery1.Parameters.AddWithValue("@ShippingID", comboBox1.Text);
                SqlCommand deleteQuery2 = new SqlCommand("DELETE FROM ShippingRecord WHERE ShippingID = @ShippingID", sqlcon); //delete the shipping id after update
                deleteQuery2.Parameters.AddWithValue("@ShippingID", comboBox1.Text);

                deleteQuery1.ExecuteNonQuery(); //execute deletion queries
                deleteQuery2.ExecuteNonQuery();

                string message = "Shipment " + comboBox1.Text + " received, products have been updated."; //inform user of update
                MessageBox.Show(message);

                SqlCommand cmd = sqlcon.CreateCommand();//create a sql command object
                cmd.CommandText = "SELECT * FROM ShippingRecord";//set sql command to look for everything in shippingrecord entity
                SqlDataAdapter adap = new SqlDataAdapter(cmd);//create a sql data adapter object with sql command
                ds.Clear();//clear data set
                adap.Fill(ds);//fill adapter with dataset
                dt = ds.Tables[0];//set data table to everything in data set index 0

                read.Close();//close data reader
                sqlcon.Close();//close database

                update_main_form(); //update main grid
                update_combobox();
            }
            else
            {
                MessageBox.Show("Shipping ID Not Selected.");//show message box
            }
        }
    
    }
}
