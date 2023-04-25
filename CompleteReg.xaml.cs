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

            //Timer which updates the top-right textbox with the current time every second
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection sqlCon = new SqlConnection(conStr);
            sqlCon.Open();

            //Since the id of the newly registered user is temporarily stored in the curUser table
            //We recover it from there when we switch pages
            //This could have easily been done with a variable, but I guess I just didn't think of that at the time
            string findIDQ = "select id from curUser";
            SqlCommand findID = new SqlCommand(findIDQ, sqlCon);
            findID.CommandType = CommandType.Text;
            string id = Convert.ToString(findID.ExecuteScalar());

            //Finds the password of the newly registered user from the Users table
            string findPassQ = "select password from Users where id=@id";
            SqlCommand findPass = new SqlCommand(findPassQ, sqlCon);
            findPass.CommandType= CommandType.Text;
            findPass.Parameters.AddWithValue("@id", id);
            string pass = Convert.ToString(findPass.ExecuteScalar());

            //We empty the curUser table as we no longer need it to store the new user's id
            string curUQuery = "delete from curUser";
            SqlCommand upCurU = new SqlCommand(curUQuery, sqlCon);
            upCurU.CommandType = CommandType.Text;
            upCurU.ExecuteNonQuery();

            //Update the textblock with the new user's credentials
            _id.Text = $"ID: {id}";
            _pass.Text = $"Password: {pass}";
            sqlCon.Close();

        }

        //Timer event handler to update the time
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }

        //Returns to login window
        private void Register(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}
