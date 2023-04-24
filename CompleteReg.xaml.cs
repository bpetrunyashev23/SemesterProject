using Microsoft.Data.SqlClient;
using System.Data;
using System;
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

namespace SemesterProject
{
    /// <summary>
    /// Interaction logic for CompleteReg.xaml
    /// </summary>
    public partial class CompleteReg : Window
    {
        public CompleteReg()
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
            string id = Convert.ToString(findID.ExecuteScalar());

            string findPassQ = "select password from Users where id=@id";
            SqlCommand findPass = new SqlCommand(findPassQ, sqlCon);
            findPass.CommandType= CommandType.Text;
            findPass.Parameters.AddWithValue("@id", id);
            string pass = Convert.ToString(findPass.ExecuteScalar());

            string curUQuery = "delete from curUser";
            SqlCommand upCurU = new SqlCommand(curUQuery, sqlCon);
            upCurU.CommandType = CommandType.Text;
            upCurU.ExecuteNonQuery();

            _id.Text = $"ID: {id}";
            _pass.Text = $"Password: {pass}";
            sqlCon.Close();

        }
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}
