using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Student_Attendance_Management_System.SuperAdmin.Pages
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        public bool showOldPass = false;
        public bool showNewPass = false;
        private AdminModel _adminModel;
        public Profile(int userID)
        {
            InitializeComponent();
            DataContext = new AdminVM(userID);
            AdminVM adminVM = new AdminVM(userID);
            if (DataContext is AdminVM vm)
            {
                _adminModel = vm.AdminData;
            }
        }

        //View Information
        private void ViewInfo_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        //show hide old pass
        private void ShowOldPass_Click(object sender, RoutedEventArgs e)
        {
            showOldPass = !showOldPass;
            if (!showOldPass)
            {
                OldPassIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.EyeSlash;
                passwordBox.Visibility = Visibility.Visible;
                passwordTextBox.Visibility = Visibility.Collapsed;  
                passwordBox.Password = passwordTextBox.Text;
                passwordBox.Focus();    
                return;
            }
            OldPassIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Eye;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordTextBox.Text = passwordBox.Password;  
            passwordTextBox.Visibility = Visibility.Visible;
            passwordTextBox.Focus();
        }

        //hide new pass
        private void HideNewPass_Click(object sender, RoutedEventArgs e)
        {
            showNewPass = !showNewPass;
            if (!showNewPass)
            {
                NewPassIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.EyeSlash;
                newPasswordBox.Visibility = Visibility.Visible;
                newPasswordTextBox.Visibility = Visibility.Collapsed;
                newPasswordBox.Password = newPasswordTextBox.Text;
                newPasswordBox.Focus();
                return;
            }
            NewPassIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Eye;
            newPasswordBox.Visibility = Visibility.Collapsed;
            newPasswordTextBox.Text = newPasswordBox.Password;
            newPasswordTextBox.Visibility = Visibility.Visible;
            newPasswordTextBox.Focus();
        }
        //Update ui mode
        private void UpdateMode_Click(object sender, RoutedEventArgs e)
        {
            ChangeUI();
        }

        private void ChangeUI()
        {
            pencilUpdateIcon.Visibility = Visibility.Collapsed;
            //hide textBlocks
            fnameText.Visibility = Visibility.Collapsed;
            lnameText.Visibility = Visibility.Collapsed;
            emailText.Visibility = Visibility.Collapsed;

            //show textbox & passbox
            fnameBox.Visibility = Visibility.Visible;
            lnameBox.Visibility = Visibility.Visible;
            emailBox.Visibility = Visibility.Visible;
            updateBtnsBox.Visibility = Visibility.Visible;
            oldPassBox.Visibility = Visibility.Visible;
            newPassBox.Visibility = Visibility.Visible;
            topRibbonBox.Visibility = Visibility.Visible;

            //set the values of current data in update components
            fnameBox.Text = fnameText.Text;
            lnameBox.Text = lnameText.Text;
            emailBox.Text = emailText.Text;
        }

        //Proceed Update
        private void ProceedUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(passwordBox.Password+ " " + passwordTextBox.Text + " " + _adminModel.Password);
            if (string.IsNullOrEmpty(fnameBox.Text) ||
                string.IsNullOrEmpty(lnameBox.Text) ||
                string.IsNullOrEmpty(emailBox.Text))
            {
                MessageBox.Show("All fields must be filled", "Missing Inputs", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //length of fname and lname
            if (fnameBox.Text.Length > 30)
            {
                MessageBox.Show("First Name is too long", "Invalid First Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                fnameBox.Focus();
                return;
            }

            if (lnameBox.Text.Length > 30)
            {
                MessageBox.Show("Last Name is too long", "Invalid Last Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                lnameBox.Focus();
                return;
            }
            //if fname and lname is only letters
            bool validateFName = Regex.IsMatch(fnameBox.Text, @"^[a-zA-Z\s]+$");
            bool validateLName = Regex.IsMatch(lnameBox.Text, @"^[a-zA-Z\s]+$");

            if (!validateFName)
            {
                MessageBox.Show("First name is not acceptable", "Invalid First Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                fnameBox.Focus();
                return;
            }

            if (!validateLName)
            {
                MessageBox.Show("Last name is not acceptable", "Invalid Last Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                lnameBox.Focus();
                return;
            }
            // email validation
            bool isEmail = Regex.IsMatch(emailBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!isEmail)
            {
                MessageBox.Show("Email is not acceptable", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                emailBox.Focus();
                return;
            }

            bool verifyPass = false;

            string enteredPassword = passwordTextBox.IsVisible ? passwordTextBox.Text : passwordBox.Password;

            verifyPass = BCrypt.Net.BCrypt.Verify(enteredPassword, _adminModel.Password);
            
            string password = "";
            password = BCrypt.Net.BCrypt.HashPassword(newPasswordTextBox.IsVisible ? newPasswordTextBox.Text : newPasswordBox.Password);

            if (string.IsNullOrEmpty(password))
            {
                password = _adminModel.Password;
            }

            AdminOperations adminOP = new();
            AdminModel newData = new AdminModel
            {
                Admin_id = _adminModel.Admin_id,
                User_id = _adminModel.User_id,
                First_name = fnameBox.Text,
                Last_name = lnameBox.Text,
                Email = emailBox.Text,
                Password = password,
                Role = _adminModel.Role,
                Created_at = _adminModel.Created_at,
                Updated_at = DateTime.Now,
            };

            if(adminOP.UpdateSuperAdmin(newData))
            {
                AdminModel adminModel = new();
                AdminVM adminVm = new(newData.User_id);
                adminVm.FullName = $"{newData.First_name} {newData.Last_name}";
                adminVm.Role = "[Super Admin]";
                ResetUI();
            }
        }

        //Cancel Update
        private void CancelUpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }
        //Back to view info
        private void ResetUI()
        {
            //hide textbox & passbox
            fnameBox.Visibility = Visibility.Collapsed;
            lnameBox.Visibility = Visibility.Collapsed;
            emailBox.Visibility = Visibility.Collapsed;
            updateBtnsBox.Visibility = Visibility.Collapsed;
            oldPassBox.Visibility = Visibility.Collapsed;
            newPassBox.Visibility = Visibility.Collapsed;
            topRibbonBox.Visibility = Visibility.Collapsed;

            //show textBlocks
            pencilUpdateIcon.Visibility = Visibility.Visible;
            fnameText.Visibility = Visibility.Visible;
            lnameText.Visibility = Visibility.Visible;
            emailText.Visibility = Visibility.Visible;
        }
    }
}
