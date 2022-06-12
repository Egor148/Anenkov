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
