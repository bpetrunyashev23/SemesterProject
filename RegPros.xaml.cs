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
    /// Interaction logic for RegPros.xaml
    /// </summary>
    public partial class RegPros : Window
    {
        public RegPros()
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

        //variable matches role column in the Users table; 1 = Police officer, 0 = Prosecutor
        static int role = 0;
        
        //Attempts to register the new User
        private void Register(object sender, RoutedEventArgs e)
        {
            try
            {
                string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                
                //Checks whether a number has been entered in the bar id text box
                if (int.TryParse(id_num.Text, out int i) == false)
                {
                    MessageBox.Show("You must enter a number!");
                }
                else
                {

                    SqlConnection sqlCon = new SqlConnection(conStr);
                    sqlCon.Open();
                    
                    //Command to check whether a prosecutor with this bar_id exists in the bar_ids table
                    string query = "select count(*) from bar_ids where bar_id = @num";
                    SqlCommand checkBadge = new SqlCommand(query, sqlCon);
                    checkBadge.CommandType = CommandType.Text;
                    checkBadge.Parameters.AddWithValue("@num", id_num.Text);

                    //Command to check whether a prosecutor with this id number has already been registered
                    string outIDQ = "select count(*) from Users where outID = @num and role=@role";
                    SqlCommand outID = new SqlCommand(outIDQ, sqlCon);
                    outID.CommandType = CommandType.Text;
                    outID.Parameters.AddWithValue("@num", id_num.Text);
                    outID.Parameters.AddWithValue("@role", role);

                    //If the prosecutor exists in bar_id and has not yet been registered
                    if (Convert.ToInt32(checkBadge.ExecuteScalar()) == 1 && Convert.ToInt32(outID.ExecuteScalar()) == 0)
                    {
                        //string that will hold the password
                        string pass;

                        //Command to check whether the password has already been assigned to another user
                        string unPassQ = "select count (*) from Users where password = @pass";
                        SqlCommand unPass = new SqlCommand(unPassQ, sqlCon);
                        unPass.CommandType = CommandType.Text;
                        
                        //Generates a random string of 7 lowercase letters
                        do
                        {
                            int length = 7;
                            StringBuilder str_build = new StringBuilder();
                            Random random = new Random();
                            for (int k = 0; k < length; k++)
                            {
                                str_build.Append(Convert.ToChar(random.Next(97, 123)));
                            }
                            
                            //passes the string to the command as a parameter
                            pass = str_build.ToString();
                            unPass.Parameters.Clear();
                            unPass.Parameters.AddWithValue("@pass", pass);
                        }
                        
                        //generates a new password until it is unique for the Users table
                        while (Convert.ToInt32(unPass.ExecuteScalar()) != 0);
                        
                        //inserts the new user into the User table
                        string addUQ = "insert into Users values (@pass,@role,@outID)";
                        SqlCommand addU = new SqlCommand(addUQ, sqlCon);
                        addU.CommandType = CommandType.Text;
                        addU.Parameters.AddWithValue("@pass", pass);
                        addU.Parameters.AddWithValue("@role", role);
                        addU.Parameters.AddWithValue("@outID", Convert.ToInt32(id_num.Text));
                        addU.ExecuteNonQuery();
                        
                        //since the User id is generated automatically by incrementing the previous User's id by 1
                        //this command recovers the id from the table to display it to the User
                        string findIDQ = "select id from Users where password=@pass";
                        SqlCommand findID = new SqlCommand(@findIDQ, sqlCon);
                        findID.CommandType = CommandType.Text;
                        findID.Parameters.AddWithValue("@pass", pass);
                        string _id = Convert.ToString(findID.ExecuteScalar());

                        //We will use the curUser table to store the id of the registered user to use it to recover their data in the next page
                        string curUQuery = "delete from curUser" +
                            "\r\ninsert into curUser values (@id)";
                        SqlCommand upCurU = new SqlCommand(curUQuery, sqlCon);
                        upCurU.CommandType = CommandType.Text;
                        upCurU.Parameters.AddWithValue("@id", _id);
                        upCurU.ExecuteNonQuery();

                        sqlCon.Close();
                        
                        //Show the registration completion window (which displays the user's id and password)
                        CompleteReg regInfo = new CompleteReg();
                        regInfo.Show();
                        this.Close();
                    }
                    
                    //Output if a prosecutor with this id has already been registered
                    else if (Convert.ToInt32(outID.ExecuteScalar()) != 0)
                    {
                        MessageBox.Show("This id number has already been registered!");
                    }
                    
                    //Output if no prosecutor with this id has been found in the bar_ids table
                    else
                    {
                        MessageBox.Show("Invalid id number!");
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
