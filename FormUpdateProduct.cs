using login.classes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace login
{
    public partial class FormUpdateProduct : Form
    {
        SqlConnection sqlcon = null;//sql connection variable
        List<Supplier> suppliers = new List<Supplier>();
        public FormUpdateProduct(FormMain form)
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            update_combobox();

            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT ProductID FROM Product;", sqlcon);//get shipping id from Shipping Record entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                comboBox2.Items.Add(read.GetString(0));//add items to combobox1
            }
            read.Close();//close data reader
            sqlcon.Close();//close database;
        }

        public delegate void UpdateDelegate(object sender, UpdateEventArgs args);
        public event UpdateDelegate UpdateEventHandler;

        private void update_combobox()//update combobox
        {
            comboBox1.Items.Clear();//clear combobox
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT SupplierID, SupplierName FROM Supplier;", sqlcon);//get product brand and product name from Product entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                Supplier s = new Supplier();
                s.SupplierID = read.GetString(0);
                s.SupplierName = read.GetString(1);
                suppliers.Add(s);
                comboBox1.Items.Add(s.SupplierName);//add items to combobox1
            }
            read.Close();//close data reader
            sqlcon.Close();//close database
        }

        public class UpdateEventArgs : EventArgs
        {
            public string Data { get; set; }
        }

        protected void update_main_form()//update gridview in main form
        {
            UpdateEventArgs args = new UpdateEventArgs();//create new update event args object
            UpdateEventHandler.Invoke(this, args);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            //update
            if (comboBox2.SelectedIndex == -1)
            {
                //make sure productID is selected
                MessageBox.Show("Please select a Product ID.");
            }
            else if (String.IsNullOrEmpty(brandBox.Text) && String.IsNullOrEmpty(nameBox.Text) && String.IsNullOrEmpty(priceBox.Text) &&
                String.IsNullOrEmpty(quantBox.Text) && String.IsNullOrEmpty(expBox.Text) && String.IsNullOrEmpty(locBox.Text) && String.IsNullOrEmpty(textBox1.Text) && comboBox1.SelectedIndex == -1)
            //make sure at least one update field is filled
            {
                MessageBox.Show("Fill at least one field to update.");
            }
            else
            {

                sqlcon.Open();
                SqlCommand query = new SqlCommand("SELECT SupplierID FROM Supplier WHERE SupplierName = @SupplierName;", sqlcon);

                var updateStr = "UPDATE Product SET "; //begin SQL string
                //update SQL string for each textbox
                updateStr += update("productBrand", brandBox.Text);
                updateStr += update("productName", nameBox.Text);
                updateStr += update("price", priceBox.Text);
                updateStr += update("quantity", quantBox.Text);
                updateStr += update("expirationDate", expBox.Text);
                updateStr += update("productLocation", locBox.Text);
                updateStr += update("Barcode", textBox1.Text);

                //update supplier from combobox
                if (comboBox1.SelectedIndex > -1)//check if something is selected in combobox1
                {
                    query.Parameters.AddWithValue("@SupplierName", comboBox1.Text);
                    string supplierid = query.ExecuteScalar().ToString();
                    updateStr += update("supplierID", supplierid);
                }

                updateStr = updateStr.Trim().TrimEnd(','); //remove comma
                updateStr += " where productId = '" + comboBox2.Text + "'"; //finish the SQL string

                SqlCommand updateQuery = new SqlCommand(updateStr, sqlcon); //assign query strin
                updateQuery.ExecuteNonQuery(); //execute query
                update_main_form(); //update form after product update

                MessageBox.Show("Product ID: " + comboBox2.Text + " successfully updated.");
                comboBox2.SelectedIndex = -1;
                brandBox.Text = String.Empty;
                nameBox.Text = String.Empty;
                priceBox.Text = String.Empty;
                quantBox.Text = String.Empty;
                expBox.Text = String.Empty;
                locBox.Text = String.Empty;
                textBox1.Text = String.Empty;
                comboBox1.SelectedIndex = -1;

                sqlcon.Close();
            }

        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            //Exit
            Close();
        }

        //function will update SQL string if corresponding text box is not empty
        public string update (String s, String t) 
        {
            string updateQuery = string.Empty;

            if (!String.IsNullOrEmpty(t))
            {
                updateQuery = " " + s + "='" + t + "', ";
            }

            return updateQuery;
        }
    }
}
