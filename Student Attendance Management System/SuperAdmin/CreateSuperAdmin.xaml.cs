using MySql.Data.MySqlClient;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.SuperAdmin
{
    /// <summary>
    /// Interaction logic for CreateSuperAdmin.xaml
    /// </summary>
    public partial class CreateSuperAdmin : Window
    {
        public CreateSuperAdmin()
        {
            
            InitializeComponent();
        }
        //Sign up Super Admin
        private void SignUpSA_Click(object sender, RoutedEventArgs e)
        {
            //are inputs have value?
            if (string.IsNullOrEmpty(fname.Text) ||
                string.IsNullOrEmpty(lname.Text) ||
                string.IsNullOrEmpty(email.Text) ||
                string.IsNullOrEmpty(password.Password) ||
                string.IsNullOrEmpty(passwordConfirm.Password))
            {
                MessageBox.Show("All fields must be filled", "All Fields Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //length of fname and lname
            if (fname.Text.Length > 30)
            {
                MessageBox.Show("First Name is too long", "Invalid First Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                fname.Focus();
                return;
            }

            if (lname.Text.Length > 30)
            {
                MessageBox.Show("Last Name is too long", "Invalid Last Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                lname.Focus();
                return;
            }
            //if fname and lname is only letters
            bool validateFName = Regex.IsMatch(fname.Text, @"^[a-zA-Z\s]+$");
            bool validateLName = Regex.IsMatch(fname.Text, @"^[a-zA-Z\s]+$");

            if (!validateFName)
            {
                MessageBox.Show("First name is not acceptable", "Invalid First Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                fname.Focus();
                return;
            }

            if (!validateLName)
            {
                MessageBox.Show("Last name is not acceptable", "Invalid Last Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                lname.Focus();
                return;
            }
            // email validation
            bool isEmail = Regex.IsMatch(email.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!isEmail)
            {
                MessageBox.Show("Email is not acceptable", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                email.Focus();
                return;
            }
            //check if pass is too short
            if (password.Password.Length <= 9)
            {
                MessageBox.Show("Password must have at least 10 characters", "Invalid Password", MessageBoxButton.OK, MessageBoxImage.Warning);
                password.Focus();
                return;
            }

            //check if passconfirm is same as password  

            if (password.Password != passwordConfirm.Password)
            {
                MessageBox.Show("Password isn't match", "Password Confimation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                passwordConfirm.Focus();
                return;
            }

            //create user
            AdminModel admin = new AdminModel
            {
                First_name = fname.Text,
                Last_name = lname.Text,
                Email = email.Text,
                Password = password.Password,
                Role = "Super Admin"
            };
            AdminOperations adminOperations = new();
            bool isSuccess = adminOperations.AddSuperAdmin(admin);
            if (isSuccess)
            {
                UserModel? user = verifyUser();
                // get the admin data by email and pass
                AdminModel? adminData = adminOperations.GetAdminData(user.User_id);

                SuperAdminLayout SAadminLayout = new SuperAdminLayout(adminData);
                SAadminLayout.Show();
                this.Close();
            }
        }

        //Verify user information
        public UserModel? verifyUser()
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

                        if (BCrypt.Net.BCrypt.Verify(password.Password, hashedPass))
                        {
                            user.User_id = Convert.ToInt32(reader["user_id"]);
                            user.Email = reader["email"].ToString();
                            user.Password = reader["password"].ToString();
                            user.Role = reader["role"].ToString();
                            auditOP.AddLogs(user.User_id, user.Role, "Sign in account ", "Sign in", "Success");

                            return user;
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Password!", "Wrong password", MessageBoxButton.OK, MessageBoxImage.Error); auditOP.AddLogs(user.User_id, user.Role, "Sign in account ", "Sign in", "Failed");
                            return null;
                        }
                    }
                }
            }
            MessageBox.Show("Email doesn't exist", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }
}