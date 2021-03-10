using login.classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace login
{
    public partial class FormViewAccounts : Form
    {
        SqlConnection sqlcon = null;//sql connection object

        public FormViewAccounts()
        {
            InitializeComponent();

            Connection open = new Connection();//create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
        }

        private void FormViewAccounts_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'loginDataSet.Login' table. You can move, or remove it, as needed.
            this.loginTableAdapter.Fill(this.loginDataSet.Login);
            loginGrid.DataSource = Source();
        }

        private DataTable dt = new DataTable();//data table object
        private DataSet ds = new DataSet();//data set object

        public DataTable Source()
        {
            sqlcon.Open();//open database
            SqlCommand cmd = sqlcon.CreateCommand();//create a sql command object
            cmd.CommandText = "SELECT * FROM Login";//set sql command to look for everything in Login entity
            SqlDataAdapter adap = new SqlDataAdapter(cmd);//create a sql data adapter object with sql command
            ds.Clear();//clear data set
            adap.Fill(ds);//fill adapter with dataset
            dt = ds.Tables[0];//set data table to everything in data set index 0
            sqlcon.Close();//close database
            return dt;//return data table
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
