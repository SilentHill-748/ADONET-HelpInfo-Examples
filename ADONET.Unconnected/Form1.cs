using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

using Microsoft.Data.SqlClient;


namespace ADONET.Unconnected
{
    public partial class Form1 : Form
    {
        private readonly BindingSource _studentSource;
        private DataSet _dataset;



        public Form1()
        {
            this.InitializeComponent();
            this._studentSource = new BindingSource();
            this._dataset = new DataSet();
            // this.ReadData();
            // this.LoadListView();
            // this.DataAdapterUpdateVersion2();
            // AddColumnToFilledDataSet();
            // TestRelationsOnProgram();
            //FillTreeView();
            CreateView();
            GetSchemaToDb();
        }



        //1
        private async Task<IDbConnection> CreateConnectionAsync()
        {
            string connString = ConfigurationManager.ConnectionStrings["sqlString"].ConnectionString;
            SqlConnection connection = new (connString);
            await connection.OpenAsync();
            return connection;
        }

        private async void ReadData()
        {
            SqlConnection connect = (SqlConnection)await this.CreateConnectionAsync();
            var command = connect.CreateCommand();
            command.CommandText = "SELECT * FROM Students;";
            command.Prepare();

            using SqlDataAdapter adapter = new (command);
            adapter.Fill(this._dataset);
            await connect.CloseAsync();

            _studentSource.DataSource = this._dataset.Tables[0];
            dataTest.DataSource = _dataset.Tables[0];

            // Если уберешь строку, то вылетит ошибка в методе ниже.
            _dataset.Tables[0].PrimaryKey = new DataColumn[] { _dataset.Tables[0].Columns[0] };

            TestFindMethod();
        }

        private async void Save_Click(object sender, EventArgs e)
        {
            using var connect = (SqlConnection)await this.CreateConnectionAsync();
            using SqlDataAdapter adapter = new ("SELECT * FROM Students;",
                connect);

            // Update command
            #region Update
            adapter.UpdateCommand = new SqlCommand("UPDATE Students SET name = @newName, " +
                "group_id = @new_group_id WHERE id = @id;");

            adapter.UpdateCommand.Parameters.Add("@newName", SqlDbType.NVarChar,
                20, "name");
            adapter.UpdateCommand.Parameters.Add("@new_group_id", SqlDbType.Int,
                10, "group_id");
            adapter.UpdateCommand.Parameters.Add("@id", SqlDbType.Int,
                10, "id");
            adapter.UpdateCommand.Connection = connect;
            #endregion

            // Insert command
            #region Insert
            adapter.InsertCommand = new SqlCommand("INSERT INTO Students " +
                "VALUES (@insertId, @insertName, @insertSurName, @insert_group_id, @insert_role_id);", connect);
            adapter.InsertCommand.Parameters.Add("@insertId", SqlDbType.Int, 10, "id");
            adapter.InsertCommand.Parameters.Add("@insertName", SqlDbType.NVarChar, 20, "name");
            adapter.InsertCommand.Parameters.Add("@insertSurName", SqlDbType.NVarChar, 30, "surname");
            adapter.InsertCommand.Parameters.Add("@insert_group_id", SqlDbType.Int, 10, "group_id");
            adapter.InsertCommand.Parameters.Add("@insert_role_id", SqlDbType.Int, 10, "role_id");
            #endregion

            // Delete command
            #region Delete
            adapter.DeleteCommand = new SqlCommand("Delete from students where id = @del_Id", connect);
            adapter.DeleteCommand.Parameters.Add("@del_id", SqlDbType.Int, 10, "id");
            #endregion

            adapter.Update((DataTable)dataTest.DataSource);
            this.UpdateGrid(adapter);
        }

        private void UpdateGrid(SqlDataAdapter adapter)
        {
            DataTable table = new ();
            adapter.Fill(table);
            dataTest.DataSource = table;
            adapter.Dispose();
        }

        private void LoadListView()
        {
            this.Read();
            GC.Collect();
        }

