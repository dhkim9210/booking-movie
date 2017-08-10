using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TEST
{
    public partial class Reservation : Form
    {
        Main main;
        int mile=0;
        int result;
        dbbinding s = new dbbinding();
        public Reservation()
        {
            InitializeComponent();
        }
        public Reservation(Main _form)
        {
            InitializeComponent();
            main = _form;
            textBox1.Text = Main.Passvalue_cost;
            textBox2.Text = Main.Passvalue_mile;            //Main에서 해당 고객의 마일리지 불러오기
            radioButton2.Checked = true;
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("결제하시겠습니까? ", "결제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MessageBox.Show(textBox1.Text + "원 결제가 완료되었습니다.");
                result = Int32.Parse(textBox1.Text);
                if (radioButton1.Checked == true)
                {
                    string sql = "update MILE set MILE=" + mile.ToString() + " where CLIENT_NAME='" + Main.Passvalue_ID + "'";
                    DataTable dt = s.getDT(sql);
                    mile = result * 5 / 100;
                    MessageBox.Show(mile.ToString() + "원 적립되었습니다.");
                    string sql2 = "update MILE set MILE=" + mile.ToString() + " where CLIENT_NAME='" + Main.Passvalue_ID + "'";
                    DataTable dt2 = s.getDT(sql2);
                }
                else
                {
                    mile = result * 5 / 100;
                    mile = Int32.Parse(Main.Passvalue_mile) + mile;
                    MessageBox.Show(mile.ToString() + "원 적립되었습니다.");
                    string sql2 = "update MILE set MILE=" + mile.ToString() + " where CLIENT_NAME='" + Main.Passvalue_ID + "'";
                    DataTable dt2 = s.getDT(sql2);
                }
                this.Hide();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = (Convert.ToInt32(Main.Passvalue_cost) - Convert.ToInt32(Main.Passvalue_mile)).ToString();       // 마일리지 적
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = Main.Passvalue_cost;
        }
    }
}
