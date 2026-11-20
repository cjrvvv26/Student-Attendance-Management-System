using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : Page
    {
        private AllUsers allUsersPage;
        public Users()
        {
            InitializeComponent();

            allUsersPage = new AllUsers();
            UsersContent.Navigate(allUsersPage);
            Dispatcher.BeginInvoke(new Action(() => {
                if (allUsersPage.DisplayAllUsers.Items.Count > 0)
                {
                    noUserDisplayText.Visibility = Visibility.Collapsed;
                    UsersContent.Visibility = Visibility.Visible;
                }
                else
                {
                    noUserDisplayText.Visibility = Visibility.Visible;
                    UsersContent.Visibility = Visibility.Collapsed;
                }
                //It delays the code execution until UI rendering is done.
            }), System.Windows.Threading.DispatcherPriority.Background);
            ObservableCollection<UserModel> models = new();
            
        }

        public void AllUsers_Click(object sender, RoutedEventArgs e)
        {
            AllUsers allUsers = new();
            Dispatcher.BeginInvoke(new Action(() => {
                if (allUsers.DisplayAllUsers.Items.Count > 0)
                {
                    noUserDisplayText.Visibility = Visibility.Collapsed;
                    UsersContent.Visibility = Visibility.Visible;
                }
                else
                {
                    noUserDisplayText.Visibility = Visibility.Visible;
                    UsersContent.Visibility = Visibility.Collapsed;
                }
            }));
            UsersContent.Navigate(new AllUsers());
        }

        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new AddTeacher());
            }
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null) { 
                mainPageAdmin.SAdminContent.Navigate(new AddStudent());
            }
        }
    }
}
