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
    public partial class FormDeleteSupplier : Form
    {
        SqlConnection sqlcon = null;
        int count = 0;

        public FormDeleteSupplier(FormMain form)
        {
            InitializeComponent();
            Connection open = new Connection();
            this.sqlcon = open.connect();
            update_combobox();//initilize combobox
            update();
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

        private void update_combobox()//refresh combobox
        {
            comboBox1.Items.Clear();
            sqlcon.Open();
            SqlCommand query = new SqlCommand("SELECT SupplierName FROM Supplier;", sqlcon);
            SqlDataAdapter da = new SqlDataAdapter(query);
            DataSet ds = new DataSet();
            da.Fill(ds);
            count = ds.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                comboBox1.Items.Add(ds.Tables[0].Rows[i][0]);
            }
            count--;
            sqlcon.Close();
        }

        public void update()
        {
            this.supplierTableAdapter.Fill(this.loginDataSet.Supplier);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sqlcon.Open();
            SqlCommand query = new SqlCommand("DELETE FROM Supplier WHERE SupplierID = @SupplierID", sqlcon);
            SqlCommand query1 = new SqlCommand("SELECT SupplierID FROM Supplier WHERE SupplierName = '" + comboBox1.SelectedItem + "';", sqlcon);
            SqlCommand query2 = new SqlCommand("DELETE FROM Product WHERE SupplierID = @SupplierID", sqlcon);
            SqlDataReader read = query1.ExecuteReader();
            read.Read();
            string output = read.GetString(0);
            read.Close();
            query2.Parameters.AddWithValue("@SupplierID", output);
            query2.ExecuteNonQuery();
            query.Parameters.AddWithValue("@SupplierID", output);
            query.ExecuteNonQuery();
            sqlcon.Close();
            MessageBox.Show("Supplier Deleted.");
            update_combobox();
            comboBox1.Text = string.Empty;
            update();
            update_main_form();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormDeleteSupplier_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Supplier' table. You can move, or remove it, as needed.
            this.supplierTableAdapter.Fill(this.loginDataSet.Supplier);

        }
    }
}
