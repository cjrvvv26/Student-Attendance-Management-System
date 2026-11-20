using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
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

namespace Student_Attendance_Management_System.Student.Components
{
    /// <summary>
    /// Interaction logic for NotificationList.xaml
    /// </summary>
    public partial class NotificationList : UserControl
    {
        public ObservableCollection<NotificationModel> NotificationLists { get; set; } = new();
        private int currentStudentUserID;
        public NotificationList(int studentUserID)
        {
            InitializeComponent();
            DataContext = this;
            currentStudentUserID = studentUserID;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notifs.Items.Count > 0)
                {
                    notifs.Visibility = Visibility.Visible;
                    noMsgText.Visibility = Visibility.Collapsed;
                    return;
                }
                notifs.Visibility = Visibility.Collapsed;
                noMsgText.Visibility = Visibility.Visible;
            }),System.Windows.Threading.DispatcherPriority.Background);
        }

        public void LoadUnreadMessages(int userID)
        {
            NotificationOperation notificationOperation = new();
            NotificationLists = notificationOperation.GetAllUnreadNotifications(userID);
        }

        private void trashBtn_Click(object sender, RoutedEventArgs e)
        {
            NotificationModel notification = (NotificationModel)((Button)sender).Tag;
            if (MessageBox.Show("Delete Message", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                NotificationVM notificationVM = new(currentStudentUserID);
                notificationVM.DeleteNotification(notification);
                NotificationLists.Remove(notification);
            }
        }

        private void notifBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is NotificationModel notification)
            {

                notification.Status = "Read";
                UpdateNotificationStatus(notification);
            }
        }

        private void UpdateNotificationStatus(NotificationModel notification)
        {
            NotificationOperation notificationOP = new();

            NotificationVM notificationVM = new(currentStudentUserID);
            notificationVM.UpdateNotificationStatus(notification);
        }
    }
}
