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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : Page
    {
        private int currentAdminUserID;
        public Notification(int adminUserID)
        {
            InitializeComponent();
            currentAdminUserID = adminUserID;
            DataContext = new NotificationVM(adminUserID);
            all.Foreground = (Brush)FindResource("DefaultPrimary");
        }

        private void notifBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is NotificationModel notification)
            {

                notification.Status = "Read";
                NotificationVM notifVM = (NotificationVM)DataContext;
                UpdateNotificationStatus(notification);
            }
        }

        private void UpdateNotificationStatus(NotificationModel notification)
        {
            NotificationOperation notificationOP = new();

            NotificationVM notificationVM = new(currentAdminUserID);
            notificationVM.UpdateNotificationStatus(notification);
        }

        private void trashBtn_Click(object sender, RoutedEventArgs e)
        {
            NotificationModel notification = (NotificationModel)((Button)sender).Tag;
            if (MessageBox.Show("Delete Message", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                NotificationVM notificationVM = new(currentAdminUserID);
                notificationVM.DeleteNotification(notification);
                DataContext = null;
                DataContext = new NotificationVM(currentAdminUserID);
            }
        }

        private void ShowAll_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new NotificationVM(currentAdminUserID);
            all.Foreground = (Brush)FindResource("DefaultPrimary");
            read.Foreground = (Brush)FindResource("DefaultGray");
            unread.Foreground = (Brush)FindResource("DefaultGray");
        }

        private void ShowRead_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotificationVM notificationVM)
            {
                notificationVM.ShowAllReadNotification(currentAdminUserID);
                all.Foreground = (Brush)FindResource("DefaultGray");
                read.Foreground = (Brush)FindResource("DefaultPrimary");
                unread.Foreground = (Brush)FindResource("DefaultGray");
            }
        }

        private void ShowUnread_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotificationVM notificationVM)
            {
                notificationVM.ShowAllUnreadNotification(currentAdminUserID);
                all.Foreground = (Brush)FindResource("DefaultGray");
                read.Foreground = (Brush)FindResource("DefaultGray");
                unread.Foreground = (Brush)FindResource("DefaultPrimary");
            }
        }
    }
}
