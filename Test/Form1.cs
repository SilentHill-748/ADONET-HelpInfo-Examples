using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;

namespace Test
{
    public partial class Form1 : Form
    {
        private readonly string connectionStr = ConfigurationManager.ConnectionStrings["sqlConnString"].ConnectionString;
        private readonly string selectStudents = "SELECT s.name 'Имя', s.surname 'Фамилия', g.name 'Группа', s.year 'Год', " +
                        "r.name 'Роль в группе' FROM Students s JOIN Groups g ON g.id = s.group_id " +
                        "LEFT JOIN StudentRoles r ON r.id = s.role_id;";

        public Form1()
        {
            InitializeComponent();

            FillListView();
        }

        private async void FillListView()
        {
            using (var connect = new SqlConnection(connectionStr))
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();

                SqlCommand command = new SqlCommand(selectStudents, connect);
                SqlDataReader studentsReader = await command.ExecuteReaderAsync();

                try
                {
                    if (studentsReader.HasRows)
                    {
                        ListViewItem item = new ListViewItem();
                        for (var k = 0; k < studentsReader.FieldCount; k++)
                            listView1.Items.Add(studentsReader.GetName(k));
                        
                        while (studentsReader.Read())
                        {
                            for (var l = 0; l < listView1.Items.Count; l++)
                            {
                                listView1.Items[l].SubItems.Add(studentsReader.GetValue(l).ToString());
                            }
                        }
                    }
                }
                catch (SqlException)
                {
                    errorLabel.Text = "Чтение через SqlDataReader не удалось.";
                }
                catch (InvalidOperationException ex)
                {
                    errorLabel.Text = "Чтение через SqlDataReader не удалось.\n" + ex.Message;
                }
                finally
                {
                    studentsReader.Close();
                }
            }
        }

    }
}
