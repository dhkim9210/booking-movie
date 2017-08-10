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
    public partial class Main : Form
    {
        public static string Passvalue_ID { get; set; }         // Reservation 폼으로 ID 값을 넘겨주기 위해 설정
        public static string Passvalue_cost { get; set; }       // Reservation 폼으로 가격값을 계산하여 넘겨주기 위해 설정
        public static string Passvalue_mile { get; set; }       // Reservation 폼으로 해당고객 마일리지 값을 넘겨주기 위해 설정
        
        //초기 필요 변수들 설정
        string date;
        int count=0, re_uid=0;              
        string re_name,re_place, re_theater,re_movie,re_date,re_time;
        string re_seatnum, re_seatletter;
        string[,] seat = new string[100, 2];
        int k = 0, j = 0;
        // 어느폼에서나 쓸수 있게 dbbinding 클래스 객체 선언
        dbbinding s = new dbbinding();
        public Main()
        {
            InitializeComponent();
        }
        // VIEW를 생성하는 부분
        public void createlist(string seatletter, string seatnum)
        {
            // 자리 선택을 하고 2차원 배열로 자리값 생성
            seat[k,j] = seatletter;
            seat[k,j + 1] = seatnum;

            // listview 생성
            listView1.View = View.Details;
            listView1.BeginUpdate();
            ListViewItem lvi = new ListViewItem(re_date);
            lvi.SubItems.Add(re_place);
            lvi.SubItems.Add(re_theater);
            lvi.SubItems.Add(re_movie);
            lvi.SubItems.Add(re_time);
            lvi.SubItems.Add(seat[k,j]+seat[k,j+1]);
            listView1.Items.Add(lvi);
            count++;                        // 결제금액 계산을 위해 count 값 1증가
            k++;                            // 2차원 배열로 생성한 자리 index 값 설정
            // listview Column 들 가운데 정렬과 자동 열너비 맞춤
            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listView1.Columns[i].TextAlign = HorizontalAlignment.Center;
                listView1.Columns[i].Width = -2;
            }
            listView1.EndUpdate();
        }
        private void createview(string local, string theater_letter, string movie, string time)
        {
            // DB에서 값을 각각 불러와 GRID라는 VIEW를 생성하는 SQL 구문
            string sqlcreate = "CREATE VIEW GRID AS SELECT T.LOCAL, T.THEATER_LETTER, M.MOVIE_NAME, A.TIME FROM THEATER T, MOVIE M, TIME A where T.LOCAL = '" + local 
                + "' and M.MOVIE_NAME = '" + movie + "' and A.TIME = '" + time + "'";
            DataTable dt = s.getDT(sqlcreate);
        }

        // Reset 버튼 
        private void Reset_Click(object sender, EventArgs e)
        {
            place.SelectedIndex = 0;
            movie.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
            time.SelectedIndex = 0;
            //GRID VIEW 삭제
           DataTable dt = s.getDT("DROP VIEW GRID");
        }
        // Load 버튼을 누르면 createview 함수에서 GRID라는 VIEW를 생성해서 전체선택하여 가져옴
        private void button2_Click(object sender, EventArgs e)
        {
            createview(place.SelectedItem.ToString(), "0", movie.SelectedItem.ToString(), time.SelectedItem.ToString());
            DataTable set = s.getDT("SELECT * FROM GRID");
            dataGridView1.DataSource = set;             // GRID를 datagridview1에 datasource로 설정
        }
        // Login 후 name 설정
        private void button3_Click(object sender, EventArgs e)
        {
            LOGIN login = new LOGIN();
            login.ShowDialog();
            //원래 초기에 RESERVATION TABLE을 생성하고 TUPLE을 각각 삽입하는 방식으로 하려하였음
//            string sqlcreate = "CREATE TABLE RESERVATION (RE_UID int(11), NAME char(10), LOCAL char(10), THEATER char(10), MOVIE char(10), TIME varchar(15), SEAT_LETTER char(10), SEAT_NUMBER int(11));";
//            DataTable dt = s.getDT(sqlcreate);
            // SEAT 2차원배열에 저장되어 있던 값 초기화
            for (int i = 0; i < seat.Length; i++)
            {
                if (seat[i, 0] == null)
                    break;
                seat[i, 0] = null;
                seat[i, 1] = null;
                k = 0;
            }
            //CLIENT TABLE에서 값 호출
            DataSet LOGINNAME = s.VIEW("SELECT ID,NAME FROM CLIENT");
            foreach (DataRow r in LOGINNAME.Tables[0].Rows)
            {
                // 입력한 ID, PW가 CLIENT의 ID,PW와 같은지 확인
                if (r["ID"].ToString() == LOGIN.Passvalue_login){
                    textBox1.Text = r["NAME"].ToString();
                    Passvalue_ID = r["ID"].ToString();
                    re_name = textBox1.Text;
                    break;
                }
            }
            listView1.Items.Clear();
            
            // Dataset set에 고객 마일리지 정보
            DataSet CLIENT = s.VIEW("SELECT CLIENT_NAME,MILE FROM MILE");
            foreach(DataRow r in CLIENT.Tables[0].Rows)
            {
                if (r["CLIENT_NAME"].ToString() == LOGIN.Passvalue_login)
                {
                    textBox2.Text = r["MILE"].ToString();
                    Passvalue_mile = r["MILE"].ToString();
                    break;
                }
            }
            place.Items.Clear();
            DataSet THEATER = s.VIEW("SELECT DISTINCT LOCAL FROM THEATER");
            foreach(DataRow r in THEATER.Tables[0].Rows)
            {
                place.Items.Add(r["LOCAL"].ToString());
            }
            place.SelectedIndex = 0;
            movie.Items.Clear();
            DataSet MOVIE = s.VIEW("SELECT DISTINCT MOVIE_NAME FROM MOVIE");
            foreach (DataRow r in MOVIE.Tables[0].Rows)
            {
                movie.Items.Add(r["MOVIE_NAME"].ToString());
            }
            movie.SelectedIndex = 0;
            time.Items.Clear();
            DataSet TIME = s.VIEW("SELECT * FROM TIME");
            foreach(DataRow r in TIME.Tables[0].Rows)
            {
                time.Items.Add(r["TIME"].ToString());
            }
            time.SelectedIndex = 0;
            // listview 컬럼 생성
            listView1.Columns.Add("DATE");
            listView1.Columns.Add("PLACE");
            listView1.Columns.Add("THEATER");
            listView1.Columns.Add("MOVIE");
            listView1.Columns.Add("TIME");
            listView1.Columns.Add("SEAT");
        }
        //예약내역 확인하는 부분
        private void button4_Click(object sender, EventArgs e)
        {
            Information inform = new Information();
            inform.ShowDialog();
        }

        private void movie_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(movie.SelectedIndex!=0)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = System.Drawing.Image.FromFile(@"C:\movie_image\" + movie.SelectedItem.ToString() + ".jpg");
                textBox4.Text = movie.SelectedItem.ToString();
                DataSet SCORE = s.VIEW("SELECT * FROM SCORE where MOVIE = '" + movie.SelectedItem.ToString() + "'");
                foreach (DataRow r in SCORE.Tables[0].Rows)
                {
                    textBox3.Text = r["Score"].ToString();
                    break;
                }
            }
            else
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = System.Drawing.Image.FromFile(@"C:\movie_image\AugustRush.jpg");
                textBox4.Text = "AugustRush";
                dbbinding s = new dbbinding();
                DataSet SCORE = s.VIEW("SELECT * FROM SCORE where MOVIE = 'AugustRush'");
                foreach (DataRow r in SCORE.Tables[0].Rows)
                {
                    textBox3.Text = r["Score"].ToString();
                    break;
                }
            }
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(MessageBox.Show("날짜 : "+ date + "\r" + "지역 : " + dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString() + "\r" + "영화관 : " + dataGridView1.Rows[e.RowIndex].Cells[1].FormattedValue.ToString() + "\r"+"영화 : " +
                dataGridView1.Rows[e.RowIndex].Cells[2].FormattedValue.ToString() + "\r" + "시간 : " + dataGridView1.Rows[e.RowIndex].Cells[3].FormattedValue.ToString() + "\r" + "자리를 예약하시겠습니까?","예약내역 확인", 
                MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                re_date = date;
                re_place = dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
                re_theater = dataGridView1.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                re_movie = dataGridView1.Rows[e.RowIndex].Cells[2].FormattedValue.ToString();
                re_time = dataGridView1.Rows[e.RowIndex].Cells[3].FormattedValue.ToString();
            }
        }
        private void place_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            DataSet THEATER_LETTER = s.VIEW("SELECT DISTINCT THEATER_LETTER FROM THEATER where LOCAL='" + place.SelectedItem.ToString() + "'");
            foreach (DataRow r in THEATER_LETTER.Tables[0].Rows)
                comboBox1.Items.Add(r["THEATER_LETTER"].ToString());
            comboBox1.SelectedIndex = 0;
        }
        private void reservationdate_ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = reservationdate.Value;
            string str = string.Format("{0}월 {1}일을 선택하셨습니다.", dt.Month, dt.Day);
            MessageBox.Show(str, "선택 날짜");
            date = reservationdate.Value.ToString("yyyy-MM-dd");
        }
        private void Reserve_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("다음 좌석을 예약하시겠습니까?","예약확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Passvalue_cost = (count * 7000).ToString();
                Reservation reserve = new Reservation(this);
                reserve.Show();
                for (int a = 0; a < seat.Length; a++)
                {
                    if(seat[a,0]==null)
                        break;
                    string sql = "insert into RESERVATION values (" + re_uid++.ToString() + ",'" + re_name + "','" + re_place + "','" + re_theater + "','" + re_movie + "','" + re_time + "','" + seat[a, 0] + "'," + seat[a, 1] +");";
                    DataTable dt = s.getDT(sql);
                }
            }
        }
        private void seat_a1_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "1";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a2_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "2";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a3_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "3";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a4_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "4";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a5_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "5";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a6_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "6";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a7_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "7";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a8_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "8";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a9_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "9";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_a10_Click(object sender, EventArgs e)
        {
            re_seatletter = "A";
            re_seatnum = "10";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b1_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "1";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b2_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "2";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b3_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "3";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b4_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "4";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b5_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "5";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b6_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "6";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b7_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "7";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b8_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "8";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b9_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "9";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_b10_Click(object sender, EventArgs e)
        {
            re_seatletter = "B";
            re_seatnum = "10";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c1_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "1";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c2_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "2";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c3_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "3";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c4_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "4";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c5_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "5";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c6_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "6";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c7_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "7";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c8_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "8";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c9_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "9";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_c10_Click(object sender, EventArgs e)
        {
            re_seatletter = "C";
            re_seatnum = "10";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d1_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "1";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d2_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "2";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d3_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "3";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d4_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "4";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d5_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "5";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d6_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "6";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d7_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "7";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d8_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "8";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d9_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "9";
            createlist(re_seatletter, re_seatnum);
        }

        private void seat_d10_Click(object sender, EventArgs e)
        {
            re_seatletter = "D";
            re_seatnum = "10";
            createlist(re_seatletter, re_seatnum);
        }
        //EXIT 버튼 이벤트 부분
        private void button5_Click(object sender, EventArgs e)
        {
            DataTable dt = s.getDT("DROP VIEW GRID");
            Application.Exit();
        }
        //Listview에서 delete 버튼 이벤트 부분
        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("선택하신 항목이 삭제 됩니다.\r계속 하시겠습니까?", "항목 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    int index = listView1.FocusedItem.Index;
                    listView1.Items.RemoveAt(index);
                    count--;                // 결제금액 계산을 위해 count 값 1감소
                }
                else
                    MessageBox.Show("선택된 항목이 없습니다.");
            }
        }
        // Database reset 버튼 이벤트 부분 (RESERVATION 테이블 초기화)
        private void button6_Click(object sender, EventArgs e)
        {
            DataTable dt = s.getDT("DELETE FROM RESERVATION");
        }
    }
    // C#과 Raspberry pi내부에 구성된 Mysql 연동을 위하여 다른폼에서도 사용할 수 있게 공유 class dbbinding설정
    class dbbinding
    {
        string strConn = "Server=192.168.0.78;Database=MOVIE_Reservation;Uid=root;Pwd=rlaehdgkr;";
        public DataSet VIEW(string sql)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = new MySqlCommand(sql, conn);
            adapter.Fill(ds);
            return ds;
        }
        public DataTable getDT(string sql)
        {
            MySqlConnection conn = new MySqlConnection(strConn);
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            adapter.SelectCommand = new MySqlCommand(sql, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }
    }
}
