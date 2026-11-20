using CommunityToolkit.Mvvm.ComponentModel;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student_Attendance_Management_System.ViewModel
{
    public partial class NotificationVM : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<NotificationModel> notifications = new();

        private int currentUserID;
        public NotificationVM(int userID)
        {
            NotificationData(userID);
        }

        //Get all notification data
        public void NotificationData(int userID)
        { 
            NotificationOperation notificationOP = new NotificationOperation();
            Notifications = notificationOP.GetAllNotifications(userID);
        }

        //Get all notification read data
        public void ShowAllReadNotification(int userID)
        {
            NotificationOperation notificationOP = new();
            Notifications = notificationOP.GetReadNotifications(userID);
        }

        //Get all notification unread data
        public void ShowAllUnreadNotification(int userID)
        {
            NotificationOperation notificationOP = new();
            Notifications = notificationOP.GetAllUnreadNotifications(userID);
        }

        //Notification Update
        public void UpdateNotificationStatus(NotificationModel notification)
        {
            NotificationOperation notificationOP = new();
            notificationOP.UpdateNotificationStatus(notification.Notification_id);
        }

        //Notification Delete
        public void DeleteNotification(NotificationModel notification)
        {
            NotificationOperation notificationOP = new();
            notificationOP.DeleteNotification(notification.Notification_id);
        }
    }
}
