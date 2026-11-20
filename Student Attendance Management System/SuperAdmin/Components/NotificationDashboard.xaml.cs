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

namespace Student_Attendance_Management_System.SuperAdmin.Components
{
    /// <summary>
    /// Interaction logic for NotificationDashboard.xaml
    /// </summary>
    public partial class NotificationDashboard : UserControl
    {
        private int currentAdminUserID;
        public NotificationDashboard(int adminUserID)
        {
            InitializeComponent();
            DataContext = new NotificationVM(adminUserID);
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (itemControlBox.Items.Count <= 0)
                {
                    noMessagesText.Visibility = Visibility.Visible;
                    itemControlBox.Visibility = Visibility.Collapsed;
                    return;
                }
                noMessagesText.Visibility = Visibility.Collapsed;
                itemControlBox.Visibility = Visibility.Visible;
            }), System.Windows.Threading.DispatcherPriority.Background);
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
    }
}
