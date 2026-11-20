using Microsoft.Win32;
using Org.BouncyCastle.Utilities;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.Models;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ViewUser.xaml
    /// </summary>
    public partial class ViewUser : Page
    {
        private object _user { get; set; }
        private string _gender;
        private byte[]? imgBytes;
        public ViewUser(object user)
        {
            InitializeComponent();
            courseCombo.SelectedIndex = 0;
            SelectCourseSection();
            DataContext = user;
            _user = user;
            if (user is StudentModel student)
            {   
                departLabel.Text = "Course";
                List<string> courses = new List<string> { "BEEd", "BSIT", "BT-AUTO", "BTLEd" };

                SectionOperations sectionOp = new();
                SectionModel selectedSection = sectionOp.GetSectionData(student.Section_id);

                if (courses.Contains(selectedSection.Course))
                {
                    departText.Text = selectedSection.Course;
                    phoneText.Text = selectedSection.Year_level.ToString();
                    courseCombo.SelectedItem = selectedSection.Course;
                    yearCombo.SelectedItem = $"{selectedSection.Year_level}{selectedSection.Section}";
                }
                phoneLabel.Text = "Year & Section";
                bdayText.Text = student.Birth_date?.ToString("yyyy-MM-dd") ?? "";
                profileBox.Visibility = Visibility.Collapsed;
                defaultProfileBox.Visibility = Visibility.Visible;
            }

            if (user is TeacherModel teacher)
            {
                byte[]? profile_pic = teacher.Profile_pic;
                if (profile_pic != null)
                {
                    using (MemoryStream ms = new MemoryStream(profile_pic))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        teacherProfile.ImageSource = bitmap;
                    }
                }else
                {
                    profileBox.Visibility = Visibility.Collapsed;
                    defaultProfileBox.Visibility = Visibility.Visible;
                }
            }
            View.Visibility = Visibility.Collapsed;
            ProceedUpdate.Visibility = Visibility.Collapsed;
        }
        private void SelectCourseSection()
        {
            SectionOperations secOp = new();
            List<string> courses = secOp.GetAllCourse();
            courseCombo.ItemsSource = courses;
            GenerateSections(courseCombo.SelectedItem.ToString());
            yearCombo.SelectedIndex = 0;
        }

        private void GenerateSections(string course)
        {
            SectionOperations secOp = new();
            List<int> sections = secOp.GetAllYearLevelByCourse(course);
            yearCombo.ItemsSource = sections;
        }
        private void courseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectCourseSection();
        }

        private void UpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            UIUpdate();
            SelectCourseSection();
            topRibbonUpdateUser.Visibility = Visibility.Visible;
            editBtn.Visibility = Visibility.Collapsed;
            uploadImgBtn.Visibility = Visibility.Visible;
            UpdateUser();
            if (_user is StudentModel student)
            {
                uploadImgBtn.Visibility = Visibility.Collapsed;
                departText.Visibility = Visibility.Collapsed;
                phoneText.Visibility = Visibility.Collapsed;
            }
        }

        private string UpdateUser()
        {
            if (_user is TeacherModel)
            {
                //call update method
                return "Update Data Successfully";
            }else if (_user is StudentModel)
            {
                //call update method
                return "Update Data Successfully";
            }
            return "Failed to update data";
        }

        private void UIUpdate()
        {
            UpdateUIHelper(fnameText, fnameBox);
            UpdateUIHelper(mnameText, mnameBox);
            UpdateUIHelper(lnameText, lnameBox);
            UpdateComboAndDate(genderText, genderBox, bdayBox);
        }

        private void UpdateUIHelper(TextBlock block, TextBox box)
        {
                box.Text = block.Text;
                box.Visibility = Visibility.Visible;
                block.Visibility = Visibility.Collapsed;
                View.Visibility = Visibility.Visible;
                ProceedUpdate.Visibility = Visibility.Visible;
        }

        private void UpdateComboAndDate(TextBlock block, Grid box, DatePicker date)
        {
            block.Visibility = Visibility.Collapsed;
            box.Visibility = Visibility.Visible;
            if (genderText.Text == "Male")
            {
                maleRadio.IsChecked = true;
                femaleRadio.IsChecked = false;
            }else if (genderText.Text == "Female")
            {
                femaleRadio.IsChecked = true;
                maleRadio.IsChecked = false;
            }
            bdayBox.Visibility = Visibility.Visible;
            bdayBox.SelectedDate = DateTime.Parse(bdayText.Text);
            if (_user is StudentModel student)
            {
                courseCombo.Visibility = Visibility.Visible;
                yearCombo.Visibility = Visibility.Visible;
                return;
            }
            else if (_user is TeacherModel teacher)
            {
                departBox.ItemsSource = new List<string> { "Information Technology", "Elementary Education", "Livelihood Education", "Automotive"  };
                departBox.Visibility = Visibility.Visible;
                departBox.Text = teacher.Department;
                phoneBox.Visibility = Visibility.Visible;
                phoneBox.Text = phoneText.Text;
            }
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            ViewOnlyUI(fnameText, fnameBox);
            ViewOnlyUI(mnameText, mnameBox);
            ViewOnlyUI(lnameText, lnameBox);
            ViewComboAndDate(genderText, genderBox, bdayBox);
            uploadImgBtn.Visibility = Visibility.Collapsed;
            editBtn.Visibility = Visibility.Visible;
            if (_user is StudentModel student)
            {
                departText.Visibility = Visibility.Visible;
                phoneText.Visibility = Visibility.Visible;
            }
        }

        private void ViewOnlyUI(TextBlock block, TextBox box)
        {
            box.Visibility = Visibility.Collapsed;
            block.Visibility = Visibility.Visible;
            View.Visibility = Visibility.Collapsed;
            ProceedUpdate.Visibility = Visibility.Collapsed;
        }

        private void ViewComboAndDate(TextBlock block, Grid box, DatePicker date)
        {
            block.Visibility = Visibility.Visible;
            box.Visibility = Visibility.Collapsed;
                bdayBox.Visibility = Visibility.Collapsed;
                bdayBox.SelectedDate = DateTime.Parse(bdayText.Text);
            courseCombo.Visibility = Visibility.Collapsed;
            yearCombo.Visibility = Visibility.Collapsed;
            departBox.Visibility = Visibility.Collapsed;
            phoneBox.Visibility = Visibility.Collapsed;
        }



        //Update User
        private void ProceedUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (maleRadio.IsChecked == true)
            {
                _gender = "Male";
            }else if(femaleRadio.IsChecked == true) {
                _gender = "Female";
            }
            if (_user is TeacherModel teacher)
            {
                  teacher.Profile_pic = imgBytes;
                  teacher.First_name = fnameBox.Text;
                  teacher.Middle_name = mnameBox.Text;
                  teacher.Last_name = lnameBox.Text;
                  teacher.Gender = _gender;
                  teacher.Birth_date = bdayBox.SelectedDate;
                  teacher.Age = Convert.ToInt32(ageText.Text);
                  teacher.Department = departBox.Text;
                  teacher.Phone_no = phoneBox.Text;
                  teacher.Updated_at = DateTime.Now;
                TeacherOperations teacherOP = new();
                teacherOP.UpdateTeacher(teacher.User_id, teacher, "Super admin");
            }

            if (_user is StudentModel student)
            {
                SectionOperations sectionOp = new();
                SectionModel secModel = sectionOp.GetSectionDataByCourseYear(courseCombo.SelectedItem.ToString(), Convert.ToInt32(yearCombo.SelectedItem.ToString()));
                student.First_name = fnameBox.Text;
                student.Middle_name = mnameBox.Text;
                student.Last_name = lnameBox.Text;
                student.Gender = _gender;
                student.Section_id = secModel.Section_id;
                student.Birth_date = bdayBox.SelectedDate;
                student.Age = Convert.ToInt32(ageText.Text);
                student.Updated_at = DateTime.Now;
                StudentOperations studentOP = new();
                studentOP.UpdateStudent(student.User_id, student,  "");
            }

            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new Users());
            }
        }

        private void bdayBox_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            calculateAge();
        }
        private void calculateAge()
        {
            if (bdayBox.SelectedDate.HasValue)
            {
                DateTime currentDate = DateTime.Today;
                int age = currentDate.Year - bdayBox.SelectedDate.Value.Year;
                if (bdayBox.SelectedDate.Value > currentDate.AddYears(-age))
                {
                    age--;
                }
                    ageText.Text = age.ToString();
            }
        }

        private void uploadImgBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (fileDialog.ShowDialog() == true)
            {
                imgBytes = File.ReadAllBytes(fileDialog.FileName);
                using (MemoryStream ms = new MemoryStream(imgBytes))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    teacherProfile.ImageSource = bitmap;
                }
            }
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            Users users = new();
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new Users());
            }
        }

        private void viewUserTopRibbon_Click(object sender, RoutedEventArgs e)
        {
            Users users = new();
            SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
            if (_user is TeacherModel teacherData)
            {
                TeacherOperations teacherOP = new();
                TeacherModel? teacher = teacherOP.GetTeacherData(teacherData.User_id);
                if (teacher == null)
                {
                    MessageBox.Show("User doesn't found", "Not found", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (_user is StudentModel studentData)
            {
                StudentOperations studentOP = new();
                StudentModel? student = studentOP.GetStudentData(studentData.User_id, DataHelper.connectDB());
                if (student == null)
                {
                    MessageBox.Show("User doesn't found", "Not found", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (mainPageAdmin != null)
            {
                mainPageAdmin.SAdminContent.Navigate(new ViewUser(_user));
            }
            else
            {
                MessageBox.Show("Main window is not open! Please try again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
