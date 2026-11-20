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
    class TargetUserLog
    {
        public int User_id { get; set; }
        public string UserNo { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
    }
    class AuditLogsOperations
    {
        //Get all logs
        public ObservableCollection<AuditLogsModel> GetAllLogs()
        {
            ObservableCollection<AuditLogsModel> logs = new();
            string query = "SELECT * FROM audit_logs";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new AuditLogsModel
                        {
                            Log_id = reader.GetInt32("log_id"),
                            User_no = reader.GetString("user_no"),
                            User_name = reader.GetString("user_name"),
                            Role = reader.GetString("user_role"),
                            Affected_entity = reader.GetString("affected_entity"),
                            Action = reader.GetString("action"),
                            Action_type = reader.GetString("action_type"),
                            Status = reader.GetString("status"),
                            Created_at = reader.GetDateTime("created_at")
                        });
                    }
                    return logs;
                }
            }
        }

        //Add logs to the database
        public void AddLogs(int userID, string entity, string action, string type, string status)
        {
            TargetUserLog targetUser = GetTargetLog(userID);

            string query = "INSERT INTO audit_logs (user_id, user_no, user_name, user_role, affected_entity, action, action_type, status, created_at) " +
                "VALUES(@user_id, @user_no, @user_name, @user_role, @entity, @action, @action_type, @status, @created_at);";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@user_id", targetUser.User_id);
                cmd.Parameters.AddWithValue("@user_no", targetUser.UserNo);
                cmd.Parameters.AddWithValue("@user_name", targetUser.Name);
                cmd.Parameters.AddWithValue("@user_role", targetUser.Role);
                cmd.Parameters.AddWithValue("@entity", entity);
                cmd.Parameters.AddWithValue("@action", action);
                cmd.Parameters.AddWithValue("@action_type", type);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                
                try
                {
                    cmd.ExecuteNonQuery();
                }catch(MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while adding logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //Get the target log
        public TargetUserLog GetTargetLog(int userID)
        {
            TargetUserLog targetUser = new();
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                //teacher query
                string teacherQuery = "SELECT employee_number, user_id, first_name, last_name, role FROM teachers WHERE user_id = @user_id";

                using (MySqlCommand cmd = new MySqlCommand(teacherQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            targetUser = new TargetUserLog
                            {
                                User_id = reader.GetInt32("user_id"),
                                UserNo = reader.GetString("employee_number"),
                                Name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Role = reader.GetString("role")
                            };
                            return targetUser;
                        }
                    }
                }

                //student query
                string studentQuery = "SELECT user_id, student_number, first_name, last_name, role FROM students WHERE user_id = @user_id";

                using (MySqlCommand cmd = new(studentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userID);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            targetUser = new TargetUserLog
                            {
                                User_id = reader.GetInt32("user_id"),
                                UserNo = reader.GetString("student_number"),
                                Name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                                Role = reader.GetString("role")
                            };
                            return targetUser;
                        }
                    }
                }

                //Super admin query
                string superAdminQuery = "SELECT admin_id, user_id, first_name, last_name, role FROM admin WHERE user_id = @userID";

                using MySqlCommand SACmd = new(superAdminQuery, conn);
                SACmd.Parameters.AddWithValue("@userID", userID);
                using (MySqlDataReader reader = SACmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        targetUser = new TargetUserLog
                        {
                            User_id = reader.GetInt32("user_id"),
                            UserNo = Convert.ToString(reader.GetInt32("admin_id")),
                            Name = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                            Role = reader.GetString("role")
                        };
                        return targetUser;
                    }
                }

            }
            return targetUser;
        }

        public void DeleteLog(int logID)
        {
            string query = "DELETE FROM audit_logs WHERE log_id = @log_id";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@log_id", logID);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred while deleting logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
