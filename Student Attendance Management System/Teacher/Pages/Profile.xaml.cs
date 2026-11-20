using Microsoft.VisualBasic;
using Microsoft.Win32;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.SuperAdmin.Pages;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Student_Attendance_Management_System.Teacher.Pages
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        private TeacherModel _teacher;
        public Profile(TeacherModel teacher)
        {
            InitializeComponent();
            DataContext = teacher;
            _teacher = teacher;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (teacher.Profile_pic != null)
                {
                    using (MemoryStream ms = new(teacher.Profile_pic))
                    {
                        BitmapImage bitmapImage = new();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        teacherProfile.ImageSource = bitmapImage;
                    }
                    defaultProfileBox.Visibility = Visibility.Collapsed;
                    return;
                }
                profileBox.Visibility = Visibility.Collapsed;
                defaultProfileBox.Visibility = Visibility.Visible;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        // back viewing by clicking top ribbons
        private void ViewTeacherTopRibbon(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        //Upload New Image
        private void UploadImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (fileDialog.ShowDialog() == true)
            {
                _teacher.Profile_pic = File.ReadAllBytes(fileDialog.FileName);
                using (MemoryStream ms = new MemoryStream(_teacher.Profile_pic))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    teacherProfile.ImageSource = bitmap;
                }
                profileBox.Visibility = Visibility.Visible;
                defaultProfileBox.Visibility = Visibility.Collapsed;
            }
        }

        //Change the UI into Update Mode
        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            ChangeUI();
        }
        private void ChangeUI ()
        {
            //Hide textblocks
            editBtn.Visibility = Visibility.Collapsed;
            fnameText.Visibility = Visibility.Collapsed;
            mnameText.Visibility = Visibility.Collapsed;
            lnameText.Visibility = Visibility.Collapsed;
            genderText.Visibility = Visibility.Collapsed;
            bdayText.Visibility = Visibility.Collapsed;
            emailText.Visibility = Visibility.Collapsed;
            departText.Visibility = Visibility.Collapsed;
            phoneText.Visibility = Visibility.Collapsed;

            //Show the textboxes and comboxes
            fnameBox.Visibility = Visibility.Visible;
            mnameBox.Visibility = Visibility.Visible;
            lnameBox.Visibility = Visibility.Visible;
            emailBox.Visibility = Visibility.Visible;
            genderBox.Visibility = Visibility.Visible;
            phoneBox.Visibility = Visibility.Visible;
            departBox.Visibility = Visibility.Visible;
            bdayBox.Visibility = Visibility.Visible;
            UploadImgBtn.Visibility = Visibility.Visible;
            topRibbonsBtn.Visibility = Visibility.Visible;
            topRibbonsBtn.Visibility = Visibility.Visible;
            oldPassContainer.Visibility = Visibility.Visible;
            newPassContainer.Visibility = Visibility.Visible;
            UpdateBtnBox.Visibility = Visibility.Visible;

            //Set the value of textblocks in textbox
            fnameBox.Text = fnameText.Text;
            mnameBox.Text = mnameText.Text;
            lnameBox.Text = lnameText.Text;
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
            //birthdate last
            bdayBox.Text = bdayText.Text;
            departBox.ItemsSource = DepartmentSelection();
            departBox.SelectedIndex = 0;
            phoneBox.Text = phoneText.Text;
            emailBox.Text = emailText.Text;
        }

        //Reset the UI to viewing
        private void ResetUI()
        {
            //Hide textblocks
            editBtn.Visibility = Visibility.Visible;
            fnameText.Visibility = Visibility.Visible;
            mnameText.Visibility = Visibility.Visible;
            lnameText.Visibility = Visibility.Visible;
            genderText.Visibility = Visibility.Visible;
            bdayText.Visibility = Visibility.Visible;
            emailText.Visibility = Visibility.Visible;
            departText.Visibility = Visibility.Visible;
            phoneText.Visibility = Visibility.Visible;

            //Show the textboxes and comboxes
            fnameBox.Visibility = Visibility.Collapsed;
            mnameBox.Visibility = Visibility.Collapsed;
            lnameBox.Visibility = Visibility.Collapsed;
            emailBox.Visibility = Visibility.Collapsed;
            genderBox.Visibility = Visibility.Collapsed;
            phoneBox.Visibility = Visibility.Collapsed;
            departBox.Visibility = Visibility.Collapsed;
            bdayBox.Visibility = Visibility.Collapsed;
            UploadImgBtn.Visibility = Visibility.Collapsed;
            topRibbonsBtn.Visibility = Visibility.Collapsed;
            topRibbonsBtn.Visibility = Visibility.Collapsed;
            oldPassContainer.Visibility = Visibility.Collapsed;
            newPassContainer.Visibility = Visibility.Collapsed;
            UpdateBtnBox.Visibility = Visibility.Collapsed;
        }

        //Generate Available Courses for depart selection
        private List<string> DepartmentSelection()
        {
            SectionOperations sectionOp = new();
            return sectionOp.GetAllCourse();
        }

        private void bdayBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            calculateAge();
        }
        private void calculateAge()
        {
            if (bdayBox.SelectedDate.HasValue)
            {
                DateTime birthDate = bdayBox.SelectedDate.Value;
                DateTime currentDate = DateTime.Today;

                int age = currentDate.Year - birthDate.Year;
                if (birthDate > currentDate.AddYears(-age))
                {
                    age--; // Birthday hasn't occurred yet this year
                }

                if (age < 15)
                {
                    MessageBox.Show("Invalid age. Must be at least 15 years old.", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Reset to original date (assuming _teacher.Birth_date is your stored value)
                    bdayBox.Text = _teacher.Birth_date?.ToString("yyyy-MM-dd") ?? "";
                    ageText.Text = _teacher.Age.ToString();
                    return;
                }

                ageText.Text = age.ToString();
            }
        }

        //Cancel update and Reset the UI
        private void View_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }
        //Proceed update
        private void ProceedUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(fnameBox.Text) ||
                string.IsNullOrWhiteSpace(mnameBox.Text) ||
                string.IsNullOrWhiteSpace(lnameBox.Text) ||
                string.IsNullOrWhiteSpace(emailBox.Text) ||
                string.IsNullOrWhiteSpace(departBox.Text) ||
                string.IsNullOrWhiteSpace(ageText.Text) ||
                string.IsNullOrWhiteSpace(phoneBox.Text))
            {
                MessageBox.Show("All fields are required", "Required all fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Email validation
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(emailBox.Text.Trim(), emailPattern))
            {
                MessageBox.Show("Invalid email address", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Verify if the email is already existing
            UsersOperations userOp = new();
            if (userOp.VerifyEmail(emailBox.Text, _teacher.User_id))
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

            string numberPattern = @"^(09\d{9}|\+639\d{9})$";
            Regex phoneValidate = new Regex(numberPattern);

            if (!phoneValidate.IsMatch(phoneBox.Text))
            {
                MessageBox.Show("Invalid Phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Age validation
            if (!int.TryParse(ageText.Text.Trim(), out int age) || age <= 25 || age > 80)
            {
                MessageBox.Show("Age must be between 25 and 80", "Invalid Age", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Prepare updated model
            string storedPass = _teacher.Password;

            // Password update logic
            if ((!string.IsNullOrEmpty(oldPassBox.Password) || !string.IsNullOrEmpty(oldPassTextBox.Text)) &&
                (!string.IsNullOrEmpty(newPassBox.Password) || !string.IsNullOrEmpty(newPassTextBox.Text)))
            {
                string enteredOldPass = oldPassBox.Visibility == Visibility.Visible ? oldPassBox.Password : oldPassTextBox.Text;
                if (!BCrypt.Net.BCrypt.Verify(enteredOldPass, _teacher.Password))
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

            // Set values
            TeacherModel teacherModel = new()
            {
                Teacher_id = _teacher.Teacher_id,
                User_id = _teacher.User_id,
                Profile_pic = _teacher.Profile_pic,
                First_name = fnameBox.Text.Trim(),
                Middle_name = mnameBox.Text.Trim(),
                Last_name = lnameBox.Text.Trim(),
                Department = departBox.SelectedItem.ToString(),
                Email = emailBox.Text.Trim(),
                Birth_date = bdayBox.SelectedDate,
                Password = storedPass,
                Gender = gender,
                Age = age,
                Role = "Teacher",
                Phone_no = phoneBox.Text,
                Updated_at = DateTime.Now,
            };

            // Update teacher
            TeacherOperations teacherOp = new();
            int teacherUserID = teacherOp.UpdateTeacher(_teacher.User_id, teacherModel, "Teacher");

            TeacherModel updatedTeacherModel = new();
            updatedTeacherModel = teacherOp.GetTeacherDataByUserID(teacherUserID);
            DataContext = updatedTeacherModel;
            if (teacherUserID != 0)
            {
                // Create and show the new layout
                updatedTeacherModel.Full_name = $"{fnameBox.Text} {lnameBox.Text}";
                updatedTeacherModel.Role = "[Teacher]";
                TeacherLayout newLayout = new(updatedTeacherModel);
                newLayout.DataContext = updatedTeacherModel;
                newLayout.Show();

                // Navigate to the updated profile inside the new frame
                newLayout.TeacherFrame.Navigate(new Profile(updatedTeacherModel));

                // Close the old TeacherLayout window
                var oldLayout = Application.Current.Windows
                    .OfType<TeacherLayout>()
                    .FirstOrDefault(w => w != newLayout);

                oldLayout?.Close();
            }
        }
    }
}
