using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.Teacher;
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

namespace Student_Attendance_Management_System.Student.Pages
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        private StudentModel studentData;
        public Profile(StudentModel student)
        {
            InitializeComponent();
            studentData = student;
            SectionOperations sectionOp = new();
            SectionModel sectionModel = new();
            sectionModel = sectionOp.GetSectionData(student.Section_id);
            student.Course = sectionModel.Course;
            student.Year_section = $"{sectionModel.Year_level}{sectionModel.Section}";
            DataContext = student;
        }

        //View Student Info
        private void ViewStudentTopRibbon(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }
        //Change UI
        private void UpdateUI_Click(object sender, RoutedEventArgs e)
        {
            UpdateModeUI();
        }
        //Proceed Update Profile
        private void ProceedUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(fnameTextBox.Text) ||
                string.IsNullOrWhiteSpace(lnameTextBox.Text) ||
                string.IsNullOrWhiteSpace(mnameTextBox.Text) ||
                string.IsNullOrWhiteSpace(emailTextBox.Text) ||
                string.IsNullOrWhiteSpace(ageText.Text))
            {
                MessageBox.Show("All fields are required", "Required all fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Email validation
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(emailTextBox.Text.Trim(), emailPattern))
            {
                MessageBox.Show("Invalid email address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Verify if the email is already existing
            UsersOperations userOp = new();
            if (userOp.VerifyEmail(emailTextBox.Text, studentData.User_id))
            {
                MessageBox.Show("This email already exists", "Invalid email", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate gender
            string gender = maleRadio.IsChecked == true ? "Male" :
                            femaleRadio.IsChecked == true ? "Female" : "";

            if (string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Teacher must have gender", "Required all fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Age validation
            if (!int.TryParse(ageText.Text.Trim(), out int age) || age <= 15 || age > 60)
            {
                MessageBox.Show("Age must be between 15 and 60", "Invalid Age", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Prepare updated model
            string storedPass = studentData.Password;

            // Password update logic
            if ((!string.IsNullOrEmpty(oldPassBox.Password) || !string.IsNullOrEmpty(oldPassTextBox.Text)) &&
                (!string.IsNullOrEmpty(newPassBox.Password) || !string.IsNullOrEmpty(newPassTextBox.Text)))
            {
                string enteredOldPass = oldPassBox.Visibility == Visibility.Visible ? oldPassBox.Password : oldPassTextBox.Text;
                if (!BCrypt.Net.BCrypt.Verify(enteredOldPass, studentData.Password))
                {
                    MessageBox.Show("Old password isn't match", "Invalid Old Password", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    return;
                }

                string enteredNewPass = newPassBox.Visibility == Visibility.Visible ? newPassBox.Password : newPassTextBox.Text;
                if (enteredNewPass.Length < 9)
                {
                    MessageBox.Show("Password must be at least 9 characters", "Invalid Password", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    return;
                }

                storedPass = BCrypt.Net.BCrypt.HashPassword(enteredNewPass);
            }
            SectionOperations secOp = new();
            SectionModel secID = secOp.GetSectionDataByCourseYear(courseText.Text, Convert.ToInt32(yearText.Text.Remove(1, 1)));
            //Set Values
            StudentModel model = new()
            {
                Student_id = studentData.Student_id,
                User_id = studentData.User_id,
                Student_number = studentData.Student_number,
                First_name = fnameTextBox.Text,
                Middle_name = mnameTextBox.Text,
                Last_name = lnameTextBox.Text,
                Email = emailTextBox.Text,
                Birth_date = bdayTextBox.SelectedDate,
                Password = storedPass,
                Section_id = secID.Section_id,
                Gender = gender,
                Age = age,
                Role = "Student",
                Updated_at = DateTime.Now,
            };

            StudentOperations studentOp = new();
            studentOp.UpdateStudent(studentData.User_id, model, "Student");

            StudentModel? updatedStudentModel = new();
            updatedStudentModel = studentOp.GetStudentData(studentData.User_id, DataHelper.connectDB());
            DataContext = updatedStudentModel;
            if (studentData.User_id != 0)
            {
                // Create and show the new layout
                updatedStudentModel.Full_name = $"{fnameTextBox.Text} {lnameTextBox.Text}";
                updatedStudentModel.Role = "[Student]";
                StudentPageLayout newLayout = new(updatedStudentModel);
                newLayout.DataContext = updatedStudentModel;
                newLayout.Show();

                // Navigate to the updated profile inside the new frame
                newLayout.StudentLayout.Navigate(new Profile(updatedStudentModel));

                // Close the old StudentLayout window
                var oldLayout = Application.Current.Windows
                    .OfType<StudentPageLayout>()
                    .FirstOrDefault(w => w != newLayout);

                oldLayout?.Close();
            }
        }

        //Change into Update Mode
        private void UpdateModeUI()
        {
            //hide textblocks
            fnameText.Visibility = Visibility.Collapsed;
            lnameText.Visibility = Visibility.Collapsed;
            mnameText.Visibility = Visibility.Collapsed;
            emailText.Visibility = Visibility.Collapsed;
            genderText.Visibility = Visibility.Collapsed;
            bdayText.Visibility = Visibility.Collapsed;
            UpdateUI.Visibility = Visibility.Collapsed;

            //show textbox
            emailTextBox.Visibility = Visibility.Visible;
            genderTextBox.Visibility = Visibility.Visible;
            bdayTextBox.Visibility = Visibility.Visible;
            fnameTextBox.Visibility = Visibility.Visible;
            lnameTextBox.Visibility = Visibility.Visible;
            mnameTextBox.Visibility = Visibility.Visible;
            updateBtns.Visibility = Visibility.Visible;
            passBox.Visibility = Visibility.Visible;
            topRibbonsBtn.Visibility = Visibility.Visible;

            //set the value of textblocks to textbox
            fnameTextBox.Text = fnameText.Text;
            lnameTextBox.Text = lnameText.Text;
            mnameTextBox.Text = mnameText.Text;
            emailTextBox.Text = emailText.Text;
            bdayTextBox.Text = bdayText.Text;
            if (genderText.Text == "Male")
            {
                maleRadio.IsChecked = true;
                femaleRadio.IsChecked = false;
            }
            else
            {
                femaleRadio.IsChecked = true;
                maleRadio.IsChecked = false;
            }
        }
        private void CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        private void ResetUI()
        {
            //hide textblocks
            fnameText.Visibility = Visibility.Visible;
            lnameText.Visibility = Visibility.Visible;
            mnameText.Visibility = Visibility.Visible;
            emailText.Visibility = Visibility.Visible;
            genderText.Visibility = Visibility.Visible;
            bdayText.Visibility = Visibility.Visible;
            UpdateUI.Visibility = Visibility.Visible;

            //hide textbox
            emailTextBox.Visibility = Visibility.Collapsed;
            genderTextBox.Visibility = Visibility.Collapsed;
            bdayTextBox.Visibility = Visibility.Collapsed;
            fnameTextBox.Visibility = Visibility.Collapsed;
            passBox.Visibility = Visibility.Collapsed;
            lnameTextBox.Visibility = Visibility.Collapsed;
            mnameTextBox.Visibility = Visibility.Collapsed;
            updateBtns.Visibility = Visibility.Collapsed;
            topRibbonsBtn.Visibility = Visibility.Collapsed;
        }
        private bool showOldPass = false;
        private bool showNewPass = false;
        private void oldPassShowIcon_Click(object sender, RoutedEventArgs e)
        {
            showOldPass = !showOldPass;
            if (!showOldPass)
            {
                oldPassShowIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.EyeSlash;
                oldPassBox.Visibility = Visibility.Visible;
                oldPassTextBox.Visibility = Visibility.Collapsed;
                oldPassBox.Password = oldPassTextBox.Text;
                oldPassBox.Focus();
                return;
            }
            oldPassShowIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Eye;
            oldPassBox.Visibility = Visibility.Collapsed;
            oldPassTextBox.Text = oldPassBox.Password;
            oldPassTextBox.Visibility = Visibility.Visible;
            oldPassTextBox.Focus();
        }

        private void newPassShowIcon_Click(object sender, RoutedEventArgs e)
        {
            showNewPass = !showNewPass;
            if (!showNewPass)
            {
                newPassShowIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.EyeSlash;
                newPassBox.Visibility = Visibility.Visible;
                newPassTextBox.Visibility = Visibility.Collapsed;
                newPassBox.Password = newPassTextBox.Text;
                newPassBox.Focus();
                return;
            }
            newPassShowIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Eye;
            newPassBox.Visibility = Visibility.Collapsed;
            newPassTextBox.Text = newPassBox.Password;
            newPassTextBox.Visibility = Visibility.Visible;
            newPassTextBox.Focus();
        }

        private void bdayTextBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            calculateAge();
        }
        private void calculateAge()
        {
            if (bdayTextBox.SelectedDate.HasValue)
            {
                DateTime birthDate = bdayTextBox.SelectedDate.Value;
                DateTime currentDate = DateTime.Today;

                int age = currentDate.Year - birthDate.Year;
                if (birthDate > currentDate.AddYears(-age))
                {
                    age--; // Birthday hasn't occurred yet this year
                }

                if (age < 15)
                {
                    MessageBox.Show("Invalid age. Must be at least 15 years old.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    
                    bdayTextBox.Text = studentData.Birth_date?.ToString("yyyy-MM-dd") ?? "";
                    ageText.Text = studentData.Age.ToString();
                    return;
                }

                ageText.Text = age.ToString();
            }
        }
    }
}
