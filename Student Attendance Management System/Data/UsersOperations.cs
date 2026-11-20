using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.Data
{
    class UsersOperations
    {
        //Get All Users
        public ObservableCollection<UserModel> GetAllUsers()
        {
            ObservableCollection<UserModel> users = new();
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                string SELECT_ALL_QUERY = "SELECT u.user_id, COALESCE(s.student_id, t.teacher_id) AS user_pr_id, COALESCE(s.student_number, t.employee_number, ' ') AS user_no, COALESCE(s.first_name, t.first_name) AS first_name, COALESCE(s.middle_name, t.middle_name) AS middle_name, COALESCE(s.last_name, t.last_name) AS last_name, u.email, COALESCE(s.gender, t.gender) AS gender, COALESCE(s.age, t.age) AS age, u.role FROM users u" +
                    " LEFT JOIN students s ON u.user_id = s.user_id " +
                    "LEFT JOIN teachers t ON u.user_id = t.user_id " +
                    "WHERE u.role != 'Super Admin'";
                using (MySqlCommand cmd = new MySqlCommand(SELECT_ALL_QUERY, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            users.Add(new UserModel
                            {
                                User_no = reader.GetString("user_no"),
                                First_name = reader.GetString("first_name"),
                                Last_name = reader.GetString("last_name"),
                                User_id = reader.GetInt32("user_id"),
                                User_PrimaryID = reader.IsDBNull("user_pr_id") ? null : reader.GetInt32("user_pr_id"),
                                Email = reader.GetString("email"),
                                Role = reader.GetString("role"),
                            });
                        }
                    }
                }
            }
            return users;
        }

        //Check if super admin existing
        public bool CheckSuperAdmin()
        {
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                string query = "SELECT COUNT(*) FROM users WHERE role = 'Super Admin'";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        //Verify if email is already existing
        public bool VerifyEmail(string email, int userID)
        {
            string query = "SELECT COUNT(*) FROM users WHERE email = @emailText AND user_id != @userID";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using(MySqlCommand cmd = new(query,conn))
                {
                    cmd.Parameters.AddWithValue("userID", userID);
                    cmd.Parameters.AddWithValue("emailText", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        //INFORMATION: This action is for super admin only
        //Delete user by userId
        public void DeleteUserData(int userID)
        {
            int ID = 0;
            string role = "N/A";
            NotificationOperation notificationOP = new();
            notificationOP.AddNotification(userID, "Super admin", "Delete account");

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                string getQuery = @"
            SELECT u.user_id, 
                   COALESCE(s.role, t.role) AS role 
            FROM users u
            LEFT JOIN students s ON u.user_id = s.user_id
            LEFT JOIN teachers t ON u.user_id = t.user_id
            WHERE u.user_id = @user_id";

                using (MySqlCommand cmd = new(getQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ID = reader.GetInt32("user_id");
                            role = reader.GetString("role");
                        }
                    }
                }

                if (ID != 0)
                {
                    AuditLogsOperations auditOP = new();
                    auditOP.AddLogs(ID, role, "Delete account", "Delete", "Success");
                }

                // Step 1: Delete from the child table (triggers will handle archiving and user deletion)
                if (role.ToLower() == "teacher")
                {
                    string deleteTeacherQuery = "DELETE FROM teachers WHERE user_id = @user_id";
                    using (MySqlCommand cmd = new(deleteTeacherQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userID);
                        cmd.ExecuteNonQuery(); // Triggers will run here
                    }
                }
                else if (role.ToLower() == "student")
                {
                    string deleteStudentQuery = "DELETE FROM students WHERE user_id = @user_id";
                    using (MySqlCommand cmd = new(deleteStudentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userID);
                        cmd.ExecuteNonQuery(); // Triggers will run here
                    }
                }

                // Refresh the main page
                SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                if (mainPageAdmin != null)
                {
                    mainPageAdmin.SAdminContent.Navigate(new Users());
                }
            }
        }
    }
}