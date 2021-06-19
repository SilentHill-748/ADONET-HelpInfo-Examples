using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data.Common;

using Dapper;
using System.IO;

namespace ADONET.Connected
{
    //https://metanit.com/sharp/adonetcore/2.6.php

    public partial class Form1 : Form
    {
        private readonly string connectionStr = ConfigurationManager.ConnectionStrings["sqlConnString"].ConnectionString;
        private readonly string selectStudents = "SELECT * FROM StudentsView;";

        public Form1()
        {
            InitializeComponent();

            //OpenConnectionAsync();
            //LoadStudents();
            //SqlDataReaderTests();
            //DapperTests();
            //SqlParameters();
            //OutputSqlParameters();
            //ProcedureExecute();
            //ExecuteProcedureWithOutParameter();
            //LoadFileById(5);
            //ExecuteFunction("GetPictureById", 1);
            //LoadAllStudents2("Students");
            //TestExecuteReader();

            SqlConnection.ClearAllPools();
        }

        private IDbConnection CreateConnection()
        {
            IDbConnection connection = new SqlConnection(connectionStr);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }

        private void LoadStudents()
        {
            using (SqlConnection connect = new SqlConnection(connectionStr))
            {
                SqlCommand command = connect.CreateCommand();
                DataSet set = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                try
                {
                    command.CommandText = selectStudents;
                    adapter.Fill(set);
                    dataGridView1.DataSource = set.Tables[0];
                    //countLabel.Text = "Таблица Students загружена!";
                }
                catch (SqlException)
                {
                    errorLabel.Text = "Запрос уже выполняется...";
                }
                finally
                {
                    adapter.Dispose();
                    set.Dispose();
                }
            }
        }

        private async void OpenConnectionAsync()
        {
            using (SqlConnection connect = new SqlConnection(connectionStr))
            {
                if (connect.State == ConnectionState.Closed)
                    await connect.OpenAsync();
                //connectStateLabel.Text = "Connected!";

                string sql = "SELECT COUNT(*) FROM Students;";
                using (SqlCommand command = connect.CreateCommand())
                {
                    command.CommandText = sql;
                    int count = (int)await command.ExecuteScalarAsync();
                    countLabel.Text = count.ToString();
                }
            }
        }

        private async void CreateDb_Click(object sender, EventArgs e)
        {
            string sql = "CREATE DATABASE Testdb;";
            string connectionString = ConfigurationManager.ConnectionStrings["sqlConnString"].ConnectionString;

            using (var connect = new SqlConnection(connectionString))
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();
                SqlCommand command = connect.CreateCommand();
                try
                {
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    countLabel.Text = "База данных успешно создана!";
                }
                catch (SqlException)
                {
                    errorLabel.Text = "База данных уже создана!";
                }
            }
        }

