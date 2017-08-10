using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
namespace TEST
{
    public partial class LOGIN : Form
    {
        public static string login_value;
        public static string Passvalue_login{get;set;}
        string strConn = "Server=192.168.0.78;Database=MOVIE_Reservation;Uid=root;Pwd=rlaehdgkr;";
        public LOGIN()
        {
            InitializeComponent();
/*            MySqlConnection conn = new MySqlConnection(strConn);
            conn.Open();
            if (conn.Ping() == true)
                MessageBox.Show("Server connected");
            else
                MessageBox.Show("Server disconnected");
*/        }
        private void button2_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(strConn);
            DataSet ds = new DataSet();
            string sql = "SELECT ID,PASSWORD,NAME FROM CLIENT where ID ='" + textBox1.Text + "' and PASSWORD ='" + textBox2.Text + "'";
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = new MySqlCommand(sql, conn);
            adapter.Fill(ds);
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                if (textBox1.Text == r["ID"].ToString() && textBox2.Text == r["PASSWORD"].ToString())
                {
                    Passvalue_login = r["ID"].ToString();
                    this.Hide();
                    conn.Close();
                    break;
                }
                else
                {
                    MessageBox.Show("Incorrect ID");
                    textBox1.ResetText();
                    textBox2.ResetText();
                }
                
            }
      }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
