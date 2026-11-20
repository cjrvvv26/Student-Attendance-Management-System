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

namespace Student_Attendance_Management_System.Teacher.Components
{
    /// <summary>
    /// Interaction logic for ListOfUnreadMessages.xaml
    /// </summary>
    public partial class ListOfUnreadMessages : UserControl
    {
        public ObservableCollection<NotificationModel> NotificationList { get; set; } = new();
        private int currentTeacherUserID;
        public ListOfUnreadMessages(int teacherUserID)
        {
            InitializeComponent();
            DataContext = this;
            currentTeacherUserID = teacherUserID;
        }

        public void LoadUnreadMessages(int userID)
        {
            NotificationOperation notificationOperation = new();
            NotificationList = notificationOperation.GetAllUnreadNotifications(userID);
        }

        private void trashBtn_Click(object sender, RoutedEventArgs e)
        {
            NotificationModel notification = (NotificationModel)((Button)sender).Tag;
            if (MessageBox.Show("Delete Message", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                NotificationVM notificationVM = new(currentTeacherUserID);
                notificationVM.DeleteNotification(notification);
                NotificationList.Remove(notification);
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

            NotificationVM notificationVM = new(currentTeacherUserID);
            notificationVM.UpdateNotificationStatus(notification);
        }
    }
}