        private async void Read()
        {
            using var connection = (SqlConnection)await CreateConnectionAsync();
            DataSet set = new();

            using SqlDataAdapter adapter = new ("SELECT * FROM Students;", connection);
            adapter.Fill(set);
            await connection.CloseAsync();

            set.Tables[0].Rows[0].Delete();
            set.Tables[0].Rows[0].AcceptChanges();

            foreach (DataColumn column in set.Tables[0].Columns)
            {
                listView1.Columns.Add(column.Caption);
            }

            foreach (DataRow row in set.Tables[0].Rows)
            {
                row.BeginEdit();
                row[1] = "Test";
                row.EndEdit();
                ListViewItem item = listView1.Items.Add(row.ItemArray[0].ToString());
                for (var i = 1; i < row.ItemArray.Length; i++)
                {
                    item.SubItems.Add(row.ItemArray[i].ToString());
                }
            }
        }

        private void MethodsOfDataRow()
        {
            DataRow row = _dataset.Tables[0].Rows[1];

            row.AcceptChanges();    // Принимает все изменения.
            row.BeginEdit();        // Начать редактирование.
            //row.RowState;         // Указывает состояние строки.
            row.Delete();           // Удаляет строку.
            row.RejectChanges();    // Отменаяет редактирование.
        }



        //2
        private async void DataAdapterUpdateVersion2()
        {
            SqlConnection connect = (SqlConnection)await CreateConnectionAsync();
            string sql = "Select * from Students;";
            using SqlDataAdapter adapter = new (sql, connect);

            DataSet set = new ();
            adapter.Fill(set);

            DataTable table = set.Tables[0];
            DataRow newRow = table.NewRow();
            newRow["id"] = 20;
            newRow["name"] = "ТЕСТЕР";
            newRow["surname"] = "ТЕСТЕРОВ";
            newRow["group_id"] = 1;
            newRow["role_id"] = 3;

            table.Rows.Add(newRow);

            SqlCommandBuilder commandBuilder = new (adapter);
            adapter.Update(set);
            set.Clear();

            adapter.Fill(set);
            dataTest.DataSource = set.Tables[0];
            connect.Close();
        }



        //3
        private DataTable InitTable()
        {
            DataTable table = new ();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Товар", typeof(string));
            table.Columns.Add("Цена", typeof(decimal));
            table.Columns.Add("Количество", typeof(int));
            table.Columns.Add("Налог", typeof(decimal), "Цена * Количество * 0.18");

            table.RowChanged += TestTable_RowChanged;
            return table;
        }

        private void TestTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            string message = $"В таблице были произведены изменения. Была выполнена операция: {e.Action}" +
                $"\nСостояние строки: {e.Row.RowState}";

            MessageBox.Show(message);
        }

        private void AddValues(DataTable table, params object[] values)
        {
            DataRow row = table.NewRow();
            row.ItemArray = values;
            table.Rows.Add(row);
        }

        private void LoadTable_Click(object sender, EventArgs e)
        {
            DataTable table = InitTable();
            if (dataGridView1.DataSource is null)
                dataGridView1.DataSource = table;
            AddValues(table, 1, "Томат", 2568.12, 745); // RowChanged|Add|Added
            AddValues(table, 2, "Огурец", 7.83, 7);     // RowChanged|Add|Added

            object[] dataRow = {3, "Банан", 11.85, 7 };
            // Чтобы через адаптер обновить БД, то вместо true надо указать false.
            table.LoadDataRow(dataRow, true);    // RowChanged|Add|Added
            EditTable(table);
        }



        //4
        private async void AddColumnToFilledDataSet()
        {
            using SqlConnection connect = (SqlConnection)await CreateConnectionAsync();
            using SqlDataAdapter adapter = new ("SELECT * FROM Students;", connect);

            DataSet set = new ();
            adapter.Fill(set);
            set.Tables[0].Columns.Add("full name", 
                typeof(string), "name + ' ' + surname");
            UniqueConstraint uc = new (set.Tables[0].Columns["id"]);
            set.Tables[0].Constraints.Add(uc);

            dataGridView2.DataSource = set.Tables[0];
        }

