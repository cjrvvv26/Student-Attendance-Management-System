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

namespace Student_Attendance_Management_System.Teacher.Pages
{
    /// <summary>
    /// Interaction logic for Notifications.xaml
    /// </summary>
    public partial class Notifications : Page
    {
        private TeacherModel teacherModel;
        public Notifications(TeacherModel teacher)
        {
            InitializeComponent();
            teacherModel = teacher;
            DataContext = new NotificationVM(teacher.User_id);
            all.Foreground = (Brush)FindResource("DefaultPrimary");
        }

        //update notif stats when its clicked
        private void notifBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is NotificationModel notification)
            {

                notification.Status = "Read";
                NotificationVM notifVM = (NotificationVM)DataContext;
                UpdateNotificationStatus(notification);
            }
        }

        //update
        private void UpdateNotificationStatus(NotificationModel notification)
        {
            NotificationOperation notificationOP = new();

            NotificationVM notificationVM = new(teacherModel.User_id);
            notificationVM.UpdateNotificationStatus(notification);
        }

        //delete
        private void trashBtn_Click(object sender, RoutedEventArgs e)
        {
            NotificationModel notification = (NotificationModel)((Button)sender).Tag;
            if (MessageBox.Show("Delete Message", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                NotificationVM notificationVM = new(teacherModel.User_id);
                notificationVM.DeleteNotification(notification);
                DataContext = null;
                DataContext = new NotificationVM(teacherModel.User_id);
            }
        }
        //view unread
        private void unread_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotificationVM notificationVM)
            {
                notificationVM.ShowAllUnreadNotification(teacherModel.User_id);
                all.Foreground = (Brush)FindResource("DefaultGray");
                read.Foreground = (Brush)FindResource("DefaultGray");
                unread.Foreground = (Brush)FindResource("DefaultPrimary");
            }
        }
        //view all
        private void all_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new NotificationVM(teacherModel.User_id);
            all.Foreground = (Brush)FindResource("DefaultPrimary");
            read.Foreground = (Brush)FindResource("DefaultGray");
            unread.Foreground = (Brush)FindResource("DefaultGray");
        }
        //view read
        private void read_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotificationVM notificationVM)
            {
                notificationVM.ShowAllReadNotification(teacherModel.User_id);
                all.Foreground = (Brush)FindResource("DefaultGray");
                read.Foreground = (Brush)FindResource("DefaultPrimary");
                unread.Foreground = (Brush)FindResource("DefaultGray");
            }
        }
    }
}
