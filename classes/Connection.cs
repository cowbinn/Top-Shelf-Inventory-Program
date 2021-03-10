using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace login.classes
{
    class Connection
    {

        public SqlConnection connect()//Connection class connection function
        {
            string currentDirectory = Directory.GetCurrentDirectory();//gets the directory of the project file
            string path = Path.Combine(currentDirectory, "bin");//combine the project direcory with /bin
            path = Path.Combine(currentDirectory, "Debug");//combine the updated project directory with /Debug
            path = Path.Combine(currentDirectory, "Login.mdf");//combine the updated project direcotry with /Login.mdf
            //final project directory = project directory on ur device\login\bin\Login.mdf
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + path + ";Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=true;";//connection string
            SqlConnection sqlcon = new SqlConnection(connectionString);//create a new sql connection with connection string

            return sqlcon;//return sql conneciton object
        }
    }
}