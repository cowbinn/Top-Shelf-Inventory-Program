using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Dapper;
using login.classes;

namespace login
{
    public partial class FormNewOrder : Form
    {
        SqlConnection sqlcon = null;

        public FormNewOrder()
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
        }

        private void FormNewOrder_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.OrderRecord' table. You can move, or remove it, as needed.
            this.orderRecordTableAdapter.Fill(this.loginDataSet.OrderRecord);

            using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["login.Properties.Settings.LoginConnectionString"].ConnectionString))
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                }
                string query = "SELECT * FROM OrderRecord;";

                orderRecordBindingSource.DataSource = db.Query<Orders>(query, commandType: CommandType.Text);
            }

            orderStatusDataGridViewTextBoxColumn.Items.Add("New");
            orderStatusDataGridViewTextBoxColumn.Items.Add("In Progress");
            orderStatusDataGridViewTextBoxColumn.Items.Add("Shipped");
            orderStatusDataGridViewTextBoxColumn.Items.Add("Ready For Pick Up");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //View
            Orders obj = orderRecordBindingSource.Current as Orders;
            if (obj != null)
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["login.Properties.Settings.LoginConnectionString"].ConnectionString))
                {
                    if (db.State == ConnectionState.Closed)
                    {
                        db.Open();
                    }
                    // need adjustments
                    
                    using (FormOrderInfo frm = new FormOrderInfo(obj))
                    {
                        frm.ShowDialog();
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            sqlcon.Open();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[3].Value != null)
                {
                    SqlCommand query = new SqlCommand("UPDATE OrderRecord SET OrderStatus = '" + row.Cells[3].Value.ToString() + "' WHERE OrderID = '" + row.Cells[0].Value.ToString() + "';", sqlcon);
                    query.ExecuteNonQuery();
                }
                if (row.Cells[5].Value != null)
                {
                    if(row.Cells[5].Value.ToString() != " ")
                    {
                        SqlCommand query1 = new SqlCommand("UPDATE OrderRecord SET TrackingNumber = '" + row.Cells[5].Value.ToString() + "' WHERE OrderID = '" + row.Cells[0].Value.ToString() + "';", sqlcon);
                        query1.ExecuteNonQuery();
                    }
                }
            }
            sqlcon.Close();
            this.orderRecordTableAdapter.Fill(this.loginDataSet.OrderRecord);
            MessageBox.Show("Database updated.");
        }
    }
}
