using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

using Microsoft.Data.Sqlite;
using System.IO;

namespace ADONET.SQLite
{
    // https://metanit.com/sharp/adonetcore/4.4.php
    public partial class Form1 : Form
    {
        private readonly string _connectString = 
            ConfigurationManager.ConnectionStrings["SqliteConnect"].ConnectionString;

        public Form1()
        {
            InitializeComponent();
        }

        private SqliteConnection CreateConnection()
        {
            SqliteConnection connection = new(_connectString);
            connection.Open();
            return connection;
        }

        private DataTable GetTable(string tablename)
        {
            string sql = $"SELECT * FROM {tablename}";
            using SqliteConnection connect = CreateConnection();
            SqliteCommand command = connect.CreateCommand();
            command.CommandText = sql;

            SqliteDataReader reader = command.ExecuteReader();
            return GetTableFromReader(reader);
        }

        private DataTable GetTableFromReader(SqliteDataReader reader)
        {
            DataTable table = new();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                Type columnType = reader.GetFieldType(i);
                table.Columns.Add(reader.GetName(i), columnType);
            }

            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    object[] values = new object[5];
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
            } 

            reader.Close();
            return table;
        }

        private void CreateStudentsTable()
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Students 
                (
                    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [Name] VARCHAR(20) NOT NULL,
                    [Surname] VARCHAR(30) NOT NULL,
                    [Group] VARCHAR(20) NOT NULL,
                    [Role] VARCHAR(30) NULL
                );";

            using SqliteConnection connect = CreateConnection();
            using SqliteCommand command = connect.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        private void CreateStudents_Click(object sender, EventArgs e)
        {
            CreateStudentsTable();
        }

        // Отобразить данные таблицы.
        private void ShowStudents_Click(object sender, EventArgs e)
        {
            studentsData.DataSource = GetTable("Students");
        }

        // DataGridView умеет работать со своим источником данных.
        // Если вставить 2 строки в имеющиеся - они будут отображены и в DataTable.
        private List<DataRow> GetAddedRows()
        {
            DataTable table = (DataTable)studentsData.DataSource;
            List<DataRow> list = new();
            foreach (DataRow row in table.Rows)
                if (row.RowState == DataRowState.Added)
                    list.Add(row);
            return list;
        }

        private void UpdateAddedRows()
        {
            List<DataRow> addedRowsFromGrid = GetAddedRows();
            using SqliteConnection connection = CreateConnection();
            using SqliteTransaction transaction = connection.BeginTransaction();
            using SqliteCommand command = connection.CreateCommand();

            string[] parameters = { "@id", "@name", "@surname", "@group", "@role" };
            try
            {
                command.Transaction = transaction;
                for (var i = 0; i < addedRowsFromGrid.Count; i++)
                {
                    command.CommandText = "INSERT INTO Students VALUES (@id, @name, @surname, @group, @role)";
                    for (var j = 0; j < parameters.Length; j++)
                    {
                        object value = addedRowsFromGrid[i][j];
                        command.Parameters.AddWithValue(parameters[j], value);
                    }
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            UpdateAddedRows();
        }

        private void CreateTableFiles()
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS Files 
                (
                    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [Title] TEXT NOT NULL,
                    [DataImage] BLOB NOT NULL
                );";

            using SqliteConnection connect = CreateConnection();
            using SqliteCommand command = connect.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }

        private void SavePicture_Click(object sender, EventArgs e)
        {
            CreateTableFiles();
            SaveFileToDb(@"D:\Фото\Рабочий стол\planeta-gazovaya-ogon-kosmos.jpg");
        }

        private void SaveFileToDb(string path)
        {
            if (!File.Exists(path))
                throw new Exception("File is not exist!");
            string sql = "INSERT INTO Files(Title, DataImage) VALUES (@title, @data)";

            using SqliteConnection connect = CreateConnection();
            using SqliteTransaction transaction = connect.BeginTransaction();
            using SqliteCommand command = connect.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;

            using FileStream fs = new(path, FileMode.Open);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);

            string title = new FileInfo(path).Name;
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@data", data);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        private async void LoadPicture_Click(object sender, EventArgs e)
        {
            await LoadFileFromDb(1);
        }

        // Выгружает массив байт файла из БД.
        private async Task LoadFileFromDb(int id)
        {
            string sql = "SELECT DataImage FROM Files WHERE Id = @id";
            using SqliteConnection connect = CreateConnection();
            using SqliteTransaction transaction = connect.BeginTransaction();
            using SqliteCommand command = connect.CreateCommand();

            command.CommandText = sql;
            command.Parameters.AddWithValue("@id", id);
            byte[] data = (byte[])command.ExecuteScalar();

            await Task.Run(() => 
            {
                using MemoryStream ms = new();
                ms.Write(data, 0, data.Length);
                pictureBox1.Image = Image.FromStream(ms);
            });
        }
    }
}