        private void EditTable(DataTable table)
        {
            // При изменении 1 строки, генерятся ивенты Column[Changed|Changing], Row[Cahnged|Changing].
            DataRow row = table.Rows[0];
            row[1] = "Товар 1"; // RowChanged|Change|Added

            // Если надо для N строк провернуть такое, то лучше юзать BeginEdit/EndEdit
            // Но у класса DataTable нет таких методов. Возможно, они используются аналогично, но для грида.
        }

        private void TestFindMethod()
        {
            // У таблицы можно получить строку по индексу/значению первичного ключа.
            try
            {
                DataRow row = _dataset.Tables[0].Rows.Find(1); // Получить строку по индексу 1.
                MessageBox.Show($"Найденные данные: \n{string.Join(", ", row.ItemArray)}");
            }
            catch (MissingPrimaryKeyException ex)
            {
                // Но если такого ключа нет, то Find кинет Exception.
                MessageBox.Show(ex.Message);
            }
        }

        private void TestDeleteRow()
        {
            DataTable table = InitTable();
            table.RowChanged -= TestTable_RowChanged;
            table.Rows[0].Delete();     // Первый способ.
            table.Rows.RemoveAt(0);     // Второй способ.

            // При этом, есть 2 события RowDeleted|RowDeleting. Пояснения излишне.
            table.AcceptChanges();
        }

        //5
        DataSet set = new ();
        private async void TestRelationsOnProgram()
        {
            // 2 таблицы можно связать и в коде.
            using var connect = (SqlConnection)await CreateConnectionAsync();
            string selectCommand = @"
                SELECT * FROM Students;
                SELECT * FROM Groups;";

            using var adapter = new SqlDataAdapter(selectCommand, connect);
            adapter.Fill(set);

            DataRelation relation =
                new ("students_groups",
                set.Tables[1].Columns[0],
                set.Tables[0].Columns[3]);
            set.Relations.Add(relation);

            // Выгрузка 2 таблиц в свои формы.
            groupsGrid.DataSource = set.Tables[1];
            studentsGrid.DataSource = set.Tables[0];
        }

        private void GetResultOfRelation_Click(object sender, EventArgs e)
        {
            // Отобраение полной картины связи студент-группа.
            string students = "";
            int index = groupsGrid.CurrentRow.Index;
            foreach (DataRow child in set
                .Tables[1]
                .Rows[index]
                .GetChildRows("students_groups"))
            {
                students += $"{child[1]} {child[2]}\n";
            }
            // Вывод всех студентов, у которых указана выбранная группа.
            MessageBox.Show($"Студенты указанной группы:\n{students}");
        }

        private void GetGroupByStudent_Click(object sender, EventArgs e)
        {
            // Аналогично можно вытянуть инфу и родительской строки.
            int index = studentsGrid.CurrentRow.Index;

            DataRow row = set
                .Tables[0]
                .Rows[index]
                .GetParentRow("students_groups");

            // Вывод инфо по группе.
            string group = $"Данные группы:" +
                $"\nКод: {row["code"]}" +
                $"\nНазвание: {row["name"]}";
            MessageBox.Show(group);
        }

        private void ShowConstraintsCount_Click(object sender, EventArgs e)
        {
            UniqueConstraint uc = 
                (UniqueConstraint)set.Tables[1].Constraints[0];
            ForeignKeyConstraint fkc =
                (ForeignKeyConstraint)set.Tables[0].Constraints[0];

            // Класс DataRelation имеет свойства ParentKeyConstraint|ChildKeyConstraint.
            // Первый - объект класса UniqueConstraint, второй - ForeignKeyConstraint.
            string uniqueConstr = $"UniqueColumns: {uc.Columns[0].Caption}";
            string foreignKeyConstr = $"ForeignKey: {fkc.Columns[0].Caption}";

            MessageBox.Show($"{uniqueConstr}\n{foreignKeyConstr}");
        }



