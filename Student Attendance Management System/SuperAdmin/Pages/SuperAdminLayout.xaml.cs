using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
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

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for SuperAdminLayout.xaml
    /// </summary>
    public partial class SuperAdminLayout : Window
    {
        private AdminModel _admin;
        public SuperAdminLayout(AdminModel admin)
        {
            InitializeComponent();
            DataContext = new AdminVM(admin.User_id);
            _admin = admin;
            SAdminContent.Navigate(new Dashboard(_admin.User_id));
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new Dashboard(_admin.User_id));
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new Profile(_admin.User_id));
        }

        private void Class_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new ViewAllClasses(_admin.User_id));
        }

        private void Sections_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new ViewAllSections(_admin.User_id));
        }

        private void Subjects_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new ViewSubjects(_admin.User_id));
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new Users());
        }

        private void Notification_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new Notification(_admin.User_id));
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new Reports());
        }

        private void Audit_Logs_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new AuditLogs());
        }

        private void Trash_Click(object sender, RoutedEventArgs e)
        {
            SAdminContent.Navigate(new Trash(_admin.User_id));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                        "Do you want to exit the application or go back to the login page?",
                        "Logout Confirmation",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    // Exit the app
                    Application.Current.Shutdown();
                    break;

                case MessageBoxResult.No:
                    // Go back to login page (MainWindow)
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close(); // Assuming this is called from another window like Dashboard
                    break;

                case MessageBoxResult.Cancel:
                    // Do nothing, stay on current window
                    break;
            }

        }

    }
}
