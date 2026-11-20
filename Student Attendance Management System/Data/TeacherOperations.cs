using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
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
    class TeacherOperations
    {
        // get all teachers data
        public static ObservableCollection<TeacherModel> getAllTeacher()
        {
            ObservableCollection<TeacherModel> teachers = new();
            using(MySqlConnection conn = DataHelper.connectDB()) {
                
                string query = "SELECT * FROM teachers";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    teachers.Add(new TeacherModel
                    {
                        Teacher_id = reader.GetInt32("teacher_id"),
                        Employee_number = reader.GetString("employee_number"),
                        Profile_pic = (byte[])reader["profile_pic"],
                        First_name = reader.GetString("first_name"),
                        Middle_name = reader.GetString("middle_name"),
                        Last_name = reader.GetString("last_name"),
                        Email = reader.GetString("email"),
                        Gender = reader.GetString("gender"),
                        Age = reader.GetInt32("age"),
                        Birth_date = reader.GetDateTime("birth_date"),
                        Phone_no = reader.GetString("phone_no"),
                        Department = reader.GetString("department")
                    });
                }
                return teachers;
            }
        }

        //Add teacher data
        public bool AddTeacher(TeacherModel teacher)
        {
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                MySqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    //Teacher Gender Validation

                    if (teacher.Gender != "Male" && teacher.Gender != "Female")
                    {
                        MessageBox.Show("Please select a gender");
                    }
                    // Teacher Default Password Generate
                    string? password = teacher.Birth_date?.ToString("MMddyyyy");
                    var hashPass = BCrypt.Net.BCrypt.HashPassword(password);

                    //Phone No. Validation
                    if (teacher.Phone_no.Length != 11)
                    {
                        MessageBox.Show("Phone number must be 11 digits");
                        return false;
                    }

                    //Insert teacher in user's table
                    string query = "INSERT into users (email ,password, role) VALUES (@email, @password, @role)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", teacher.Email);
                    cmd.Parameters.AddWithValue("@password", hashPass);
                    cmd.Parameters.AddWithValue("@role", "Teacher");
                    cmd.ExecuteNonQuery();

                    long userLastId = cmd.LastInsertedId;
                    string teacher_no = $"TC{teacher.Birth_date?.Year}{userLastId:D4}";
                    string teacherQuery = "INSERT into teachers (user_id, employee_number, profile_pic, first_name, middle_name, last_name, email, password, " +
                        "birth_date, age, gender, phone_no, department, role, created_at, updated_at) VALUES (@user_id, @emp_no, @profile_pic, @fname, @mname, @lname, @email, @password, @bdate, @age" +
                        ", @gender, @phone_no, @department, @role, @created_at, @updated_at);";

                    MySqlCommand teacherCmd = new MySqlCommand(teacherQuery, conn);
                    teacherCmd.Parameters.AddWithValue("@user_id", userLastId);
                    teacherCmd.Parameters.AddWithValue("@emp_no", teacher_no);
                    teacherCmd.Parameters.AddWithValue("@profile_pic", teacher.Profile_pic);
                    teacherCmd.Parameters.AddWithValue("@fname", teacher.First_name);
                    teacherCmd.Parameters.AddWithValue("@mname", teacher.Middle_name);
                    teacherCmd.Parameters.AddWithValue("@lname", teacher.Last_name);
                    teacherCmd.Parameters.AddWithValue("@email", teacher.Email);
                    teacherCmd.Parameters.AddWithValue("@password", hashPass);
                    teacherCmd.Parameters.AddWithValue("@bdate", teacher.Birth_date);
                    teacherCmd.Parameters.AddWithValue("@age", teacher.Age);
                    teacherCmd.Parameters.AddWithValue("@gender", teacher.Gender);
                    teacherCmd.Parameters.AddWithValue("@phone_no", Convert.ToInt64(teacher.Phone_no));
                    teacherCmd.Parameters.AddWithValue("@role", "Teacher");
                    teacherCmd.Parameters.AddWithValue("@department", teacher.Department);
                    teacherCmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                    teacherCmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                    if (teacherCmd.ExecuteNonQuery() > 0)
                    {
                        transaction.Commit();
                        MessageBox.Show("Account was successfully created", "Successfully Created", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        NotificationOperation notificationOP = new();
                        notificationOP.AddNotification(Convert.ToInt32(userLastId), "Super admin", "Create account");
                        AuditLogsOperations auditLogsOP = new();
                        auditLogsOP.AddLogs(Convert.ToInt32(userLastId), "Teacher", "Create account", "Create", "Success");
                        Users allUserList = new Users();
                        allUserList.UsersContent.Navigate(new AllUsers());
                        return true;
                    }
                    transaction.Rollback();

                    AuditLogsOperations auditLogsOPs = new();
                    auditLogsOPs.AddLogs(Convert.ToInt32(userLastId), "Teacher", "Create account", "Create", "Failed");
                    return false;
                }catch (MySqlException ex)
                {
                    MessageBox.Show("An error occurred while adding teacher: " + ex.Message);
                    return false;
                } finally
                {
                    conn.Close();
                }
            }
        }

        // Update Teacher's Data
        public int UpdateTeacher(int userID, TeacherModel teacher, string triggered_by)
        {
            List<string> updates = new();
            Dictionary<string, object> parameters = new();

            foreach (PropertyInfo prop in teacher.GetType().GetProperties())
            {
                var value = prop.GetValue(teacher);
                if (prop.Name == "Employee_number" || prop.Name == "User_id" || value == null || (value is string str && string.IsNullOrEmpty(str)))
                    continue;

                string columnName = prop.Name.ToLower();
                string paramName = $"@{prop.Name}";

                updates.Add($"{columnName} = {paramName}");
                parameters[paramName] = value;
            }

            string teacherQuery = $"UPDATE teachers SET {string.Join(", ", updates)} WHERE user_id = @id";
            string userQuery = $"UPDATE users SET email = @Email, password = @Password WHERE user_id = @UserID";

            parameters["@id"] = userID;

            using (MySqlConnection conn = DataHelper.connectDB()) 
            {
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update user table if triggered by the teacher (changing email or password)
                        if (triggered_by == "Teacher")
                        {
                            using MySqlCommand userCmd = new(userQuery, conn, transaction);
                            userCmd.Parameters.AddWithValue("@UserID", userID);
                            userCmd.Parameters.AddWithValue("@Email", teacher.Email);
                            userCmd.Parameters.AddWithValue("@Password", teacher.Password);
                            userCmd.ExecuteNonQuery();
                        }

                        using MySqlCommand teacherCmd = new(teacherQuery, conn, transaction);
                        foreach (var param in parameters)
                            teacherCmd.Parameters.AddWithValue(param.Key, param.Value);

                        int affectedRows = teacherCmd.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            transaction.Commit();

                            MessageBox.Show("Account has been successfully updated!", "Successfully Updated", MessageBoxButton.OK, MessageBoxImage.Information);

                            NotificationOperation notificationOP = new();
                            AuditLogsOperations auditOP = new();
                            auditOP.AddLogs(userID, "Teacher", "Update account", "Update", "Success");
                            notificationOP.AddNotification(userID, teacher.Role, "Update account");
                            return userID;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        AuditLogsOperations auditOP = new();
                        auditOP.AddLogs(userID, "Teacher", "Update account", "Update", "Failed");
                        return 0;
                    }
                }
            }
        }


        // Teacher's data by user id
        public TeacherModel GetTeacherData(int userID)
        {
            TeacherModel teacher = new();
            string query = " SELECT t.teacher_id, t.user_id, u.user_id, t.profile_pic, t.employee_number, t.first_name, t.middle_name, t.last_name, t.age, t.gender, t.birth_date, t.department, t.role, t.password, t.email, t.phone_no FROM teachers t LEFT JOIN users u ON u.user_id = t.user_id WHERE t.user_id = @user_id;";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@user_id", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("profile_pic")))
                        {
                            long length = reader.GetBytes(reader.GetOrdinal("profile_pic"), 0, null, 0, 0);
                            byte[] profilePic = new byte[length];
                            reader.GetBytes(reader.GetOrdinal("profile_pic"), 0, profilePic, 0, (int)length);
                            teacher.Profile_pic = profilePic;
                        }
                        teacher.Teacher_id = reader.GetInt32("teacher_id");
                        teacher.User_id = reader.GetInt32("user_id");
                        teacher.Employee_number = reader.GetString("employee_number");
                        teacher.First_name = reader.GetString("first_name");
                        teacher.Middle_name = reader.GetString("middle_name");
                        teacher.Last_name = reader.GetString("last_name");
                        teacher.Gender = reader.GetString("gender");
                        teacher.Age = reader.GetInt32("age");
                        teacher.Birth_date = reader.GetDateTime("birth_date");
                        teacher.Role = "Teacher";
                        teacher.Department = reader.GetString("department");
                        teacher.Email = reader.GetString("email");
                        teacher.Password = reader.GetString("password");
                        teacher.Phone_no = Convert.ToString(reader.GetInt64("phone_no"));
                    }
                }
                return teacher;
            }
        }

        //Get teacher data by teacher id
        public TeacherModel GetTeacherDataByID(int userID)
        {
            TeacherModel teacher = new();
            string query = " SELECT teacher_id, user_id, profile_pic, employee_number, first_name, middle_name, last_name, age, gender, birth_date, department, role, password, email, phone_no FROM teachers WHERE teacher_id = @user_id;";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@user_id", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("profile_pic")))
                        {
                            long length = reader.GetBytes(reader.GetOrdinal("profile_pic"), 0, null, 0, 0);
                            byte[] profilePic = new byte[length];
                            reader.GetBytes(reader.GetOrdinal("profile_pic"), 0, profilePic, 0, (int)length);
                            teacher.Profile_pic = profilePic;
                        }
                        teacher.Teacher_id = reader.GetInt32("teacher_id");
                        teacher.User_id = reader.GetInt32("user_id");
                        teacher.Employee_number = reader.GetString("employee_number");
                        teacher.First_name = reader.GetString("first_name");
                        teacher.Middle_name = reader.GetString("middle_name");
                        teacher.Last_name = reader.GetString("last_name");
                        teacher.Gender = reader.GetString("gender");
                        teacher.Age = reader.GetInt32("age");
                        teacher.Birth_date = reader.GetDateTime("birth_date");
                        teacher.Role = "Teacher";
                        teacher.Department = reader.GetString("department");
                        teacher.Email = reader.GetString("email");
                        teacher.Password = reader.GetString("password");
                        teacher.Phone_no = Convert.ToString(reader.GetInt64("phone_no"));
                    }
                }
                return teacher;
            }
        }

        //Get teacher data by user id
        public TeacherModel GetTeacherDataByUserID(int userID)
        {
            TeacherModel teacher = new();
            string query = " SELECT teacher_id, user_id, profile_pic, employee_number, first_name, middle_name, last_name, age, gender, birth_date, department, role, password, email, phone_no FROM teachers WHERE user_id = @user_id;";
            using (MySqlConnection conn = DataHelper.connectDB())
            {
                MySqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@user_id", userID);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(reader.GetOrdinal("profile_pic")))
                        {
                            long length = reader.GetBytes(reader.GetOrdinal("profile_pic"), 0, null, 0, 0);
                            byte[] profilePic = new byte[length];
                            reader.GetBytes(reader.GetOrdinal("profile_pic"), 0, profilePic, 0, (int)length);
                            teacher.Profile_pic = profilePic;
                        }
                        teacher.Teacher_id = reader.GetInt32("teacher_id");
                        teacher.User_id = reader.GetInt32("user_id");
                        teacher.Employee_number = reader.GetString("employee_number");
                        teacher.First_name = reader.GetString("first_name");
                        teacher.Middle_name = reader.GetString("middle_name");
                        teacher.Last_name = reader.GetString("last_name");
                        teacher.Gender = reader.GetString("gender");
                        teacher.Age = reader.GetInt32("age");
                        teacher.Birth_date = reader.GetDateTime("birth_date");
                        teacher.Role = "Teacher";
                        teacher.Department = reader.GetString("department");
                        teacher.Email = reader.GetString("email");
                        teacher.Password = reader.GetString("password");
                        teacher.Phone_no = Convert.ToString(reader.GetInt64("phone_no"));
                    }
                }
                return teacher;
            }
        }

        //Get all teachers by department
        public ObservableCollection<TeacherModel> GetTeacherByDepart(string department)
        {
            ObservableCollection<TeacherModel> teachers = new();

            string query = "SELECT * FROM teachers WHERE department = @department";
            try
            {
                using (MySqlConnection conn = DataHelper.connectDB())
                {
                    using MySqlCommand cmd = new(query, conn);

                    cmd.Parameters.AddWithValue("@department", department);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            teachers.Add(new TeacherModel()
                            {
                                Teacher_id = reader.GetInt32("teacher_id"),
                                Employee_number = reader.GetString("employee_number"),
                                First_name = reader.GetString("first_name"),
                                Last_name = reader.GetString("last_name"),
                                Age = reader.GetInt32("age"),
                                Gender = reader.GetString("gender"),
                                Department = reader.GetString("department")

                            });
                        }
                        return teachers;
                    }

                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("An error occurred while fetching teacher's by its department: ", ex.Message);
                return null;
            }
        }

        //Count all teachers
        public int CountAllTeachers()
        {
            string query = @"SELECT COUNT(*) AS total_teachers FROM teachers;";
            using MySqlConnection conn = DataHelper.connectDB();
            using MySqlCommand cmd = new(query,conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            int count = 0;
            if (reader.Read())
            {
                count = reader.GetInt32("total_teachers");
            }
            return count;
        }
    }
}
