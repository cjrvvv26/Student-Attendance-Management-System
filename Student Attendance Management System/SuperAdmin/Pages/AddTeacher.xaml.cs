using Microsoft.Win32;
using Student_Attendance_Management_System.Data;
using Student_Attendance_Management_System.ViewModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for AddTeacher.xaml
    /// </summary>
    public partial class AddTeacher : Page
    {
        private byte[]? imgBytes = null;
        private string _department;
        private string _gender;
        public AddTeacher()
        {
            InitializeComponent();
            DataContext = new TeacherVM();
            LoadDepartment();
            if (profileImg.ImageSource == null)
            {
                profileBox.Visibility = Visibility.Collapsed;
            }else
            {
                defaultProfileBox.Visibility = Visibility.Visible;
            }
        }

        private void UploadImg_Click(object sender, RoutedEventArgs e)
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
                    profileImg.ImageSource = bitmap;
                }
                profileBox.Visibility = Visibility.Visible;
                defaultProfileBox.Visibility = Visibility.Collapsed;
            }
        }

        private void AddTeacher_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TeacherVM teacherVM)
            {
                if (teacherVM.AddTeacher(imgBytes, _department, _gender))
                {
                    SuperAdminLayout? mainPageAdmin = Application.Current.Windows.OfType<SuperAdminLayout>().FirstOrDefault();
                    if (mainPageAdmin != null)
                    {
                        mainPageAdmin.SAdminContent.Navigate(new Users());
                    }
                }
            }
        }

        private void LoadDepartment()
        {
            SectionOperations sectionOP = new();
            department_box.ItemsSource = sectionOP.GetAllCourse();
            department_box.SelectedIndex = 0;
            _department = department_box.SelectedItem.ToString() ?? string.Empty;
        }

        private void maleRadio_Checked(object sender, RoutedEventArgs e)
        {
            _gender = "Male";
        }

        private void femaleRadio_Checked(object sender, RoutedEventArgs e)
        {
            _gender = "Female";
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
    }
}