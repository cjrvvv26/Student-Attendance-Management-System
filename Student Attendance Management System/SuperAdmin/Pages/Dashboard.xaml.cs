using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Components;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public AdminModel adminModel;
        public Dashboard(int adminUserID)
        {
            InitializeComponent();
            AdminVM adminVm = new(adminUserID);
            adminModel = adminVm.AdminData;
            NotificationDashboard notifDash = new(adminModel.User_id);
            notifBox.Children.Add(notifDash);
        }

        private void courseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void sectionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void termCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void trashBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void notifBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        //redirect to audit logs
        private void ViewMoreLogs_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            
            if (mainPage != null)
            {
                mainPage.SAdminContent.Navigate(new AuditLogs());
            }
        }
        //redirect to notifications
        private void ViewMoreNotification_Click(object sender, RoutedEventArgs e)
        {
            SuperAdminLayout? mainPage = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();

            if (mainPage != null)
            {
                mainPage.SAdminContent.Navigate(new Notification(adminModel.User_id));
            }
        }
    }
}
