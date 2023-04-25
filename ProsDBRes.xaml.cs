using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.Data.SqlClient;

namespace SemesterProject
{
    /// <summary>
    /// Interaction logic for ProsDBRes.xaml
    /// </summary>
    public partial class ProsDBRes : Window
    {
        public ProsDBRes()
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

            //Finds curUser id
            string findIDQ = "select id from curUser";
            SqlCommand findID = new SqlCommand(findIDQ, sqlCon);
            findID.CommandType = CommandType.Text;
            int id = Convert.ToInt32(findID.ExecuteScalar());

            string role = "Prosecutor";

            //Updates id & role text in the top-left
            ID_info.Text = $"ID: {id}\r\nRole: {role}";

            sqlCon.Close();
        }

        //Timer tick event handler
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }

        private void Return(object sender, RoutedEventArgs e)
        {
            //Returns to the prosecution query page
            ProsDB prosDBStart = new ProsDB();
            prosDBStart.Show();
            this.Close();
        }
    }
}
