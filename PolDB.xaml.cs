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
    /// Interaction logic for PolDB.xaml
    /// </summary>
    public partial class PolDB : Window
    {
        public PolDB()
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
            SqlCommand findIDs = new SqlCommand(findIDQ, sqlCon);
            findIDs.CommandType = CommandType.Text;
            ID_info.Text = $"ID: {Convert.ToString(findIDs.ExecuteScalar())} \r\nRole: Police officer";

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

        private void Insert(object sender, RoutedEventArgs e)
        {
            try
            {
                //Checks if any field is empty
                if (_ssn.Text == "" || _loc.Text == "" || _date.Text == "" || _dob.Text == "" || _name.Text=="" || _reas.Text=="")
                {
                    MessageBox.Show("You cannot leave empty fields!");
                }
                else
                {
                    string conStr = @"Data Source=DESKTOP-HD9RKJ8;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                    SqlConnection sqlCon = new SqlConnection(conStr);
                    sqlCon.Open();

                    //Checks whether person with this SSN exists in the arrest record
                    string checkArrQ = "select count(*) from Arrests where ssn=@ssn";
                    SqlCommand checkArr = new SqlCommand(checkArrQ, sqlCon);
                    checkArr.CommandType = CommandType.Text;
                    checkArr.Parameters.AddWithValue("@ssn", _ssn.Text);
                    //If the person exists
                    if (Convert.ToInt32(checkArr.ExecuteScalar()) > 0)
                    {
                        //Checks whether the name & date of birth of person with this ssn match those entered in the text boxes
                        string checkPersDetQ = "select count(*) from Arrests where ssn=@ssn and name=@name and dob=@dob";
                        SqlCommand checkPersDet = new SqlCommand(checkPersDetQ, sqlCon);
                        checkPersDet.CommandType = CommandType.Text;
                        checkPersDet.Parameters.AddWithValue("@ssn", _ssn.Text);
                        checkPersDet.Parameters.AddWithValue("@name", _name.Text);
                        checkPersDet.Parameters.AddWithValue("@dob", _dob.Text);
                        //If there is no match of the name or date of birth
                        if (Convert.ToInt32(checkPersDet.ExecuteScalar()) == 0)
                        {
                            MessageBox.Show("Name and/or date of birth does not match the SSN");
                        }
                        else
                        {
                            string insertArrQ = "insert into Arrests values (@ssn, @dob, @loc, @arrdate, @reason, @name)";
                            SqlCommand insertArr = new SqlCommand(insertArrQ, sqlCon);
                            insertArr.CommandType = CommandType.Text;
                            insertArr.Parameters.AddWithValue("@ssn", _ssn.Text);
                            insertArr.Parameters.AddWithValue("@loc", _loc.Text);
                            insertArr.Parameters.AddWithValue("@reason", _reas.Text);
                            insertArr.Parameters.AddWithValue("@arrdate", _date.Text);
                            insertArr.Parameters.AddWithValue("@dob", _dob.Text);
                            insertArr.Parameters.AddWithValue("@name", _name.Text);
                            insertArr.ExecuteNonQuery();

                            MessageBox.Show("Action Complete!");
                        }
                    }
                    //Just insert values for person arrested for the first time (their ssn is not in the arrest record)
                    else
                    {
                        string insertArrQ = "insert into Arrests values (@ssn, @dob, @loc, @arrdate, @reason, @name)";
                        SqlCommand insertArr = new SqlCommand(insertArrQ, sqlCon);
                        insertArr.CommandType = CommandType.Text;
                        insertArr.Parameters.AddWithValue("@ssn", _ssn.Text);
                        insertArr.Parameters.AddWithValue("@loc", _loc.Text);
                        insertArr.Parameters.AddWithValue("@reason", _reas.Text);
                        insertArr.Parameters.AddWithValue("@arrdate", _date.Text);
                        insertArr.Parameters.AddWithValue("@dob", _dob.Text);
                        insertArr.Parameters.AddWithValue("@name", _name.Text);
                        insertArr.ExecuteNonQuery();

                        MessageBox.Show("Action Complete!");
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
