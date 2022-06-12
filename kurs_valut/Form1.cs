using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kurs_valut
{
    public partial class Form1 : Form
    {
        double c1, c2;
        public Form1()
        {
            InitializeComponent();
            //объект класса Valute
            Valute val = new Valute();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
        public string get_kurs(string name)
        {

            string url = "http://www.cbr.ru/scripts/XML_daily.asp";
            //string url = "https://tursportopt.ru/category/rybolovnye-tovary-optom/";
            //XmlDocument xml_doc = new XmlDocument();
            // xml_doc.Load(url);
            DataSet ds = new DataSet();
            ds.ReadXml(url);
            DataTable currency = ds.Tables["Valute"];
            foreach (DataRow row in currency.Rows)
            {
                if (row["CharCode"].ToString() == name)//Ищу нужный код валюты
                {
                    return row["Value"].ToString(); //Возвращаю значение курсы валюты
                }
            }
            return "";
        }
        private void kurs() //отношение курсов
        {
            string selectedState1 = comboBox1.SelectedItem.ToString();
            string selectedState2 = comboBox2.SelectedItem.ToString();
            if (selectedState1 != "RUB" && selectedState2 != "RUB")
            {
                string selected = get_box1();
                string val = get_kurs(selected);
                c1 = Convert.ToDouble(val); // наше значение 1 колонки

                selected = get_box2();
                val = get_kurs(selected);
                c2 = Convert.ToDouble(val); // значение 2 колонки

                string nn = textBox1.Text;
                double n = Convert.ToDouble(nn);

                textBox4.Text = Convert.ToString((c1 / c2) * n);
            }
            else
            {
                string selected1 = get_box1();
                string selected2 = get_box2();

                if (selected1 == "RUB") // если первая колонка рубль
                {
                    string val2 = get_kurs(selected2);
                    c2 = Convert.ToDouble(val2); // значение 2 колонки
                    textBox4.Text = "1";
                    string nn = textBox1.Text;
                    double n = Convert.ToDouble(nn);
                    textBox1.Text = Convert.ToString(c2);
                }
                else if (selected2 == "RUB")
                {
                    string val1 = get_kurs(selected1);
                    c1 = Convert.ToDouble(val1); // значение 2 колонки
                    textBox1.Text = "1";
                    string nn = textBox4.Text;
                    double n = Convert.ToDouble(nn);
                    textBox4.Text = Convert.ToString(c1);
                }
            }
        }
        public string get_box1()//выбор comboBox1
        {
            string selectedState = comboBox1.SelectedItem.ToString();
            return selectedState;
        }
        public string get_box2()//выбор comboBox2
        {
            string selectedState = comboBox2.SelectedItem.ToString();
            return selectedState;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox2.SelectedItem.ToString();
            if (selectedState != "RUB")
            {
                string val = get_kurs(selectedState);
                c2 = Convert.ToDouble(val);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedState = comboBox1.SelectedItem.ToString();
            if (selectedState != "RUB")
            {
                string val = get_kurs(selectedState);
                c1 = Convert.ToDouble(val);
            }
        }
    }
}
