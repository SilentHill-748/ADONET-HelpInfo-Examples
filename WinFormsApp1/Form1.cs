using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private async Task<SqlConnection> CreateConnectionAsync()
        {
            string connectionString = "Server=localhost;Database=university;Trusted_Connection=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            if (connection.State != ConnectionState.Open)
               await connection.OpenAsync();
            return connection;
        }

    }
}
