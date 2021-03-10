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
    public partial class FormInsertSupplier : Form
    {

        SqlConnection sqlcon = null;//sql connection variable

        public FormInsertSupplier()//constructor
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            update();
            update_id();
        }

        public void update()
        {
            this.supplierTableAdapter.Fill(this.loginDataSet.Supplier);
        }

        private void update_id()//auto increment product id
        {
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT MAX(SupplierID) FROM Supplier;", sqlcon);//get the highest product id from Product enyity
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            sqlcon.Close();//close database
            if(output == "")
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

        private void Button1_Click(object sender, EventArgs e)
        {
            sqlcon.Open();
            SqlCommand query = new SqlCommand("INSERT INTO Supplier VALUES (@SupplierID, @SupplierName, @SupplierEmail, @SupplierPhoneNumber);", sqlcon);
            query.Parameters.AddWithValue("@SupplierID", textBox1.Text);
            query.Parameters.AddWithValue("@SupplierName", textBox2.Text);
            query.Parameters.AddWithValue("@SupplierEmail", textBox3.Text);
            query.Parameters.AddWithValue("@SupplierPhoneNumber", textBox4.Text);
            query.ExecuteNonQuery();
            sqlcon.Close();
            MessageBox.Show("Supplier " + textBox1.Text + " inserted.");
            update();
            //update gridview in form2
            update_id();
            textBox2.Text = String.Empty;
            textBox3.Text = String.Empty;
            textBox4.Text = String.Empty;
        }

        private void FormInsertSupplier_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Supplier' table. You can move, or remove it, as needed.
            this.supplierTableAdapter.Fill(this.loginDataSet.Supplier);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
