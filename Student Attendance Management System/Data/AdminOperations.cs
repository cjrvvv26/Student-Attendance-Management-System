using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Student_Attendance_Management_System.Data
{
    public class AdminOperations
    {
        public ObservableCollection<AdminModel> GetAllAdmins()
        {
            string query = "SELECT * FROM admin";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                ObservableCollection<AdminModel> admins = new();
                using MySqlCommand cmd = new(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        AdminModel adminModel = new()
                        {
                            Admin_id = reader.GetInt32("admin_id"),
                            User_id = reader.GetInt32("user_id"),
                            First_name = reader.GetString("first_name"),
                            Last_name = reader.GetString("last_name"),
                            Email = reader.GetString("email"),
                            Password = reader.GetString("password"),
                            Role = reader.GetString("role"),
                            Created_at = reader.GetDateTime("created_at"),
                            Updated_at = reader.GetDateTime("updated_at"),
                        };
                        admins.Add(adminModel);
                    }
                    return admins;
                }
            }
        }
        //Fetch the super admin data
        public AdminModel GetAdminData (int userID)
        {
            string query = "SELECT * FROM admin WHERE user_id = @userID";

            using (MySqlConnection conn = DataHelper.connectDB())
            {
                AdminModel adminData = new();
                using MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    adminData.Admin_id = reader.GetInt32("admin_id");
                    adminData.User_id = reader.GetInt32("user_id");
                    adminData.First_name = reader.GetString("first_name");
                    adminData.Last_name = reader.GetString("last_name");
                    adminData.Email = reader.GetString("email");
                    adminData.Password = reader.GetString("password");
                    adminData.Role = reader.GetString("role");
                    adminData.Created_at = reader.GetDateTime("created_at");
                    adminData.Updated_at = reader.GetDateTime("updated_at");
                }
                return adminData;
            }
        }
        // Sign Up Super Admin
        public bool AddSuperAdmin(AdminModel admin)
            //USER INSERT VALUE
        {   string userQuery = "INSERT INTO users (email, password, role, created_at) " +
                "VALUES (@email, @pass, @role, @createdAt)";
            //SUPER ADMIN INSERT VALUE
            string query = "INSERT INTO admin (user_id, first_name, last_name, email, password, role, created_at, updated_at) " +
                "VALUES (@userID, @Fname, @Lname, @email, @pass, @role, @createdAt, @updatedAt)";

            using (MySqlConnection conn = DataHelper.connectDB())
            {   MySqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    string hashPass = BCrypt.Net.BCrypt.HashPassword(admin.Password);
                    using MySqlCommand userCmd = new(userQuery, conn, transaction);
                    userCmd.Parameters.AddWithValue("@email", admin.Email);
                    userCmd.Parameters.AddWithValue("@pass", hashPass);
                    userCmd.Parameters.AddWithValue("@role", admin.Role);
                    userCmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    // Execute insert into users table
                    userCmd.ExecuteNonQuery();
                    long userID = userCmd.LastInsertedId;

                    using MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@userID", userID);
                    cmd.Parameters.AddWithValue("@Fname", admin.First_name);
                    cmd.Parameters.AddWithValue("@Lname", admin.Last_name);
                    cmd.Parameters.AddWithValue("@email", admin.Email);
                    cmd.Parameters.AddWithValue("@pass", hashPass);
                    cmd.Parameters.AddWithValue("@role", admin.Role);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@updatedAt", DateTime.Now);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        transaction.Commit();
                        MessageBox.Show("Successfully created a super admin account!", "Sign Up Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notifOP = new();
                        notifOP.AddNotification(Convert.ToInt32(userID), admin.Role, "Create account");
                        AuditLogsOperations auditLogsOperations = new AuditLogsOperations();
                        auditLogsOperations.AddLogs(Convert.ToInt32(userID), admin.Role, "Create super admin", "Create account", "Success");
                        return true;
                    }
                    transaction.Rollback();
                    MessageBox.Show("Failed to create a super admin account!", "Sign Up Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    AuditLogsOperations auditLogsOPs = new();
                    auditLogsOPs.AddLogs(Convert.ToInt32(userID), "Super Admin", "Create account", "Create", "Failed");
                    return false;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        //Update Super Admin Data
        public bool UpdateSuperAdmin(AdminModel admin)
        {
            List<string> updates = new();
            Dictionary<string, object> param = new();

            foreach(PropertyInfo prop in admin.GetType().GetProperties())
            {
                var value = prop.GetValue(admin);

                if (value == null || value is string str && string.IsNullOrEmpty(str))
                {
                    continue;
                }

                string colName = prop.Name.ToLower();
                string paramName = $"@{prop.Name}";

                updates.Add($"{colName} = {paramName}");
                param[paramName] = value;
            }
            string userQuery = "UPDATE users SET email = @email, password = @password WHERE user_id = @userID";

            string query = $"UPDATE admin SET {string.Join(", ", updates)} WHERE user_id = @id";

            param["@id"] = admin.User_id;

            try
            {
                AuditLogsOperations auditOP = new();
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    MySqlTransaction transaction = conn.BeginTransaction();
                    using MySqlCommand userCmd = new(userQuery, conn);
                    userCmd.Parameters.AddWithValue("@userID", admin.User_id);
                    userCmd.Parameters.AddWithValue("@email", admin.Email);
                    userCmd.Parameters.AddWithValue("@password", admin.Password);
                    userCmd.ExecuteNonQuery();

                    using MySqlCommand cmd = new(query, conn);

                    foreach(var paramProps in param)
                    {
                        cmd.Parameters.AddWithValue(paramProps.Key, paramProps.Value);
                    }

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        transaction.Commit();
                        MessageBox.Show("Your infomation has been successfully updated.", "Updated Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
                        NotificationOperation notifOP = new();
                        notifOP.AddNotification(admin.User_id, admin.Role, "Update account");
                        
                        auditOP.AddLogs(admin.User_id, admin.Role, "Update super admin account", "Update account", "Success");
                        return true; 
                    }
                    transaction.Rollback();
                    MessageBox.Show("Updating information is failed.", "Updated Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    auditOP.AddLogs(admin.User_id, admin.Role, "Update super admin account", "Update account", "Failed");
                    return false;
                }
            }catch (MySqlException ex)
            {
                MessageBox.Show($"An error occurred while updating admin information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }
    }
}