        private async void DeleteDb_Click(object sender, EventArgs e)
        {
            string sql = "DROP DATABASE Testdb;";

            using (var connect = new SqlConnection(connectionStr))
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();
                SqlCommand command = connect.CreateCommand();
                try
                {
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    countLabel.Text = "База данных успешно удалена!";
                }
                catch (SqlException)
                {
                    errorLabel.Text = "Такой базы данных не существует!";
                }
            }
        }

        private async Task SendTransaction()
        {
            string sql = "USE University; INSERT INTO Students VALUES (11, 'Никита', 'Палин', 3, 2018, 2);";
            
            using (var connect = new SqlConnection(connectionStr))
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();
                SqlCommand command = connect.CreateCommand();
                SqlTransaction transaction = connect.BeginTransaction();
                try
                {
                    command.CommandText = sql;
                    command.Transaction = transaction;
                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                    countLabel.Text = "Транзакция отменена через RollBack().";
                    LoadStudents();
                }
                catch (SqlException)
                {
                    errorLabel.Text = "Транзакция не выполнена! Произошла ошибка!";
                    transaction.Rollback();
                }
            }
        }

        private async void BeginTransaction_Click(object sender, EventArgs e)
        {
            await SendTransaction();
        }

        private async void SqlDataReaderTests()
        {
            using (var connect = new SqlConnection(connectionStr))
            {
                if (connect.State != ConnectionState.Open)
                    connect.Open();
                string sql = "SELECT * FROM Students;";

                SqlCommand command = new SqlCommand(sql, connect);
                SqlDataReader studentsReader = null;
                try
                {
                    studentsReader = await command.ExecuteReaderAsync();
                    if (studentsReader.HasRows)
                    {
                        List<string> values = new List<string>();
                        while (await studentsReader.ReadAsync())
                        {
                            string name = studentsReader.GetString(1);
                            //values.Add(name);
                        }
                        comboBox1.Items.AddRange(values.ToArray());
                        //countLabel.Text = $"RecordsAffected = " + studentsReader.RecordsAffected.ToString();
                    }
                }
                catch (SqlException ex)
                {
                    errorLabel.Text = "Чтение через SqlDataReader не удалось.\n" + ex.Message;
                }
                catch (InvalidOperationException ex)
                {
                    errorLabel.Text = "Чтение через SqlDataReader не удалось.\n" + ex.Message;
                }
                catch (Exception ex)
                {
                    errorLabel.Text = "Ошибка!\n" + ex.Message;
                }
                finally
                {
                    studentsReader?.Close();
                }
            }
        }

        private void DapperTests()
        {
            using (IDbConnection connection = new SqlConnection(connectionStr))
            {
                try
                {
                    List<Student> students = new List<Student>();
                    students = connection.Query<Student>("SELECT * FROM Students;").ToList();
                    comboBox1.Items.Clear();
                    comboBox1.Items.AddRange(students.Select(s => s.Group_Id).ToArray());
                }
                catch (Exception ex)
                {
                    errorLabel.Text = ex.Message;
                }
            }
        }

        private void SqlParameters()
        {
            IDbConnection connect = CreateConnection();
            string sql = "SELECT * FROM StudentsView WHERE [Фамилия] = @surname;";

            try
            {
                IDbCommand command = connect.CreateCommand();
                command.CommandText = sql;
                SqlParameter surname = new SqlParameter("@surname", SqlDbType.NVarChar, 30);
                surname.Value = "Иванов";
                surname.IsNullable = false;

                command.Parameters.Add(surname);

                DataSet set = new DataSet();
                using (var adapter = new SqlDataAdapter((SqlCommand)command))
                {
                    adapter.Fill(set);
                    dataGridView1.DataSource = set.Tables[0];
                }
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
            finally
            {
                connect.Close();
            }
        }

        private void OutputSqlParameters()
        {
            IDbConnection connection = CreateConnection();
            string sql = "SET @name = (SELECT name FROM Students WHERE surname = @surname);";

            try
            {
                DbCommand command = (SqlCommand)connection.CreateCommand();
                DbParameter name = new SqlParameter()
                {
                    ParameterName = "@name",
                    Direction = ParameterDirection.Output,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 100
                };
                DbParameter surname = new SqlParameter("@surname", "Иванов");

                command.Parameters.AddRange(new DbParameter[] { name, surname });
                command.CommandText = sql;
                command.ExecuteNonQuery();
                MessageBox.Show($"{name.Value}", "Сообщение!");
            }
            catch (SqlException sqlEx)
            {
                errorLabel.Text = sqlEx.Message;
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        private bool ProcedureCreate()
        {
            string sqlExpression = "create procedure dbo.InsertStudent " +
                "@id int, @name nvarchar(20), @surname nvarchar(30), " +
                "@group_id int, @role_id int AS " +
                "BEGIN Insert into students values (@id, @name, @surname, @group_id, @role_id) " +
                "SELECT @id END";
            IDbConnection connection = CreateConnection();

            try
            {
                DbCommand command = (SqlCommand)connection.CreateCommand();
                command.CommandText = sqlExpression;
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
                return false;
            }
        }

        private void ProcedureExecute()
        {
            string sql = "dbo.InsertStudent";
            IDbConnection connection = CreateConnection();
            DbCommand command = (SqlCommand)connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.StoredProcedure;

            DbParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", 13),
                new SqlParameter("@name", "Никита"),
                new SqlParameter("@surname", "Палин"),
                new SqlParameter("@group_id", 1),
                new SqlParameter("@role_id", 1)
            };
            command.Parameters.AddRange(parameters);

            try
            {
                ProcedureCreate();
                object addedid = command.ExecuteScalar();
                MessageBox.Show($"ID добавленного студента: {addedid}", "Сообщение!");
                LoadStudents();
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        private void ExecuteProcedureWithOutParameter()
        {
            DbCommand command = new SqlCommand("GetStudentRoleByID", (SqlConnection)CreateConnection());
            command.CommandType = CommandType.StoredProcedure;

            DbParameter id = new SqlParameter("@id", 1);
            DbParameter rolename = new SqlParameter()
            {
                ParameterName = "@rolename",
                Size = 20,
                SqlDbType = SqlDbType.NVarChar,
                Direction = ParameterDirection.Output
            };

            command.Parameters.AddRange(new object[] {id, rolename });
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show($"Роль студента под ID = 1: {command.Parameters["@rolename"].Value}",
                    "Сообщение.");
            }
            catch (Exception ex)
            {
                errorLabel.Text = ex.Message;
            }
        }

        // Отправка в БД всех файлов. directoryPath - путь к папкке с картинками.
        private void AddFilesToDatabase(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new Exception("Такой папки нет!");
            DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
            FileInfo[] files = dirInfo.GetFiles();

            IDbConnection connect = CreateConnection();
            DbCommand command = new SqlCommand("", (SqlConnection)connect);
            command.Parameters.Add(new SqlParameter("@imageData", SqlDbType.Image));
            for (var i = 0; i < files.Length; i++)
            {
                // Add files to db.
                string filename = files[i].Name;
                byte[] imageBytes;

                using (FileStream fs = new FileStream(files[i].FullName, FileMode.Open))
                {
                    imageBytes = new byte[fs.Length];
                    fs.Read(imageBytes, 0, imageBytes.Length);
                    command.Parameters["@imageData"].Value = imageBytes;
                    command.Parameters["@imageData"].Size = imageBytes.Length;
                }
                command.CommandText = $"INSERT INTO Files (FileName, Title, ImageData) VALUES " +
                    $"('{filename}', '{dirInfo.Name}', @imageData); ";
                command.ExecuteNonQuery();
            }
            connect.Close();
        }

        private void ShowFileInDatabase()
        {
            DataSet set = new DataSet();
            var connection = CreateConnection();
            SqlCommand command = new SqlCommand("SELECT * FROM Files", (SqlConnection)connection);
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(set);
                dataGridView1.DataSource = set.Tables[0];
            }
            connection.Close();
        }

        private void LoadFileById(int i)
        {
            ShowFileInDatabase();

            string sql = "SELECT ImageData FROM Files WHERE Id = @id";
            var connect = (SqlConnection)CreateConnection();
            SqlCommand command = new SqlCommand(sql, connect);

            command.Parameters.Add(new SqlParameter("@id", i));
            byte[] imageData = command.ExecuteScalar() as byte[];

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(imageData, 0, imageData.Length);
                pictureBox1.Image = Image.FromStream(ms);
            }
            connect.Close();
        }

        private void ExecuteFunction(string funcName, int id)
        {
            var connect = (SqlConnection)CreateConnection();
            SqlCommand command = new SqlCommand(funcName, connect);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter outParam = new SqlParameter()
            {
                ParameterName = "@picture",
                SqlDbType = SqlDbType.VarBinary,
                Direction = ParameterDirection.ReturnValue
            };
            command.Parameters.Add(outParam);
            command.Parameters.AddWithValue("@id", id);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException sqlex)
            {
                errorLabel.Text = sqlex.Message;
                return;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] picture = command.Parameters["@picture"].Value as byte[];
                ms.Write(picture, 0, picture.Length);
                pictureBox1.Image = Image.FromStream(ms);
            }
        }

        private void LoadAllStudents2(string tableName)
        {
            var connect = (SqlConnection)CreateConnection();
            SqlCommand command = connect.CreateCommand();
            command.CommandText = tableName;
            // В .NET Framework тип комманды ниже не поддерживается для MSSQL Server.
            command.CommandType = CommandType.TableDirect;

            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }
        }

        private void TestExecuteReader()
        {
            var connect = (SqlConnection)CreateConnection();
            var command = connect.CreateCommand();
            command.CommandText = "SELECT * FROM Students;";
            command.Prepare();

            using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
            {
                
            }
            errorLabel.Text = connect.State.ToString();
        }
    }
}
