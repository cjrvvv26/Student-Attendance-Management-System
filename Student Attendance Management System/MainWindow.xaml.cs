using BCrypt.Net;
using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Student;
using Student_Attendance_Management_System.SuperAdmin;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using Student_Attendance_Management_System.Teacher;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System;

/// <summary>   
/// Interaction logic for MainWindow.xaml
/// </summary> 

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        //Check if theres a super admin
        UsersOperations userOP = new();
        bool isExisting = userOP.CheckSuperAdmin();
        if (!isExisting)
        {
            CreateSuperAdmin superAdminSignUp = new CreateSuperAdmin();
            superAdminSignUp.Show();
            this.Close();
            return;
        }


    }

    //Verify user information
    public UserModel? verifyUser ()
    {
        using (MySqlConnection conn = DataHelper.connectDB())
        {
            string query = "SELECT user_id, email, password, role FROM users WHERE email = @email";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", email.Text);
                using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                AuditLogsOperations auditOP = new();
                UserModel user = new();
                if (reader.Read())
                    {
                        string hashedPass = reader["password"].ToString();

                        if (BCrypt.Net.BCrypt.Verify(passwordBox.Password, hashedPass))
                        {
                            MessageBox.Show("Sign in successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            user.User_id = Convert.ToInt32(reader["user_id"]);
                            user.Email = reader["email"].ToString();
                            user.Password = reader["password"].ToString();
                            user.Role = reader["role"].ToString();
                            auditOP.AddLogs(user.User_id, user.Role, "Sign in account ", "Sign in", "Success");

                        return user;
                    }
                        else
                        {
                            MessageBox.Show("Incorrect Password!", "Wrong password", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                        }
                    }
                }
        }
            MessageBox.Show("Email doesn't exist", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
    }
    

    //Sign In User with Role Based Access
    private void SignIn_Click(object sender, RoutedEventArgs e)
    {
        UserModel? user = verifyUser();
        if (user == null)
        {
            email.Focus();
            return;
        }

        switch (user.Role)
        {
            case "Student":
                StudentOperations studentOP = new();
                StudentModel? student = studentOP.GetStudentData(user.User_id, DataHelper.connectDB());
                StudentPageLayout studentLayout = new StudentPageLayout(student);
                studentLayout.Show();
                this.Close();
                break;
            
            case "Teacher":
                TeacherOperations teacherOp = new();
                TeacherModel teacher = teacherOp.GetTeacherDataByUserID(user.User_id);
                TeacherLayout teacherLayout = new TeacherLayout(teacher);
                teacherLayout.Show();
                this.Close();
                break;

            case "Super Admin":
                AdminOperations adminOP = new();
                AdminModel admin = adminOP.GetAdminData(user.User_id);
                SuperAdminLayout SAadminLayout = new SuperAdminLayout(admin);
                SAadminLayout.Show();
                this.Close();
                break;
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}