using Microsoft.Data.SqlClient;
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
using System.Data;
using System.Security.Cryptography;

namespace SemesterProject
{
    /// <summary>
    /// Interaction logic for RegPol.xaml
    /// </summary>
    public partial class RegPol : Window
    {
        //variable matches role column in the Users table; 1 = Police officer, 0 = Prosecutor
        static int role = 1;
        
        public RegPol()
        {
            InitializeComponent();
            

            //Timer which updates the top-right text block with the current time every second
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }

        //Returns to registration screen
        private void Return(object sender, RoutedEventArgs e)
        {
            Register reg = new Register();
            reg.Show();
            this.Close();
        }

        //Attempts to register the new User
        private void Register(object sender, RoutedEventArgs e)
        {
            try
            {
                string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                
                //Checks whether a number has been entered in badge num text box
                if (int.TryParse(badge_num.Text, out int i) == false)
                {
                    MessageBox.Show("You must enter a number!");
                }
                else
                {

                    //Command to check whether a prosecutor with this badge num exists in the police_badges table
                    SqlConnection sqlCon = new SqlConnection(conStr);
                    sqlCon.Open();
                    string query = "select count(*) from police_badges where badge_num = @num";
                    SqlCommand checkBadge = new SqlCommand(query, sqlCon);
                    checkBadge.CommandType = CommandType.Text;
                    checkBadge.Parameters.AddWithValue("@num",badge_num.Text);

                    //Command to check whether a police officer with this badge number has already been registered
                    string outIDQ = "select count(*) from Users where outID = @num and role=@role";
                    SqlCommand outID = new SqlCommand(outIDQ, sqlCon);
                    outID.CommandType = CommandType.Text;
                    outID.Parameters.AddWithValue("@num", badge_num.Text);
                    outID.Parameters.AddWithValue("@role", role);

                    if (Convert.ToInt32(checkBadge.ExecuteScalar()) == 1 && Convert.ToInt32(outID.ExecuteScalar()) == 0)
                    {
                        string pass;

                        string unPassQ = "select count (*) from Users where password = @pass";
                        SqlCommand unPass = new SqlCommand(unPassQ, sqlCon);
                        unPass.CommandType = CommandType.Text;
                        do
                        {
                            int length = 7;
                            StringBuilder str_build = new StringBuilder();
                            Random random = new Random();
                            for (int k = 0; k < length; k++)
                            {
                                str_build.Append(Convert.ToChar(random.Next(97, 123)));
                            }
                            pass = str_build.ToString();
                            unPass.Parameters.Clear();
                            unPass.Parameters.AddWithValue("@pass", pass);
                        }
                        while (Convert.ToInt32(unPass.ExecuteScalar()) != 0);

                        string addUQ = "insert into Users values (@pass,@role,@outID)";
                        SqlCommand addU = new SqlCommand(addUQ, sqlCon);
                        addU.CommandType = CommandType.Text;
                        addU.Parameters.AddWithValue("@pass", pass);
                        addU.Parameters.AddWithValue("@role", role);
                        addU.Parameters.AddWithValue("@outID", Convert.ToInt32(badge_num.Text));
                        addU.ExecuteNonQuery();

                        string findIDQ = "select id from Users where password=@pass";
                        SqlCommand findID = new SqlCommand(@findIDQ, sqlCon);
                        findID.CommandType = CommandType.Text;
                        findID.Parameters.AddWithValue("@pass",pass);
                        string _id = Convert.ToString(findID.ExecuteScalar());

                        string curUQuery = "delete from curUser" +
                            "\r\ninsert into curUser values (@id)";
                        SqlCommand upCurU = new SqlCommand(curUQuery, sqlCon);
                        upCurU.CommandType = CommandType.Text;
                        upCurU.Parameters.AddWithValue("@id", _id);
                        upCurU.ExecuteNonQuery();

                        sqlCon.Close();
                        CompleteReg regInfo = new CompleteReg();
                        regInfo.Show();
                        this.Close();
                    }
                    else if (Convert.ToInt32(outID.ExecuteScalar()) != 0)
                    {
                        MessageBox.Show("This badge number has already been registered!");
                    }
                    else
                    {
                        MessageBox.Show("Invalid badge number!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
