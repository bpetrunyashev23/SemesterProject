using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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
    /// Interaction logic for PolDBRes.xaml
    /// </summary>
    public partial class PolDBRes : Window
    {
        public string role = "";
        public PolDBRes()
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

            //Commands to find role and id of user viewing the page
            //This page may be viewed by both police officers and prosecutors, so the role must be checked based on the user id
            string findIDQ = "select id from curUser";
            SqlCommand findID = new SqlCommand(findIDQ, sqlCon);
            findID.CommandType = CommandType.Text;
            int id = Convert.ToInt32(findID.ExecuteScalar());

            string findRoleQ = "select role from Users where id=" + Convert.ToString(id);
            SqlCommand findRole = new SqlCommand (findRoleQ, sqlCon);
            findRole.CommandType = CommandType.Text;

            //Updates the id & role text accordingly
            if (Convert.ToInt32(findRole.ExecuteScalar()) == 1)
            {
                role = "Police officer";
            }
            else if (Convert.ToInt32(findRole.ExecuteScalar()) == 0)
            {
                role = "Prosecutor";
            }

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
            //Returns to police query page for police officers
            if (role == "Police officer")
            {
                PolDB prosStart = new PolDB();
                prosStart.Show();
                this.Close();
            }
            //Returns to police query page for prosecutors
            else if (role == "Prosecutor")
            {
                ProsPolDB prosStart = new ProsPolDB();
                prosStart.Show();
                this.Close();
            }    
        }
    }
}
