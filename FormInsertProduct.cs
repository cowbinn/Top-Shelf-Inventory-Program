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
    public partial class FormInsertProduct : Form
    {
        SqlConnection sqlcon = null;//sql connection variable

        public FormInsertProduct(FormMain form2)//constructor
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function

            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT SupplierName FROM Supplier;", sqlcon);//get all the supplier name from Supplier entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                comboBox1.Items.Add(read.GetString(0));//add items to combobox1
            }
            read.Close();//close data reader
            sqlcon.Close();//close database

            update_id();//auto increment product id
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

        private void update_id()//auto increment product id
        {
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT MAX(ProductID) FROM Product;", sqlcon);//get the highest product id from Product enyity
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            sqlcon.Close();//close database
            if (output == "")
            {
                textBox1.Text = "0001";
            }
            else
            {
                int id = Int32.Parse(output);//convert output to integer and set it to id
                id++;//increment id
                output = id.ToString().PadLeft(4, '0');//set output to id
                textBox1.Text = output.ToString();//put output in textbox1
            }
        }

        private bool duplicated()
        {
            SqlCommand query = new SqlCommand("SELECT ProductBrand, ProductName, SupplierID FROM Product;", sqlcon);
            string brand, name1;
            sqlcon.Open();
            SqlDataReader read = query.ExecuteReader();
            while (read.Read())
            {
                brand = read.GetString(0).Trim();
                name1 = read.GetString(1).Trim();
                if(brand == textBox2.Text.Trim() && name1 == textBox3.Text.Trim())
                {
                    read.Close();
                    sqlcon.Close();
                    return true;
                }
            }
            read.Close();
            sqlcon.Close();
            return false;
        }

        private void Button2_Click(object sender, EventArgs e)//exit insert product form
        { 
            Close();//close insert product form
        }

        private void Button1_Click(object sender, EventArgs e)//insert product 
        {
            SqlCommand query = new SqlCommand("INSERT INTO Product VALUES (@productID, @productBrand, @productName, @price, @quantity, @expirationDate, @productLocation, @supplierID, @Barcode);", sqlcon);//insert product into database
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))//check if any of the text box is empty
            {
                MessageBox.Show("Need more information!");//show message
            }
            else
            {
                if(duplicated() == false) 
                {
                    query.Parameters.AddWithValue("@productID", textBox1.Text);//set product id to text in textbox1
                    query.Parameters.AddWithValue("@productBrand", textBox2.Text);//set product brand to text in textbox2
                    query.Parameters.AddWithValue("@productName", textBox3.Text);//set product name to text in textbox3
                    query.Parameters.AddWithValue("@price", textBox4.Text);//set price to text in textbox4
                    query.Parameters.AddWithValue("@quantity", textBox5.Text);//set quantity to text in textbox5
                    query.Parameters.AddWithValue("@expirationDate", textBox6.Text);//set expiration date to text in textbox6
                    query.Parameters.AddWithValue("@productLocation", textBox7.Text);//set product location to text in textbox7
                    query.Parameters.AddWithValue("@Barcode", textBox8.Text);//set product barcode to text in textbox8
                    SqlCommand query1 = new SqlCommand("SELECT SupplierID FROM Supplier WHERE SupplierName = '" + comboBox1.SelectedItem + "';", sqlcon);//get supplier id from supplier name in combobox1
                    if (comboBox1.SelectedIndex > -1)//check if something is selected in combobox1
                    {//if true
                        sqlcon.Open();//open database
                        string output = query1.ExecuteScalar().ToString();//set output to value output from executeing the query
                        query.Parameters.AddWithValue("@SupplierID", output);//set supplier id to text in value in output
                        query.ExecuteNonQuery();//execute query
                        update_main_form();//update grid view in main form
                        MessageBox.Show("Product " + textBox1.Text + " inserted.");//show message box   
                        sqlcon.Close();//close database
                        update_id();//increment product id and update in textbox1
                        textBox2.Text = String.Empty;//empty textbox2
                        textBox3.Text = String.Empty;//empty textbox3
                        textBox4.Text = String.Empty;//empty textbox4
                        textBox5.Text = String.Empty;//empty textbox5
                        textBox6.Text = String.Empty;//empty textbox6
                        textBox7.Text = String.Empty;//empty textbox7
                        textBox8.Text = String.Empty;//empty textbox7
                        comboBox1.SelectedIndex = -1;//reset combobox1
                    }
                    else
                    {//if false
                        MessageBox.Show("Supplier not selected!");//show message box
                    }
                }
                else
                {
                    MessageBox.Show("Product Brand + Product Name already exist in database!");
                }
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Product' table. You can move, or remove it, as needed.
            this.productTableAdapter.Fill(this.loginDataSet.Product);
        }

    }
}