        //6
        private async void FillTreeView()
        {
            // Связь можно делать и с 1 таблицей. Ссылка на саму себя.
            using var connect = (SqlConnection)await CreateConnectionAsync();
            using var adapter = new SqlDataAdapter(
                "SELECT * FROM Positions;", connect);

            set.Clear();
            adapter.Fill(set);
            await connect.CloseAsync();

            DataRelation relation = new (
                "positions",
                set.Tables[0].Columns[0],
                set.Tables[0].Columns[1]);
            set.Relations.Add(relation);
            
            foreach (Constraint c in set.Tables[0].Constraints)
                if (c is ForeignKeyConstraint fc)
                {
                    fc.DeleteRule = Rule.None;
                    fc.UpdateRule = Rule.None;
                }

            foreach (DataRow row in set.Tables[0].Rows)
            {
                if (row.IsNull(1))
                    AddTreenode(row, null);
                else
                    break;
            }

            set.Tables[0].Columns.Add(
                "Число подчинённых", 
                typeof(int), 
                "COUNT(Child.idPosition)");
            dataGridView3.DataSource = set.Tables[0];
            // Переопределение кода информированию пользователя об ошибке.
            dataGridView3.DataError += DataGridView3_DataError;
            // Настройка процесса удаления
            dataGridView3.UserDeletingRow += DataGridView3_UserDeletingRow;
        }

        private void DataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Ты что, дурак, блять?");
        }

        private void DataGridView3_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            // Если при удалении 1 строки в 6 табе, тут будет false - Вылетил ошибка, которая
            // вернет текст ошибки выше.
            e.Cancel = true;
        }

        private void AddTreenode(DataRow row, TreeNode node)
        {
            TreeNode currnode;
            if (node == null)
                currnode = treeView1.Nodes.Add(row.ItemArray[2].ToString());
            else
                currnode = node.Nodes.Add(row.ItemArray[2].ToString());

            foreach (DataRow currow in row.GetChildRows("positions"))
                AddTreenode(currow, currnode);
        }



        //7
        private void GetStudentByFilter()
        { 
            string filter = $"positionname LIKE '{nameInput.Text}%'";
            DataRow[] rows = _dataset.Tables[0].Select(filter, "positionname ASC");
            string found = "";
            if (rows.Length == 0)
            {
                MessageBox.Show($"Данных со значением '{nameInput.Text}' не найденно!");
                return;
            }
            foreach (DataRow row in rows)
                found += row.ItemArray[2].ToString() + "\n";
            MessageBox.Show(found, "Найденные имена");
        }

        private async void GetByFilter_Click(object sender, EventArgs e)
        {
            using var connect = (SqlConnection)await CreateConnectionAsync();
            using SqlDataAdapter adapter = 
                new ("select * from positions;", connect);
            _dataset = new DataSet();
            adapter.Fill(_dataset);
            dataGridView4.DataSource = _dataset.Tables[0];
            GetStudentByFilter();
        }



        //8
        private DataTable testTable;
        private DataView testView;

        private async void CreateView()
        {
            testTable = await LoadTableAsync();
            testView = new DataView(testTable);
            dataGridView5.DataSource = testView;
            SetSort("surname DESC");
            SetFilter("name LIKE 'Н%'");
            AddNewRowToView();
            DataRowView[] rows = testView.FindRows("Палин");
            rows[0].Row[2] = "Отредачено!";
        }

        private void SetSort(string columnName)
        {
            testView.Sort = columnName;
        }

        private void SetFilter(string filter)
        {
            testView.RowFilter = filter;
        }

        private async Task<DataTable> LoadTableAsync()
        {
            using var connect = (SqlConnection)await CreateConnectionAsync();
            using var command = connect.CreateCommand();
            command.CommandText = "SELECT * FROM Students;";

            DataSet set = new();
            using SqlDataAdapter adapter = new(command);
            adapter.Fill(set);

            return set.Tables[0];
        }

        private void AddNewRowToView()
        {
            object[] values = { 30, "Никита", "Палин", 1, 1 };
            DataRowView rowView = testView.AddNew();
            rowView["id"] = 30;
            rowView["name"] = "Никита";
            rowView["surname"] = "Палин";
            rowView["group_id"] = 1;
            rowView["role_id"] = 1;
            rowView.EndEdit();
        }

        //9
        private async void GetSchemaToDb()
        {
            using var connect = (SqlConnection)await CreateConnectionAsync();
            DataTable table = connect.GetSchema("Databases");

            dataGridView6.DataSource = table;
        }
    }
}
