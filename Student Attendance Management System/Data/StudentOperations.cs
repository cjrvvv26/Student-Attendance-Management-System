using BCrypt.Net;
using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Student;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;

namespace Student_Attendance_Management_System.Data
{
    class StudentOperations
    {
        //Display all students
        public static ObservableCollection<StudentModel> getStudents()
        {
            ObservableCollection<StudentModel> students = new ObservableCollection<StudentModel>();
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                string query = "SELECT * FROM students";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new StudentModel
                    {
                        Student_id = reader.GetInt32("student_id"),
                        Student_number = reader.GetString("student_number"),
                        User_id = reader.GetInt32("user_id"),
                        First_name = reader.GetString("first_name"),
                        Middle_name = reader.GetString("middle_name"),
                        Last_name = reader.GetString("last_name"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Gender = reader.GetString("gender"),
                        Section_id = reader.GetInt32("section_id"),
                        Birth_date = Convert.ToDateTime(reader.GetDateTime("birth_date").ToString("MM-dd-yyyy")),
                    });
                }
            }
            return students;
        }

        //Get one student data
        public StudentModel? GetStudentData(long userId, MySqlConnection conn)
        {
            string query = "SELECT student_id, student_number, first_name, middle_name, last_name, email, password, birth_date, age, gender, password, " +
                "section_id, user_id, role, created_at, updated_at " +
                "FROM students " +
                "WHERE user_id = @user_id";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@user_id", userId);

            using(MySqlDataReader reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                {
                    return new StudentModel
                    {
                        Student_id = reader.GetInt32("student_id"),
                        User_id = reader.GetInt32("user_id"),
                        Student_number = reader.GetString("student_number"),
                        First_name = reader.GetString("first_name"),
                        Middle_name = reader.GetString("middle_name"),
                        Password = reader.GetString("password"),
                        Last_name = reader.GetString("last_name"),
                        Email = reader.GetString("email"),
                        Birth_date = reader.GetDateTime("birth_date"),
                        Gender = reader.GetString("gender"),
                        Age = Convert.ToInt32(reader["age"]),
                        Section_id = reader.GetInt32("section_id"),
                        Role = reader.GetString("role")
                    };
                }
            }

            return null;
        }
        
        //Add Student
        public static bool AddStudent(StudentModel student)
        {
            var hashPass = BCrypt.Net.BCrypt.HashPassword(student.Password);
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                MySqlTransaction transaction = conn.BeginTransaction();
                try
                {

                    if (student.Gender != "Male" && student.Gender != "Female")
                    {
                        throw new Exception("Please select a gender");
                    }
                     
                    // Insert into users table
                    string userQuery = "INSERT INTO users (email, password, role) VALUES (@email, @password, @role)";
                    MySqlCommand userCmd = new MySqlCommand(userQuery, conn);
                    userCmd.Parameters.AddWithValue("@email", student.Email);
                    userCmd.Parameters.AddWithValue("@password", hashPass);
                    userCmd.Parameters.AddWithValue("@role", student.Role);
                    userCmd.ExecuteNonQuery();

                    long userId = userCmd.LastInsertedId;
                    string studentNumber = $"SN{student.Birth_date?.Year}{userId:D4}";
                    // Insert into students table
                    string query = "INSERT INTO students (student_number, email, first_name, middle_name, last_name, password, birth_date, age, gender, role, user_id, section_id, created_at, updated_at) " +
                    "VALUES (@student_no, @userEmail, @first_name, @middle_name, @last_name, @userPassword, @birth_date, @age, @gender, @userRole, @user_id, @secID, @created_at, @updated_at)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@student_no", studentNumber);
                    cmd.Parameters.AddWithValue("@userEmail", student.Email);
                    cmd.Parameters.AddWithValue("@first_name", student.First_name);
                    cmd.Parameters.AddWithValue("@middle_name", student.Middle_name);
                    cmd.Parameters.AddWithValue("@last_name", student.Last_name);
                    cmd.Parameters.AddWithValue("@userPassword", hashPass);
                    cmd.Parameters.AddWithValue("@birth_date", student.Birth_date);
                    cmd.Parameters.AddWithValue("@age", student.Age);
                    cmd.Parameters.AddWithValue("@gender", student.Gender);
                    cmd.Parameters.AddWithValue("@userRole", student.Role);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.Parameters.AddWithValue("@secID", student.Section_id);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                    cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        // Commit transaction
                        transaction.Commit();
                        // Success message
                        MessageBox.Show("Successfully created an account!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        SignUp studentSignUp = new();
                        NotificationOperation notificationOP = new();
                        AuditLogsOperations auditLogOP = new();
                        if (studentSignUp.isRegisterClick)
                        {
                            notificationOP.AddNotification(Convert.ToInt32(userId), "Student", "Create account");
                            auditLogOP.AddLogs(Convert.ToInt32(userId), "Student", "Create account", "Create", "Success");
                        }
                        else {
                            notificationOP.AddNotification(Convert.ToInt32(userId), "Super admin", "Create account");
                            auditLogOP.AddLogs(Convert.ToInt32(userId), "Student", "Create account", "Create", "Success");
                        }
                        return true;
                    }
                    transaction.Rollback();
                    AuditLogsOperations auditLogOPs = new();
                    auditLogOPs.AddLogs(Convert.ToInt32(userId), "Student", "Create account", "Create", "Failed");
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while adding a student: {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        //Update Student
        public void UpdateStudent(int userID, StudentModel student, string triggered_by)
        {
            List<string> updates = new();
            Dictionary<string, object> param = new Dictionary<string, object>();

            foreach(PropertyInfo prop in student.GetType().GetProperties())
            {
                // Skip 'student_status'
                if (prop.Name.Equals("student_status", StringComparison.OrdinalIgnoreCase)
                    )
                    continue;

                if (prop.Name.Equals("canUpdate", StringComparison.OrdinalIgnoreCase)
                    )
                    continue;

                var value = prop.GetValue(student);
                if (value == null || value is string str && string.IsNullOrEmpty(str))
                {
                    continue;
                }

                string colName = prop.Name.ToLower();
                string paramName = $"@{prop.Name}";

                updates.Add($"{colName} = {paramName}");
                param[paramName] = value;
            }

            string query = $"UPDATE students SET {string.Join(", ", updates)} WHERE user_id = @id";
            string userQuery = $"UPDATE users SET email = @Email, password = @Password WHERE user_id = @UserID";
            param["@id"] = userID;
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    if (triggered_by == "Student")
                    {
                        using MySqlCommand userCmd = new(userQuery, conn, transaction);
                        userCmd.Parameters.AddWithValue("@UserID", userID);
                        userCmd.Parameters.AddWithValue("@Email", student.Email);
                        userCmd.Parameters.AddWithValue("@Password", student.Password);
                        userCmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd = new(query, DataHelper.connectDB()))
                    {
                        foreach (var paramValue in param)
                        {
                            cmd.Parameters.AddWithValue(paramValue.Key, paramValue.Value);
                        }

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Account has been successfully updated!", "Successfully Updated", MessageBoxButton.OK, MessageBoxImage.Information);

                                NotificationOperation notificationOP = new();
                                notificationOP.AddNotification(student.User_id, student.Role, "Update account");
                                AuditLogsOperations auditLogOP = new();
                                auditLogOP.AddLogs(student.User_id, "Student", "Update account", "Update", "Success");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            AuditLogsOperations auditLogOP = new();
                            auditLogOP.AddLogs(student.User_id, "Student", "Update account", "Update", "Failed");
                        }
                    }
                }
            }
        }

        public void UpdateStudentByStudent(int userId, StudentModel model)
        {
            using (var connection = DataHelper.connectDB())
            {
                string query = @"UPDATE students SET
                            first_name = @FirstName,
                            middle_name = @MiddleName,
                            last_name = @LastName,
                            email = @Email,
                            password = @Password,
                            birth_date = @BirthDate,
                            age = @Age,
                            gender = @Gender,
                            section_id = @SectionId,
                            updated_at = @UpdatedAt
                         WHERE user_id = @UserId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", model.First_name);
                    command.Parameters.AddWithValue("@MiddleName", model.Middle_name);
                    command.Parameters.AddWithValue("@LastName", model.Last_name);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Password", model.Password);
                    command.Parameters.AddWithValue("@BirthDate", model.Birth_date?.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@Age", model.Age);
                    command.Parameters.AddWithValue("@Gender", model.Gender);
                    command.Parameters.AddWithValue("@SectionId", model.Section_id);
                    command.Parameters.AddWithValue("@UpdatedAt", model.Updated_at);
                    command.Parameters.AddWithValue("@UserId", userId);

                    command.ExecuteNonQuery();
                }
            }
        }

        //Count all students
        public int CountAllStudents()
        {
            string query = @"SELECT COUNT(*) AS total_students FROM students;";
            using MySqlConnection conn = DataHelper.connectDB();
            using MySqlCommand cmd = new(query, conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            int count = 0;
            if (reader.Read())
            {
                count = reader.GetInt32("total_students");
            }
            return count;
        }
    }
}
