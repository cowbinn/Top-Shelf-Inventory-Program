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
    public partial class FormUpdateSupplier : Form
    {
        SqlConnection sqlcon = null;//sql connection variable

        public FormUpdateSupplier()
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function

            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT SupplierID FROM Supplier;", sqlcon);//get shipping id from Shipping Record entity
            SqlDataReader read = query.ExecuteReader();//execute query and store values to data reader
            while (read.Read())//while reading data from data reader
            {
                comboBox1.Items.Add(read.GetString(0));//add items to combobox1
            }
            read.Close();//close data reader
            sqlcon.Close();//close database;

            update();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //update
            if (comboBox1.SelectedIndex == -1)
            {
                //make sure productID is selected
                MessageBox.Show("Please select a Supplier Name.");
            }
            else if (String.IsNullOrEmpty(textBox1.Text) && String.IsNullOrEmpty(textBox2.Text) && String.IsNullOrEmpty(textBox3.Text))
            //make sure at least one update field is filled
            {
                MessageBox.Show("Fill at least one field to update.");
            }
            else
            {
                sqlcon.Open();

                var updateStr = "UPDATE Supplier SET "; //begin SQL string
                //update SQL string for each textbox
                updateStr += update_supplier("SupplierName", textBox1.Text);
                updateStr += update_supplier("SupplierEmail", textBox2.Text);
                updateStr += update_supplier("SupplierPhoneNumber", textBox3.Text);

                updateStr = updateStr.Trim().TrimEnd(','); //remove comma
                updateStr += " Where SupplierId = '" + comboBox1.Text + "'"; //finish the SQL string

                SqlCommand updateQuery = new SqlCommand(updateStr, sqlcon); //assign query strin
                updateQuery.ExecuteNonQuery(); //execute query

                MessageBox.Show("Supplier ID: " + comboBox1.Text + " successfully updated.");
                comboBox1.SelectedIndex = -1;
                textBox1.Text = String.Empty;
                textBox2.Text = String.Empty;
                textBox3.Text = String.Empty;

                sqlcon.Close();
                update(); //update form after product update
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormUpdateSupplier_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Supplier' table. You can move, or remove it, as needed.
            this.supplierTableAdapter.Fill(this.loginDataSet.Supplier);

        }

        public string update_supplier(String s, String t)
        {
            string updateQuery = string.Empty;

            if (!String.IsNullOrEmpty(t))
            {
                updateQuery = " " + s + "='" + t + "', ";
            }

            return updateQuery;
        }

        public void update()
        {
            this.supplierTableAdapter.Fill(this.loginDataSet.Supplier);
        }
    }
}
