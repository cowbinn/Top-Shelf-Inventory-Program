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
    public partial class FormOrderInfo : Form
    {
        Orders _orders;
        SqlConnection sqlcon = null;

        public FormOrderInfo(Orders orders)
        {
            InitializeComponent();
            _orders = orders;

            //_list = list;
            Connection open = new Connection();
            this.sqlcon = open.connect();
        }

        private void FormOderInfo_Load(object sender, EventArgs e)
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
                new Microsoft.Reporting.WinForms.ReportParameter("pOrderNumber",_orders.OrderID.ToString()),
                new Microsoft.Reporting.WinForms.ReportParameter("pOrderDate",_orders.OrderDate.ToString("MM/dd/yyyy")),
                new Microsoft.Reporting.WinForms.ReportParameter("pClientID",_orders.ClientID.ToString()),
                new Microsoft.Reporting.WinForms.ReportParameter("pAddress",_orders.DeliveryAddress.ToString())

            };

            this.reportViewer1.LocalReport.SetParameters(p);
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(rds);

            this.reportViewer1.LocalReport.Refresh();
            this.reportViewer1.RefreshReport();
        }

    }
}
