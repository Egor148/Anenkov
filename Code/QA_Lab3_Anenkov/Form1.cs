using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OS_9
{
    delegate void SetTextCallback(string text);

 
    ///
    /// @brief Класс для формы проекта
    /// 
    /// Реализует интерфейс программы
    /// 
    public partial class Form1 : Form
    {
        int coll;
        int valueTextBox;
        int countTh;
        int delay;
        int currentIndexWhenLoad;
        bool isload;
        StreamWriter writer;
        StreamReader reader;
        List<Thread> threads;
        List<Square> Square;
        List<int> removedIndex;
        Random rand;
        Thread th1, th2;
        public delegate void MyDelegate();
        /// <summary>
        /// Форма
        /// Программа реализует работу нескольких функций параллельно, через потоки
        /// Одна часть программы рисует квадраты, вторая считает числа и выводит их на форму
        /// Так же во время выполнения программы есть возможность вручную вводить цифры на форму в textbox
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            th1 = new Thread(MyThread);
            th2 = new Thread(updater);
        }


        /// <summary>
        /// Функция возвращающая текст
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            listBox1.Items.Add(text);
        }
        private void AddString()
        {
            Thread.Sleep(1000);
            int i;
            for (i = 0; i < 100000; i++)
            {
                if (listBox1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    Invoke(d, new object[] { Convert.ToString(i) });
                }
                else
                {
                    listBox1.Items.Add(Convert.ToString(i));
                    
                }
            }
        }
        /// <summary>
        /// Старт потоков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button5_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(AddString));
            th.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int k;
            string s;
            s = textBox4.Text;
            listBox2.Items.Add(s);

        }
        /// <summary>
        /// метод для работы с потоками
        /// </summary>
        public void MyThread()//метод для работы с потоками
        {
            for (int i = 0; i < countTh; i++)
            {
                Thread m = new Thread(threadsDraw);
                m.Name = dataGridView1.Rows[i + coll].Cells[0].Value.ToString();
                threads.Add(m);
                m.Start();
                th2 = new Thread(updater);
                th2.Start();
                Thread.Sleep(delay);
            }
            th2 = new Thread(updater);
            th2.Start();
            
        }

        public void updater()
        {
            dataGridView1.Invoke((MethodInvoker)delegate ()
            {
                for (int i = 0; i < threads.Count; i++)
                    dataGridView1.Rows[i].Cells[1].Value = threads[i].ThreadState;
            });
        }
        /// <summary>
        /// поток для рисования
        /// </summary>
        public void threadsDraw()
        {
            if (!isload)
            {
                Square square = new Square();
                Square.Add(square);
                Graphics a = this.pictureBox1.CreateGraphics();
                a.DrawRectangle(square.pen, square.points[0].X, square.points[0].Y, square.width, square.height);
            }
            else
            {
                Graphics a = this.pictureBox1.CreateGraphics();
                //a.DrawRectangle(Square[currentIndexWhenLoad].pen, Square[currentIndexWhenLoad].points[0].X, Square[currentIndexWhenLoad].points[0].Y, Square[currentIndexWhenLoad].width, Square[currentIndexWhenLoad].height);
            }
        }
        /// <summary>
        /// Метод отвечающий за загрузку формы
        /// Создает таблицу и объект thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
    public void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("colName", "Имя потока");
            dataGridView1.Columns.Add("colState", "Статус");
            dataGridView1.Columns.Add("colPrior", "Приоритет");
            dataGridView1.Columns[0].Width = 70;
            dataGridView1.Columns[1].Width = 70;
            dataGridView1.Columns[2].Width = 70;
            valueTextBox = 0;
            delay = 1000;
            coll = 0;
            textBox1.Text = delay.ToString();
            isload = false;
            threads = new List<Thread>();
            Square = new List<Square>();
            removedIndex = new List<int>();
        }
        /// <summary>
        /// Метод для удаления потока
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedCells[0].RowIndex;
            dataGridView1.Rows.RemoveAt(row);
            countTh--;
            if (row < threads.Count)
            {
                threads.RemoveAt(row);
                Square.RemoveAt(row);
            }
        }
       
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                writer = new StreamWriter("threads.txt");
                for (int i = 0; i < threads.Count; i++)
                {
                    writer.WriteLine(threads[i].Name);
                    writer.WriteLine(Square[i].color.ToArgb().ToString());
                    writer.WriteLine(Square[i].bold.ToString());
                    writer.WriteLine(Square[i].points[0].X.ToString());
                    writer.WriteLine(Square[i].points[0].Y.ToString());
                    writer.WriteLine(Square[i].width.ToString());
                }
                writer.Close();
                MessageBox.Show("Запись произведена.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception excep)
            {
                MessageBox.Show(this, excep.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                writer.Close();
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics a = this.pictureBox1.CreateGraphics();
            a.Clear(this.BackColor);
            try
            {
                reader = new StreamReader("threads.txt");
                if (reader.EndOfStream)
                    throw new EndOfStreamException("Файл пуст.");
                int count = 0;
                while (!reader.EndOfStream)
                {
                    reader.ReadLine();
                    count++;
                }

                Thread[] tmpThread = new Thread[count / 6];
                Point[] tmpPoint = new Point[1];
                Color tmpColor = new Color();
                int tmpWidth = 0;
                float tmpBold = 0.0f;
                fileToolStripMenuItem.Enabled = false;
                reader = new StreamReader("threads.txt");
                if (reader.EndOfStream)
                    throw new EndOfStreamException("Файл пуст.");
                threads.Clear();
                Square.Clear();
                dataGridView1.Rows.Clear();
                isload = true;
                count = 0;
                while (!reader.EndOfStream)
                {
                    tmpThread[count] = new Thread(threadsDraw);
                    tmpThread[count].Name = reader.ReadLine();
                    tmpColor = Color.FromArgb(int.Parse(reader.ReadLine()));
                    tmpBold = float.Parse(reader.ReadLine());
                    tmpPoint[0].X = int.Parse(reader.ReadLine());
                    tmpPoint[0].Y = int.Parse(reader.ReadLine());
                    tmpWidth = int.Parse(reader.ReadLine());
                    Square tmpEllipseInSquare = new Square(tmpPoint, tmpColor, tmpWidth, tmpBold);
                    Square.Add(tmpEllipseInSquare);
                    threads.Add(tmpThread[count]);
                    count++;
                }
                reader.Close();
                Thread thhhh = new Thread(drowIfLoad);
                thhhh.Start();
                for (int i = 0; i < threads.Count; i++)
                {
                    dataGridView1.Rows.Add(threads[i].Name, threads[i].ThreadState);
                }
                }
            catch (Exception exc)
            {
                MessageBox.Show(this, exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fileToolStripMenuItem.Enabled = true;
            }
        }
        public void drowIfLoad()
        {
            for (int i = 0; i < threads.Count; i++)
            {
                currentIndexWhenLoad = i;
                threads[i].Start();
                th2 = new Thread(updater);
                th2.Start();
                Thread.Sleep(delay);
            }
            th2 = new Thread(updater);
            th2.Start();
            isload = false;
            Invoke((MethodInvoker)delegate ()
            {
                fileToolStripMenuItem.Enabled = true;
            });
        }
        /// <summary>
        /// Метод для работы с приоритетами потоков
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = dataGridView1.SelectedCells[0].RowIndex;
            textBox3.Text = th1.Priority.ToString();

        }
        /// <summary>
        /// Инициализация приоритета потоков рисования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ![Описание метода](image.png)
        public void button1_Click(object sender, EventArgs e)
        {
            int p;
            p = Convert.ToInt32(textBox3.Text);
            if (p == 1)
                th1.Priority = ThreadPriority.Lowest;
            else if (p == 2)
                th1.Priority = ThreadPriority.BelowNormal;
            else if (p == 3)
                th1.Priority = ThreadPriority.Normal;
            else if (p == 4)
                th1.Priority = ThreadPriority.AboveNormal;
            else if (p == 5)
                th1.Priority = ThreadPriority.Highest;

        }

       

        private void button3_Click(object sender, EventArgs e)
        {
            
            countTh = Convert.ToInt32(textBox2.Text);
            delay = Convert.ToInt32(textBox1.Text);
            coll = dataGridView1.Rows.Count - 1;//количество потоков в таблице
            dataGridView1.Rows.Add(countTh);
            for (int i = coll; i < coll + countTh; i++)
            {
                dataGridView1.Rows[i].SetValues("Квадрат"+ (i-coll+valueTextBox).ToString(), "Unstarted", th1.Priority.ToString());
            }
            th1 = new Thread(MyThread);
            th1.Start();
        }
        
    }
    ///
    /// @brief Класс для рисования объектов
    /// 
    /// Имеет все нужные кисти, цвета и другие объекты для рисования.
    /// Рисует объект в случайной точке случайным цветом
    ///
    class Square
    {
        const int pictureWidth = 500;
        const int pictureHeight = 500;
        Random rand;
        public Point[] points = new Point[1];
        public Color color;
        public int width;
        public int height;
        public float bold;
        public Pen pen;
        /*!
     \brief Вычисление точки рисования
     \
     \
     Данная функция вычисляет значение точки откуда рисовать \f$ points[]\f$, определяемое по формуле:
     \f[
          points[0].X = rand.Next(0, pictureWidth / 2); points[0].Y = rand.Next(0, pictureHeight / 2);
     \f]
     \А так же цвет рисуемого объекта \f$ color\f$, определяемый по формуле:
     \f[
          color = Color.FromArgb(rand.Next(250), rand.Next(250), rand.Next(250));
     \f]
*/
        public Square()
        {
            rand = new Random();
            points[0].X = rand.Next(0, pictureWidth / 2); points[0].Y = rand.Next(0, pictureHeight / 2);
            width = rand.Next(0, pictureWidth / 2);
            height = width;
            color = Color.FromArgb(rand.Next(250), rand.Next(250), rand.Next(250));
            bold = (float)rand.NextDouble() * 10;
            pen = new Pen(color, bold);
        }
        public Square(Point[] points, Color color, int width, float bold)
        {
            this.points[0].X = points[0].X;
            this.points[0].Y = points[0].Y;
            this.color = color;
            this.width = width;
            this.height = this.width;
            this.bold = bold;
            this.pen = new Pen(color, bold);
        }

        internal static void Add(Square square)
        {
            throw new NotImplementedException();
        }
    }
}
