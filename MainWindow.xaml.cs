using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Timers;
using System.Data.SqlClient;
using System.Data;

namespace SemesterProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                if (int.TryParse(_id.Text, out int i) == false)
                {
                    MessageBox.Show("Your ID must be a number!");
                }
                else if (_pass.Password == "")
                {
                    MessageBox.Show("You need to enter a password!");
                }
                else
                {
                    SqlConnection sqlCon = new SqlConnection(conStr);
                    sqlCon.Open();
                    string query = "select count(*) from Users" +
                        " where id=@id and password=@pass and role=@role";
                    SqlCommand checkUser = new SqlCommand(query, sqlCon);
                    checkUser.CommandType = CommandType.Text;
                    checkUser.Parameters.AddWithValue("@id", int.Parse(_id.Text));
                    checkUser.Parameters.AddWithValue("@pass", _pass.Password);
                    checkUser.Parameters.AddWithValue("@role", _role.SelectedIndex);
                    if (Convert.ToInt32(checkUser.ExecuteScalar()) == 1)
                    {
                        string curUQuery = "delete from curUser" +
                            "\r\ninsert into curUser values (@id)";
                        SqlCommand upCurU = new SqlCommand(curUQuery, sqlCon);
                        upCurU.CommandType = CommandType.Text;
                        upCurU.Parameters.AddWithValue("@id", _id.Text);
                        upCurU.ExecuteNonQuery();
                        sqlCon.Close();

                        if (_role.SelectedIndex == 0)
                        {
                            ProsPolDB prosStart = new ProsPolDB();
                            prosStart.Show();
                            this.Close();
                        }

                        else if (_role.SelectedIndex == 1)
                        {
                            PolDB polStart = new PolDB();
                            polStart.Show();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid credentials!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Register reg = new Register();
                reg.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
