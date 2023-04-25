using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
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
    /// Interaction logic for ProsDB.xaml
    /// </summary>
    public partial class ProsDB : Window
    {
        public ProsDB()
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
            SqlCommand findIDs = new SqlCommand(findIDQ, sqlCon);
            findIDs.CommandType = CommandType.Text;

            //Updates role & id text in the top-left
            ID_info.Text = $"ID: {Convert.ToString(findIDs.ExecuteScalar())} \r\nRole: Prosecutor";

            sqlCon.Close();
        }
        //Timer tick event handler
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }


        //Button to go to the police db query page for prosecutors
        private void Open_ProsPolDB(object sender, RoutedEventArgs e)
        {
            ProsPolDB prosStart = new ProsPolDB();
            prosStart.Show();
            this.Close();
        }

        //Button to return to login
        private void Ret_Login(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void Query(object sender, RoutedEventArgs e)
        {
            try
            {
                //If all fields have been left empty - show all records from the Trials table
                if (_ssn.Text + _dob.Text + hold_loc.Text + tri_date.Text + _char.Text + _name.Text == "")
                {
                    string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    SqlConnection sqlCon = new SqlConnection(conStr);

                    sqlCon.Open();
                    string query = "select distinct Trials.ssn, Arrests.[name], Arrests.dob, holdloc, charge, trialdate from Trials\r\nleft join Arrests on Arrests.ssn = Trials.ssn";
                    SqlCommand cmd = new SqlCommand(query, sqlCon);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ProsDBRes displayQ = new ProsDBRes();

                    displayQ.data_grid.ItemsSource = dt.DefaultView;
                    adapter.Update(dt);

                    MessageBox.Show("Successful loading");
                    sqlCon.Close();

                    //Opens results window
                    displayQ.Show();

                    this.Close();
                }
                else
                {
                    string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    SqlConnection sqlCon = new SqlConnection(conStr);
                    sqlCon.Open();

                    //A list to hold all the conditions from fields that are not empty
                    List<string> queryL = new List<string>();

                    //The initial query before the conditions are added
                    string query = "select * from (select distinct Trials.ssn, Arrests.[name], Arrests.dob, holdloc, charge, trialdate from Trials\r\nleft join Arrests on Arrests.ssn = Trials.ssn) as thing\r\nwhere ";

                    //Checks which fields are not empty and adds their condition to the list
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
                    if (hold_loc.Text != "")
                    {
                        queryL.Add($"holdloc = '{hold_loc.Text}'");
                        queryL.Add(" and ");
                    }
                    if (tri_date.Text != "")
                    {
                        queryL.Add($"trialdate = '{tri_date.Text}'");
                        queryL.Add(" and ");
                    }
                    if (_char.Text != "")
                    {
                        queryL.Add($"charge = '{_char.Text}'");
                        queryL.Add(" and ");
                    }
                    if (_name.Text != "")
                    {
                        queryL.Add($"name = '{_name.Text}'");
                        queryL.Add(" and ");
                    }

                    //Removes the last 'and' in the list
                    queryL.RemoveAt(queryL.Count - 1);

                    //Updates the query with the specific conditions
                    foreach (var i in queryL)
                    {
                        query += i;
                    }

                    MessageBox.Show(query);

                    //Queries db
                    SqlCommand cmd = new SqlCommand(query, sqlCon);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ProsDBRes displayQ = new ProsDBRes();

                    displayQ.data_grid.ItemsSource = dt.DefaultView;
                    adapter.Update(dt);

                    MessageBox.Show("Successful loading");
                    sqlCon.Close();

                    //Opens results window
                    displayQ.Show();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Insert(object sender, RoutedEventArgs e)
        {
            try
            {
                //Checks for empty fields
                if (_ssn.Text == "" || hold_loc.Text == "" || tri_date.Text == "" || _char.Text == "")
                {
                    MessageBox.Show("You cannot leave empty any of the fields listed above!");
                }
                else
                {
                    string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    SqlConnection sqlCon = new SqlConnection(conStr);
                    sqlCon.Open();

                    //Checks whether person with this ssn has been arrested
                    string checkArrQ = "select count(*) from Arrests where ssn=@ssn";
                    SqlCommand checkArr = new SqlCommand(checkArrQ, sqlCon);
                    checkArr.CommandType = CommandType.Text;
                    checkArr.Parameters.AddWithValue("@ssn", _ssn.Text);

                    //Inserts a trial for the arrested person
                    if (Convert.ToInt32(checkArr.ExecuteScalar()) > 0)
                    {
                        string insertTriQ = "insert into Trials values (@ssn, @holdloc, @charge, @tridate)";
                        SqlCommand insertTri = new SqlCommand(insertTriQ, sqlCon);
                        insertTri.CommandType = CommandType.Text;
                        insertTri.Parameters.AddWithValue("@ssn", _ssn.Text);
                        insertTri.Parameters.AddWithValue("@holdloc", hold_loc.Text);
                        insertTri.Parameters.AddWithValue("@charge", _char.Text);
                        insertTri.Parameters.AddWithValue("@tridate", tri_date.Text);
                        insertTri.ExecuteNonQuery();

                        //updates holding location of the person for every trial
                        //they can only be in a single place at once (home arrest, prison, precinct, etc.)
                        string updateHoldLocsQ = "update Trials\r\nset holdloc= @holdloc where ssn = @ssn";
                        SqlCommand updateHoldLocs = new SqlCommand(updateHoldLocsQ, sqlCon);
                        updateHoldLocs.CommandType = CommandType.Text;
                        updateHoldLocs.Parameters.AddWithValue("@holdloc", $"{hold_loc.Text}");
                        updateHoldLocs.Parameters.AddWithValue("@ssn", _ssn.Text);
                        updateHoldLocs.ExecuteNonQuery();

                        MessageBox.Show("Action Complete!");
                    }
                    else
                    {
                        MessageBox.Show("No person with this SSN found in Arrests record!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Update button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection sqlCon = new SqlConnection(conStr);
            sqlCon.Open();

            //Updates the charge for a specific trials (based on ssn & trial date)
            if (_char.Text != "")
            {
                string updateCharQ = "update Trials set charge=@char where ssn=@ssn and trialdate=@tridate";

                SqlCommand updateChar = new SqlCommand(updateCharQ, sqlCon);
                updateChar.CommandType = CommandType.Text;
                updateChar.Parameters.AddWithValue("@char", _char.Text);
                updateChar.Parameters.AddWithValue("@ssn", _ssn.Text);
                updateChar.Parameters.AddWithValue("@tridate", tri_date.Text);
                updateChar.ExecuteNonQuery();

                MessageBox.Show("Action Complete!");
            }

            //Updates the holding location of a single arrested person for all trials
            if (hold_loc.Text != "")
            {
                string updateHoldLocQ = "update Trials set holdloc=@holdloc  where ssn=@ssn";
                SqlCommand updateHoldLoc = new SqlCommand(updateHoldLocQ, sqlCon);
                updateHoldLoc.CommandType = CommandType.Text;
                updateHoldLoc.Parameters.AddWithValue("@holdloc", hold_loc.Text);
                updateHoldLoc.Parameters.AddWithValue("@ssn", _ssn.Text);
                updateHoldLoc.ExecuteNonQuery();

                MessageBox.Show("Action Complete!");
            }
        }
    }
}
