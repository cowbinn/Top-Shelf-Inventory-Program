using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace login.classes
{

    class Alert
    {
        const int QUANTITY = 5; //will alert when stock is less than this number
        const int EXPIRATION = 5; //will alert when this many days from expiration (or already expired)

        SqlConnection sqlcon = null; //sql connection variable

        public void checkStock ()
        {
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            sqlcon.Open();

            SqlCommand query = new SqlCommand("SELECT ProductID, ProductName, Quantity FROM Product WHERE Quantity < " + QUANTITY, sqlcon); //get products under certain quantity
            SqlDataReader read = query.ExecuteReader(); //execute query and store values to data reader
            List<Product> products = new List<Product>();
            while (read.Read())//while reading data from data reader
            {
                Product p = new Product(); //create a new product object and put in list
                p.ProductID = read.GetString(0);
                p.ProductName = read.GetString(1);
                p.Quantity = read.GetString(2);
                products.Add(p);
            }
            read.Close();//close data reader
            sqlcon.Close();//close database

            //iterate through list and make the string for the message box
            string message = string.Empty;
            foreach (var p in products) //add a line for each low product
            {
                message += "ProductID #" + p.ProductID + ", " + p.ProductName + ", is low on stock. " + p.Quantity + " stock remaining.\n";
            }
            MessageBox.Show(message);
        }
        public void checkExp ()
        {
            Connection open = new Connection();// create a connection object
            this.sqlcon = open.connect();//set sqlcon to the sql connection object returned from the connect function
            sqlcon.Open();

            SqlCommand query = new SqlCommand("SELECT ProductID, ProductName, ExpirationDate FROM Product", sqlcon); //get products under certain quantity
            SqlDataReader read = query.ExecuteReader(); //execute query and store values to data reader
            List<Product> products = new List<Product>();
            while (read.Read())//while reading data from data reader
            {
                Product p = new Product(); //create a new product object and put in list
                p.ProductID = read.GetString(0);
                p.ProductName = read.GetString(1);
                p.ExpirationDate = read.GetString(2);
                products.Add(p);
            }
            read.Close();//close data reader
            sqlcon.Close();//close database

            //iterate through list and make the string for the message box
            string message = string.Empty;
            DateTime nowDate = DateTime.Now; //get current date
            foreach (var p in products) //add a line for each low product
            {
                DateTime expDate = Convert.ToDateTime(p.ExpirationDate); //convert date string to date object
                

                if (expDate <= nowDate)
                    message += "ProductID #" + p.ProductID + ", " + p.ProductName + ", expired on " + p.ExpirationDate + ".\n";
                else if (expDate <= nowDate.AddDays(EXPIRATION))
                    message += "ProductID #" + p.ProductID + ", " + p.ProductName + ", will expire on " + p.ExpirationDate + ".\n";

            }
            MessageBox.Show(message);
        }
    }

}
