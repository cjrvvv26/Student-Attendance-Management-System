using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.Data
{
    class TargetUser
    {
        public int Id { get; set; }
        public string UserNo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    class NotificationOperation
    {
        //Get all the notification
        public ObservableCollection<NotificationModel> GetAllNotifications(int userID)
        {
            ObservableCollection<NotificationModel> notifications = new();

            string query = "SELECT * FROM notifications WHERE user_id = @userID ORDER BY (status = 'Unread') DESC, created_at DESC";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@userID", userID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notifications.Add(new NotificationModel
                        {
                            Notification_id = reader.GetInt32("notification_id"),
                            Title = reader.GetString("title"),
                            Message = reader.GetString("message"),
                            Type = reader.GetString("type"),
                            User_id = reader.GetInt32("user_id").ToString(),
                            Status = reader.GetString("status"),
                            Triggered_by = reader.GetString("triggered_by"),
                            Created_at = reader.GetDateTime("created_at"),
                        });
                    }
                    return notifications;
                }
            }
        }

        //Get all the notification that is read
        public ObservableCollection<NotificationModel> GetReadNotifications(int userID)
        {
            ObservableCollection<NotificationModel> notifications = new();
            string query = "SELECT * FROM notifications WHERE status = 'Read' AND user_id = @userID ORDER BY created_at DESC";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notifications.Add(new NotificationModel
                        {
                            Notification_id = reader.GetInt32("notification_id"),
                            Title = reader.GetString("title"),
                            Message = reader.GetString("message"),
                            Type = reader.GetString("type"),
                            User_id = reader.GetInt32("user_id").ToString(),
                            Status = reader.GetString("status"),
                            Triggered_by = reader.GetString("triggered_by"),
                            Created_at = reader.GetDateTime("created_at"),
                        });
                    }
                    return notifications;
                }
            }
        }

        //Get all the notification that is unread
        public ObservableCollection<NotificationModel> GetAllUnreadNotifications(int userID)
        {
            ObservableCollection<NotificationModel> notifications = new();
            string query = "SELECT * FROM notifications WHERE status = 'Unread' AND user_id = @userID ORDER BY created_at DESC";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notifications.Add(new NotificationModel
                        {
                            Notification_id = reader.GetInt32("notification_id"),
                            Title = reader.GetString("title"),
                            Message = reader.GetString("message"),
                            Type = reader.GetString("type"),
                            User_id = reader.GetInt32("user_id").ToString(),
                            Status = reader.GetString("status"),
                            Triggered_by = reader.GetString("triggered_by"),
                            Created_at = reader.GetDateTime("created_at"),
                        });
                    }
                    return notifications;
                }
            }
        }

        //Insert Notif
        public void AddNotification(int targetUserID, string role, string type)
        {
            AdminCreateAccountNotification(targetUserID, role, type);
        }

        //Type:  account
        public static void AdminCreateAccountNotification (int targetUserID, string role, string type)
        {
            TargetUser? user = GetTargetUserInfo(targetUserID);

            if (user == null)
            {
                MessageBox.Show("Target user doesn't found");
                return;
            }

            string title = "", message = "";
            AdminOperations adminOp = new();
            switch(type)
            {
                case "Create account":
                    title = "New Account Created";
                    message = $"A new {user.Role} account has been created for {user.Name}.";
                    //Super admin Notif NEED TO HARD CODED THE ID OF SUPER ADMIN
                    foreach(var admin in adminOp.GetAllAdmins())
                    {
                        NotificationHelper(admin.User_id, title, message, type, "Unread", role);
                    }
                    //User notif
                    string userMsg = $"Hi! {user.Name}, you're our new {user.Role}.";
                    NotificationHelper(user.Id, $"Welcome, {user.Name}!", userMsg, type, "Unread", role);
                    break;
                case "Update account":
                    title = $"An account has been updated";
                    message = $"There's an update on {user.Name}'s account.";
                    //Super Admin Notif
                    foreach (var admin in adminOp.GetAllAdmins())
                    {
                        NotificationHelper(admin.User_id, title, message, type, "Unread", role);
                    }
                    //User notif
                    string updateMsg = $"Hi there {user.Name}! You successfully update for account.";
                    NotificationHelper(user.Id, "Account Successfully Update!", updateMsg, type, "Unread", role);
                    break;
                    //INFORMATION: This notification is for super admin only
                case "Delete account":
                    title = $"Account deleted successfully!";
                    message = $"The account of {user.Name} : {user.UserNo} was deleted successfully.";
                    foreach (var admin in adminOp.GetAllAdmins())
                    {
                        NotificationHelper(admin.User_id, title, message, type, "Unread", role);
                    }
                    break;
            }
        }

        //Type: CRUD Non-user
        public void AdminNonUserNotification(int userID, string entity, string entity_name, string type, string role)
        {
            string title = "";
            string message = "";

            switch(entity)
            {
                case "Subject":
                    switch(type)
                    {
                        case "Create subject":
                            title = "New Subject Created";
                            message = $"A new subject named {entity_name} has been created.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Update subject":
                            title = "Subject Updated";
                            message = $"A subject named {entity_name} has been updated.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Delete subject":
                            title = "Subject Deleted";
                            message = $"A subject named {entity_name} has been deleted.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Restore subject":
                            title = "Subject Restored";
                            message = $"{entity_name} subject has been successfully restored.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                    }
                    break;
                case "Section":
                    switch (type)
                    {
                        case "Create section":
                            title = "New Section Created";
                            message = $"A new section named {entity_name} has been created.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Update section":
                            title = "Section Updated";
                            message = $"A section named {entity_name} has been updated.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Delete section":
                            title = "Section Deleted";
                            message = $"A section named {entity_name} has been deleted.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Restore section":
                            title = "Course and Section Restored";
                            message = $"{entity_name} has been successfully restored.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                    }
                    break;
                case "Class":
                    switch (type)
                    {
                        case "Create class":
                            title = "New Class Created";
                            message = $"A new class has been created.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Update class":
                            title = "Class Updated";
                            message = $"A class has been updated.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Delete class":
                            title = "Class Deleted";
                            message = $"A class has been deleted.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                        case "Restore class":
                            title = "Class Restored";
                            message = $"A class for {entity_name} has been successfully restored.";
                            NonUserNotificationHelper(userID, title, message, type, "Unread", role);
                            break;
                    }
                    break;
            }

        }

        //Notification for non-user helper  FOR UPDATE: ADD SUPER ADMIN ID
        private static void NonUserNotificationHelper(int userID, string title, string message, string type, string status, string triggeredByRole)
        {
            string query = "INSERT INTO notifications (user_id, title, type, message, status, triggered_by, created_at) " +
                   "VALUES (@userId, @title, @type, @message, @status, @triggeredBy, @createdAt)";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using (MySqlCommand cmd = new(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userID); // add super admin's id
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@triggeredBy", triggeredByRole);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Add Notification Helper
        private static void NotificationHelper(int userID, string title, string message, string type, string status, string triggeredByRole)
        {
            string query = "INSERT INTO notifications (user_id, title, type, message, status, triggered_by, created_at) " +
                   "VALUES (@userId, @title, @type, @message, @status, @triggeredBy, @createdAt)";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using (MySqlCommand cmd = new(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userID);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@triggeredBy", triggeredByRole);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Update Notification Status 
        public void UpdateNotificationStatus(int notifID)
        {
            string query = "UPDATE notifications SET status = 'Read' WHERE notification_id = @notifID";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@notifID", notifID);
                cmd.ExecuteNonQuery();
            }
        }

        //Delete Notification
        public void DeleteNotification(int notifID)
        {
            string query = "DELETE FROM notifications WHERE notification_id = @notifID";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@notifID", notifID);
                cmd.ExecuteNonQuery();
            }
        }

        //Identify the target user
        public static TargetUser GetTargetUserInfo(int userID)
        {
            TargetUser user = new();

            using (MySqlConnection conn = DataHelper.connectDB()) {
                //Check first the teacher's table
                string teacherQuery = "SELECT employee_number, user_id, first_name, last_name, role FROM teachers WHERE user_id = @user_id";
                using (MySqlCommand cmd = new(teacherQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new TargetUser
                            {
                                UserNo = reader.GetString("employee_number"),
                                Id = reader.GetInt32("user_id"),
                                Name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Role = "Teacher"
                            };
                            return user;
                        }
                    }
                }
                //student table
                string studentQuery = "SELECT student_number, user_id, first_name, last_name, role FROM students WHERE user_id = @student_id";
                using MySqlCommand studentCmd = new(studentQuery, conn);
                studentCmd.Parameters.AddWithValue("@student_id", userID);
                using (MySqlDataReader reader = studentCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new TargetUser
                        {
                            UserNo = reader.GetString("student_number"),
                            Id = reader.GetInt32("user_id"),
                            Name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Role = "Student"
                        };
                        return user;
                    }
                }

                //super admin table
                string superAdminQuery = "SELECT admin_id, user_id, first_name, last_name, role FROM admin WHERE user_id = @userID";

                using MySqlCommand SACmd = new(superAdminQuery, conn);
                SACmd.Parameters.AddWithValue("@userID", userID);
                using (MySqlDataReader reader = SACmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new TargetUser
                        {
                            UserNo = Convert.ToString(reader.GetInt32("admin_id")),
                            Id = reader.GetInt32("user_id"),
                            Name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Role = "Super Admin"
                        };
                        return user;
                    }
                }
            }
            return null;
        }
    }
}
