using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;//подключаю библиотеку для работы с БД
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestFormOne
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;//подключение SQL в C# к внутренной БД
        private SqlDataAdapter adapter = null;//обновляется Бд из set 
        private SqlCommandBuilder builder = null;//строя 5 поле на комманды (удалить, изменить добавить)
        private DataSet set = null;//времменая таблица для ввода данных и изменния бд после(проще говоря посредник перед изменениямв таблице и изменения в GridView)
        private bool newRowAdding = false;// для команды добавить (insert)

        private void LoadDate()//загружаю данные из таблицы
        {
            try
            {
                adapter = new SqlDataAdapter("SELECT *, 'Delete' AS [Command] FROM [List of clients]", sqlConnection); // подключаю в Базе данных эту таблицу и завожу 5 поле как Delete

                builder = new SqlCommandBuilder(adapter);

                builder.GetInsertCommand();//Завожу комманды
                builder.GetUpdateCommand();//Завожу комманды
                builder.GetDeleteCommand();//Завожу комманды

                set = new DataSet();

                adapter.Fill(set, "List of clients");//читаю(Заполняю) таблицу

                dataGridView1.DataSource = set.Tables["List of clients"];//вывожу таблицу на GridView

                for(int i=0;i<dataGridView1.Rows.Count; i++)//прохожу по полям в записе и вывожу новое поле на последнюю ячейку в записи
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();//Завожу новый объект linkCell построенный на основе Поле GridView

                    dataGridView1[5, i] = linkCell;//индекс поле будет 6 , и индекс(последняя) по записи
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);//вывод ошибки , если вышел из исключения
            }
        }

        private void ReloadDate()//Тот самый посредник, который берет данные из БД , но не изменяет их а выводит
        {
            try
            {
                set.Tables["List of clients"].Clear();//очищение БД , что бы не дублировались данные

                adapter.Fill(set, "List of clients");

                dataGridView1.DataSource = set.Tables["List of clients"];

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[5, i] = linkCell;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);//вывод ошибки, научился после прошлой программы
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)//в этом методе обрабатываю вывод на GriedView информации с БД при запуске программы
        {
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\dekstop\Programming\GitHub\последняя попытка для SIMPL Иванов Д.С\Исходники\TestFormOne\TestFormOne\DataBaseClientOfPhone.mdf;Integrated Security=True");// создаю соеденения для подключения БД

            sqlConnection.Open();// открываю соеденение
           

            LoadDate();//начинаю загрузку БД из таблицы

        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            ReloadDate();//реализую метод
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_close_MouseLeave(object sender, EventArgs e)
        {

            btn_close.ForeColor = Color.White;
        }
        Point last_point;
        private void btn_close_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last_point.X;
                this.Top += e.Y - last_point.Y;
            }
        }

        private void btn_close_MouseDown(object sender, MouseEventArgs e)
        {
            last_point = new Point(e.X, e.Y);
        }

        private void btn_close_MouseEnter(object sender, EventArgs e)
        {
            btn_close.ForeColor = Color.Red;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)//обрабатываю нажатия в ячейку
        {
            try
            {
                if (e.ColumnIndex == 5)//определяю в каком поле будет команды для (удалить, обновить , добавить)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();//получаю текст
                    if (task == "Delete")//для команнды удалить
                    {
                        if (MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)//вывожу сообщение, и проверка пользователя на согласие удаление записи
                        {//и если пользователь дает комманду да , то захожу в тело условия
                            int rowIndex = e.RowIndex;

                            dataGridView1.Rows.RemoveAt(rowIndex);//

                            set.Tables["List of clients"].Rows[rowIndex].Delete();//удаляю эту запись из (давайте будеи говорить посредник) посредника

                            adapter.Update(set, "List of clients");//обновляет Базу данных и видет если запись удалена
                        }
                    }
                    else if (task == "Insert")// для комманды добавить
                    {
                        int rowIndex = dataGridView1.Rows.Count - 2;//для перехода на 3 поле

                        DataRow row = set.Tables["List of clients"].NewRow();//для временной табл

                        row["Логин"]= dataGridView1.Rows[rowIndex].Cells["Логин"].Value;//записывание данных с grid views в новые переменые row
                        row["Пароль"]= dataGridView1.Rows[rowIndex].Cells["Пароль"].Value;
                        row["ФИО пользователя"] = dataGridView1.Rows[rowIndex].Cells["ФИО пользователя"].Value;
                        row["Номер телефона"]= dataGridView1.Rows[rowIndex].Cells["Номер телефона"].Value;

                        set.Tables["List of clients"].Rows.Add(row);//вносим данные в БД
                        set.Tables["List of clients"].Rows.RemoveAt(set.Tables["List of clients"].Rows.Count - 1);//во все кроме последней, потому что последнее поле проверка 

                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";

                        adapter.Update(set, "List of clients");

                        newRowAdding =  false;

                    }
                    else if (task == "Update")//для комманды изменить
                    {
                        int r = e.RowIndex;

                        set.Tables["List of clients"].Rows[r]["Логин"] = dataGridView1.Rows[r].Cells["Логин"].Value; 
                        set.Tables["List of clients"].Rows[r]["Пароль"] = dataGridView1.Rows[r].Cells["Пароль"].Value; 
                        set.Tables["List of clients"].Rows[r]["ФИО пользователя"] = dataGridView1.Rows[r].Cells["ФИО пользователя"].Value; 
                        set.Tables["List of clients"].Rows[r]["Номер телефона"] = dataGridView1.Rows[r].Cells["Номер телефона"].Value;

                        adapter.Update(set, "List of clients");
                        dataGridView1.Rows[e.RowIndex].Cells[5].Value = "Delete";
                    }
                    ReloadDate();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)//для команды Insert Добавить
        {
            try
            {
                if( newRowAdding == false)
                {
                    newRowAdding = true;//для добавление новостроки
                    int lastRow = dataGridView1.Rows.Count - 2; //что бы перекинуть в 3 поле 

                   DataGridViewRow row = dataGridView1.Rows[lastRow];//экзампляр класса

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[5, lastRow] = linkCell;// что в 5 поле

                    row.Cells["Command"].Value = "Insert";//меняю строку с Удалить на добавить

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(newRowAdding == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                    DataGridViewRow eddetingRow = dataGridView1.Rows[rowIndex];
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[5, rowIndex] = linkCell;
                    eddetingRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)//контроль Введеных данных
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);
            if(dataGridView1.CurrentCell.ColumnIndex == 4)//что бы не вводили strnig значения(строки) в поле номера телефона
            {
                TextBox textBox = e.Control as TextBox;

                if(textBox != null)//важное условие
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
        }
        private void Column_KeyPress(object sender,KeyPressEventArgs e)//не обращает внимание на нажатие кнопки
        {
            if(!char.IsControl(e.KeyChar)&& !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)//для того что бы перемещать окно верхнее
        {
            last_point = new Point(e.X, e.Y); 
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                {
                   this.Left += e.X - last_point.X;
                  this.Top += e.Y - last_point.Y;
              }

        }

        private void Тест_MouseDown(object sender, MouseEventArgs e)
        {
            last_point = new Point(e.X, e.Y);
        }

        private void Тест_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last_point.X;
                this.Top += e.Y - last_point.Y;
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            last_point = new Point(e.X, e.Y);
        }

        private void label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - last_point.X;
                this.Top += e.Y - last_point.Y;
            }
        }
    }
}

      
