using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace TEST
{
    static class Program
    {
        [STAThread]
        public static void Main()
        {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
        }
    }
}
