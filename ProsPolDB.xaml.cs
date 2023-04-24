using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SemesterProject
{
    /// <summary>
    /// Interaction logic for ProsPolDB.xaml
    /// </summary>
    public partial class ProsPolDB : Window
    {
        public ProsPolDB()
        {
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection sqlCon = new SqlConnection(conStr);
            sqlCon.Open();

            string findIDQ = "select id from curUser";
            SqlCommand findID = new SqlCommand(findIDQ, sqlCon);
            findID.CommandType = CommandType.Text;
            ID_info.Text = $"ID: {Convert.ToString(findID.ExecuteScalar())}\r\nRole: Prosecutor";

            sqlCon.Close();
        }
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }

        private void Ret_Login(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void Open_Pros(object sender, RoutedEventArgs e)
        {
            ProsDB prosDB = new ProsDB();
            prosDB.Show();
            this.Close();
        }

        private void Query(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_ssn.Text + _dob.Text + _loc.Text + _date.Text + _reas.Text + _name.Text == "")
                {
                    string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    SqlConnection sqlCon = new SqlConnection(conStr);

                    sqlCon.Open();
                    string query = "select * from Arrests";
                    SqlCommand cmd = new SqlCommand(query, sqlCon);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    PolDBRes displayQ = new PolDBRes();

                    displayQ.data_grid.ItemsSource = dt.DefaultView;
                    adapter.Update(dt);

                    MessageBox.Show("Successful loading");
                    sqlCon.Close();

                    displayQ.Show();

                    this.Close();
                }
                else
                {
                    string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    SqlConnection sqlCon = new SqlConnection(conStr);
                    sqlCon.Open();

                    List<string> queryL = new List<string>();
                    string query = "select * from Arrests where ";
                    if (_ssn.Text != "")
                    {
                        queryL.Add($"ssn = {_ssn.Text}");
                        queryL.Add(" and ");
                    }
                    if (_dob.Text != "")
                    {
                        queryL.Add($"dob = '{_dob.Text}'");
                        queryL.Add(" and ");
                    }
                    if (_loc.Text != "")
                    {
                        queryL.Add($"loc = '{_loc.Text}'");
                        queryL.Add(" and ");
                    }
                    if (_date.Text != "")
                    {
                        queryL.Add($"arrdate = '{_date.Text}'");
                        queryL.Add(" and ");
                    }
                    if (_reas.Text != "")
                    {
                        queryL.Add($"reason = '{_reas.Text}'");
                        queryL.Add(" and ");
                    }
                    if (_name.Text != "")
                    {
                        queryL.Add($"name = '{_name.Text}'");
                        queryL.Add(" and ");
                    }
                    queryL.RemoveAt(queryL.Count - 1);
                    foreach (var i in queryL)
                    {
                        query += i;
                    }

                    MessageBox.Show(query);

                    SqlCommand cmd = new SqlCommand(query, sqlCon);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    PolDBRes displayQ = new PolDBRes();

                    displayQ.data_grid.ItemsSource = dt.DefaultView;
                    adapter.Update(dt);

                    MessageBox.Show("Successful loading");
                    sqlCon.Close();

                    displayQ.Show();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
