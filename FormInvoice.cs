using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using System.Data.SqlClient;
using login.classes;
using Microsoft.Reporting.WinForms;

namespace login
{
    public partial class FormInvoice : Form
    {
        Orders _orders;
        User user;
        Client client;
        SqlConnection sqlcon = null;

        public FormInvoice(Orders orders, User u, Client c)
        {
            InitializeComponent();
            _orders = orders;
           
            //_list = list;
            Connection open = new Connection();
            this.sqlcon = open.connect();
            user = u;
            client = c;
        }

        private void FormPrint_Load(object sender, EventArgs e)
        {
            //Init data source 
            sqlcon.Open();
            //orderDetailBindingSource.DataSource = _list; // missing // for displaying OrderList for the order
            OrderDetail OD = new OrderDetail();
            SqlCommand cmd = sqlcon.CreateCommand();
            string queryString = "SELECT * FROM OrderList WHERE OrderID = '" + _orders.OrderID + "';";
            cmd.CommandText = queryString;
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            adap.Fill(OD, OD.Tables[0].TableName);

            ReportDataSource rds = new ReportDataSource("OrderDetail", OD.Tables[0]);
            //Set parameter for your report
            Microsoft.Reporting.WinForms.ReportParameter[] p = new Microsoft.Reporting.WinForms.ReportParameter[]
            {
                new Microsoft.Reporting.WinForms.ReportParameter("pOrderID",_orders.OrderID.ToString()),
                new Microsoft.Reporting.WinForms.ReportParameter("pOrderDate",_orders.OrderDate.ToString("MM/dd/yyyy")),
                new Microsoft.Reporting.WinForms.ReportParameter("pUsername",user.Username),
                new Microsoft.Reporting.WinForms.ReportParameter("pAddress",client.Address),
                new Microsoft.Reporting.WinForms.ReportParameter("pPhoneNumber",client.PhoneNumber),
                new Microsoft.Reporting.WinForms.ReportParameter("pUserID",client.ClientID)

            };

            this.reportViewer.LocalReport.SetParameters(p);
            this.reportViewer.LocalReport.DataSources.Clear();
            this.reportViewer.LocalReport.DataSources.Add(rds);

            this.reportViewer.LocalReport.Refresh();
            this.reportViewer.RefreshReport();
        }

    }
}