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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
            
            //Timer which updates the top-right textbox with the current time every second
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this._time.Text = DateTime.Now.ToString("dd/MM/yy\nHH:mm\nAbb 108");
        }

        //Returns to login screen
        private void Ret_Login(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        //Opens the registration window for prosecutors
        private void Pros_Role(object sender, RoutedEventArgs e)
        {
            RegPros regpros = new RegPros();
            regpros.Show();
            this.Close();
        }

        //Opens the registration window for police officers
        private void Pol_Role(object sender, RoutedEventArgs e)
        {
            RegPol regpol = new RegPol();
            regpol.Show();
            this.Close();
        }
    }
}
