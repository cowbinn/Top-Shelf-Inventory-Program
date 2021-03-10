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
    public partial class FormOrder : Form
    {

        SqlConnection sqlcon = null;//sql connection variable
        Client client;
        User user;
        ComboBox combo;
        double total = 0;
        int rowIndex;

        public FormOrder(User u, Client c)
        {
            InitializeComponent();
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            user = u;
            client = c;
        }

        private void FormOrder_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Product' table. You can move, or remove it, as needed.
            this.productTableAdapter.Fill(this.loginDataSet.Product);

            SelectQuantity.Items.Add(" ");
            for(int i = 0; i < 10; i++)
            {
                int num = i + 1;
                string temp = num.ToString();
                SelectQuantity.Items.Add(temp);
            }

            comboBox1.Items.Add("USPS");
            comboBox1.Items.Add("UPS");
            comboBox1.Items.Add("FedEx");
            comboBox1.Items.Add("Pick Up");
        }


        private string update_orderid()//auto increment product id
        {
            sqlcon.Open();//open database
            SqlCommand query = new SqlCommand("SELECT MAX(OrderID) FROM OrderRecord;", sqlcon);//get the highest product id from Product enyity
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            sqlcon.Close();//close database
            if(output == "")
            {
                return "00001";
            }
            int id = Int32.Parse(output);//convert output to integer and set it to id
            id++;//increment id
            output = id.ToString().PadLeft(5, '0');//set output to id
            return output.ToString();
        }

        private string update_orderlistid()//auto increment product id
        {
            SqlCommand query = new SqlCommand("SELECT MAX(OrderListID) FROM OrderList;", sqlcon);//get the highest product id from Product enyity
            string output = query.ExecuteScalar().ToString();//set output to value output from executed query
            if (output == "")
            {
                return "00001";
            }
            int id = Int32.Parse(output);//convert output to integer and set it to id
            id++;//increment id
            output = id.ToString().PadLeft(5, '0');//set output to id
            return output.ToString();
        }

        private int check1()
        {
            int temp = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[5].Value != null)
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

        private bool check2()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if(row.Cells[5].Value != null)
                {
                    if (row.Cells[5].Value.ToString() != " ")
                    {
                        if (Convert.ToInt32(row.Cells[4].Value) < Convert.ToInt32(row.Cells[5].Value))
                        {
                            MessageBox.Show("Not enough " + row.Cells[1].Value + " " + row.Cells[2].Value + " in stock!");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void refresh()
        {
            this.productTableAdapter.Fill(this.loginDataSet.Product);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (check1() != 0)
            {
                if (check2())
                {
                    if(comboBox1.SelectedIndex > -1)
                    {
                        string id = update_orderid();
                        string queryCommand = "INSERT INTO OrderRecord (OrderID, ClientID, OrderDate, OrderStatus, DeliveryAddress, ShippingMethod) VALUES ('" + id + "', '" + client.ClientID + "', convert(date,getdate()), 'New', '" + client.Address + "', '" + comboBox1.SelectedItem.ToString() + "')";
                        SqlCommand query = new SqlCommand(queryCommand, sqlcon);
                        sqlcon.Open();//open database
                        query.ExecuteNonQuery();//execute query
                        sqlcon.Close();//close database
                        double num1, num2, num3;
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells[5].Value != null)
                            {
                                if (row.Cells[5].Value.ToString() != " ")
                                {
                                    sqlcon.Open();
                                    num1 = double.Parse(row.Cells[3].Value.ToString());
                                    num2 = double.Parse(row.Cells[4].Value.ToString());
                                    num3 = double.Parse(row.Cells[5].Value.ToString());
                                    num2 = num2 - num3;
                                    SqlCommand query1 = new SqlCommand("INSERT INTO OrderList VALUES ('" + update_orderlistid() + "', '" + row.Cells[0].Value.ToString() + "', '" + row.Cells[1].Value.ToString() + "', '" + row.Cells[2].Value.ToString() + "', '" + num3 + "', '" + num1 + "', '" + (num1 * num3).ToString() + "', '" + id + "')", sqlcon);
                                    SqlCommand query2 = new SqlCommand("UPDATE Product SET Quantity = '" + num2 + "' WHERE ProductID = '" + row.Cells[0].Value.ToString() + "';", sqlcon);
                                    query1.ExecuteNonQuery();
                                    query2.ExecuteNonQuery();
                                    sqlcon.Close();
                                }
                            }
                        }
                        MessageBox.Show("Order " + id + " is placed.");//show message box   
                        refresh();
                    }
                    else
                    {
                        MessageBox.Show("Shipping method not selected.");
                    }
                }
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            combo = e.Control as ComboBox;
            if (combo != null){
                rowIndex = dataGridView1.CurrentCell.RowIndex;
                combo.SelectedIndexChanged -= new EventHandler(combo_SelectedIndexChanged);

                combo.SelectedIndexChanged += combo_SelectedIndexChanged;
            }
        }

        private void combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if(row.Cells[5].Value != null)
                {
                    if(row.Cells[5].Value.ToString() != " ")
                    {
                        total += Convert.ToDouble(row.Cells[5].Value) * Convert.ToDouble(row.Cells[3].Value);
                    }
                }
            }
            double selected = Convert.ToDouble((sender as ComboBox).SelectedIndex);
            total += selected * Convert.ToDouble(dataGridView1.Rows[rowIndex].Cells[3].Value);
            label1.Text = "$" + total;
            total = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormAccount form = new FormAccount(user, client);
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormOrderHistory form = new FormOrderHistory(user, client);
            form.ShowDialog();
        }
    }
}
