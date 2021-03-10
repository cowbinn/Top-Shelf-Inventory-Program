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
    public partial class FormInsertShippingInfomation : Form
    {
        SqlConnection sqlcon = null;//sql connection variable

        public FormInsertShippingInfomation()
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

            update_shippingid();//auto increment product id
        }

        private void update_shippingid()//auto increment product id
        {
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT MAX(ShippingID) FROM ShippingRecord;", sqlcon);//get the highest product id from Product enyity
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            sqlcon.Close();//close database
            if(output == "")
            {
                textBox1.Text = "001";
            }
            else
            {
                int id = Int32.Parse(output);//convert output to integer and set it to id
                id++;//increment id
                output = id.ToString().PadLeft(3, '0');//set output to id
                textBox1.Text = output.ToString();//put output in textbox1
            }
        }

        private string update_productid()//auto increment product id
        {
            SqlCommand query = new SqlCommand("SELECT MAX(IncomingProductID) FROM IncomingProduct;", sqlcon);//get the highest product id from Product enyity
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            if(output == "")
            {
                return "00001";
            }
            int id = Int32.Parse(output);//convert output to integer and set it to id
            id++;//increment id
            output = id.ToString().PadLeft(5, '0');//set output to id
            return output.ToString();
        }

        private int check()
        {
            int temp = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[3].Value != null)
                {
                    temp++;
                }
            }
            if (temp == 0)
            {
                MessageBox.Show("Please selected the quantity of at least 1 product!");
                return temp;
            }
            return temp;
        }

        private void refresh()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[3].Value = null;
            }
        }

        private void FormInsertShippingInfomation_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Product' table. You can move, or remove it, as needed.
            this.productTableAdapter.Fill(this.loginDataSet.Product);
            DataGridViewComboBoxColumn box = new DataGridViewComboBoxColumn();
            box.HeaderText = "Product Quantity";
            box.Name = "Product Quantity";
            box.Items.Add(" ");
            int num;
            string temp;
            for (int i = 0; i < 10; i++)
            {
                num = i + 1;
                temp = num.ToString();
                box.Items.Add(temp);
            }
            dataGridView1.Columns.Add(box);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand query = new SqlCommand("INSERT INTO ShippingRecord VALUES (@ShippingID, @DepartureDate, @ArrivalDate, @SupplierID);", sqlcon);//insert shipping time into database
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Need more information!");//show message
            }
            else
            {
                SqlCommand query1 = new SqlCommand("SELECT SupplierID FROM Supplier WHERE SupplierName = '" + comboBox1.SelectedItem + "';", sqlcon);//get supplier id from supplier name in combobox1
                if (comboBox1.SelectedIndex > -1)//check if something is selected in combobox1
                {//if true
                    if (check() != 0)
                    {
                        sqlcon.Open();//open database
                        query.Parameters.AddWithValue("@ShippingID", textBox1.Text);//set shipping id to text in textbox1
                        query.Parameters.AddWithValue("@DepartureDate", textBox2.Text);//set departure time to text in textbox2
                        query.Parameters.AddWithValue("@ArrivalDate", textBox3.Text);//set arrival time to text in textbox3
                        string output = query1.ExecuteScalar().ToString();//set output to value output from executeing the query
                        query.Parameters.AddWithValue("@SupplierID", output);//set supplier id to text in value in output
                        query.ExecuteNonQuery();//execute query
                        sqlcon.Close();//close database
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[3].Value != null)
                            {
                                if(row.Cells[3].Value.ToString() != " ")
                                {
                                    sqlcon.Open();
                                    SqlCommand query2 = new SqlCommand("INSERT INTO IncomingProduct VALUES ('" + update_productid() + "', '" + row.Cells[0].Value.ToString() + "', '" + row.Cells[3].Value.ToString() + "', '" + textBox1.Text + "')", sqlcon);
                                    query2.ExecuteNonQuery();
                                    sqlcon.Close();
                                }
                            }
                        }
                        MessageBox.Show("Shipping information " + textBox1.Text + " inserted.");//show message box   
                        update_shippingid();//increment product id and update in textbox1
                        textBox2.Text = String.Empty;//empty textbox2
                        textBox3.Text = String.Empty;//empty textbox3
                        comboBox1.SelectedIndex = -1;//reset combobox1
                        refresh();
                    }
                }
                else
                {//if false
                    MessageBox.Show("Supplier not selected!");//show message box
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
