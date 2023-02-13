using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
namespace часы
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       // string connStr = "server=192.168.25.23;port=44444;user=st_1_20_6;database=is_1_20_st6_KURS;password=40112334;";
        string connStr = "server=chuc.caseum.ru;port=33333;user=st_1_20_6;database=is_1_20_st6_KURS;password=40112334;";
        //Переменная соединения
           //string connStr = "server=10.90.12.110;port=33333;user=st_1_20_6;database=is_1_20_st6_KURS;password=40112334;";
        MySqlConnection conn;
        public void SetMyCustomFormat()
        {
            // Set the Format type and the CustomFormat string.
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "YYYY-MM-MM";
        }
        public string data1(DateTimePicker k)
        {
           var DataTime1 = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
            return DataTime1.ToString();
           
        }
            private void button1_Click(object sender, EventArgs e)
        {

           
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите мне имя", "Ошибка");
                return;

            }
            if (textBox2.Text == "")
            {
                MessageBox.Show("Какова моя цена?", "Ошибка");
                return;

            }

            if (dateTimePicker1.Text == "")
            {
                MessageBox.Show("Дата изготовки", "Ошибка");
                return;

            }
            string ca = data1(dateTimePicker1);

            MySqlCommand command = new MySqlCommand($"INSERT INTO Eda (Name,Price,Data) VALUES(@Name, @Price,'{ca}');", conn);

            conn.Open();
            command.Parameters.Add("@Name", MySqlDbType.VarChar, 25).Value = textBox1.Text;
            command.Parameters.Add("@Price", MySqlDbType.Float, 25).Value = textBox2.Text;
            try
            {
                if (command.ExecuteNonQuery() == 1)
                {
                   
                    MessageBox.Show("Вы успешно добавили");
                    table.Clear();
                    GetListUsers();
                    ChangeColorDGV();
                }
                else
                {

                    MessageBox.Show("Произошла ошибка");
                }
            }
            catch(Exception ex)
            {
               
                MessageBox.Show("Ошибка" + ex);
            }
            finally
            {
                conn.Close();
            }
        }
      
        DataTable table = new DataTable();
        //Объявляем адаптер
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        //Объявляем команду
        private BindingSource bSource = new BindingSource();
       
        public void GetListUsers()
        {
            dataGridView1.DataSource = default;
            //Запрос для вывода строк в БД

            string commandStr = $"SELECT * FROM Eda";
            conn = new MySqlConnection(connStr);
            //Открываем соединение

            conn.Open();
            
            //Объявляем команду, которая выполнить запрос в соединении conn
            adapter.SelectCommand = new MySqlCommand(commandStr, conn);
            //Заполняем таблицу записями из БД
            adapter.Fill(table);
            //Указываем, что источником данных в bindingsource является заполненная выше таблица
            bSource.DataSource = table;
            //Указываем, что источником данных ДатаГрида является bindingsource 
            dataGridView1.DataSource = bSource;
            //Закрываем соединение
            conn.Close();
            //Отражаем количество записей в ДатаГриде
            int count_rows = dataGridView1.RowCount - 1;
            label1.Text = (count_rows).ToString();
           

        }
      
        private void Form1_Load(object sender, EventArgs e)
        {
              conn = new MySqlConnection(connStr);
            //Вызываем метод для заполнение дата Грида
           GetListUsers();
            //Видимость полей в гриде
            dataGridView1.Columns[0].Visible = true;
            dataGridView1.Columns[1].Visible = true;
            dataGridView1.Columns[2].Visible = true;
            dataGridView1.Columns[3].Visible = true;
          

            //Ширина полей
            dataGridView1.Columns[0].FillWeight = 15;
            dataGridView1.Columns[1].FillWeight = 40;
            dataGridView1.Columns[2].FillWeight = 15;
            dataGridView1.Columns[3].FillWeight = 15;
        
            //Режим для полей "Только для чтения"
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[2].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
           
            //Растягивание полей грида
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
          
            //Убираем заголовки строк
            dataGridView1.RowHeadersVisible = false;
            //Показываем заголовки столбцов
            dataGridView1.ColumnHeadersVisible = true;
            //Вызываем метод покраски ДатаГрид
            ChangeColorDGV();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
         
   
         
        private void ChangeColorDGV()
          {
              //Отражаем количество записей в ДатаГриде
              int count_rows = dataGridView1.RowCount - 1;
              label1.Text = (count_rows).ToString();


            // var caa = data2(DateTime.Now);
            var DataTime4 = DateTime.Now.AddDays(-6);
            var DataTime2 = DateTime.Now.AddDays(-3);
            var DataTime3 = DateTime.Now;
          
            //Проходимся по ДатаГриду и красим строки в нужные нам цвета, в зависимости от статуса студента
            for (int i = 0; i < count_rows; i++)
              {
               
                //статус конкретного студента в Базе данных, на основании индекса строки
                DateTime id_selected_status = Convert.ToDateTime(dataGridView1.Rows[i].Cells[3].Value);
                  //Логический блок для определения цветности
                
                  if ( DataTime3 > id_selected_status ) 
                  {
                      //Красим в зелёный
                      dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                  }
                  if ( DataTime2 > id_selected_status )
                  {
                      //Красим в желтый
                      dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                  }
                if (DataTime4 > id_selected_status)
                {
                    //Красим в красный

                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }
            }
          }
       
        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string csa = data1(dateTimePicker2);
            conn = new MySqlConnection(connStr);
            //Открываем соединение

            conn.Open();
            string sql = $"SELECT * FROM Ucheba WHERE Data='{csa}'";
            //  DataTable table = new DataTable();

            //   MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM Eda WHERE Id='{selected_id}'", conn);
            //    table.Clear();
            //    adapter.Fill(table);
            table.Clear();
            adapter.Fill(table);
            MySqlCommand command = new MySqlCommand(sql, conn);
            // объект для чтения ответа сервера
            MySqlDataReader reader = command.ExecuteReader();
            int count_rows = dataGridView1.RowCount - 1;

            Regex regex = new Regex($"SELECT * FROM Eda WHERE Id='{csa}'");
            MatchCollection matches = regex.Matches(csa);
            for (int i = 0; i < count_rows; i++)
            {
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                        Auth.Id = reader[0].ToString();
                    Auth.Name = reader[1].ToString();
                    Auth.Price = reader[2].ToString();
                    Auth.Data = reader[3].ToString();
                }
                else
                {
                    MessageBox.Show("Совпадений не найдено");
                }
            }
            conn.Close();
          /*  string selected_id = textBox1.Text;
            string sql = $"SELECT * FOM Ucheba WHERE Id='{selected_id}'";
            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM Eda WHERE Id='{selected_id}'", conn);
            table.Clear();
            adapter.Fill(table);
            MySqlCommand command = new MySqlCommand(sql, conn);
            // объект для чтения ответа сервера
            MySqlDataReader reader = command.ExecuteReader();
            // читаем результат
            while (reader.Read())
            {
                // элементы массива [] - это значения столбцов из запроса SELECT
                Auth.Id = reader[0].ToString();
                Auth.Name = reader[1].ToString();
                Auth.Price = reader[2].ToString();
                Auth.Data = reader[3].ToString();
                //   Auth.auth_role = Convert.ToInt32(reader[3].ToString());
            }*/
           
        }
        public class Auth
        {


            public static string Id = null;
            public static string Name = null;
            public static string Price = null;
            public static string Data = null;
        }
        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string st1 = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            //Формируем строку запроса на добавление строк
            string sql_delete_user = "DELETE FROM Eda WHERE Id=" + st1;

            //Посылаем запрос на обновление данных
            MySqlCommand delete_user = new MySqlCommand(sql_delete_user, conn);
            try
            {
                conn.Open();
                delete_user.ExecuteNonQuery();
                MessageBox.Show("Удаление прошло успешно", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления строки \n" + ex, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                conn.Close();
                //Вызов метода обновления ДатаГрида
                table.Clear();
                GetListUsers();
            }
        }



    }



}
