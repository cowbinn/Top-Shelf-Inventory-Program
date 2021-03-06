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
    public partial class FormProductBarcode : Form
    {

        SqlConnection sqlcon = null;//sql connection variable

        public FormProductBarcode()//constructor
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function

            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT ProductBrand, ProductName FROM Product;", sqlcon);//get product brand and product name from Product entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                comboBox1.Items.Add(read.GetString(0) + " " + read.GetString(1));//add items to combobox1
            }
            read.Close();//close data reader
            sqlcon.Close();//close database;
        }

        private void button1_Click(object sender, EventArgs e)//search product quantity
        {
            SqlCommand query = new SqlCommand("SELECT ProductID FROM Product WHERE ProductBrand + ' ' + ProductName = '" + comboBox1.SelectedItem + "';", sqlcon);//get product id from product brand + product name in combobox1
            if (comboBox1.SelectedIndex > -1)//check if something is selected in combobox1
            {
                sqlcon.Open();//open database
                string output = query.ExecuteScalar().ToString();//set output to value output from executeing the query
                SqlCommand query1 = new SqlCommand("SELECT Barcode FROM Product WHERE ProductID = @ProductID", sqlcon);//get product brand from product entity
                query1.Parameters.AddWithValue("@ProductID", output);//set product id to text in value in output
                SqlDataReader read = query1.ExecuteReader();
                read.Read();
                string barcodeNumber = read.GetString(0);//set quantity to value output from executing the query
                read.Close();
                Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                pictureBox1.Image = barcode.Draw(barcodeNumber, 500);
                label3.Text = barcodeNumber;
                sqlcon.Close();//close database
            }
            else
            {
                MessageBox.Show("Product Not Selected.");//show message box
            }
        }

        private void button2_Click(object sender, EventArgs e)//exit search product quantity form
        {
            Close();//close search product quantity form
        }
    }
}
