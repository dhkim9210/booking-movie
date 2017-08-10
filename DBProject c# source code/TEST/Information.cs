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
    public partial class Information : Form
    {
        dbbinding s = new dbbinding();
        public Information()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable set = s.getDT("SELECT * FROM RESERVATION");
            dataGridView1.DataSource = set;
        }
    }
}
