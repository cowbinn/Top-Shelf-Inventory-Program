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
    public partial class FormMain : Form
    {

        SqlConnection sqlcon = null;//sql connection object
        User user; //user object

        public FormMain(User u)
        {
            InitializeComponent();
            user = u;
            if (u.Group == "Admin")
                adminTab.Visible = true;
            else if (u.Group == "Stocker")
            {
                insertToolStripMenuItem.Visible = false;
                deleteToolStripMenuItem.Visible = false;
                updateToolStripMenuItem.Visible = false;
            }

            Connection open = new Connection();//create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
        }

        private void button1_Click(object sender, EventArgs e)//open search product quantity form
        {
            FormSearchProductQuantity form = new FormSearchProductQuantity();//create search product quantity form object
            form.ShowDialog();//show search product quantity form
        }


        private void Button2_Click(object sender, EventArgs e)//open search product location form
        {
            FormSearchProductLocation form = new FormSearchProductLocation();//create search product location form object
            form.ShowDialog();//show search product location form
        }

        private void button3_Click(object sender, EventArgs e)//open search shipment expected arrival time form
        {
            FormSearchShipmentTime form = new FormSearchShipmentTime(this);//create search expected arrival time form object
            form.UpdateEventHandler += FormSearchShipmentTime_UpdateEventHandler;//update gridview when updateeventhandler is called in delete product form object
            form.ShowDialog();//show seach expected arrival time form
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Product' table. You can move, or remove it, as needed.
            this.productTableAdapter.Fill(this.loginDataSet.Product);
            dataGridView1.DataSource = Source();//fill grid view with data table from source() function

        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            SqlCommand query = new SqlCommand("SELECT COUNT(OrderID) FROM OrderRecord WHERE OrderStatus = 'New';", sqlcon);
            SqlCommand query1 = new SqlCommand("SELECT COUNT(OrderID) FROM OrderRecord WHERE OrderStatus = 'In Progress';", sqlcon);
            sqlcon.Open();
            string count1 = query.ExecuteScalar().ToString();
            string count2 = query1.ExecuteScalar().ToString();
            sqlcon.Close();
            MessageBox.Show("You have " + count1 + " new order(s) and " + count2 + " order(s) in progress.");

            //check for low and soon to expire/expired stock
            Alert a = new Alert();
            a.checkStock();
            a.checkExp();
        }

        private DataTable dt = new DataTable();//data table object
        private DataSet ds = new DataSet();//data set object
        
        public DataTable Source()
        {
            sqlcon.Open();//open database
            SqlCommand cmd = sqlcon.CreateCommand();//create a sql command object
            cmd.CommandText = "SELECT * FROM Product";//set sql command to look for everything in Product entity
            SqlDataAdapter adap = new SqlDataAdapter(cmd);//create a sql data adapter object with sql command
            ds.Clear();//clear data set
            adap.Fill(ds);//fill adapter with dataset
            dt = ds.Tables[0];//set data table to everything in data set index 0
            sqlcon.Close();//close database
            return dt;//return data table
        }

        private void Form3_UpdateEventHandler(object sender, FormInsertProduct.UpdateEventArgs args)
        {
            dataGridView1.DataSource = Source();//fill grid view with data table from source() function
        }


        private void FormUpdateProduct_UpdateEventHandler(object sender, FormUpdateProduct.UpdateEventArgs args)
        {
            dataGridView1.DataSource = Source();//fill grid view with data table from source() function
        }

        private void FormSearchShipmentTime_UpdateEventHandler(object sender, FormSearchShipmentTime.UpdateEventArgs args)
        {
            dataGridView1.DataSource = Source();//fill grid view with data table from source() function
        }

        private void Form4_UpdateEventHandler(object sender, FormDeleteProduct.UpdateEventArgs args)
        {
            dataGridView1.DataSource = Source();//fill grid view with data table from source() function
        }

        private void FormDeleteSupplier_UpdateEventHandler(object sender, FormDeleteSupplier.UpdateEventArgs args)
        {
            dataGridView1.DataSource = Source();//fill grid view with data table from source() function
        }

        private void insertProductToolStripMenuItem_Click(object sender, EventArgs e)//open insert product form 
        {
            FormInsertProduct form = new FormInsertProduct(this);//create insert product form object
            form.UpdateEventHandler += Form3_UpdateEventHandler;//update gridview when updateventhandler is called in insert product form object
            form.ShowDialog();//show insert product form 
        }

        private void deleteProductToolStripMenuItem_Click(object sender, EventArgs e)//open delete product form
        {
            FormDeleteProduct form = new FormDeleteProduct(this);//create delete product form object
            form.UpdateEventHandler += Form4_UpdateEventHandler;//update gridview when updateeventhandler is called in delete product form object
            form.ShowDialog();//show delete product form
        }

        private void insertSupplierToolStripMenuItem_Click(object sender, EventArgs e)//open insert supplier form
        {
            FormInsertSupplier form = new FormInsertSupplier();//create insert supplier form object
            form.ShowDialog();//show insert supplier form
        }

        private void deleteSupplierToolStripMenuItem_Click(object sender, EventArgs e)//open delete supplier form
        {
            FormDeleteSupplier form = new FormDeleteSupplier(this);//create delete supplier form object
            form.UpdateEventHandler += FormDeleteSupplier_UpdateEventHandler;
            form.ShowDialog();//show delete supplier form 
        }

        private void updateProductToolStripMenuItem_Click(object sender, EventArgs e)//open update product form
        {
            FormUpdateProduct form = new FormUpdateProduct(this);//create update product form
            form.UpdateEventHandler += FormUpdateProduct_UpdateEventHandler;//update gridview when updateeventhandler is called in delete product form object
            form.ShowDialog();//show update product form
        }

        private void updateSupplierToolStripMenuItem_Click(object sender, EventArgs e)//open update supplier form
        {
            FormUpdateSupplier form = new FormUpdateSupplier();//create update supplier form
            form.ShowDialog();//show update supplier form
        }

        private void createAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormCreateAccount form = new FormCreateAccount();//create create account form
            form.ShowDialog();//show create account form
        }

        private void deleteAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormDeleteAccount form = new FormDeleteAccount(); //create delete account form
            form.ShowDialog(); //show delete account form
        }

        private void viewUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormViewAccounts form = new FormViewAccounts(); //create view accounts
            form.ShowDialog(); //show view account form
        }

        private void insertShippingInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormInsertShippingInfomation form = new FormInsertShippingInfomation();
            form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FormProductBarcode form = new FormProductBarcode();
            form.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormNewOrder form = new FormNewOrder();
            form.ShowDialog();
        }
    }
}
